Imports System.IO
Imports System.Reflection
Imports System.Windows.Forms
Imports System.Runtime.Serialization ' For Serialization and Deserialization

Partial Public Class Functions

#Region " Private Variables "

    Private Shared _ApplicationDataPath As String = My.Computer.FileSystem.SpecialDirectories.AllUsersApplicationData
    Private Shared _CommandLineArguments() As String = {}
    Private Shared _DebugMode As Boolean = False
#End Region

#Region " Constructor "

    Public Sub New()
        DoStartup()
    End Sub

#End Region

#Region " Properties "

    Public ReadOnly Property ApplicationDataPath() As String
        Get
            Return _ApplicationDataPath
        End Get
    End Property

    Public Shared ReadOnly Property CommandLineArguments() As String()
        Get
            Try
                If Microsoft.VisualBasic.Command.Length > 0 Then

                    If Microsoft.VisualBasic.Command.Contains("-") Then
                        _CommandLineArguments = Microsoft.VisualBasic.Command.ToString.SplitString("-")
                    Else
                        _CommandLineArguments = Microsoft.VisualBasic.Command.ToString.SplitString(" ")
                    End If

                    For Each Argument As String In _CommandLineArguments
                        If Argument.ToLower = "debug" Then
                            _DebugMode = True
                        End If
                    Next
                End If
            Catch
            End Try
            Return _CommandLineArguments
        End Get
    End Property

    Public Shared Property DebugMode() As Boolean
        Get
            Return _DebugMode
        End Get
        Set(ByVal value As Boolean)
            _DebugMode = value
        End Set
    End Property

    Public Shared ReadOnly Property CommandLineInitiated() As Boolean
        Get
            Return CBool(_CommandLineArguments.Length > 0)
        End Get
    End Property

#End Region

#Region " Public Methods "

    ''' <summary>
    ''' Recursively create a folder structure
    ''' </summary>
    ''' <param name="FullFolderPath"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateFolder(ByVal FullFolderPath As String) As Boolean
        Dim DirectoryLevel As Int32 = 0

        Dim Folders() As String = FullFolderPath.Split(CChar("\"))

        Dim DirectoryPath As String = ""
        For DirectoryLevel = 0 To Folders.Count - 1
            DirectoryPath &= Folders(DirectoryLevel) & "\"

            If Not Directory.Exists(DirectoryPath) Then Directory.CreateDirectory(DirectoryPath)
        Next
        
    End Function

    ''' <summary>
    ''' Write a trace message to the screen
    ''' </summary>
    ''' <param name="Message"></param>
    ''' <remarks></remarks>
    Public Shared Sub WriteTraceMessage(ByVal Message As String)
        Trace.WriteLine("Error!: " & Message)
    End Sub

    ''' <summary>
    ''' Converts a GUID to a string
    ''' </summary>
    ''' <param name="GUIDString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ConvertGUIDToString(ByRef GUIDString() As Byte) As String
        Dim ReturnString As String = ""

        Try
            For Count As Int32 = 0 To GUIDString.Length - 1
                ReturnString = ReturnString + Hex(GUIDString(Count))
            Next
        Catch ex As Exception
        End Try
        Return ReturnString
    End Function

    ''' <summary>
    ''' Removes non printable characters
    ''' </summary>
    ''' <param name="InputData"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function RemoveNonPrintableCharacters(ByVal InputData As String) As String
        Dim OutputData As String = Trim(InputData)

        ' Everything less than Chr(32)
        For lp As Integer = 0 To 31
            OutputData = OutputData.Replace(Chr(lp), "")
        Next

        ' Everything greater than 127
        For lp As Integer = 128 To 255
            OutputData = OutputData.Replace(Chr(lp), "")
        Next

        Return OutputData
    End Function

    ''' <summary>
    ''' Replace tags in the given string, using the current date
    ''' </summary>
    ''' <param name="InputString">The string to search</param>
    ''' <param name="ConvertToUppercase">The string to replace</param>
    ''' <returns>String containing all replaced tags</returns>
    ''' <remarks>Based on ReplaceEnvironmentVariables from Utility</remarks>
    Public Shared Function ReplaceTags(ByVal InputString As String, Optional ByVal ConvertToUppercase As Boolean = True) As String
        Dim OutputString As String = InputString

        OutputString = ReplaceTags(OutputString, Date.Now, ConvertToUppercase)

        Return OutputString
    End Function

    ''' <summary>
    ''' Perform ReplaceTags, but with an added Custom string to replace
    ''' </summary>
    ''' <param name="InputString">The string to search</param>
    ''' <param name="CustomString">The string to replace</param>
    ''' <returns>String containing all replaced tags</returns>
    ''' <remarks>Based on ReplaceEnvironmentVariables from Utility</remarks>
    Public Shared Function ReplaceTags(ByVal InputString As String, ByVal CustomString As String, Optional ByVal ConvertToUppercase As Boolean = True) As String
        Dim OutputString As String = InputString

        OutputString = OutputString.Replace("#CUSTOMSTRING#", CustomString)

        OutputString = ReplaceTags(OutputString, ConvertToUppercase)

        Return OutputString
    End Function

    ''' <summary>
    ''' Replace tags in the given string, using the provided date
    ''' </summary>
    ''' <param name="InputString">The string to search</param>
    ''' <param name="ConvertToUppercase">The string to replace</param>
    ''' <returns>String containing all replaced tags</returns>
    ''' <remarks>Based on ReplaceEnvironmentVariables from Utility</remarks>
    Public Shared Function ReplaceTags(ByVal InputString As String, ByVal InputDate As Date, ByVal ConvertToUppercase As Boolean) As String
        Dim OutputString As String = InputString
        Dim QuarterNumber As Int16 = 1
        Dim TaxQuarterNumber As Int16 = 1
        Dim PadCharacter As Char = CChar("0")

        ' TODO:  When extra is required, this has to be a small scripting engine

        ' This part allows us to pass through variables to other utilities\applications
        'OutputString = OutputString.Replace("##", "!!")

        Try
            ' This does not work for the Local System account
            OutputString = OutputString.Replace("#DESKTOP#", My.Computer.FileSystem.SpecialDirectories.Desktop)
        Catch ex As Exception
            OutputString = OutputString.Replace("[DESKTOP]", "")
        End Try

        Try
            ' This does not work for the Local System account
            OutputString = OutputString.Replace("#MYDOCS#", My.Computer.FileSystem.SpecialDirectories.MyDocuments)
        Catch ex As Exception
            OutputString = OutputString.Replace("[MYDOCS]", "")
        End Try

        Try
            ' This does not work for the Local System account
            OutputString = OutputString.Replace("#MYMUSIC#", My.Computer.FileSystem.SpecialDirectories.MyMusic)
        Catch ex As Exception
            OutputString = OutputString.Replace("[MYMUSIC]", "")
        End Try

        Try
            ' This does not work for the Local System account
            OutputString = OutputString.Replace("#MYPICS#", My.Computer.FileSystem.SpecialDirectories.MyPictures)
        Catch ex As Exception
            OutputString = OutputString.Replace("[MYPICS]", "")
        End Try

        Try
            ' This does not work for the Local System account
            OutputString = OutputString.Replace("#PROGRAMS#", My.Computer.FileSystem.SpecialDirectories.Programs)
        Catch ex As Exception
            OutputString = OutputString.Replace("[PROGRAMS]", "")
        End Try

        Dim NewGUID As System.Guid = System.Guid.NewGuid

        OutputString = OutputString.Replace("#GUID#", NewGUID.ToString)
        OutputString = OutputString.Replace("#ALLUSERSAPPDATA#", My.Computer.FileSystem.SpecialDirectories.AllUsersApplicationData)
        OutputString = OutputString.Replace("#APPDATA#", My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData)
        OutputString = OutputString.Replace("#PROGRAMFILES#", My.Computer.FileSystem.SpecialDirectories.ProgramFiles)
        OutputString = OutputString.Replace("#TEMP#", My.Computer.FileSystem.SpecialDirectories.Temp)
        OutputString = OutputString.Replace("#HOUR-1#", DateAdd(DateInterval.Hour, -1, InputDate).Hour.ToString.PadLeft(2, PadCharacter))
        OutputString = OutputString.Replace("#HOUR#", InputDate.Hour.ToString.PadLeft(2, PadCharacter))
        OutputString = OutputString.Replace("#MINUTE-1#", DateAdd(DateInterval.Minute, -1, InputDate).Minute.ToString.PadLeft(2, PadCharacter))
        OutputString = OutputString.Replace("#MINUTE#", InputDate.Minute.ToString.PadLeft(2, PadCharacter))
        OutputString = OutputString.Replace("#SECOND#", InputDate.Second.ToString.PadLeft(2, PadCharacter))
        OutputString = OutputString.Replace("#DAY-1#", DateAdd(DateInterval.Day, -1, InputDate).Day.ToString.PadLeft(2, PadCharacter))
        OutputString = OutputString.Replace("#DAY#", InputDate.Day.ToString.PadLeft(2, PadCharacter))
        OutputString = OutputString.Replace("#DATE#", InputDate.Day.ToString.PadLeft(2, PadCharacter) & InputDate.Month.ToString.PadLeft(2, PadCharacter) & InputDate.Year.ToString.PadLeft(4, PadCharacter))
        OutputString = OutputString.Replace("#DATE2#", InputDate.Day.ToString.PadLeft(2, PadCharacter) & InputDate.Month.ToString.PadLeft(2, PadCharacter) & InputDate.Year.ToString.Substring(2, 2))
        OutputString = OutputString.Replace("#DATE4#", InputDate.Day.ToString.PadLeft(2, PadCharacter) & InputDate.Month.ToString.PadLeft(2, PadCharacter) & InputDate.Year.ToString.PadLeft(4, PadCharacter))
        OutputString = OutputString.Replace("#DATE-1MONTH#", DateAdd(DateInterval.Month, -1, InputDate).Day.ToString.PadLeft(2, PadCharacter) & DateAdd(DateInterval.Month, -1, InputDate).Month.ToString.PadLeft(2, PadCharacter) & DateAdd(DateInterval.Month, -1, InputDate).Year.ToString.PadLeft(4, PadCharacter))
        OutputString = OutputString.Replace("#DATE2-1MONTH#", DateAdd(DateInterval.Month, -1, InputDate).Day.ToString.PadLeft(2, PadCharacter) & DateAdd(DateInterval.Month, -1, InputDate).Month.ToString.PadLeft(2, PadCharacter) & DateAdd(DateInterval.Month, -1, InputDate).Year.ToString.Substring(2, 2))
        OutputString = OutputString.Replace("#DATE4-1MONTH#", DateAdd(DateInterval.Month, -1, InputDate).Day.ToString.PadLeft(2, PadCharacter) & DateAdd(DateInterval.Month, -1, InputDate).Month.ToString.PadLeft(2, PadCharacter) & DateAdd(DateInterval.Month, -1, InputDate).Year.ToString.PadLeft(4, PadCharacter))
        OutputString = OutputString.Replace("#DATE+1MONTH#", DateAdd(DateInterval.Month, 1, InputDate).Day.ToString.PadLeft(2, PadCharacter) & DateAdd(DateInterval.Month, 1, InputDate).Month.ToString.PadLeft(2, PadCharacter) & DateAdd(DateInterval.Month, 1, InputDate).Year.ToString.PadLeft(4, PadCharacter))
        OutputString = OutputString.Replace("#DATE2+1MONTH#", DateAdd(DateInterval.Month, 1, InputDate).Day.ToString.PadLeft(2, PadCharacter) & DateAdd(DateInterval.Month, 1, InputDate).Month.ToString.PadLeft(2, PadCharacter) & DateAdd(DateInterval.Month, 1, InputDate).Year.ToString.Substring(2, 2))
        OutputString = OutputString.Replace("#DATE4+1MONTH#", DateAdd(DateInterval.Month, 1, InputDate).Day.ToString.PadLeft(2, PadCharacter) & DateAdd(DateInterval.Month, 1, InputDate).Month.ToString.PadLeft(2, PadCharacter) & DateAdd(DateInterval.Month, 1, InputDate).Year.ToString.PadLeft(4, PadCharacter))
        OutputString = OutputString.Replace("#TODAY#", InputDate.Day.ToString.PadLeft(2, PadCharacter) & InputDate.Month.ToString.PadLeft(2, PadCharacter) & InputDate.Year.ToString.PadLeft(4, PadCharacter))
        OutputString = OutputString.Replace("#TODAY2#", InputDate.Day.ToString.PadLeft(2, PadCharacter) & InputDate.Month.ToString.PadLeft(2, PadCharacter) & InputDate.Year.ToString.Substring(2, 2))
        OutputString = OutputString.Replace("#TODAY4#", InputDate.Day.ToString.PadLeft(2, PadCharacter) & InputDate.Month.ToString.PadLeft(2, PadCharacter) & InputDate.Year.ToString.PadLeft(4, PadCharacter))
        OutputString = OutputString.Replace("#TOMORROW#", DateAdd(DateInterval.Day, 1, InputDate).Day.ToString.PadLeft(2, PadCharacter) & DateAdd(DateInterval.Day, 1, InputDate).Month.ToString.PadLeft(2, PadCharacter) & DateAdd(DateInterval.Day, 1, InputDate).Year.ToString.PadLeft(4, PadCharacter))
        OutputString = OutputString.Replace("#TOMORROW2#", DateAdd(DateInterval.Day, 1, InputDate).Day.ToString.PadLeft(2, PadCharacter) & DateAdd(DateInterval.Day, 1, InputDate).Month.ToString.PadLeft(2, PadCharacter) & DateAdd(DateInterval.Day, 1, InputDate).Year.ToString.Substring(2, 2))
        OutputString = OutputString.Replace("#TOMORROW4#", DateAdd(DateInterval.Day, 1, InputDate).Day.ToString.PadLeft(2, PadCharacter) & DateAdd(DateInterval.Day, 1, InputDate).Month.ToString.PadLeft(2, PadCharacter) & DateAdd(DateInterval.Day, 1, InputDate).Year.ToString.PadLeft(4, PadCharacter))
        OutputString = OutputString.Replace("#YESTERDAY#", DateAdd(DateInterval.Day, -1, InputDate).Day.ToString.PadLeft(2, PadCharacter) & DateAdd(DateInterval.Day, -1, InputDate).Month.ToString.PadLeft(2, PadCharacter) & DateAdd(DateInterval.Day, -1, InputDate).Year.ToString.PadLeft(4, PadCharacter))
        OutputString = OutputString.Replace("#YESTERDAY2#", DateAdd(DateInterval.Day, -1, InputDate).Day.ToString.PadLeft(2, PadCharacter) & DateAdd(DateInterval.Day, -1, InputDate).Month.ToString.PadLeft(2, PadCharacter) & DateAdd(DateInterval.Day, -1, InputDate).Year.ToString.Substring(2, 2))
        OutputString = OutputString.Replace("#YESTERDAY4#", DateAdd(DateInterval.Day, -1, InputDate).Day.ToString.PadLeft(2, PadCharacter) & DateAdd(DateInterval.Day, -1, InputDate).Month.ToString.PadLeft(2, PadCharacter) & DateAdd(DateInterval.Day, -1, InputDate).Year.ToString.PadLeft(4, PadCharacter))
        OutputString = OutputString.Replace("#MONTH-1#", DateAdd(DateInterval.Month, -1, InputDate).Month.ToString.PadLeft(2, PadCharacter))
        OutputString = OutputString.Replace("#MONTHNAME-1#", DateAdd(DateInterval.Month, -1, InputDate).ToString("MMMM"))
        OutputString = OutputString.Replace("#MONTHNAME#", InputDate.ToString("MMMM"))
        OutputString = OutputString.Replace("#MONTH#", InputDate.Month.ToString.PadLeft(2, PadCharacter))
        OutputString = OutputString.Replace("#YEAR-1#", DateAdd(DateInterval.Year, -1, InputDate).Year.ToString.PadLeft(4, PadCharacter))
        OutputString = OutputString.Replace("#YEAR#", InputDate.Year.ToString.PadLeft(4, PadCharacter))
        OutputString = OutputString.Replace("#YEAR4-1#", DateAdd(DateInterval.Year, -1, InputDate).Year.ToString.PadLeft(4, PadCharacter))
        OutputString = OutputString.Replace("#YEAR4#", InputDate.Year.ToString.PadLeft(4, PadCharacter))
        OutputString = OutputString.Replace("#YEAR2-1#", DateAdd(DateInterval.Year, -1, InputDate).Year.ToString.Substring(2, 2))
        OutputString = OutputString.Replace("#YEAR2#", InputDate.Year.ToString.Substring(2, 2))
        OutputString = OutputString.Replace("#YEAR-1MONTH#", DateAdd(DateInterval.Month, -1, InputDate).Year.ToString.PadLeft(4, PadCharacter))
        OutputString = OutputString.Replace("#YEAR2-1MONTH#", DateAdd(DateInterval.Month, -1, InputDate).Year.ToString.Substring(2, 2))
        OutputString = OutputString.Replace("#YEAR4-1MONTH#", DateAdd(DateInterval.Month, -1, InputDate).Year.ToString.PadLeft(4, PadCharacter))
        OutputString = OutputString.Replace("#YEAR+1MONTH#", DateAdd(DateInterval.Month, 1, InputDate).Year.ToString.PadLeft(4, PadCharacter))
        OutputString = OutputString.Replace("#YEAR2+1MONTH#", DateAdd(DateInterval.Month, 1, InputDate).Year.ToString.Substring(2, 2))
        OutputString = OutputString.Replace("#YEAR4+1MONTH#", DateAdd(DateInterval.Month, 1, InputDate).Year.ToString.PadLeft(4, PadCharacter))
        OutputString = OutputString.Replace("#USERNAME#", My.User.Name.Replace("\", "_"))
        OutputString = OutputString.Replace("#COMPUTERNAME#", My.Computer.Name)
        OutputString = OutputString.Replace("#APPPATH#", My.Application.Info.DirectoryPath)
        OutputString = OutputString.Replace("#ASSEMBLY#", My.Application.Info.AssemblyName)
        OutputString = OutputString.Replace("#COMPANY#", My.Application.Info.CompanyName)
        OutputString = OutputString.Replace("#PRODUCT#", My.Application.Info.ProductName)
        OutputString = OutputString.Replace("#APPMAJORVERSION#", My.Application.Info.Version.Major.ToString)
        OutputString = OutputString.Replace("#APPMINORVERSION#", My.Application.Info.Version.Minor.ToString)
        OutputString = OutputString.Replace("#APPREVISION#", My.Application.Info.Version.Revision.ToString)
        OutputString = OutputString.Replace("#APPMINORREVISION#", My.Application.Info.Version.MinorRevision.ToString)
        OutputString = OutputString.Replace("#APPBUILD#", My.Application.Info.Version.Build.ToString)

        Select Case InputDate.Month
            Case 1, 2, 3
                QuarterNumber = 1
            Case 4, 5, 6
                QuarterNumber = 2
            Case 7, 8, 9
                QuarterNumber = 3
            Case 10, 11, 12
                QuarterNumber = 4
        End Select
        OutputString = OutputString.Replace("#QUARTER#", QuarterNumber.ToString)
        OutputString = OutputString.Replace("#QUARTER2#", QuarterNumber.ToString).PadLeft(2, PadCharacter)

        Select Case InputDate.Month
            Case 7, 8, 9
                TaxQuarterNumber = 1
            Case 10, 11, 12
                TaxQuarterNumber = 2
            Case 1, 2, 3
                TaxQuarterNumber = 3
            Case 4, 5, 6
                TaxQuarterNumber = 4
        End Select
        OutputString = OutputString.Replace("#TAXQUARTER#", TaxQuarterNumber.ToString)
        OutputString = OutputString.Replace("#TAXQUARTER2#", TaxQuarterNumber.ToString).PadLeft(2, PadCharacter)

        If ConvertToUppercase Then OutputString = OutputString.ToUpper

        'OutputString = OutputString.Replace("!!", "#")

        Return OutputString
    End Function

    ''' <summary>
    ''' Check if a Filename is Valid
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <returns>Boolean if true</returns>
    ''' <remarks></remarks>
    Public Shared Function IsValidFilename(ByVal Filename As String) As Boolean

        For Each charItem In System.IO.Path.GetInvalidFileNameChars()
            If Filename.Contains(charItem) Then
                Return False
            End If
        Next

        Return True

    End Function

    ''' <summary>
    ''' Check if a Pathname is Valid
    ''' </summary>
    ''' <param name="Pathname"></param>
    ''' <returns>Boolean if true</returns>
    ''' <remarks></remarks>
    Public Shared Function IsValidPathname(ByVal Pathname As String) As Boolean

        For Each charItem In System.IO.Path.GetInvalidPathChars()
            If Pathname.Contains(charItem) Then
                Return False
            End If
        Next

        Return True

    End Function

    ''' <summary>
    ''' Check if Path and Filename is Valid
    ''' </summary>
    ''' <param name="PathAndFilename"></param>
    ''' <returns>Boolean if true</returns>
    ''' <remarks>If you use Path Characters in the FileName it sees that as a Path and returns no error</remarks>
    Public Shared Function IsValidPathAndFilename(ByVal PathAndFilename As String) As Boolean

        Dim FileName = System.IO.Path.GetFileName(PathAndFilename)
        Dim PathName = System.IO.Path.GetDirectoryName(PathAndFilename)

        If IsValidFilename(FileName) = False Then
            Return False
        End If

        If IsValidPathname(PathName) = False Then
            Return False
        End If

        Return True

    End Function

    ''' <summary>
    ''' Pause the given period (in mS)
    ''' </summary>
    ''' <param name="mS"></param>
    ''' <remarks></remarks>
    Public Shared Sub Pause(ByVal mS As Long, Optional ByVal DoEvents As Boolean = True)
        Dim Watch As New Stopwatch

        Watch.Start()
        Do
            If DoEvents Then Application.DoEvents()
        Loop Until Watch.ElapsedMilliseconds >= mS
    End Sub

    ''' <summary>
    ''' Pause the given period (in mS) utilising Thread.Sleep
    ''' </summary>
    ''' <param name="mS"></param>
    ''' <remarks></remarks>
    Public Shared Sub Pause2(ByVal mS As Long, Optional ByVal DoEvents As Boolean = True)
        For Counter As Int64 = 1 To mS Step 100
            Threading.Thread.Sleep(CInt(mS / 100))
            If DoEvents Then Application.DoEvents()
        Next Counter
    End Sub

    ''' <summary>
    ''' Return current Application GUID
    ''' </summary>
    ''' <returns>GUID</returns>
    ''' <remarks></remarks>
    Public Shared Function ApplicationGUID() As String
        Dim attrb As Attribute() = DirectCast((Assembly.GetEntryAssembly().GetCustomAttributes(GetType(System.Runtime.InteropServices.GuidAttribute), True)), Attribute())

        Return (DirectCast((attrb(0)), System.Runtime.InteropServices.GuidAttribute)).Value
    End Function

    ''' <summary>
    ''' Return current Assembly GUID
    ''' </summary>
    ''' <returns>GUID of Common.DLL</returns>
    ''' <remarks></remarks>
    Public Shared Function AssemblyGUID() As String
        Dim attrb As Attribute() = DirectCast((Assembly.GetCallingAssembly.GetCustomAttributes(GetType(System.Runtime.InteropServices.GuidAttribute), True)), Attribute())

        Return (DirectCast((attrb(0)), System.Runtime.InteropServices.GuidAttribute)).Value
    End Function

    ''' <summary>
    ''' Loads an object from disk
    ''' </summary>
    ''' <param name="PathAndFilename">The input filename</param>
    ''' <param name="DeserializedObject">The deserialized object</param>
    ''' <returns>True if success</returns>
    ''' <remarks>V1.0  D. Robinson</remarks>
    Public Shared Function Deserialize(ByVal PathAndFilename As String, ByRef DeserializedObject As Object) As Boolean
        Dim fs As New IO.FileStream(PathAndFilename, System.IO.FileMode.Open)
        Dim formatter As New Formatters.Soap.SoapFormatter

        Try
            DeserializedObject = formatter.Deserialize(fs)

            fs.Flush()
            fs.Close()
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Saves an object to disk
    ''' </summary>
    ''' <param name="Filename">The output filename</param>
    ''' <param name="ObjectToSerialize">An object to serialize</param>
    ''' <returns>True if success</returns>
    ''' <remarks>V1.0  D. Robinson</remarks>
    Public Shared Function Serialize(ByVal Filename As String, ByVal ObjectToSerialize As Object) As Boolean
        Dim FS As New IO.FileStream(Filename, System.IO.FileMode.Create)
        Dim Formatter As New Formatters.Soap.SoapFormatter

        Try
            Formatter.Serialize(FS, ObjectToSerialize)

            FS.Close()
            FS = Nothing
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Returns a List(Of String) of delimited strings from the supplied String
    ''' </summary>
    ''' <param name="InputString">The string to search through</param>
    ''' <param name="Delimiters">Delimiters</param>
    ''' <returns>Delimited list</returns>
    ''' <remarks></remarks>
    Public Shared Function SplitString(ByVal InputString As String, ByVal ParamArray Delimiters() As String) As List(Of String)
        Dim Results() As String
        Dim StringEncoding As New Text.ASCIIEncoding()

        Using MemStream As New MemoryStream(StringEncoding.GetBytes(InputString))
            Using Parser As New FileIO.TextFieldParser(MemStream)
                Parser.Delimiters = Delimiters
                Parser.HasFieldsEnclosedInQuotes = True
                Results = Parser.ReadFields()
            End Using
        End Using

        Return New List(Of String)(Results)
    End Function

    ''' <summary>
    ''' Ping the Address
    ''' </summary>
    ''' <param name="Address"></param>
    ''' <returns>Boolean indicating success (True) or Failure (False)</returns>
    ''' <remarks></remarks>
    Public Shared Function PingAddress(ByVal Address As String, Optional ByVal Timeout As Int32 = 1000) As Boolean
        Dim IPAddress As String = ""
        Dim PortNumber As String = ""
        Dim SplitAddress As String() = {}
        Dim SplitCharacter As String = "'"

        If InStr(Address, ",") > 0 Then SplitCharacter = ","
        If InStr(Address, ":") > 0 Then SplitCharacter = ":"
        If InStr(Address, " ") > 0 Then SplitCharacter = " "

        SplitAddress = Address.Split((SplitCharacter).ToCharArray)

        If SplitAddress.Length = 1 Then
            IPAddress = SplitAddress.ElementAt(0)
        ElseIf SplitAddress.Length = 2 Then
            IPAddress = SplitAddress.ElementAt(0)
            PortNumber = SplitAddress.ElementAt(1)
        Else
            Return False
        End If

        Try
            My.Computer.Network.Ping(IPAddress, Timeout)
        Catch ex As Exception
            Return False
        End Try

        Return True

    End Function

    ''' <summary>
    ''' Swap the given value between Little and Big endian
    ''' </summary>
    ''' <param name="Value">The Value to convert</param>
    ''' <returns>Converted value</returns>
    ''' <remarks></remarks>
    Public Shared Function SwapEndian(ByVal Value As Int32) As Int32
        Dim Buffer() As Byte = BitConverter.GetBytes(Value)
        Dim Copy(Buffer.Length - 1) As Byte

        Dim Index As Integer = 0
        For Scan As Integer = Buffer.Length - 1 To 0 Step -1
            Copy(Index) = Buffer(Scan)
            Index += 1
        Next Scan
        Return BitConverter.ToInt32(Copy, 0)
    End Function

#End Region

#Region " Private Methods "

    Private Sub DoStartup()
        GetCommandlineArguments()
    End Sub

    ''' <summary>
    ''' Extract command line arguments
    ''' </summary>
    ''' <remarks>Arguments are in the form:
    ''' AppName.exe" –u:sysadmin –p:password –d:50000
    ''' </remarks>
    Private Sub GetCommandlineArguments()
        Try
            If Microsoft.VisualBasic.Command.Length > 0 Then
                _CommandLineArguments = Microsoft.VisualBasic.Command.ToString.SplitString("-")

                For Each Argument As String In _CommandLineArguments
                    If Argument.ToLower = "debug" Then
                        _DebugMode = True
                    End If
                Next
            End If
        Catch
        End Try
    End Sub

#End Region

End Class
