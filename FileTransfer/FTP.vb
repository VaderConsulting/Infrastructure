#Region " Imports "

Imports System
Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Net.Sockets
Imports Core.FileTransfer
Imports Core.Compression.Zip

#End Region

Public Class FTP : Implements IDisposable
    Private Const BLOCK_SIZE As Integer = 512

#Region " Enums "

    Public Enum TransferMode
        Binary = 1
        Ascii = 2
    End Enum

    Public Enum ResumeMode
        [Off] = 0
        [On] = 1
    End Enum

#End Region

#Region " Private Variables "

    Private m_RemoteHost As String = ""
    Private m_RemotePort As Integer = 21
    Private m_RemotePath As String = ""
    Private m_RemoteUsername As String = ""
    Private m_RemotePassword As String = ""
    Private m_DebugMode As Boolean = False

    Private m_Message As String = ""
    Private m_BytesTransferred As Integer = 0
    Private m_ClientSocket As Socket = Nothing
    Private m_ReturnValue As Integer = 0
    Private m_LoggedIn As Boolean = False
    Private m_Reply As String = ""
    Private m_Buffer As Byte() = New Byte(BLOCK_SIZE - 1) {}
    Private m_ASCII As Encoding = Encoding.ASCII
    Private m_UseCompression As Boolean = True
    
#End Region

#Region " Properties "

    ''' <summary>
    ''' Name of the FTP server to connect to
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RemoteHost() As String
        Get
            Return m_RemoteHost
        End Get
        Set(ByVal value As String)
            m_RemoteHost = value
        End Set
    End Property

    ''' <summary>
    ''' Port number to use for FTP
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RemotePort() As Integer
        Get
            Return m_RemotePort
        End Get
        Set(ByVal value As Integer)
            m_RemotePort = value
        End Set
    End Property

    ''' <summary>
    ''' Remote directory path
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RemotePath() As String
        Get
            Return m_RemotePath
        End Get
        Set(ByVal value As String)
            m_RemotePath = value.Replace("\", "/")
        End Set
    End Property

    ''' <summary>
    ''' User name to use for logging into the remote server
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RemoteUsername() As String
        Get
            Return m_RemoteUsername
        End Get
        Set(ByVal value As String)
            m_RemoteUsername = value
        End Set
    End Property

    ''' <summary>
    ''' Password to user for logging into the remote server
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RemotePassword() As String
        Get
            Return m_RemotePassword
        End Get
        Set(ByVal value As String)
            m_RemotePassword = value
        End Set
    End Property

    ''' <summary>
    ''' Debug mode
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DebugMode() As Boolean
        Get
            Return m_DebugMode
        End Get
        Set(ByVal value As Boolean)
            m_DebugMode = value
        End Set
    End Property

    ''' <summary>
    ''' Compression
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UseCompression() As Boolean
        Get
            Return m_UseCompression
        End Get
        Set(ByVal value As Boolean)
            m_UseCompression = value
        End Set
    End Property

#End Region

#Region " Public Methods "

    ''' <summary>
    ''' Return a string array containing the remote directory's file list
    ''' </summary>
    ''' <param name="mask"></param>
    ''' <returns>String array</returns>
    ''' <remarks></remarks>
    Public Function GetFileList(ByVal Mask As String) As String()
        If Not m_LoggedIn Then
            Login()
        End If

        Dim cSocket As Socket = CreateDataSocket()

        SendCommand("NLST " & Mask)
        If Not (m_ReturnValue = 150 OrElse m_ReturnValue = 125) Then
            Throw New IOException(m_Reply.Substring(4))
        End If

        m_Message = ""
        While True
            Dim bytes As Integer = cSocket.Receive(m_Buffer, m_Buffer.Length, 0)
            m_Message += m_ASCII.GetString(m_Buffer, 0, bytes)
            If bytes < m_Buffer.Length Then
                Exit While
            End If
        End While

        Dim Separator As Char() = {ControlChars.Lf}
        Dim Message As String() = m_Message.Split(Separator)

        cSocket.Close()
        ReadReply()
        If m_ReturnValue <> 226 Then
            Throw New IOException(m_Reply.Substring(4))
        End If

        Return Message
    End Function

    ''' <summary>
    ''' Return the size of a file
    ''' </summary>
    ''' <param name="fileName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFileSize(ByVal FileName As String) As Long
        If Not m_LoggedIn Then
            Login()
        End If

        Dim fi As New FileInfo(FileName)

        SendCommand("SIZE " & fi.Name)
        Dim size As Long = 0

        If m_ReturnValue = 213 Then
            size = Int64.Parse(m_Reply.Substring(4))
        Else
            Trace.WriteLine("GetFileSize failed: " & m_Reply.Substring(4).Replace(vbCr, ""))
            'Throw New IOException(m_Reply.Substring(4))
        End If

        Return size
    End Function

    ''' <summary>
    ''' Login to the remote server
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Login()
        m_ClientSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)

        Dim ep As New IPEndPoint(Dns.GetHostEntry(m_RemoteHost).AddressList(0), m_RemotePort)

        Try
            m_ClientSocket.Connect(ep)
        Catch generatedExceptionName As Exception
            Debug.Print("Login failed (" & generatedExceptionName.Message & ")")
            Throw New IOException("Couldn't connect to remote server")
        End Try

        ReadReply()
        If m_ReturnValue <> 220 Then
            Close()
            Throw New IOException(m_Reply.Substring(4))
        End If

        Debug.Print("USER " & m_RemoteUsername)

        SendCommand("USER " & m_RemoteUsername)
        If Not (m_ReturnValue = 331 OrElse m_ReturnValue = 230) Then
            Cleanup()
            Throw New IOException(m_Reply.Substring(4))
        End If

        If m_ReturnValue <> 230 Then
            Debug.Print("PASS xxx")
            SendCommand("PASS " & m_RemotePassword)
            If Not (m_ReturnValue = 230 OrElse m_ReturnValue = 202) Then
                Cleanup()
                Throw New IOException(m_Reply.Substring(4))
            End If
        End If

        ' Adjust the remote path to ensure it fits the URI laws
        m_RemotePath = m_RemotePath.Replace("\", "/")

        m_LoggedIn = True
        Debug.Print("Connected to " & m_RemoteHost)
        Try
            ChangeRemoteDirectory(m_RemotePath)
        Catch
            CreateRemoteDirectory(m_RemotePath)
            ChangeRemoteDirectory(m_RemotePath)
        End Try
    End Sub

    ''' <summary>
    ''' Set binary mode for downloads, otherwise, set Ascii mode
    ''' </summary>
    ''' <param name="mode"></param>
    ''' <remarks></remarks>
    Public Sub SetMode(ByVal Mode As TransferMode)
        Select Case Mode
            Case TransferMode.Ascii
                SendCommand("TYPE A")
            Case TransferMode.Binary
                SendCommand("TYPE I")
        End Select

        If m_ReturnValue <> 200 Then
            Throw New IOException(m_Reply.Substring(4))
        End If
    End Sub

    ''' <summary>
    ''' Download a file to the Assembly's local directory, keeping the same file name
    ''' </summary>
    ''' <param name="RemoteFilename"></param>
    ''' <remarks></remarks>
    Public Sub Download(ByVal RemoteFilename As String)
        Download(RemoteFilename, "", ResumeMode.Off)
    End Sub

    ''' <summary>
    ''' Download a remote file to the Assembly's local directory, keeping the same file name, and set the resume flag
    ''' </summary>
    ''' <param name="RemoteFilename"></param>
    ''' <param name="Resume"></param>
    ''' <remarks></remarks>
    Public Sub Download(ByVal RemoteFilename As String, ByVal [Resume] As ResumeMode)
        Download(RemoteFilename, "", [Resume])
    End Sub

    ''' <summary>
    ''' Download a remote file to a local file name which can include a path. The local file name will be created or overwritten, but the path must exist.
    ''' </summary>
    ''' <param name="RemoteFilename"></param>
    ''' <param name="LocalFilename"></param>
    ''' <remarks></remarks>
    Public Sub Download(ByVal RemoteFilename As String, ByVal LocalFilename As String)
        Download(RemoteFilename, LocalFilename, ResumeMode.Off)
    End Sub

    ''' <summary>
    ''' Download a remote file to a local file name which can include a path, and set the resume flag. The local file name will be created or overwritten, but the path must exist.
    ''' </summary>
    ''' <param name="RemoteFilename"></param>
    ''' <param name="LocalFilename"></param>
    ''' <param name="Resume"></param>
    ''' <remarks></remarks>
    Public Sub Download(ByVal RemoteFilename As String, ByVal LocalFilename As String, ByVal [Resume] As ResumeMode)
        If Not m_LoggedIn Then
            Login()
        End If

        SetMode(TransferMode.Binary)
        Trace.WriteLine((("Downloading file " & RemoteFilename & " from ") + m_RemoteHost & "/") + m_RemotePath)

        If LocalFilename.Length = 0 Then
            LocalFilename = RemoteFilename
        End If

        If Not File.Exists(LocalFilename) Then
            Dim st As Stream = File.Create(LocalFilename)
            st.Close()
        End If

        Dim Output As New FileStream(LocalFilename, FileMode.Open)
        Dim cSocket As Socket = CreateDataSocket()
        Dim Offset As Long = 0

        If [Resume] = ResumeMode.On Then
            Offset = Output.Length
            If Offset > 0 Then
                SendCommand("REST " & Offset)
                If m_ReturnValue <> 350 Then
                    'throw new IOException(reply.Substring(4));
                    'Some servers may not support resuming.
                    Offset = 0
                End If
            End If
            If Offset > 0 Then
                If m_DebugMode Then
                    Trace.WriteLine("seeking to " & Offset)
                End If
                Dim npos As Long = Output.Seek(Offset, SeekOrigin.Begin)
                Trace.WriteLine("new pos=" & npos)
            End If
        End If

        SendCommand("RETR " & RemoteFilename)
        If Not (m_ReturnValue = 150 OrElse m_ReturnValue = 125) Then
            Throw New IOException(m_Reply.Substring(4))
        End If

        While True
            m_BytesTransferred = cSocket.Receive(m_Buffer, m_Buffer.Length, 0)
            Output.Write(m_Buffer, 0, m_BytesTransferred)
            If m_BytesTransferred <= 0 Then
                Exit While
            End If
        End While

        Output.Close()
        If cSocket.Connected Then
            cSocket.Close()
        End If

        Trace.WriteLine("")
        ReadReply()

        If Not (m_ReturnValue = 226 OrElse m_ReturnValue = 250) Then
            Throw New IOException(m_Reply.Substring(4))
        Else
            If m_UseCompression Then DecompressFile(LocalFilename)
        End If
    End Sub

    ''' <summary>
    ''' Upload a file with Resume set to off
    ''' </summary>
    ''' <param name="fileName"></param>
    ''' <remarks></remarks>
    Public Sub Upload(ByVal Filename As String)
        Upload(Filename, "", False)
    End Sub

    ''' <summary>
    ''' Upload a file to the given filename with Resume set to off
    ''' </summary>
    ''' <param name="fileName"></param>
    ''' <remarks></remarks>
    Public Sub Upload(ByVal Filename As String, ByVal DestinationFilename As String)
        Upload(Filename, DestinationFilename, False)
    End Sub

    ''' <summary>
    ''' Upload a file to the given filename and set the resume option
    ''' </summary>
    ''' <param name="fileName"></param>
    ''' <param name="resume"></param>
    ''' <remarks></remarks>
    Public Sub Upload(ByVal Filename As String, ByVal DestinationFilename As String, ByVal [Resume] As ResumeMode)
        If Not m_LoggedIn Then
            Login()
        End If

        If m_UseCompression Then
            CompressFile(Filename, DestinationFilename, True, False)
        Else
            DestinationFilename = Filename
        End If

        Dim FileToUpload As String = DestinationFilename
        Dim DataSocket As Socket = CreateDataSocket()
        Dim Offset As Long = 0

        If [Resume] = ResumeMode.On Then
            Try
                SetMode(TransferMode.Binary)
                Try
                    Offset = GetFileSize(FileToUpload)
                Catch ex As Exception
                    ' Ignore
                    Trace.WriteLine("Error retrieving Filesize of " & FileToUpload & ".  Resume now set to 'Off'.")
                    [Resume] = ResumeMode.Off
                    Offset = 0
                End Try
            Catch generatedExceptionName As Exception
                Offset = 0
            End Try
        End If

        If Offset > 0 Then
            SendCommand("REST " & Offset)
            If m_ReturnValue <> 350 Then
                'throw new IOException(reply.Substring(4));
                'Remote server may not support resuming.
                Trace.WriteLine("Remote Server does not support resume.")
                Offset = 0
            End If
        End If

        SendCommand("STOR " & Path.GetFileName(FileToUpload))
        If Not (m_ReturnValue = 125 OrElse m_ReturnValue = 150) Then
            Throw New IOException(m_Reply.Substring(4))
        End If

        ' open input stream to read source file
        Dim Input As New FileStream(FileToUpload, FileMode.Open)

        If Offset <> 0 Then
            If m_DebugMode Then
                Trace.WriteLine("seeking to " & Offset)
            End If
            Input.Seek(Offset, SeekOrigin.Begin)
        End If

        Trace.WriteLine(("Uploading file " & FileToUpload & " to ") + m_RemotePath)
        While (InlineAssignHelper(m_BytesTransferred, Input.Read(m_Buffer, 0, m_Buffer.Length))) > 0
            DataSocket.Send(m_Buffer, m_BytesTransferred, 0)
        End While

        Input.Close()
        Trace.WriteLine("")
        If DataSocket.Connected Then
            DataSocket.Close()
        End If
        ReadReply()
        If Not (m_ReturnValue = 226 OrElse m_ReturnValue = 250) Then
            Throw New IOException(m_Reply.Substring(4))
        End If
    End Sub

    ''' <summary>
    ''' Delete a file from the remote FTP server
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <remarks></remarks>
    Public Sub DeleteRemoteFile(ByVal Filename As String)
        If Not m_LoggedIn Then
            Login()
        End If
        SendCommand("DELE " & Filename)
        If m_ReturnValue <> 250 Then
            Throw New IOException(m_Reply.Substring(4))
        End If
    End Sub

    ''' <summary>
    ''' Rename a file on the remote FTP server
    ''' </summary>
    ''' <param name="OldFilename"></param>
    ''' <param name="NewFilename"></param>
    ''' <remarks></remarks>
    Public Sub RenameRemoteFile(ByVal OldFilename As String, ByVal NewFilename As String)
        If Not m_LoggedIn Then
            Login()
        End If
        SendCommand("RNFR " & OldFilename)
        If m_ReturnValue <> 350 Then
            Throw New IOException(m_Reply.Substring(4))
        End If
        ' known problem
        ' rnto will not take care of existing file.
        ' i.e. It will overwrite if newFileName exist
        SendCommand("RNTO " & NewFilename)
        If m_ReturnValue <> 250 Then
            Throw New IOException(m_Reply.Substring(4))
        End If
    End Sub

    ''' <summary>
    ''' Create a directory on the remote FTP server
    ''' </summary>
    ''' <param name="DirectoryName"></param>
    ''' <remarks></remarks>
    Public Sub CreateRemoteDirectory(ByVal DirectoryName As String)
        If Not m_LoggedIn Then
            Login()
        End If
        SendCommand("MKD " & DirectoryName)
        If m_ReturnValue > 400 Then
            Trace.WriteLine("MKD failed (" & m_Reply.Substring(4) & ")")
            Throw New IOException(m_Reply.Substring(4))
        End If
    End Sub

    ''' <summary>
    ''' Delete a directory on the remote FTP server
    ''' </summary>
    ''' <param name="DirectoryName"></param>
    ''' <remarks></remarks>
    Public Sub DeleteRemoteDirectory(ByVal DirectoryName As String)
        If Not m_LoggedIn Then
            Login()
        End If
        SendCommand("RMD " & DirectoryName)
        If m_ReturnValue <> 250 Then
            Throw New IOException(m_Reply.Substring(4))
        End If
    End Sub

    ''' <summary>
    ''' Change the current working directory on the remote FTP server
    ''' </summary>
    ''' <param name="DirectoryName"></param>
    ''' <remarks></remarks>
    Public Sub ChangeRemoteDirectory(ByVal DirectoryName As String)
        If DirectoryName.Equals(".") Then
            Exit Sub
        End If
        If Not m_LoggedIn Then
            Login()
        End If
        SendCommand("CWD " & DirectoryName)
        If m_ReturnValue <> 250 Then
            Throw New IOException(m_Reply.Substring(4))
        End If
        Me.m_RemotePath = DirectoryName
        Trace.WriteLine("Current directory is " & m_RemotePath)
    End Sub

    ''' <summary>
    ''' Close the FTP connection
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Close()
        Trace.WriteLine("Closing...")
        If m_ClientSocket IsNot Nothing Then
            SendCommand("QUIT")
        End If

        Cleanup()
    End Sub

    ''' <summary>
    ''' Implements IDisposable.Dispose
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Dispose() Implements IDisposable.Dispose
        Cleanup()
        MyBase.Finalize()
    End Sub

#End Region

#Region " Private Methods "

    Private Sub ReadReply()
        m_Message = ""
        m_Reply = ReadLine()
        m_ReturnValue = Int32.Parse(m_Reply.Substring(0, 3))
    End Sub

    Private Sub Cleanup()
        If m_ClientSocket IsNot Nothing Then
            m_ClientSocket.Close()
            m_ClientSocket = Nothing
        End If
        m_LoggedIn = False
    End Sub

    Private Function ReadLine() As String
        While True
            m_BytesTransferred = m_ClientSocket.Receive(m_Buffer, m_Buffer.Length, 0)
            m_Message = m_ASCII.GetString(m_Buffer, 0, m_BytesTransferred) '& ControlChars.Lf
            If m_BytesTransferred < m_Buffer.Length Then
                Exit While
            End If
        End While

        Dim Seperator As Char() = {ControlChars.Lf}
        Dim Message As String() = m_Message.Split(Seperator)

        If m_Message.Length > 2 Then
            m_Message = Message(Message.Length - 2)
        Else
            m_Message = Message(0)
        End If

        If Not m_Message.Substring(3, 1).Equals(" ") Then
            Return ReadLine()
        End If

        If m_DebugMode Then
            For k As Integer = 0 To Message.Length - 2
                Trace.WriteLine(Message(k))
            Next
        End If
        Return m_Message
    End Function

    Private Sub SendCommand(ByVal Command As String)
        Dim cmdBytes As Byte() = Encoding.ASCII.GetBytes((Command & vbCr & vbLf).ToCharArray())

        m_ClientSocket.Send(cmdBytes, cmdBytes.Length, 0)
        ReadReply()
    End Sub

    Private Function CreateDataSocket() As Socket
        SendCommand("PASV")
        If m_ReturnValue <> 227 Then
            Throw New IOException(m_Reply.Substring(4))
        End If

        Dim index1 As Integer = m_Reply.IndexOf("("c)
        Dim index2 As Integer = m_Reply.IndexOf(")"c)
        Dim ipData As String = m_Reply.Substring(index1 + 1, index2 - index1 - 1)
        Dim Parts As Integer() = New Integer(5) {}
        Dim ReplyLength As Integer = ipData.Length
        Dim PartCount As Integer = -1
        Dim Buffer As String = ""
        Dim CharacterCounter As Integer = 0

        While CharacterCounter < ReplyLength AndAlso PartCount <= 6
            Dim ThisChar As Char = [Char].Parse(ipData.Substring(CharacterCounter, 1))

            If [Char].IsDigit(ThisChar) Then
                Buffer += ThisChar
            ElseIf ThisChar <> ","c Then
                Throw New IOException("Malformed PASV reply: " & m_Reply)
            End If

            If ThisChar = ","c OrElse CharacterCounter + 1 = ReplyLength Then
                Try
                    Parts(System.Math.Max(System.Threading.Interlocked.Increment(PartCount), PartCount - 1)) = Int32.Parse(Buffer)
                    Buffer = ""
                Catch generatedExceptionName As Exception
                    Throw New IOException("Malformed PASV reply: " & m_Reply)
                End Try
            End If
            CharacterCounter += 1
        End While

        Dim IPAddress As String = (((CStr(Parts(0)) & ".") + CStr(Parts(1)) & ".") + CStr(Parts(2)) & ".") + CStr(Parts(3))
        Dim Port As Integer = (Parts(4) << 8) + Parts(5)
        Dim DataSocket As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        Dim EndPoint As New IPEndPoint(Dns.GetHostEntry(IPAddress).AddressList(0), Port)

        Try
            DataSocket.Connect(EndPoint)
        Catch generatedExceptionName As Exception
            Throw New IOException("Can't connect to remote server")
        End Try

        Return DataSocket
    End Function

    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
        target = value
        Return value
    End Function

    'Private Function DeleteFile(ByVal LocalFilename As String) As Boolean
    '    Dim Result As Boolean = False

    '    Try

    '        Result = True
    '    Catch ex As Exception
    '        Result = False
    '    End Try

    '    Return Result
    'End Function

#End Region

End Class
