Imports System
Imports System.IO
Imports System.IO.Ports
Imports System.Windows.Forms
Imports System.Threading

Public Class COM

#Region " API "

    'Private Declare Function InternetAutodial Lib "wininet.dll" (ByVal dwFlags As Int32, ByVal dwReserved As Int32) As Int32
    'Private Declare Function InternetAutodialHangup Lib "wininet.dll" (ByVal dwReserved As Int32) As Int32

#End Region

#Region " Constants "

    '''' <summary>
    '''' Origin: http://www.dsource.org/projects/bindings/browser/trunk/win32/ras.d?format=txt
    '''' </summary>
    '''' <remarks></remarks>
    'Public Const RASCS_PAUSED = 4096
    'Public Const RASCS_DONE = 8192

#End Region

#Region " Enums "

    '''' <summary>
    '''' Flags for InternetAutodial
    '''' </summary>
    '''' <remarks>http://msdn.microsoft.com/en-us/library/aa384336(VS.85).aspx</remarks>
    'Public Enum DialupMethods
    '    INTERNET_AUTODIAL_FORCE_ONLINE = 1 '         Forces an online Internet connection
    '    INTERNET_AUTODIAL_FORCE_UNATTENDED = 2 '     Forces an unattended Internet dial-up
    '    INTERNET_AUTODIAL_FAILIFSECURITYCHECK = 4 '  Causes InternetAutodial to fail if file and printer sharing is disabled for Windows 95 or later.  Not valid for Server 08 and Vista
    '    INTERNET_AUTODIAL_OVERRIDE_NET_PRESENT = 8 ' Causes InternetAutodial to dial the modem connection even when a network connection to the Internet is present
    'End Enum

    '''' <summary>
    '''' The RASCONNSTATE enumeration type contains values that specify the states that can occur during a RAS connection operation. 
    '''' If the RasDial function is used to establish a RAS connection, specify a window, or a RasDialFunc, RasDialFunc1, or RasDialFunc2 callback function to receive notification messages that report the current connection state. 
    '''' Also, use the RasGetConnectStatus function to get the connection state for a specified connection.
    '''' Origin: http://msdn.microsoft.com/en-us/library/aa376727(VS.85,lightweight).aspx
    '''' </summary>
    '''' <remarks>http://www.dsource.org/projects/bindings/browser/trunk/win32/ras.d?format=txt</remarks>
    'Public Enum RASCONNSTATE
    '    ''' <summary>
    '    ''' The communication port is about to be opened. 
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_OpenPort
    '    ''' <summary>
    '    ''' The communication port has been opened successfully.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_PortOpened
    '    ''' <summary>
    '    ''' A device is about to be connected. RasGetConnectStatus can be called to determine the name and type of the device being connected.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_ConnectDevice
    '    ''' <summary>
    '    ''' A device has connected successfully. RasGetConnectStatus can be called to determine the name and type of the device being connected. 
    '    ''' For a simple modem connection, RASCS_ConnectDevice and RASCS_DeviceConnected is called only once. 
    '    ''' For a dial-up X.25 PAD connection, the pair is called first for the modem, then for the PAD. 
    '    ''' If a preconnect switch is configured, the pair will be called for the switch before any other devices connect. 
    '    ''' Likewise, the pair is called for a postconnect switch after any other devices connect.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_DeviceConnected
    '    ''' <summary>
    '    ''' All devices in the device chain have successfully connected. At this point, the physical link is established.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_AllDevicesConnected
    '    ''' <summary>
    '    ''' The authentication process is starting. Remote access does not allow the remote client to generate any traffic on the LAN until authentication has been successfully completed. 
    '    ''' Remote access authentication consists of:
    '    ''' * Validating the user name/password on the specified domain. 
    '    ''' * Projecting the client onto the LAN. This means that the remote access server does what is necessary to send and receive data on the LAN on behalf of the client. For example, the remote access server might need to add a NetBIOS name that corresponds to the client's computer name. 
    '    ''' * Call-back processing in which the client hangs up and the server calls back. (The user needs special permissions on the remote access server for this.) 
    '    ''' * Calculating the link speed. This is necessary to correctly set transport time-outs to match the relatively slow speed of the remote link.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_Authenticate
    '    ''' <summary>
    '    ''' An authentication event has occurred. If dwError is zero, this event will be immediately followed by one of the more specific authentication states following. 
    '    ''' If dwError is nonzero, authentication has failed, and the error value indicates why.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_AuthNotify
    '    ''' <summary>
    '    ''' The client has requested another validation attempt with a new user name/password/domain.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_AuthRetry
    '    ''' <summary>
    '    ''' The remote access server has requested a callback number. 
    '    ''' This occurs only if the user has "Set By Caller" callback privilege on the server.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_AuthCallback
    '    ''' <summary>
    '    ''' The client has requested to change the password on the account.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_AuthChangePassword
    '    ''' <summary>
    '    ''' The projection phase is starting.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_AuthProject
    '    ''' <summary>
    '    ''' The link-speed calculation phase is starting.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_AuthLinkSpeed
    '    ''' <summary>
    '    ''' An authentication request is being acknowledged.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_AuthAck
    '    ''' <summary>
    '    ''' Reauthentication (after callback) is starting.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_ReAuthenticate
    '    ''' <summary>
    '    ''' The client has successfully completed authentication.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_Authenticated
    '    ''' <summary>
    '    ''' The line is about to disconnect in preparation for callback.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_PrepareForCallback
    '    ''' <summary>
    '    ''' The client is delaying in order to give the modem time to reset itself in preparation for callback.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_WaitForModemReset
    '    ''' <summary>
    '    ''' The client is waiting for an incoming call from the remote access server.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_WaitForCallback
    '    ''' <summary>
    '    ''' This state occurs after the RASCS_AuthProject state. 
    '    ''' It indicates that projection result information is available. 
    '    ''' Access the projection result information by calling RasGetProjectionInfo.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_Projected
    '    ''' <summary>
    '    ''' When dialing a multilink phone-book entry, this state indicates that a subentry has been connected during the dialing process. 
    '    ''' The dwSubEntry parameter of a RasDialFunc2 callback function indicates the index of the subentry. 
    '    ''' When the final state of all subentries in the phone-book entry has been determined, the connection state is RASCS_Connected if one or more subentries have been connected successfully.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_SubEntryConnected
    '    ''' <summary>
    '    ''' When dialing a multilink phone-book entry, this state indicates that a subentry has been disconnected during the dialing process. 
    '    ''' The dwSubEntry parameter of a RasDialFunc2 callback function indicates the index of the subentry.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_SubEntryDisconnected
    '    ''' <summary>
    '    ''' When dialing a multilink phone-book entry, this state occurs just before a connection goes to the RASCS_SubEntryConnected state. 
    '    ''' Note  Supported in Windows 7 and later versions of Windows.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_ApplySettings
    '    ''' <summary>
    '    ''' This state has a value of RASCS_PAUSED and corresponds to the terminal state supported by RASPHONE.EXE.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_Interactive = RASCS_PAUSED
    '    ''' <summary>
    '    ''' This state corresponds to the retry authentication state supported by RASPHONE.EXE.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_RetryAuthentication
    '    ''' <summary>
    '    ''' This state corresponds to the callback state supported by RASPHONE.EXE.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_CallbackSetByCaller
    '    ''' <summary>
    '    ''' This state corresponds to the change password state supported by RASPHONE.EXE.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_PasswordExpired
    '    ''' <summary>
    '    ''' An application can use this paused state to bring up a custom authentication UI. The application should call the RasInvokeEapUI function to invoke the custom UI. 
    '    ''' RASCS_InvokeEapUI is a paused state.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_InvokeEapUI
    '    ''' <summary>
    '    ''' This state has a value of RASCS_DONE and corresponds to a successful connection.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_Connected = RASCS_DONE
    '    ''' <summary>
    '    ''' This state indicates a disconnected or failed connection.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    RASCS_Disconnected
    'End Enum

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

#End Region

#Region " Private variables "

    Private WithEvents m_COMPort As SerialPort
    Private m_ReceivedData As String = ""
    Private m_PortName As String = "COM1"
    Private m_PortBaudRate As Int16 = 9600
    Private m_PortParity As Parity = Parity.None
    Private m_PortDatabits As Int16 = 8
    Private m_PortStopBits As StopBits = StopBits.One
    Private m_PortHandshake As Handshake = Handshake.None
    Private m_PortRTSEnable As Boolean = True
    Private m_PortDTREnable As Boolean = True
    Private m_PauseTimeSeconds As Int16 = 40
    Private m_PauseTimer As New Stopwatch
    Private m_PerformResetUponDial As Boolean = True
    Private m_DialupMethod As Integer = 2 ' 1= INTERNET_AUTODIAL_FORCE_ONLINE, 2=INTERNET_AUTODIAL_FORCE_UNATTENDED
    Private m_InWaitState As Boolean = False
    Private m_ConnectionState As ModemStateEnum = ModemStateEnum.Disconnected
    Private m_LineState As LineStateEnum = LineStateEnum.Normal
    Private m_PhoneNumber As String = ""
    Private m_DialCommand As String = "ATDT"
    Private m_DialCommandPrefix As String = "AT&F&C1E0M1L3%C3"
    Private m_EOLCharacter As String = vbCrLf
    Private m_RawData As String = ""
    Private m_FormattedData As String = ""
    Private m_ReceivedByteCount As Int64 = 0
    Private m_HangupFlag As Boolean = False
    Private m_IdleTime As TimeSpan = TimeSpan.Zero

#End Region

#Region " Properties "

    Public Property COMPort() As SerialPort
        Get
            Return m_COMPort
        End Get
        Set(ByVal value As SerialPort)
            m_COMPort = value
        End Set
    End Property

    Public ReadOnly Property ReceivedData() As String
        Get
            Return m_ReceivedData
        End Get
    End Property

    Public Property PortName() As String
        Get
            Return m_PortName
        End Get
        Set(ByVal value As String)
            m_PortName = value
        End Set
    End Property

    Public Property PortBaudRate() As Int16
        Get
            Return m_PortBaudRate
        End Get
        Set(ByVal value As Int16)
            m_PortBaudRate = value
        End Set
    End Property

    Public Property PortParity() As Parity
        Get
            Return m_PortParity
        End Get
        Set(ByVal value As IO.Ports.Parity)
            m_PortParity = value
        End Set
    End Property

    Public Property PortDatabits() As Int16
        Get
            Return m_PortDatabits
        End Get
        Set(ByVal value As Int16)
            m_PortDatabits = value
        End Set
    End Property

    Public Property PortStopBits() As StopBits
        Get
            Return m_PortStopBits
        End Get
        Set(ByVal value As IO.Ports.StopBits)
            m_PortStopBits = value
        End Set
    End Property

    Public Property PortHandshake() As Handshake
        Get
            Return m_PortHandshake
        End Get
        Set(ByVal value As IO.Ports.Handshake)
            m_PortHandshake = value
        End Set
    End Property

    Public Property PortRTSEnable() As Boolean
        Get
            Return m_PortRTSEnable
        End Get
        Set(ByVal value As Boolean)
            m_PortRTSEnable = value
        End Set
    End Property

    Public Property PortDTREnable() As Boolean
        Get
            Return m_PortDTREnable
        End Get
        Set(ByVal value As Boolean)
            m_PortDTREnable = value
        End Set
    End Property

    Public Property PauseTimeSeconds() As Int16
        Get
            Return m_PauseTimeSeconds
        End Get
        Set(ByVal value As Int16)
            m_PauseTimeSeconds = value
        End Set
    End Property

    Public Property PerformResetUponDial() As Boolean
        Get
            Return m_PerformResetUponDial
        End Get
        Set(ByVal value As Boolean)
            m_PerformResetUponDial = value
        End Set
    End Property

    Public Property DialupMethod() As Integer
        Get
            Return m_DialupMethod
        End Get
        Set(ByVal value As Integer)
            m_DialupMethod = value
        End Set
    End Property

    Public ReadOnly Property ConnectionState() As ModemStateEnum
        Get
            Return m_ConnectionState
        End Get
    End Property

    Public ReadOnly Property LineState() As ModemStateEnum
        Get
            Return m_LineState
        End Get
    End Property

    Public Property DialCommand() As String
        Get
            Return m_DialCommand
        End Get
        Set(ByVal value As String)
            m_DialCommand = value
        End Set
    End Property

    Public Property DialCommandPrefix() As String
        Get
            Return m_DialCommandPrefix
        End Get
        Set(ByVal value As String)
            m_DialCommandPrefix = value
        End Set
    End Property

#End Region

#Region " Events "

    Public Event ReceivedDataEvent(ByVal Data As String, ByVal ReceivedByteCount As Int64)
    Public Event ErrorEvent(ByVal SerialError As System.IO.Ports.SerialErrorReceivedEventArgs)
    Public Event OpenSuccessEvent()
    Public Event OpenFailureEvent(ByVal ErrorMessage As String)
    Public Event ConnectSuccessEvent()
    Public Event ConnectFailureEvent(ByVal ErrorMessage As String)
    Public Event ResetFailureEvent(ByVal ErrorMessage As String)
    Public Event ModemNoCarrierEvent()
    Public Event ModemNoDialtoneEvent()
    Public Event ModemDialingEvent(ByVal Number As String)
    Public Event ModemBusyEvent()
    Public Event ModemNoAnswerEvent()
    Public Event ModemErrorEvent()
    Public Event ModemRingEvent()
    Public Event ModemConnectedEvent()
    Public Event ModemStateChangeEvent(ByVal NewEvent As String)

#End Region

#Region " Constructors "

    Public Sub New()
        'm_COMPort = New SerialPort
    End Sub

    Public Sub New(ByVal PortName As String)
        m_PortName = PortName
    End Sub

    Public Sub New(ByVal PortName As String, ByVal PortBaudRate As Int16)
        m_PortName = PortName
        m_PortBaudRate = PortBaudRate
    End Sub

    Public Sub New(ByVal PortName As String, ByVal PortBaudRate As Int16, ByVal PortParity As IO.Ports.Parity)
        m_PortName = PortName
        m_PortBaudRate = PortBaudRate
        m_PortParity = PortParity
    End Sub

    Public Sub New(ByVal PortName As String, ByVal PortBaudRate As Int16, ByVal PortParity As IO.Ports.Parity, ByVal PortDataBits As Int16)
        m_PortName = PortName
        m_PortBaudRate = PortBaudRate
        m_PortParity = PortParity
        m_PortDatabits = PortDataBits
    End Sub

    Public Sub New(ByVal PortName As String, ByVal PortBaudRate As Int16, ByVal PortParity As IO.Ports.Parity, ByVal PortDataBits As Int16, ByVal PortStopBits As IO.Ports.StopBits)
        m_PortName = PortName
        m_PortBaudRate = PortBaudRate
        m_PortParity = PortParity
        m_PortDatabits = PortDataBits
        m_PortStopBits = PortStopBits
    End Sub

    Public Sub New(ByVal PortName As String, ByVal PortBaudRate As Int16, ByVal PortParity As IO.Ports.Parity, ByVal PortDataBits As Int16, ByVal PortStopBits As IO.Ports.StopBits, ByVal PortHandshake As IO.Ports.Handshake)
        m_PortName = PortName
        m_PortBaudRate = PortBaudRate
        m_PortParity = PortParity
        m_PortDatabits = PortDataBits
        m_PortStopBits = PortStopBits
        m_PortHandshake = PortHandshake
    End Sub

    Public Sub New(ByVal PortName As String, ByVal PortBaudRate As Int16, ByVal PortParity As IO.Ports.Parity, ByVal PortDataBits As Int16, ByVal PortStopBits As IO.Ports.StopBits, ByVal PortHandshake As IO.Ports.Handshake, ByVal PortRTSEnable As Boolean)
        m_PortName = PortName
        m_PortBaudRate = PortBaudRate
        m_PortParity = PortParity
        m_PortDatabits = PortDataBits
        m_PortStopBits = PortStopBits
        m_PortHandshake = PortHandshake
        m_PortRTSEnable = PortRTSEnable
    End Sub


#End Region

#Region " Public Methods "

    <MTAThread()> _
    Public Function Dial(ByVal Number As String)
        m_PhoneNumber = Number

        Return StartDialling()
    End Function

    Public Sub Hangup()
        m_HangupFlag = True
        DoHangUp()
    End Sub

    'Public Sub StartDUN(ByVal Name As String)

    'End Sub

    'Public Function StartDialupNetworking() As Boolean
    '    StartDialupNetworking(m_DialupMethod)
    'End Function

    'Public Function StartDialupNetworking(ByVal Method As DialupMethods) As Boolean
    '    Dim Result As Boolean = False

    '    Try
    '        Result = InternetAutodial(Method, 0&)
    '    Catch ex As Exception

    '    End Try

    '    Return Result
    'End Function

    Private Sub Pause(ByVal mS As Long)
        Dim Watch As New Stopwatch

        Watch.Start()
        Do
            Application.DoEvents()
        Loop Until watch.ElapsedMilliseconds >= mS

    End Sub

#End Region

#Region " Private Methods "

    Private Function StartDialling() As Thread
        m_HangupFlag = False

        Try
            Dim Worker As New Thread(AddressOf ThreadWork)

            Worker.Start()
            Return Worker
        Catch ex As Exception
            m_HangupFlag = True
            Return Nothing
        End Try

    End Function

    Private Sub ThreadWork()
        Dim PortOrModemError As Boolean = False

        Try
            m_COMPort = New SerialPort
            Do
                m_ReceivedByteCount = 0
                m_PauseTimer.Reset()

                m_ConnectionState = ModemStateEnum.Disconnected
                m_LineState = LineStateEnum.Normal
                m_InWaitState = False
                m_FormattedData = ""
                m_RawData = ""

                Open()
                Reset()
                Close()

                'If m_COMPort.IsOpen Then
                '    If m_PerformResetUponDial Then Reset()
                '    m_COMPort.Close()
                'End If

                m_COMPort.PortName = m_PortName
                m_COMPort.BaudRate = m_PortBaudRate
                m_COMPort.Parity = m_PortParity
                m_COMPort.DataBits = m_PortDatabits
                m_COMPort.StopBits = m_PortStopBits
                m_COMPort.Handshake = m_PortHandshake
                m_COMPort.RtsEnable = m_PortRTSEnable
                m_COMPort.DtrEnable = m_PortDTREnable
                m_COMPort.Open()

                m_ConnectionState = ModemStateEnum.Connecting
                RaiseEvent ModemStateChangeEvent("Connecting")
                m_InWaitState = True
                m_COMPort.Write(m_DialCommandPrefix & ControlChars.Cr)

                Pause(250)

                m_InWaitState = False
                m_COMPort.Write("ATS7=" & m_PauseTimeSeconds.ToString & ControlChars.Cr)

                m_ConnectionState = ModemStateEnum.Dialling
                RaiseEvent ModemStateChangeEvent("Dialling")
                Pause(250) ' There seems to be a pause required right now

                m_COMPort.Write(m_DialCommand & m_PhoneNumber & ControlChars.Cr) 'dial
                m_PauseTimer.Start()
                Do While Not m_COMPort.CDHolding AndAlso m_PauseTimer.ElapsedMilliseconds < ((m_PauseTimeSeconds) * 1000)
                    Application.DoEvents()
                Loop

                If Not m_COMPort.CDHolding Then
                    m_ConnectionState = ModemStateEnum.Disconnected
                    RaiseEvent ModemStateChangeEvent("Disconnected")
                    Select Case m_FormattedData.ToUpper
                        Case "BUSY"
                            RaiseEvent ModemBusyEvent()
                            PortOrModemError = True
                        Case "NO CARRIER"
                            RaiseEvent ModemNoCarrierEvent()
                            PortOrModemError = True
                        Case "NO DIALTONE", "NO DIAL TONE"
                            RaiseEvent ModemNoDialtoneEvent()
                            PortOrModemError = True
                        Case "NO ANSWER"
                            RaiseEvent ModemNoAnswerEvent()
                            PortOrModemError = True
                        Case ""

                            'Case Else
                            '    RaiseEvent ConnectFailureEvent(m_FormattedData)
                            '    PortOrModemError = True
                    End Select
                    m_COMPort.Close()
                Else
                    ' Connected
                    m_ConnectionState = ModemStateEnum.Connected
                    RaiseEvent ModemStateChangeEvent("Connected")
                    RaiseEvent ConnectSuccessEvent()
                    m_IdleTime = TimeSpan.Zero

                    'm_IdleTime.
                End If
                Application.DoEvents()
            Loop Until m_HangupFlag Or PortOrModemError
        Catch ex As Exception
            m_ConnectionState = ModemStateEnum.Disconnected
            RaiseEvent ModemStateChangeEvent("Disconnected")
            RaiseEvent ConnectFailureEvent(ex.Message)
        End Try
        DoHangUp()
    End Sub

    Private Sub GetData()
        Dim Buffer As String = ""

        If m_COMPort.IsOpen Then
            While m_COMPort.BytesToRead > 0
                Buffer &= m_COMPort.ReadExisting
            End While
            m_ReceivedByteCount += Buffer.Length

            If m_InWaitState Then
                m_InWaitState = False
            Else
                ProcessData(Buffer) ' extra

                If m_FormattedData.Length > 0 Then ' Buffer
                    Select Case m_FormattedData.Trim ' Buffer
                        Case "NO DIALTONE", "NO DIAL TONE"
                            m_ConnectionState = ModemStateEnum.Disconnected
                            RaiseEvent ModemStateChangeEvent("Disconnected")
                            m_LineState = LineStateEnum.NoDialtone
                            RaiseEvent ModemNoDialtoneEvent()
                        Case "NO CARRIER"
                            m_ConnectionState = ModemStateEnum.Disconnected
                            RaiseEvent ModemStateChangeEvent("Disconnected")
                            m_LineState = LineStateEnum.NoCarrier
                            RaiseEvent ModemNoCarrierEvent()
                        Case "BUSY"
                            m_ConnectionState = ModemStateEnum.Disconnected
                            RaiseEvent ModemStateChangeEvent("Disconnected")
                            m_LineState = LineStateEnum.Busy
                            RaiseEvent ModemBusyEvent()
                        Case "NO ANSWER"
                            m_ConnectionState = ModemStateEnum.Disconnected
                            RaiseEvent ModemStateChangeEvent("Disconnected")
                            m_LineState = LineStateEnum.NoAnswer
                            RaiseEvent ModemNoAnswerEvent()
                        Case "RING"
                            m_ConnectionState = ModemStateEnum.Connecting
                            RaiseEvent ModemStateChangeEvent("Connecting")
                            m_LineState = LineStateEnum.Normal
                            RaiseEvent ModemRingEvent()
                        Case "OK"
                            m_ConnectionState = ModemStateEnum.Unknown
                            RaiseEvent ModemStateChangeEvent("Unknown")
                            m_LineState = LineStateEnum.Unknown
                            RaiseEvent ReceivedDataEvent(m_FormattedData, m_ReceivedByteCount)
                        Case "ERROR"
                            m_ConnectionState = ModemStateEnum.Disconnected
                            RaiseEvent ModemStateChangeEvent("Disconnected")
                            m_LineState = LineStateEnum.Unknown
                            RaiseEvent ModemErrorEvent()
                        Case Else
                            If m_FormattedData.Contains("CONNECT") Then   ' Buffer
                                m_ConnectionState = ModemStateEnum.Connected
                                RaiseEvent ModemStateChangeEvent("Connected")
                                m_LineState = LineStateEnum.Normal
                                RaiseEvent ModemConnectedEvent()
                            Else
                                'm_ConnectionState = ModemStateEnum.Unknown
                                'RaiseEvent ModemStateChangeEvent("Unknown")
                                'm_LineState = LineStateEnum.Normal
                                'If m_FormattedData.Length > 0 Then
                                '    RaiseEvent ReceivedDataEvent(m_FormattedData, m_ReceivedByteCount)
                                'End If

                            End If
                    End Select
                End If
            End If
        End If
        Application.DoEvents()
    End Sub

    Private Sub ProcessData(ByVal Data As String)
        m_RawData &= Data
        While m_RawData.Contains(m_EOLCharacter)
            Dim Buffer As String()
            Buffer = m_RawData.Split(m_EOLCharacter)

            ' Go through each line of data
            For i As Int64 = 0 To Buffer.LongCount - 2
                m_FormattedData = Buffer(i)
            Next

            ' Check if the data ended cleanly at a EOL
            If Data.EndsWith(Buffer(Buffer.LongCount - 1)) Then
                RaiseEvent ReceivedDataEvent(m_FormattedData, m_ReceivedByteCount)
                If m_FormattedData.StartsWith("NO CARRIER") Then
                    m_ConnectionState = ModemStateEnum.Disconnected
                    RaiseEvent ModemStateChangeEvent("Disconnected")
                End If
                m_RawData = ""
            Else
                m_RawData = m_FormattedData
            End If
        End While
    End Sub

    Private Sub Open()
        If Not m_COMPort.IsOpen Then
            m_COMPort.DataBits = m_PortDatabits
            m_COMPort.Handshake = m_PortHandshake
            m_COMPort.Parity = m_PortParity
            m_COMPort.PortName = m_PortName
            m_COMPort.StopBits = m_PortStopBits
            m_COMPort.BaudRate = m_PortBaudRate
            m_COMPort.RtsEnable = m_PortRTSEnable
            m_COMPort.DtrEnable = m_PortDTREnable
            Try
                m_COMPort.Open()
                RaiseEvent OpenSuccessEvent()
                GetData()
            Catch ex As Exception
                RaiseEvent OpenFailureEvent(ex.Message)
            End Try
        End If
    End Sub

    Private Sub Close()
        If m_COMPort.IsOpen Then m_COMPort.Close()
        m_ReceivedData = ""
    End Sub

    Private Sub Reset()
        Dim ResetCharacter() As Byte = {193}

        Try
            If Not m_COMPort.IsOpen Then m_COMPort.Open()
            m_COMPort.Write(ResetCharacter, 0, 1)
        Catch ex As Exception
            RaiseEvent ResetFailureEvent(ex.Message)
        End Try
    End Sub

    Private Sub DoHangUp()
        'To disconnect an automatically dialled connection
        'InternetAutodialHangup(0&)

        ' To disconnect a modem manually
        m_COMPort.DtrEnable = False
        m_ConnectionState = ModemStateEnum.Disconnected
        RaiseEvent ModemStateChangeEvent("Disconnected")
        m_LineState = LineStateEnum.Normal
    End Sub

#End Region

#Region " Event Handlers "

    Private Sub m_SerialPort_ErrorReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialErrorReceivedEventArgs) Handles m_COMPort.ErrorReceived
        If m_InWaitState Then
            m_InWaitState = False
        Else
            RaiseEvent ErrorEvent(e)
        End If
    End Sub

    Private Sub m_SerialPort_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles m_COMPort.DataReceived
        GetData()
    End Sub

#End Region

End Class
