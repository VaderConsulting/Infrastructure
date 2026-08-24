Imports System.Threading
Imports System.ComponentModel

Public Class frmTransmit

    Dim m_FileSendCount As Long = 0
    Dim m_Port As String = "COM1"

    Private Sub btnSend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSend.Click
        cmbPorts.Enabled = False
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

        strFilename = CStr(e.Argument)

        Dim oFilestream As System.IO.FileStream = System.IO.File.OpenRead(strFilename)

        Dim intByte As Integer

        Dim COMPort As IO.Ports.SerialPort = My.Computer.Ports.OpenSerialPort(m_Port, 9600, IO.Ports.Parity.None, 8, IO.Ports.StopBits.One)

        Dim EOLCharacter As Integer = 13
        Dim strLine As String = ""
        Dim FileProgress As Integer = 0

        Do
            Try
                strLine = "" ' Clear the current line

                Do ' read each character from the input stream until we get a complete line
                    intByte = oFilestream.ReadByte
                    strLine += Chr(intByte)
                Loop Until intByte = EOLCharacter

                FileProgress = (oFilestream.Position / oFilestream.Length) * 100

                worker.ReportProgress(FileProgress)

                COMPort.Write(strLine)

                If worker.CancellationPending Then ' The user wishes to cancel the operation
                    COMPort.Close()
                    Exit Sub
                End If

                PauseRandomSeconds()

            Catch
                Dim oComplete As New System.ComponentModel.RunWorkerCompletedEventArgs(CObj(""), Nothing, False)

                WorkComplete(Me, oComplete)
                Exit Do
            End Try
        Loop

        COMPort.Close()

    End Sub

    Private Sub PauseRandomSeconds()
        Dim oRandom As Random = New Random
        Dim intRandomElements() As Integer = {0, 0, 0, 0, 0, 0, 1, 1, 2}
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
            'cmbPorts.SelectedText = cmbPorts.Items(0).ToString
            m_Port = cmbPorts.Items(0).ToString
        End If

    End Sub

    Private Sub cmbPorts_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbPorts.SelectedIndexChanged
        m_Port = cmbPorts.SelectedText
    End Sub

End Class
