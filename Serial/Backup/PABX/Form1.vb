Imports System
Imports System.IO
Imports System.IO.Ports
Imports System.Threading
Imports System.ComponentModel

Public Class frmTransmit

    Public Enum ModemStateEnum
        Unknown = -1
        Disconnecting = 0
        Disconnected = 1
        Dialling = 2
        Connecting = 3
        Connected = 4
    End Enum

    Public Enum LineStateEnum
        Unknown = -1
        Normal = 0
        NoCarrier = 1
        NoDialtone = 2
        Busy = 3
        Ringing = 4
        NoAnswer = 5
    End Enum

    Dim m_FileSendCount As Long = 0
    Private WithEvents m_COMPort As New SerialPort
    Dim m_Port As String = "COM1"
    Private m_InWaitState As Boolean = False
    Private m_ConnectionState As ModemStateEnum = ModemStateEnum.Disconnected
    Private m_LineState As LineStateEnum = LineStateEnum.Normal
    Private m_EOLCharacter As String = vbCrLf
    Private m_RawData As String = ""
    Private m_FormattedData As String = ""

    Private Sub btnSend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSend.Click
        cmbPorts.Enabled = False

        btnSend.Enabled = False

        StartSend()
    End Sub

    Private Sub StartSend()
        Do
            WorkerThread.RunWorkerAsync(Application.StartupPath & "\test.txt")

            Do
                Application.DoEvents()
            Loop Until (Not WorkerThread.IsBusy)
            m_FileSendCount += 1

            Me.lblFileCount.Text = "File send count: " & CStr(m_FileSendCount)
        Loop
    End Sub

    Private Sub frmTransmit_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Me.WorkerThread.CancelAsync() ' Stop the worker thread
        End
    End Sub

    Private Sub WorkerThread_DoWork(ByVal sender As System.Object, ByVal e As DoWorkEventArgs) Handles WorkerThread.DoWork
        Dim worker As BackgroundWorker = CType(sender, BackgroundWorker)

        SendFile(worker, e)

    End Sub

    Private Sub UpdateProgress(ByVal sender As System.Object, ByVal e As ProgressChangedEventArgs) Handles WorkerThread.ProgressChanged
        Me.pbrFile.Value = e.ProgressPercentage
    End Sub

    Private Sub WorkComplete(ByVal sender As System.Object, ByVal e As RunWorkerCompletedEventArgs) Handles WorkerThread.RunWorkerCompleted

    End Sub

    Private Sub SendFile(ByVal worker As BackgroundWorker, ByVal e As DoWorkEventArgs)
        Dim strFilename As String = ""
        Dim EOLCharacter As Integer = 13
        Dim strLine As String = ""
        Dim FileProgress As Integer = 0
        strFilename = CStr(e.Argument)

        Dim oFilestream As System.IO.FileStream = System.IO.File.OpenRead(strFilename)
        Dim intByte As Integer

        Do
            Try
                strLine = "" ' Clear the current line

                Do ' read each character from the input stream until we get a complete line
                    intByte = oFilestream.ReadByte
                    strLine += Chr(intByte)
                Loop Until intByte = EOLCharacter

                FileProgress = (oFilestream.Position / oFilestream.Length) * 100

                worker.ReportProgress(FileProgress)

                m_COMPort.Write(strLine)

                If worker.CancellationPending Then ' The user wishes to cancel the operation
                    m_COMPort.Close()
                    Exit Sub
                End If

                PauseRandomSeconds()

            Catch
                Dim oComplete As New System.ComponentModel.RunWorkerCompletedEventArgs(CObj(""), Nothing, False)

                WorkComplete(Me, oComplete)
                Exit Do
            End Try
        Loop

        m_COMPort.Close()

    End Sub

    Private Sub PauseRandomSeconds()
        Dim oRandom As Random = New Random
        Dim intRandomElements() As Integer = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 4, 10}
        Dim intElementNumber As Integer = oRandom.Next(0, intRandomElements.Length - 1)
        Dim intRandomElement As Integer = intRandomElements(intElementNumber)

        If intRandomElement > 0 Then

            Dim oTime As Date = Now

            ' Pause the specified number of seconds
            Do
                Application.DoEvents()
            Loop Until Now >= DateAdd(DateInterval.Second, CDbl(intRandomElement), oTime)

        End If

    End Sub

    Private Sub frmTransmit_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.pbrFile.Maximum = 100 ' Percent

        Dim Ports As Collections.ObjectModel.ReadOnlyCollection(Of String) = My.Computer.Ports.SerialPortNames

        For Each Port As String In Ports
            cmbPorts.Items.Add(Port)
        Next
        If cmbPorts.Items.Count > 0 Then
            cmbPorts.SelectedIndex = 0
            m_Port = cmbPorts.Items(0).ToString
        End If

    End Sub

    Private Sub cmbPorts_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbPorts.SelectedIndexChanged
        m_Port = cmbPorts.SelectedText
    End Sub

    Private Sub btnAnswer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAnswer.Click
        m_COMPort.DtrEnable = True
        m_COMPort = My.Computer.Ports.OpenSerialPort(m_Port, 9600, IO.Ports.Parity.None, 8, IO.Ports.StopBits.One)

        btnSend.Enabled = False
        btnAnswer.Enabled = False
        cmbPorts.Enabled = False

    End Sub

    Private Sub m_COMPort_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles m_COMPort.DataReceived
        GetData()
    End Sub

    Private Sub GetData()
        Dim Buffer As String = ""

        If m_COMPort.IsOpen Then
            While m_COMPort.BytesToRead > 0
                Buffer &= m_COMPort.ReadExisting
            End While

            If m_InWaitState Then
                m_InWaitState = False
            Else
                Select Case m_ConnectionState
                    Case ModemStateEnum.Connected
                        ProcessData(Buffer)
                    Case Else
                        If Buffer.Length > 0 Then
                            Select Case Buffer.Trim
                                Case "?"
                                    m_ConnectionState = ModemStateEnum.Unknown
                                    m_LineState = LineStateEnum.Unknown
                                    RaiseEvent ModemQuestionEvent()
                                Case "NO DIALTONE", "NO DIAL TONE"
                                    m_ConnectionState = ModemStateEnum.Disconnected
                                    m_LineState = LineStateEnum.NoDialtone
                                    RaiseEvent ModemNoDialtoneEvent()
                                Case "NO CARRIER"
                                    m_ConnectionState = ModemStateEnum.Disconnected
                                    m_LineState = LineStateEnum.NoCarrier
                                    RaiseEvent ModemNoCarrierEvent()
                                Case "BUSY"
                                    m_ConnectionState = ModemStateEnum.Disconnected
                                    m_LineState = LineStateEnum.Busy
                                    RaiseEvent ModemBusyEvent()
                                Case "NO ANSWER"
                                    m_ConnectionState = ModemStateEnum.Disconnected
                                    m_LineState = LineStateEnum.NoAnswer
                                    RaiseEvent ModemNoAnswerEvent()
                                Case "RING"
                                    m_ConnectionState = ModemStateEnum.Connecting
                                    m_LineState = LineStateEnum.Normal
                                    RaiseEvent ModemRingEvent()
                                Case "OK"
                                    m_ConnectionState = ModemStateEnum.Unknown
                                    m_LineState = LineStateEnum.Unknown
                                Case "ERROR"
                                    m_ConnectionState = ModemStateEnum.Disconnected
                                    m_LineState = LineStateEnum.Unknown
                                    RaiseEvent ModemErrorEvent()
                                Case Else
                                    If Buffer.Contains("CONNECT") Then
                                        m_ConnectionState = ModemStateEnum.Connected
                                        m_LineState = LineStateEnum.Normal
                                        RaiseEvent ModemConnectedEvent()
                                    ElseIf Buffer.StartsWith("ATD") Then
                                        StartSend()
                                    Else
                                        m_ConnectionState = ModemStateEnum.Unknown
                                        m_LineState = LineStateEnum.Normal
                                        RaiseEvent ModemCommandDataEvent(Buffer)
                                    End If
                            End Select
                        End If
                End Select
            End If
        End If
    End Sub

    Private Sub ProcessData(ByVal Data As String)
        m_RawData &= Data
        While m_RawData.Contains(m_EOLCharacter)
            Dim Buffer As String()
            Buffer = m_RawData.Split(m_EOLCharacter)

            ' Go through each line of data
            For i As Int64 = 0 To Buffer.LongCount - 2
                m_FormattedData = Buffer(i)
                RaiseEvent ReceivedDataEvent(m_FormattedData)
            Next

            ' Check if the data ended cleanly at a EOL
            If Data.EndsWith(Buffer(Buffer.LongCount - 1)) Then
                RaiseEvent ReceivedDataEvent(m_FormattedData)
                If m_FormattedData.StartsWith("NO CARRIER") Then m_ConnectionState = ModemStateEnum.Disconnected
                m_RawData = ""
            Else
                m_RawData = m_FormattedData
            End If
        End While
    End Sub

#Region " Events "

    Public Event ReceivedDataEvent(ByVal Data As String)
    Public Event ErrorEvent(ByVal SerialError As System.IO.Ports.SerialErrorReceivedEventArgs)
    Public Event OpenSuccessEvent()
    Public Event OpenFailureEvent(ByVal ErrorMessage As String)
    Public Event ConnectSuccessEvent()
    Public Event ConnectFailureEvent(ByVal ErrorMessage As String)
    Public Event ResetFailureEvent(ByVal ErrorMessage As String)
    Public Event ModemCommandDataEvent(ByVal Data As String)
    Public Event ModemNoCarrierEvent()
    Public Event ModemNoDialtoneEvent()
    Public Event ModemDialingEvent(ByVal Number As String)
    Public Event ModemQuestionEvent()
    Public Event ModemBusyEvent()
    Public Event ModemNoAnswerEvent()
    Public Event ModemErrorEvent()
    Public Event ModemRingEvent()
    Public Event ModemConnectedEvent()

#End Region

    Private Sub m_COMPort_PinChanged(ByVal sender As Object, ByVal e As System.IO.Ports.SerialPinChangedEventArgs) Handles m_COMPort.PinChanged
        Select Case e.EventType
            Case SerialPinChange.Break
            Case SerialPinChange.CDChanged
            Case SerialPinChange.CtsChanged
            Case SerialPinChange.DsrChanged
            Case SerialPinChange.Ring
        End Select
    End Sub
End Class
