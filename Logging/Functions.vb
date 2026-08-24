#Region " Imports "

Imports System.IO
Imports System.Net.Mail

#End Region

''' <summary>
''' Provides logging functionality to email, logfile and the Application Eventlog
''' </summary>
''' <remarks></remarks>

Partial Public Class Functions
    Implements IDisposable


#Region " Enums "

    ''' <summary>
    ''' Indicates the type of logging to employ
    ''' </summary>
    ''' <remarks>V1.0 D. Robinson</remarks>
    Public Enum LOGGING_TYPE
        LOGFILE = 1
        EMAIL = 2
        COMPUTER_EVENT_LOG = 4
    End Enum

    ''' <summary>
    ''' Indicates the type of information to log.
    ''' </summary>
    ''' <remarks>V1.1 D. Robinson</remarks>
    Public Enum LOGGING_LEVEL
        INFORMATION = 1
        WARNING = 2
        ERRORS = 4        ' WILL ALWAYS BE LOGGED
        DEBUGGING = 255
    End Enum

    Public Enum FileType
        Text = 0
        HTML = 1
    End Enum

#End Region

#Region " Structures "

    ''' <summary>
    ''' Indicates the type of information to log.
    ''' </summary>
    ''' <remarks>V1.1 D. Robinson</remarks>
    Public Structure LogMessageTypes
        Dim LogInformationMessages As Boolean
        Dim LogWarningMessages As Boolean
        Dim LogErrorMessages As Boolean        ' WILL ALWAYS BE LOGGED
        Dim LogDebugMessages As Boolean
    End Structure

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Creates a new instance, writing to the EventLog
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        'If (_LogType And LOGGING_TYPE.COMPUTER_EVENT_LOG) Then ' If configured to use the Event Log then ensure an Event Source has been created.
        CreateEventSource()
        'End If

        If LogType = LOGGING_TYPE.LOGFILE Then
            CleanupOldFiles()
        End If

        SetLoggingLevel()
    End Sub

    ''' <summary>
    ''' Creates a new instance, writing to the specified Log type
    ''' </summary>
    ''' <param name="LogType"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal LogType As LOGGING_TYPE)
        _LogType = LogType

        'If LogType = LOGGING_TYPE.COMPUTER_EVENT_LOG Then CreateEventSource() ' If configured to use the Event Log then ensure an Event Source has been created.

        CreateEventSource()
        If LogType = LOGGING_TYPE.LOGFILE Then
            CleanupOldFiles()
        End If

        SetLoggingLevel()
    End Sub

    ''' <summary>
    ''' Creates a new instance, writing to a file
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal Filename As String)
        _LogType = LOGGING_TYPE.LOGFILE
        _LogFilename = Filename

        CreateEventSource()
        CleanupOldFiles()
        SetLoggingLevel()
    End Sub

    ''' <summary>
    ''' Creates a new instance, writing to a file, and deleting old log files
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal Filename As String, ByVal LogFileRetenionTimeDays As Int32)
        _LogType = LOGGING_TYPE.LOGFILE
        _LogFilename = Filename

        _LogfileRetentionTimeDays = LogFileRetenionTimeDays

        CleanupOldFiles()
        SetLoggingLevel()
    End Sub

    ''' <summary>
    ''' Creates a new instance, sending output to an email
    ''' </summary>
    ''' <param name="EmailTo">The SMTP address of the recipient</param>
    ''' <param name="EmailFrom">The SMTP address of the sender</param>
    ''' <param name="EmailSubject">The Email subject</param>
    ''' <param name="SMTPServerIPorName">The IP Address or Name of the SMTP Server</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal EmailTo As String, ByVal EmailFrom As String, ByVal EmailSubject As String, ByVal SMTPServerIPorName As String)
        _EmailFrom = EmailFrom
        _EmailTo = EmailTo
        _EmailSubject = EmailSubject
        _SMTPServerIPorName = SMTPServerIPorName
        SetLoggingLevel()
    End Sub

#End Region

#Region " Private Variables "

    'Private _EventLog As EventLog = Nothing
    Private _EmailSubject As String = _
            My.Application.Info.ProductName & _
            " Logged message"
    Private _EmailTo As String = ""                                  ' My.Settings.EmailTo
    Private _SMTPServerIPorName As String = ""                       ' My.Settings.SMTPServerIPorName
    Private _EmailFrom As String = ""                                ' My.Settings.EmailFrom
    Private _LogLevel As LOGGING_LEVEL = LOGGING_LEVEL.ERRORS        ' LoggingLevel.Errors + LoggingLevel.Warning
    Private Shared _LoggingConfiguration As New LogMessageTypes
    Private _LogType As LOGGING_TYPE = LOGGING_TYPE.LOGFILE          ' LoggingType.Email | LoggingType.EventLog | LoggingType.File
    Private _UserDataPath As String = IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.CompanyName & "\" & My.Application.Info.ProductName) '   My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData
    Private _LogfileExtension As String = ".htm"
    Private _LogDate As Date = Date.Now
    Private _LogFilename As String = _UserDataPath & "\" & _LogDate.Year.ToString.PadLeft(4, CChar("0")) & "-" & _LogDate.Month.ToString.PadLeft(2, "0") & "-" & _LogDate.Day.ToString.PadLeft(2, "0") & "-" & My.Application.Info.AssemblyName & "-ApplicationLog"
    Private _LogfileRetentionTimeDays As Int32 = 60
    Private _LogfileType As FileType = FileType.HTML
    Private _DebugMode As Boolean = False
    Private _Stream As StreamWriter = Nothing
    Private _DisposedValue As Boolean = False        ' To detect redundant calls

#End Region

#Region " Public Properties "

    ''' <summary>
    ''' The Log Type
    ''' </summary>
    ''' <value>LoggingType.Email | LoggingType.EventLog | LoggingType.File</value>
    ''' <returns>Integer</returns>
    ''' <remarks>V1.0 D. Robinson</remarks>
    Public Property LogType() As Integer
        Get
            Return _LogType
        End Get
        Set(ByVal value As Integer)
            _LogType = value
        End Set
    End Property

    ''' <summary>
    ''' The LogLevel of the message
    ''' </summary>
    ''' <value>The LogLevel to use</value>
    ''' <returns>The LogLevel in use</returns>
    ''' <remarks>V1.0 D. Robinson</remarks>
    Public Property LogLevel() As LOGGING_LEVEL
        Get
            Return _LogLevel
        End Get
        Set(ByVal value As LOGGING_LEVEL)
            _LogLevel = value
        End Set
    End Property

    ''' <summary>
    ''' The subject of the Email
    ''' </summary>
    ''' <value>The subject to use</value>
    ''' <returns>The subject in use</returns>
    ''' <remarks>V1.0 D. Robinson</remarks>
    Public Property EmailSubject() As String
        Get
            Return _EmailSubject
        End Get
        Set(ByVal value As String)
            _EmailSubject = value
        End Set
    End Property

    ''' <summary>
    ''' The email address of the sender
    ''' </summary>
    ''' <value>The email address to use</value>
    ''' <returns>The email address in use</returns>
    ''' <remarks>V1.0 D. Robinson</remarks>
    Public Property EmailFrom() As String
        Get
            Return _EmailFrom
        End Get
        Set(ByVal value As String)
            _EmailFrom = value
        End Set
    End Property

    ''' <summary>
    ''' The email address of the recipient
    ''' </summary>
    ''' <value>The email address to use</value>
    ''' <returns>The email address in use</returns>
    ''' <remarks>V1.0 D. Robinson</remarks>
    Public Property EmailTo() As String
        Get
            Return _EmailTo
        End Get
        Set(ByVal value As String)
            _EmailTo = value
        End Set
    End Property

    ''' <summary>
    ''' The SMTP Server name or IP Address
    ''' </summary>
    ''' <value>The SMTP Server or IP Address to use</value>
    ''' <returns>The SMTP Server or IP Address in use</returns>
    ''' <remarks>V1.0 D. Robinson</remarks>
    Public Property SMTPServerIPorName() As String
        Get
            Return _SMTPServerIPorName
        End Get
        Set(ByVal value As String)
            _SMTPServerIPorName = value
        End Set
    End Property

    ''' <summary>
    ''' The filename of the log
    ''' </summary>
    ''' <value>The Filename to use</value>
    ''' <returns>The Filename in use</returns>
    ''' <remarks>V1.0 D. Robinson</remarks>
    Public ReadOnly Property Filename() As String
        Get
            Return _LogFilename
        End Get
    End Property

    Public Property LogfileRetentionTimeDays() As Int32
        Get
            Return _LogfileRetentionTimeDays
        End Get
        Set(ByVal value As Int32)
            _LogfileRetentionTimeDays = value
        End Set
    End Property

    Public Property LogfileType() As FileType
        Get
            Return _LogfileType
        End Get
        Set(ByVal value As FileType)
            _LogfileType = value
            Select Case value
                Case FileType.HTML
                    _LogfileExtension = ".htm"
                Case FileType.Text
                    _LogfileExtension = ".txt"
            End Select
        End Set
    End Property

    Public Property DebugMode() As Boolean
        Get
            Return _DebugMode
        End Get
        Set(ByVal value As Boolean)
            _DebugMode = value
            SetLoggingLevel()
        End Set
    End Property

    ''' <summary>
    ''' The Date to use in the logfilename
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LogDate() As Date
        Get
            Return _LogDate
        End Get
        Set(ByVal value As Date)
            _LogDate = value
            SetLogDate()
            CleanupOldFiles()
        End Set
    End Property

    Public Shared Property LoggingConfiguration() As Logging.Functions.LogMessageTypes
        Get
            Return _LoggingConfiguration
        End Get
        Set(ByVal value As Logging.Functions.LogMessageTypes)
            _LoggingConfiguration = value
        End Set
    End Property

#End Region

#Region " Public Methods "

    ''' <summary>
    ''' Creates the eventlog source.
    ''' </summary>
    ''' <remarks>The User MUST be an Administrator (in Vista, you must also use Run As Administrator) for this to work.
    ''' </remarks>
    Public Sub CreateEventSource()
        Dim EventLogName As String = "Application"
        Dim SourceName As String = My.Application.Info.CompanyName.ToString & " " & My.Application.Info.ProductName.ToString

        Try
            ' This code requires Security rights to read the Security log, which is a bit daft.
            If Not EventLog.SourceExists(SourceName) Then

                Dim Log As EventLog = New EventLog(EventLogName)
                If Not EventLog.SourceExists(SourceName) Then
                    EventLog.CreateEventSource(SourceName, Log.LogDisplayName)
                End If

            End If
        Catch ex As Exception
            ' Error whilst creating Event Source.  Do we have permissions to create the Event Source?
        End Try


        ' This code does not need rights to the Security log, but doesn't quite work
        'Dim EventLog As New EventLog

        'EventLog.Log = EventLogName
        'EventLog.Source = SourceName

        '' Check whether registry key for source exists
        'Dim KeyName As String = "HKLM\SYSTEM\CurrentControlSet\Services\EventLog\" + EventLogName + "\" + SourceName
        'Dim rkEventSource As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(KeyName)

        '' Check whether key exists
        'If rkEventSource Is Nothing Then
        '    ' Key doesnt exist. Create key which represents source
        '    Dim Proc As New Process()
        '    Dim ProcStartInfo As New ProcessStartInfo("Reg.exe")

        '    ProcStartInfo.Arguments = "add " + Chr(32) & KeyName & Chr(32)
        '    ProcStartInfo.UseShellExecute = True
        '    ProcStartInfo.Verb = "runas"
        '    ProcStartInfo.CreateNoWindow = True
        '    ProcStartInfo.WindowStyle = ProcessWindowStyle.Hidden
        '    Proc.StartInfo = ProcStartInfo

        '    Proc.Start()
        'End If

    End Sub

    ''' <summary>
    ''' Set the logging filename
    ''' </summary>
    ''' <param name="LogFilenameAndPath">The filename to use</param>
    ''' <remarks>V1.0 D. Robinson</remarks>
    <Obsolete("This method is obsolete, and should not be used.", False)> _
    Public Sub SetFilename(ByVal LogFilenameAndPath As String)
        If LogFilenameAndPath.Length > 0 Then
            _LogFilename = LogFilenameAndPath
            _LogDate = Date.Now
        End If
    End Sub

    ''' <summary>
    ''' Write an Information Event
    ''' </summary>
    ''' <param name="Message"></param>
    ''' <returns>Boolean indicating Success (True) or Failure (False)</returns>
    ''' <remarks>V1.0 D. Robinson</remarks>
    Public Function WriteInformationEvent(ByVal Message As String) As Boolean
        Return WriteEvent(Message, LOGGING_LEVEL.INFORMATION)
    End Function

    ''' <summary>
    ''' Write a Warning Event
    ''' </summary>
    ''' <param name="Message"></param>
    ''' <returns>Boolean indicating Success (True) or Failure (False)</returns>
    ''' <remarks></remarks>
    Public Function WriteWarningEvent(ByVal Message As String) As Boolean
        Return WriteEvent(Message, LOGGING_LEVEL.WARNING)
    End Function

    ''' <summary>
    ''' Write an Error Event
    ''' </summary>
    ''' <param name="Message"></param>
    ''' <returns>Boolean indicating Success (True) or Failure (False)</returns>
    ''' <remarks></remarks>
    Public Function WriteErrorEvent(ByVal Message As String) As Boolean
        Return WriteEvent(Message, LOGGING_LEVEL.ERRORS)
    End Function

    ''' <summary>
    ''' Write a Debug Event
    ''' </summary>
    ''' <param name="Message"></param>
    ''' <returns>Boolean indicating Success (True) or Failure (False)</returns>
    ''' <remarks></remarks>
    Public Function WriteDebugEvent(ByVal Message As String) As Boolean
        Return WriteEvent(Message, LOGGING_LEVEL.DEBUGGING)
    End Function

    ''' <summary>
    ''' Write an Event of the specified EventLogEntryType
    ''' </summary>
    ''' <param name="Message">The event text</param>
    ''' <param name="Category">The EventLogEntryType</param>
    ''' <returns>Boolean indicating Success (True) or Failure (False)</returns>
    ''' <remarks></remarks>
    Public Function WriteEvent(ByVal Message As String, ByVal Category As LOGGING_LEVEL) As Boolean
        Try
            Select Case Category
                Case LOGGING_LEVEL.DEBUGGING
                    Trace.WriteLine(" ***** DEBUG " & Message)
                Case LOGGING_LEVEL.ERRORS
                    Trace.WriteLine(" !!!!! ERROR " & Message)
                Case LOGGING_LEVEL.INFORMATION
                    Trace.WriteLine(" INFORMATION " & Message)
                Case LOGGING_LEVEL.WARNING
                    Trace.WriteLine(" ??? WARNING " & Message)
            End Select

            If Category <= _LogLevel Then
                If (_LogType And LOGGING_TYPE.EMAIL) Then ' Send Email
                    SendEmail(Message, Category)
                End If

                If (_LogType And LOGGING_TYPE.COMPUTER_EVENT_LOG) Then ' Add event to Event Log
                    WriteEventLogEntry(Message, Category)
                End If

                If (_LogType And LOGGING_TYPE.LOGFILE) Then ' Log to a file
                    WriteToLogFile(Message, Category)
                End If
            End If

            Return True
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Write to the Application logfile
    ''' </summary>
    ''' <param name="Message">The event text</param>
    ''' <param name="Category">The EventLogEntryType</param>
    ''' <remarks></remarks>
    Public Sub WriteToLogFile(ByVal Message As String, ByVal Category As LOGGING_LEVEL)
        Dim TimeString As String = Date.Now.Hour.ToString.PadLeft(2, "0") & ":" & Date.Now.Minute.ToString.PadLeft(2, "0") & ":" & Date.Now.Second.ToString.PadLeft(2, "0")
        Dim ApplicationName As String = My.Application.Info.AssemblyName.PadRight(20)
        Dim Seperator As String = vbTab
        Dim Prefix As String = ""
        Dim Suffix As String = ""

        SetLogDate()
        CleanupOldFiles()

        If _LogfileType = FileType.HTML Then
            Seperator = "&nbsp;&nbsp;"
            Suffix = "</font><br/>" & vbCrLf
            Select Case Category
                Case LOGGING_LEVEL.DEBUGGING
                    Prefix = "<font style='color:gray'>"
                Case LOGGING_LEVEL.ERRORS
                    Prefix = "<font style='color:red'>"
                Case LOGGING_LEVEL.INFORMATION
                    Prefix = "<font style='color:green'>"
                Case LOGGING_LEVEL.WARNING
                    Prefix = "<font style='color:orange'>"
            End Select
        Else
            Suffix = vbCrLf
        End If

        If _Stream Is Nothing Then
            'WriteEventLogEntry("Logging started.  The log filename is " & _LogFilename & _LogfileExtension, LOGGING_LEVEL.INFORMATION)
            _Stream = New StreamWriter(_LogFilename & _LogfileExtension, True)
        End If

        SyncLock _Stream
            Try
                Select Case Category
                    Case LOGGING_LEVEL.ERRORS
                        _Stream.Write(Prefix & TimeString & Seperator & " !!!!! ERROR " & Seperator & ApplicationName & Seperator)
                    Case LOGGING_LEVEL.INFORMATION
                        _Stream.Write(Prefix & TimeString & Seperator & " INFORMATION " & Seperator & ApplicationName & Seperator)
                    Case LOGGING_LEVEL.WARNING
                        _Stream.Write(Prefix & TimeString & Seperator & " ??? WARNING " & Seperator & ApplicationName & Seperator)
                    Case LOGGING_LEVEL.DEBUGGING
                        _Stream.Write(Prefix & TimeString & Seperator & " ***** DEBUG " & Seperator & ApplicationName & Seperator)
                End Select

                _Stream.Write(Message & Suffix)
                If _Stream.BaseStream.CanWrite Then
                    _Stream.Flush()
                End If
            Catch ex As Exception
                ' Damn!  If we get an error in here, we can't use this message logging type
                WriteEventLogEntry("Error writing logfile.  The message was: " & ex.ToString, EventLogEntryType.Error)
            End Try
        End SyncLock
    End Sub

    ''' <summary>
    ''' Send an Email
    ''' </summary>
    ''' <param name="Body">The event text</param>
    ''' <param name="Category">The EventLogEntryType</param>
    ''' <remarks></remarks>
    Public Sub SendEmail(ByVal Body As String, ByVal Category As LOGGING_LEVEL, Optional ByVal SendAsHTML As Boolean = False)
        ' If mandatory settings aren't specified, exit silently
        If _SMTPServerIPorName = "" Or _EmailFrom = "" Or Body = "" Then Exit Sub

        Dim Source As String = My.Application.Info.CompanyName & " " & My.Application.Info.ProductName.ToString
        Dim TimeString As String = String.Format(Date.Now.Hour, "00") & ":" & String.Format(Date.Now.Minute, "00") & ":" & String.Format(Date.Now.Second, "00")
        Dim CategoryString As String = ""

        Select Case Category
            Case LOGGING_LEVEL.ERRORS
                CategoryString = "Error message generated by " & Source & " at " & TimeString
            Case LOGGING_LEVEL.INFORMATION
                CategoryString = "Information message generated by " & Source & " at " & TimeString
            Case LOGGING_LEVEL.WARNING
                CategoryString = "Warning message generated by " & Source & " at " & TimeString
        End Select

        'Body = CategoryString & vbCrLf & vbCrLf & Body

        Dim MailClient As New SmtpClient(_SMTPServerIPorName)
        Dim Message As New MailMessage(_EmailFrom, _EmailTo, _EmailSubject, Body)

        If SendAsHTML Then Message.IsBodyHtml = True

        Try
            MailClient.Send(Message)
        Catch ex As Exception
            ' Damn!  If we get an error in here, we can't use this message logging type
            WriteToLogFile("Error sending Email.  The error message was: " & ex.ToString, EventLogEntryType.Error)
        End Try

        Message = Nothing
        MailClient = Nothing

    End Sub

    ''' <summary>
    ''' Write to the Application Event Log
    ''' </summary>
    ''' <param name="Message">The event text</param>
    ''' <param name="Category">The EventLogEntryType</param>
    ''' <remarks></remarks>
    Public Sub WriteEventLogEntry(ByVal Message As String, ByVal Category As LOGGING_LEVEL)
        Dim Source As String = My.Application.Info.CompanyName & " " & My.Application.Info.ProductName.ToString

        Select Case Category
            Case LOGGING_LEVEL.INFORMATION
                EventLog.WriteEntry(Source, Message, EventLogEntryType.Information)
            Case LOGGING_LEVEL.WARNING
                EventLog.WriteEntry(Source, Message, EventLogEntryType.Warning)
            Case LOGGING_LEVEL.ERRORS
                EventLog.WriteEntry(Source, Message, EventLogEntryType.Error)
            Case LOGGING_LEVEL.DEBUGGING
                EventLog.WriteEntry(Source, Message, EventLogEntryType.Information)
        End Select

    End Sub

    ''' <summary>
    ''' Write an application Error Event
    ''' </summary>
    ''' <param name="InputMessage"></param>
    ''' <returns>Boolean indicating Success (True) or Failure (False)</returns>
    ''' <remarks></remarks>
    Public Function WriteApplicationErrorEvent(ByVal InputMessage As String, ByVal AssemblyName As String, ByVal MethodName As String, ByVal StackTrace As String) As Boolean
        'Dim OutputMessage As String = ""
        Dim LineNumber As Integer = 0

        If StackTrace.Contains(":line") Then
            LineNumber = Convert.ToInt32(StackTrace.Substring(StackTrace.IndexOf(":line ") + 6, StackTrace.Length - StackTrace.IndexOf(":line ") - 6))
        End If

        WriteEvent("Error in " & AssemblyName & ":" & MethodName & ".", LOGGING_LEVEL.ERRORS)
        WriteEvent("The line(s) between the asterisks contain the Exception message.", LOGGING_LEVEL.ERRORS)
        WriteEvent("************************************************************************", LOGGING_LEVEL.ERRORS)
        WriteEvent(InputMessage, LOGGING_LEVEL.ERRORS)
        WriteEvent("************************************************************************", LOGGING_LEVEL.ERRORS)
        If LineNumber > 0 Then
            WriteEvent("The line number for this error is " & LineNumber.ToString, LOGGING_LEVEL.ERRORS)
        End If
    End Function

    ''' <summary>
    ''' Remove existing logfiles older than LogfileRetentionDays
    ''' </summary>
    ''' <param name="LogFileRetenionTimeDays">Number of days to retain logfiles</param>
    ''' <remarks></remarks>
    Public Sub CleanupOldFiles(ByVal LogFileRetenionTimeDays As Int32)
        _LogfileRetentionTimeDays = LogFileRetenionTimeDays

        CleanupOldFiles()
    End Sub

    ''' <summary>
    ''' Remove existing logfiles older than current LogfileRetentionDays (default of 60 days)
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CleanupOldFiles()
        Try
            Dim fi As FileInfo = New FileInfo(_LogFilename)
            Dim Path As String = _LogFilename.Substring(0, _LogFilename.LastIndexOf("\"))

            ' Create the path to the logfile
            CreateFolder(Path)
            Dim ParentFolderName As String = FileIO.FileSystem.GetParentPath(_LogFilename)
            Dim ExistingFiles() As String = IO.Directory.GetFiles(ParentFolderName, "*.txt")

            For Counter As Integer = 0 To ExistingFiles.Length - 1
                Dim WriteTime As Date = File.GetLastWriteTime(ExistingFiles(Counter))

                If DateDiff(DateInterval.Day, WriteTime, Date.Now) > _LogfileRetentionTimeDays Then
                    ' trace.writeline(ExistingFiles(Counter) & " should be deleted.")
                    Try
                        File.Delete(ExistingFiles(Counter))
                    Catch
                    End Try
                End If
            Next
        Catch ex As Exception

        End Try
    End Sub

    Public Sub SetLoggingLevel()
        'Try
        Dim CommandString As String = Command.ToString.ToLower

        ' Always log errors
        _LoggingConfiguration.LogErrorMessages = True

        If _DebugMode = True Then
            _LogLevel = _LogLevel Or LOGGING_LEVEL.DEBUGGING
            _LoggingConfiguration.LogDebugMessages = True
        End If

        _LoggingConfiguration.LogErrorMessages = True

        If CommandString.Length = 0 Then
            _LogLevel = LOGGING_LEVEL.ERRORS
        End If
        If CommandString.Contains("debug") Then
            _LogLevel = _LogLevel Or LOGGING_LEVEL.DEBUGGING
            _LoggingConfiguration.LogDebugMessages = True
        End If
        If CommandString.Contains("info") Then
            _LogLevel = _LogLevel Or LOGGING_LEVEL.INFORMATION
            _LoggingConfiguration.LogInformationMessages = True
        End If
        If CommandString.Contains("warn") Then
            _LogLevel = _LogLevel Or LOGGING_LEVEL.WARNING
            _LoggingConfiguration.LogWarningMessages = True
        End If
        If CommandString.Contains("error") Then
            _LogLevel = _LogLevel Or LOGGING_LEVEL.ERRORS
        End If

#If DEBUG Then
        _LogLevel = _LogLevel Or LOGGING_LEVEL.DEBUGGING
        _LoggingConfiguration.LogDebugMessages = True
#End If

        If _LogLevel = LOGGING_LEVEL.DEBUGGING Then
            _DebugMode = True
        End If

        'WriteEventLogEntry("Logging level set.  Log level is " & _LogLevel.ToString, LOGGING_LEVEL.INFORMATION)
        'Catch
        ' This is a VERY BAD Error!!
        'End Try
    End Sub

#End Region

#Region " Private Methods "

    Private Sub SetLogDate()
        _LogDate = Date.Now
        _LogFilename = _UserDataPath & "\" & _LogDate.Year.ToString.PadLeft(4, "0") & "-" & _LogDate.Month.ToString.PadLeft(2, "0") & "-" & _LogDate.Day.ToString.PadLeft(2, "0") & "-" & My.Application.Info.AssemblyName & "-ApplicationLog"
    End Sub

    Private Function FileNameMinusExtension(ByVal TheFileInfo As IO.FileInfo) As String
        Dim ShortFilename As String = TheFileInfo.Name
        Dim Extension As String = TheFileInfo.Extension
        Dim ExtensionLength As Int32 = Extension.Length

        If ExtensionLength > 0 Then
            ShortFilename = ShortFilename.Substring(ShortFilename.Length - ExtensionLength)
        End If

        Return ShortFilename
    End Function

    ''' <summary>
    ''' Recursively create a folder structure
    ''' </summary>
    ''' <param name="FullFolderPath"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreateFolder(ByVal FullFolderPath As String) As Boolean
        Dim DirectoryLevel As Int32 = 0

        Dim Folders() As String = FullFolderPath.Split(CChar("\"))

        Dim DirectoryPath As String = ""
        For DirectoryLevel = 0 To Folders.Count - 1
            DirectoryPath &= Folders(DirectoryLevel) & "\"

            If Not System.IO.Directory.Exists(DirectoryPath) Then System.IO.Directory.CreateDirectory(DirectoryPath)
        Next

    End Function

#End Region

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me._DisposedValue Then
            If disposing Then
                _Stream.Close()
            End If
        End If
        Me._DisposedValue = True
    End Sub

#Region " IDisposable Support "
    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
