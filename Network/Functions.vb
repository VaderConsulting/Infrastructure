#Region " Imports "

Imports System.Net.Sockets
Imports System.Net
Imports System.Text.ASCIIEncoding
Imports System.ComponentModel

#End Region

#Region " Example Usage "

'    Dim IPAddress As String = "127.0.0.1"
'    Dim Port As Int32 = 10
'    Dim Protocol As Net.Sockets.ProtocolType = Net.Sockets.ProtocolType.Tcp
'    
'    Core.Network.Functions.StartServer(IPAddress, Port, Protocol)
'
'    Console.WriteLine("TCP: " & Core.Network.Functions.CanConnectTCP(IPAddress, Port))

#End Region

Public Class Functions

#Region " Private variables "

    Private Shared _Server As Socket = Nothing
    Private Shared _Client As Socket = Nothing
    Private Shared _Message As Socket = Nothing
    Private Shared _ReceiveBuffer() As Byte = {}
    Private Shared _ServerMessage() As Byte = ASCII.GetBytes("Connection busy")
    Private Shared _Connecting As Boolean = False
    Private Shared _Connected As Boolean = False
    Private Shared _IPAddress As String = ""
    Private Shared _Port As Int32 = 80
    Private Shared WithEvents _Worker As New BackgroundWorker

#End Region

#Region " Properties "

    Public Shared Property Connecting() As Boolean
        Get
            Return _Connecting
        End Get
        Set(ByVal value As Boolean)
            _Connecting = value
        End Set
    End Property

    Public Shared Property Connected() As Boolean
        Get
            Return _Connected
        End Get
        Set(ByVal value As Boolean)
            _Connected = value
        End Set
    End Property

#End Region

#Region " Public methods "

    ''' <summary>
    ''' Attempt to connect over TCP to the given IP Address and Port
    ''' </summary>
    ''' <param name="IPAddress">The IP Address to connect to</param>
    ''' <param name="Port">The Port to connect to</param>
    ''' <returns>Boolean (True indicates connection was successful)</returns>
    ''' <remarks></remarks>
    Public Shared Sub Connect(ByVal IPAddress As String, ByVal Port As Int32)
        _Connecting = True

        _IPAddress = IPAddress
        _Port = Port

        _Worker.RunWorkerAsync()

        _Connected = Connect(IPAddress, Port, ProtocolType.Tcp)
        _Connecting = False
    End Sub

    ''' <summary>
    ''' Start a simple TCP Server to act as a simple listener that provides boolean connection results
    ''' </summary>
    ''' <param name="ListenIPAddress">The IP Address to listen to</param>
    ''' <param name="Port">The Port to listen to</param>
    ''' <param name="Protocol">The protocol to use</param>
    ''' <returns>Boolean (True indicates the server has started successfully)</returns>
    ''' <remarks>Raises one of two events:
    ''' Server_Started() 
    ''' or 
    ''' Server_Error(ByVal Message As String)</remarks>
    Public Shared Function StartServer(ByVal ListenIPAddress As String, ByVal Port As Int32, ByVal Protocol As ProtocolType) As Boolean
        Dim Address As IPAddress = IPAddress.Parse(ListenIPAddress)
        Dim EndPoint As New IPEndPoint(Address, Port)
        Dim BacklogLength As Int32 = 10 ' Number of clients that can be queued

        Console.WriteLine("Starting Server...")

        If Not _Server Is Nothing Then
            _Server.Close()
            _Server.Disconnect(True)
            _Server = Nothing
        End If

        _Server = New Socket(AddressFamily.InterNetwork, SocketType.Stream, Protocol)

        Try
            _Server.Bind(EndPoint)
            _Server.Listen(BacklogLength)
            _Server.BeginAccept(AddressOf AcceptCallback, _Server)
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

    ''' <summary>
    ''' Stop the TCP Server
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub StopServer()
        Console.WriteLine("Stopping Server...")
        If Not _Client Is Nothing Then
            _Client.Shutdown(SocketShutdown.Both)
            _Client.Disconnect(True)
            _Client.Close()
        End If
        If Not _Server Is Nothing Then
            _Server.Shutdown(SocketShutdown.Both)
            _Server.Disconnect(True)
            _Server.Close()
        End If

        _Connected = False

    End Sub

#End Region

#Region " Private methods "

    Private Shared Function Connect(ByVal IPAddress As String, ByVal Port As Int32, ByVal Protocol As ProtocolType) As Boolean
        Dim Success As Boolean = False
        Dim Socket As Net.Sockets.Socket = Nothing

        ' Synchronous
        Socket = New Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, Protocol)
        Try
            Console.Write("Attempting connection to " & IPAddress & ":" & Port.ToString & "...")
            Socket.Connect(IPAddress, Port)
            Socket.Disconnect(True)
            Socket.Close()
            Socket = Nothing
            Console.WriteLine("Connected")
            Return True
        Catch ex As SocketException
            Console.WriteLine("No connection")
            Return False
        Catch ex As Exception
            Console.WriteLine("Error!")
            Throw New Exception("Unexpected error during socket connection attempt:" & ex.ToString)
        End Try

    End Function

    Private Shared Sub AcceptCallback(ByVal Result As IAsyncResult)
        System.Threading.Thread.CurrentThread.Name = "Connected"

        Console.WriteLine("Inside AcceptCallback - a client has connected")

        _Connected = True

        Try
            If _Client Is Nothing Then
                _Client = _Server.EndAccept(Result)
                _Client.BeginReceive(_ReceiveBuffer, 0, _ReceiveBuffer.Length, SocketFlags.None, AddressOf ReceiveCallback, _ReceiveBuffer)
            Else
                _Message = _Server.EndAccept(Result)

                _Message.Send(_ServerMessage)

                _Message.Shutdown(SocketShutdown.Both)
                _Message.Close()
                _Message = Nothing
            End If
            _Server.BeginAccept(AddressOf AcceptCallback, _Server)
        Catch ex As Exception
            Console.WriteLine("Error in AcceptCallback: " & ex.ToString)
        End Try
    End Sub

    Private Shared Sub ReceiveCallback(ByVal Result As IAsyncResult)
        Dim ByteCount As Int32 = 0

        Console.WriteLine("Inside ReceiveCallback")

        System.Threading.Thread.CurrentThread.Name = "Received data"
        _ReceiveBuffer = CType(Result.AsyncState, Byte())

        Try
            ByteCount = _Client.EndReceive(Result)
        Catch
            Console.WriteLine("Leaving ReceiveCallback (Error)")
            Exit Sub
        End Try

        If ByteCount = 0 Then
            Console.WriteLine("Leaving ReceiveCallback (No data)")
            Exit Sub
        End If

        Console.WriteLine("...Processing data")
        ProcessIncomingData(ByteCount)

        Console.WriteLine("...Setting client to receive data")
        _Client.BeginReceive(_ReceiveBuffer, 0, _ReceiveBuffer.Length, SocketFlags.None, AddressOf ReceiveCallback, _ReceiveBuffer)
        Console.WriteLine("Leaving ReceiveCallback")
    End Sub

    Private Shared Sub ProcessIncomingData(ByVal ByteCount As Int32)
        Dim Data As String = ""

        Console.WriteLine("Inside ProcessIncomingData")

        For i As Integer = 0 To ByteCount - 1
            Data &= Chr(_ReceiveBuffer(i))
        Next
        Console.WriteLine("Leaving ProcessIncomingData")
    End Sub

#End Region

#Region " Finalize "

    Protected Overrides Sub Finalize()

        Console.WriteLine("Finalising Server")

        If Not _Client Is Nothing Then
            _Client.Close()
            'RaiseEvent Server_Disconnected()
        End If

        If Not _Server Is Nothing Then
            _Server.Close()
            'RaiseEvent Server_Finalised()
        End If

        _Client = Nothing
        _Server = Nothing

        _Connected = False

        MyBase.Finalize()
    End Sub

#End Region

    Private Shared Sub _Worker_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles _Worker.DoWork
        Console.WriteLine("Connecting ASync")
        Connect(_IPAddress, _Port, ProtocolType.Tcp)
    End Sub

    Private Shared Sub _Worker_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles _Worker.RunWorkerCompleted
        Console.WriteLine("Connection attempt complete")
    End Sub
End Class
