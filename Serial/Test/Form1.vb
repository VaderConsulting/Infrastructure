Imports System
Imports System.IO
Imports System.IO.Ports

Public Class Form1

#Region " Delegates "

    Private Delegate Sub UpdateByteCountDelegate()
    Private Delegate Sub UpdateMessageListDelegate(ByVal Message As String)
    Private Delegate Sub UpdateReceivedByteCountDelegate(ByVal Bytes As Int64)
    Private Delegate Sub UpdateStateDelegate(ByVal State As String)

#End Region

    Private WithEvents m_COMPort As New Serial.COM
    Private WithEvents m_RefreshTimer As New Timer

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        'm_COMPort.Close()
        m_COMPort.Hangup()
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim Ports As Collections.ObjectModel.ReadOnlyCollection(Of String) = My.Computer.Ports.SerialPortNames

        For Each Port As String In Ports
            cmbPorts.Items.Add(Port)
        Next
        If cmbPorts.Items.Count > 0 Then
            cmbPorts.SelectedIndex = 0
            m_COMPort.PortName = cmbPorts.Items(0).ToString
        End If

        Me.Show()

    End Sub

    Private Sub UpdateRawByteCountMessage()
        If lblReceivedByteCount.InvokeRequired Then
            Dim oDelegate As New UpdateByteCountDelegate(AddressOf UpdateRawByteCountMessage)
            Me.Invoke(oDelegate, Nothing)
        Else
            'Me.lblRawByteCount.Text = m_DataReader.RawData.Length
            Me.Refresh()
        End If
    End Sub

    'Private Sub m_DataReader_ReceivedData(ByVal Data As String) Handles m_DataReader.ReceivedData
    '    Trace.WriteLine("(Reader) Received Data event: " & Data)
    '    UpdateRawByteCountMessage()
    'End Sub

    Private Sub UpdateMessages(ByVal Message As String)
        lstMessages.Items.Add(Message)
        lstMessages.SelectedIndex = lstMessages.Items.Count - 1
        lstMessages.Refresh()
    End Sub

    Private Sub UpdateReceivedByteCount(ByVal ByteCount As Int64)
        lblReceivedByteCount.Text = ByteCount.ToString
        lblReceivedByteCount.Refresh()
    End Sub

    Private Sub UpdateState(ByVal State As String)
        lblState.Text = State
        lblState.Refresh()
    End Sub

#Region " COM Event handlers "

    Private Sub m_COMPort_ConnectFailureEvent(ByVal ErrorMessage As String) Handles m_COMPort.ConnectFailureEvent
        If lstMessages.InvokeRequired Then
            Dim oDelegate As New UpdateMessageListDelegate(AddressOf UpdateMessages)
            Me.Invoke(oDelegate, ErrorMessage)
        Else
            UpdateMessages(ErrorMessage)
        End If
    End Sub

    Private Sub m_COMPort_ConnectSuccessEvent() Handles m_COMPort.ConnectSuccessEvent
        If lstMessages.InvokeRequired Then
            Dim oDelegate As New UpdateMessageListDelegate(AddressOf UpdateMessages)
            Me.Invoke(oDelegate, "Connected")
        Else
            UpdateMessages("Connected")
        End If
        'lstMessages.Items.Add("Connected")
        'lstMessages.Refresh()
    End Sub

    Private Sub m_COMPort_ModemBusyEvent() Handles m_COMPort.ModemBusyEvent
        If lstMessages.InvokeRequired Then
            Dim oDelegate As New UpdateMessageListDelegate(AddressOf UpdateMessages)
            Me.Invoke(oDelegate, "Line Busy")
        Else
            UpdateMessages("Line Busy")
        End If
        'lstMessages.Items.Add("Line Busy")
        'lstMessages.Refresh()
    End Sub

    'Private Sub m_COMPort_ModemCommandDataEvent(ByVal Data As String) Handles m_COMPort.ModemCommandDataEvent
    '    Trace.WriteLine("Modem Command data received")
    '    lstMessages.Items.Add("Modem Command data received (" & Data & ")")
    '    Me.Refresh()
    'End Sub

    'Private Sub m_COMPort_ModemConnectedEvent() Handles m_COMPort.ModemConnectedEvent
    '    Trace.WriteLine("Modem Connected")
    '    lstMessages.Items.Add("Modem Connected")
    '    Me.Refresh()
    'End Sub

    Private Sub m_COMPort_ModemDialingEvent(ByVal Number As String) Handles m_COMPort.ModemDialingEvent
        If lstMessages.InvokeRequired Then
            Dim oDelegate As New UpdateMessageListDelegate(AddressOf UpdateMessages)
            Me.Invoke(oDelegate, "Modem Dialling event received (" & Number & ")")
        Else
            UpdateMessages("Modem Dialling event received (" & Number & ")")
        End If
        'lstMessages.Items.Add("Modem Dialling event received (" & Number & ")")
        'lstMessages.Refresh()
    End Sub

    Private Sub m_COMPort_ModemErrorEvent() Handles m_COMPort.ModemErrorEvent
        If lstMessages.InvokeRequired Then
            Dim oDelegate As New UpdateMessageListDelegate(AddressOf UpdateMessages)
            Me.Invoke(oDelegate, "Modem Error")
        Else
            UpdateMessages("Modem Error")
        End If
        'lstMessages.Items.Add("Modem Error")
        'lstMessages.Refresh()
    End Sub

    Private Sub m_COMPort_ModemNoAnswerEvent() Handles m_COMPort.ModemNoAnswerEvent
        If lstMessages.InvokeRequired Then
            Dim oDelegate As New UpdateMessageListDelegate(AddressOf UpdateMessages)
            Me.Invoke(oDelegate, "No Answer")
        Else
            UpdateMessages("No Answer")
        End If
        'lstMessages.Items.Add("No Answer")
        'lstMessages.Refresh()
    End Sub

    'Private Sub m_COMPort_ModemNoCarrierEvent() Handles m_COMPort.ModemNoCarrierEvent
    '    lstMessages.Items.Add("Modem: No Carrier")
    '    Me.Refresh()
    'End Sub

    Private Sub m_COMPort_ModemNoDialtoneEvent() Handles m_COMPort.ModemNoDialtoneEvent
        If lstMessages.InvokeRequired Then
            Dim oDelegate As New UpdateMessageListDelegate(AddressOf UpdateMessages)
            Me.Invoke(oDelegate, "Modem: No Dialtone")
        Else
            UpdateMessages("Modem: No Dialtone")
        End If
        'lstMessages.Items.Add("Modem: No Dialtone")
        'lstMessages.Refresh()
    End Sub

    Private Sub m_COMPort_ModemStateChangeEvent(ByVal NewEvent As String) Handles m_COMPort.ModemStateChangeEvent
        If lblState.InvokeRequired Then
            Dim oDelegate As New UpdateStateDelegate(AddressOf UpdateState)
            Me.Invoke(oDelegate, NewEvent)
        Else
            UpdateState(NewEvent)
        End If
        'lblState.Text = NewEvent
        'lblState.Refresh()
    End Sub

    Private Sub m_COMPort_OpenFailureEvent(ByVal ErrorMessage As String) Handles m_COMPort.OpenFailureEvent
        If lstMessages.InvokeRequired Then
            Dim oDelegate As New UpdateMessageListDelegate(AddressOf UpdateMessages)
            Me.Invoke(oDelegate, "Error opening ComPort: " & ErrorMessage)
        Else
            UpdateMessages("Error opening ComPort: " & ErrorMessage)
        End If
        'lstMessages.Items.Add("Error opening ComPort: " & ErrorMessage)
        'lstMessages.Refresh()
    End Sub

    Private Sub m_COMPort_OpenSuccessEvent() Handles m_COMPort.OpenSuccessEvent
        If lstMessages.InvokeRequired Then
            Dim oDelegate As New UpdateMessageListDelegate(AddressOf UpdateMessages)
            Me.Invoke(oDelegate, "COM Port open")
        Else
            UpdateMessages("COM Port open")
        End If
        'lstMessages.Items.Add("COM Port open")
        'lstMessages.Refresh()
    End Sub

    Private Sub m_COMPort_ReceivedDataEvent(ByVal Data As String, ByVal ReceivedByteCount As Int64) Handles m_COMPort.ReceivedDataEvent
        If lstMessages.InvokeRequired Then
            Dim oMessageDelegate As New UpdateMessageListDelegate(AddressOf UpdateMessages)
            Dim oByteCountDelegate As New UpdateReceivedByteCountDelegate(AddressOf UpdateReceivedByteCount)

            If Data.Length > 0 Then
                Me.Invoke(oMessageDelegate, Data)
                Me.Invoke(oByteCountDelegate, ReceivedByteCount)
            End If
        Else
            UpdateMessages(Data)
            UpdateReceivedByteCount(ReceivedByteCount)
        End If
        'If Data.Length > 0 Then
        '    lstMessages.Items.Add(Data)
        '    lstMessages.SelectedIndex = lstMessages.Items.Count - 1
        '    lblReceivedByteCount.Text = ReceivedByteCount.ToString
        '    Me.Refresh()
        'End If
    End Sub

#End Region

    Private Sub btnDial_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDial.Click
        Dim Number As String = ""
        m_RefreshTimer.Interval = 1000
        
        m_COMPort.PortName = cmbPorts.Text
        m_COMPort.DialCommandPrefix = "AT&F&C1E0M1L3%C3"
        m_COMPort.DialCommand = "ATDT"
        m_COMPort.PerformResetUponDial = True
        m_COMPort.DialPauseTimeSeconds = 40
        m_COMPort.MaxConnectTime = New TimeSpan(0, 60, 0)
        'm_COMPort.MaxReceivedByteCount = 500

        Number = "0395206192"

        lstMessages.Items.Add("[Dialing " & Number & "]")

        m_COMPort.Dial(Number)

        'If m_COMPort.Dial(Number) Then
        '    m_COMPort.Open()
        '    m_COMPort.Reset()
        'End If

        lstMessages.Items.Add("[END]")
        m_RefreshTimer.Start()

    End Sub

    Private Sub cmbPorts_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbPorts.SelectedIndexChanged
        If cmbPorts.Text.Length > 0 Then
            btnDial.Enabled = True
        Else
            btnDial.Enabled = False
        End If
    End Sub

    Private Sub btnHangup_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHangup.Click
        m_COMPort.Hangup()
        m_RefreshTimer.Stop()
    End Sub

    Private Sub m_RefreshTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_RefreshTimer.Tick
        Trace.WriteLine("Connect time: " & m_COMPort.ConnectTime.ToString & " Idle time: " & m_COMPort.IdleTime.ToString)

        If m_COMPort.ConnectTime > m_COMPort.MaxConnectTime Then
            Trace.WriteLine("Max connect time exceeded")
            m_COMPort.Hangup()
            m_RefreshTimer.Stop()
        End If

        If m_COMPort.IdleTime > m_COMPort.MaxIdleTime Then
            Trace.WriteLine("Max idle time exceeded")
            m_COMPort.Hangup()
            m_RefreshTimer.Stop()
        End If

        If m_COMPort.MaxReceivedByteCount <> -1 Then
            If m_COMPort.ReceivedByteCount > m_COMPort.MaxReceivedByteCount Then
                Trace.WriteLine("Max byte count exceeded")
                m_COMPort.Hangup()
                m_RefreshTimer.Stop()
            End If
        End If
    End Sub
End Class
