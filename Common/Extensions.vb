Imports System.Collections.Specialized
Imports System.Runtime.Serialization
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Text

Public Module Extensions

    Private _OriginalDirectoryServerAndPort As String = ""

#Region " String "

    ''' <summary>
    ''' [Extensions] Determine if the given String is formatted as a valid email address
    ''' </summary>
    ''' <param name="TheString">String to evaluate</param>
    ''' <returns>Boolean indicating true if the String is formatted as a valid email address</returns>
    ''' <remarks></remarks>
    <System.Runtime.CompilerServices.Extension()> _
    Public Function IsValidEmailAddress(ByVal TheString As String) As Boolean
        Dim TheRegularExpression As New Text.RegularExpressions.Regex("^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")

        Return TheRegularExpression.IsMatch(TheString)
    End Function

    ''' <summary>
    ''' [Extensions] Removes non-numeric characters from the provided string
    ''' </summary>
    ''' <param name="TheString">String to evaluate</param>
    ''' <returns>String containing no numeric characters</returns>
    ''' <remarks></remarks>
    <System.Runtime.CompilerServices.Extension()> _
    Public Function RemoveNonNumeric(ByVal TheString As String) As String
        If Not String.IsNullOrEmpty(TheString) Then
            Dim Result As Char() = New Char(TheString.Length) {}
            Dim ResultIndex As Integer = 0

            For Each Character As Char In TheString
                If Char.IsNumber(Character) Then
                    Result(System.Math.Max(System.Threading.Interlocked.Increment(ResultIndex), ResultIndex - 1)) = Character
                End If
            Next
            If 0 = ResultIndex Then
                TheString = String.Empty
            ElseIf Result.Length <> ResultIndex Then
                TheString = New String(Result, 0, ResultIndex)
            End If
        End If

        Return TheString
    End Function

    ''' <summary>
    ''' [Extensions] Uses built-in voice synthesis to speak the provided string
    ''' </summary>
    ''' <param name="TheString">The String to vocalise</param>
    ''' <remarks></remarks>
    <System.Runtime.CompilerServices.Extension()> _
    Public Sub Speak(ByVal TheString As String)

        Try
            Using SynthVoice = New System.Speech.Synthesis.SpeechSynthesizer
                'synth.SelectVoiceByHints(Speech.Synthesis.VoiceGender.Male)
                'SynthVoice.SelectVoice("Microsoft Anna")
                SynthVoice.Speak(TheString)
            End Using
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' [Extensions] Uses built-in voice synthesis to speak the provided string and save to the provided filename
    ''' </summary>
    ''' <param name="TheString">The String to vocalise</param>
    ''' <param name="Filename">The filename to save to</param>
    ''' <remarks></remarks>
    <System.Runtime.CompilerServices.Extension()> _
    Public Sub SpeakToFile(ByVal TheString As String, ByVal Filename As String)
        Dim Format As New System.Speech.AudioFormat.SpeechAudioFormatInfo(samplesPerSecond:=8000, bitsPerSample:=Speech.AudioFormat.AudioBitsPerSample.Sixteen, channel:=Speech.AudioFormat.AudioChannel.Mono)

        Try
            Using SynthVoice = New System.Speech.Synthesis.SpeechSynthesizer
                'synth.SelectVoiceByHints(Speech.Synthesis.VoiceGender.Male)
                SynthVoice.SelectVoice("Microsoft Anna")
                SynthVoice.SetOutputToWaveFile(Filename, Format)
                SynthVoice.Speak(TheString)
            End Using
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' [Extensions] Returns the left-most [Length] characters from the provided string
    ''' </summary>
    ''' <param name="TheString">String to evaluate</param>
    ''' <param name="Length">Length to return</param>
    ''' <returns>Substring of the provided String</returns>
    ''' <remarks></remarks>
    <System.Runtime.CompilerServices.Extension()> _
    Public Function Left(ByVal TheString As String, ByVal Length As Int32) As String
        Dim Result As String = ""

        If Length >= TheString.Length Then
            Result = TheString
        Else
            Result = TheString.Substring(0, Length)
        End If

        Return Result
    End Function

    ''' <summary>
    ''' [Extensions] Returns the right-most [Length] characters from the provided string
    ''' </summary>
    ''' <param name="TheString">String to evaluate</param>
    ''' <param name="Length">Length to return</param>
    ''' <returns>Substring of the provided String</returns>
    ''' <remarks></remarks>
    <System.Runtime.CompilerServices.Extension()> _
    Public Function Right(ByVal TheString As String, ByVal Length As Int32) As String
        Dim Result As String = ""

        If Length >= TheString.Length Then
            Result = TheString
        Else
            Result = TheString.Substring(TheString.Length - Length, Length)
        End If

        Return Result
    End Function

    ''' <summary>
    ''' [Extensions] Splits the provided string into strings according to the provided delimiter(s)
    ''' </summary>
    ''' <param name="Delimiters">Delimiters</param>
    ''' <returns>String array containing the split strings</returns>
    ''' <remarks></remarks>
    <System.Runtime.CompilerServices.Extension()> _
    Public Function SplitString(ByVal TheString As String, ByVal ParamArray Delimiters() As String) As String()
        Dim TempResults() As String
        Dim CollectionofResults As New StringCollection
        Dim FinalResults() As String = {}
        Dim StringEncoding As New Text.ASCIIEncoding

        Using MemStream As New MemoryStream(StringEncoding.GetBytes(TheString))
            Using Parser As New FileIO.TextFieldParser(MemStream, New ASCIIEncoding)
                Parser.TextFieldType = FileIO.FieldType.Delimited
                Parser.SetDelimiters(Delimiters)
                Parser.HasFieldsEnclosedInQuotes = True
                Parser.TrimWhiteSpace = True

                TempResults = Parser.ReadFields
            End Using
        End Using

        For a As Int32 = 0 To TempResults.Length - 1
            If TempResults(a).Trim.Length > 0 Then
                CollectionofResults.Add(TempResults(a))
            End If
        Next

        Dim Counter As Int32 = 0
        For Each result As String In CollectionofResults
            ReDim Preserve FinalResults(Counter)
            FinalResults(Counter) = result
            Counter += 1
        Next

        Return FinalResults
    End Function

    ''' <summary>
    ''' [Extensions] Converts the provided string into a boolean
    ''' </summary>
    ''' <returns>Boolean version of the provided string</returns>
    ''' <remarks></remarks>
    <System.Runtime.CompilerServices.Extension()> _
    Public Function ToBoolean(ByVal TheString As String) As Boolean
        Dim Result As Boolean = False

        If TheString.Trim.Length = 0 Then Result = False
        Select Case TheString.ToLower
            Case "y", "1", "-1", "yes", "true", "checked", "selected", "on"
                TheString = CStr(True)
            Case Else
                TheString = CStr(False)
        End Select
        Try
            Result = CBool(TheString)
        Catch
            Result = False
        End Try

        Return Result
    End Function

    ''' <summary>
    ''' [Extensions] Converts the provided string into a integer
    ''' </summary>
    ''' <returns>Integer version of the provided string</returns>
    ''' <remarks></remarks>
    <System.Runtime.CompilerServices.Extension()> _
    Public Function ToInteger(ByVal TheString As String) As Int32
        Dim Result As Int32 = 0

        If TheString.Trim.Length = 0 Then Result = 0
        Select Case TheString.ToLower
            Case "y", "1", "-1", "yes", "true", "checked", "selected", "on"
                TheString = CStr(1)
        End Select
        Try
            Result = CInt(TheString)
        Catch
            Result = 0
        End Try

        Return Result
    End Function

#End Region

#Region " StringBuilder "

    ''' <summary>
    ''' [Extensions] Clear the contents of the StringBuilder
    ''' </summary>
    ''' <param name="TheStringBuilder">The Stringbuilder to clear</param>
    ''' <returns>StringBuilder with length set to 0 and capacity set to 16</returns>
    ''' <remarks></remarks>
    <Runtime.CompilerServices.Extension()> _
    Public Function Clear(ByVal TheStringBuilder As Text.StringBuilder) As Text.StringBuilder
        TheStringBuilder.Length = 0
        TheStringBuilder.Capacity = 16

        Return TheStringBuilder
    End Function

#End Region

#Region " DateTime "

    ''' <summary>
    ''' [Extensions] Return the Date with Last day of the Month
    ''' </summary>
    ''' <param name="TheDate"></param>
    ''' <returns>Date with Last day of the Month</returns>
    ''' <remarks></remarks>
    <Runtime.CompilerServices.Extension()> _
    Public Function LastDayOfMonth(ByVal TheDate As DateTime) As DateTime
        Dim DateTo As DateTime = TheDate
        ' Overshoot the date by a month
        DateTo = DateTo.AddMonths(1)
        ' Remove all of the days in the next month to get bumped down to the last day
        DateTo = DateTo.AddDays(-(DateTo.Day))
        ' Return value to the last day of the month for any date passed in to the method
        Return DateTo

    End Function

    ''' <summary>
    ''' [Extensions] Return the Date with First day of the Month
    ''' </summary>
    ''' <param name="TheDate"></param>
    ''' <returns>Date with First day of the Month</returns>
    ''' <remarks></remarks>
    <Runtime.CompilerServices.Extension()> _
    Public Function FirstDayOfMonth(ByVal TheDate As DateTime) As DateTime

        Dim DateFrom As DateTime = TheDate
        ' Remove all of the days in the month except the first day
        DateFrom = DateFrom.AddDays(-(DateFrom.Day - 1))
        ' Return value to the first day of the month for any date passed in to the method
        Return DateFrom
    End Function

#End Region

#Region " Object "

    ''' <summary>
    ''' [Extensions] Determine if the provided [Enumerable] object contains the given object
    ''' </summary>
    ''' <param name="Object">Object to search for</param>
    ''' <param name="TheCollection">Enumerable object to search within</param>
    ''' <returns>Boolean indicating true if the Enumerable contains the object</returns>
    ''' <remarks></remarks>
    <System.Runtime.CompilerServices.Extension()> _
    Public Function [In](ByVal [Object] As Object, ByVal TheCollection As IEnumerable) As Boolean
        For Each Item As Object In TheCollection
            If Item.Equals([Object]) Then
                Return True
            End If
        Next

        Return False
    End Function

    ''' <summary>
    ''' [Extensions] Returns a Deep-Copy of the given Object
    ''' </summary>
    ''' <param name="Source">The object to Deep-Copy</param>
    ''' <returns>A Deep-Copy of the source object</returns>
    ''' <remarks></remarks>
    <System.Runtime.CompilerServices.Extension()> _
    Public Function DeepCopy(ByRef Source As Object) As Object
        Dim MemStream As New MemoryStream()
        Dim ObjectToReturn As Object = Nothing

        Try
            Dim BinaryFormatter As New BinaryFormatter(Nothing, New StreamingContext(StreamingContextStates.Clone))

            BinaryFormatter.Serialize(MemStream, Source)
            MemStream.Seek(0, SeekOrigin.Begin)

            ObjectToReturn = BinaryFormatter.Deserialize(MemStream)

            MemStream.Close()
        Catch ex As Exception

        End Try
        Return ObjectToReturn
    End Function

    ''' <summary>
    ''' [Extensions] Loads an object from disk
    ''' </summary>
    ''' <param name="Source">The object to Deserialize</param>
    ''' <param name="PathAndFilename">The input filename</param>
    ''' <returns>True if success</returns>
    ''' <remarks></remarks>
    <System.Runtime.CompilerServices.Extension()> _
    Public Function Deserialize(ByRef Source As Object, ByVal PathAndFilename As String) As Boolean
        Dim fs As New IO.FileStream(PathAndFilename, System.IO.FileMode.Open)
        Dim formatter As New Formatters.Soap.SoapFormatter

        Try
            Source = formatter.Deserialize(fs)

            fs.Flush()
            fs.Close()
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' [Extensions] Saves an object to disk
    ''' </summary>
    ''' <param name="Source">The object to Serialize</param>
    ''' <param name="Filename">The output filename</param>
    ''' <returns>True if success</returns>
    ''' <remarks></remarks>
    <System.Runtime.CompilerServices.Extension()> _
    Public Function Serialize(ByRef Source As Object, ByVal Filename As String) As Boolean
        Dim FS As New IO.FileStream(Filename, System.IO.FileMode.Create)
        Dim Formatter As New Formatters.Soap.SoapFormatter

        Try
            Formatter.Serialize(FS, Source)

            FS.Close()
            FS = Nothing
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

#End Region

#Region " Enumerable "

    ''' <summary>
    ''' [Extensions] Determines if the provided Enumerable contains all the same values
    ''' </summary>
    ''' <param name="ThisCollection">Enumerable object to search within</param>
    ''' <returns>Boolean indicating true if all values within the Enumerable are the same</returns>
    ''' <remarks></remarks>
    <System.Runtime.CompilerServices.Extension()> _
    Public Function ContentsAllEqual(ByRef ThisCollection As IEnumerable) As Boolean
        Return (From ce In ThisCollection Select ce Distinct).Count = 1
    End Function

#End Region

#Region " Collections "

    ''' <summary>
    ''' [Extensions] Determines if the provided System.Collections.Specialized.OrderedDictionary contains this value
    ''' </summary>
    ''' <param name="ThisCollection">System.Collections.Specialized.OrderedDictionary to search within</param>
    ''' <returns>Boolean indicating true if the value is in the System.Collections.Specialized.OrderedDictionary</returns>
    ''' <remarks></remarks>
    <System.Runtime.CompilerServices.Extension()> _
    Public Function ContainsValue(ByRef ThisCollection As OrderedDictionary, ByVal Value As String) As Boolean
        For Each Element In ThisCollection.Values
            If Element.ToString = Value Then Return True
        Next

        Return False
    End Function

#End Region

#Region " FileInfo "

    ''' <summary>
    ''' [Extensions] Return the Filename without the extension
    ''' </summary>
    ''' <param name="TheFileInfo"></param>
    ''' <returns>A filename without the extension</returns>
    ''' <remarks></remarks>
    <Runtime.CompilerServices.Extension()> _
    Public Function FileNameMinusExtension(ByVal TheFileInfo As IO.FileInfo) As String
        Dim ShortFilename As String = TheFileInfo.Name
        Dim Extension As String = TheFileInfo.Extension
        Dim ExtensionLength As Int32 = Extension.Length

        If ExtensionLength > 0 Then
            ShortFilename = ShortFilename.Left(ShortFilename.Length - ExtensionLength)
        End If

        Return ShortFilename
    End Function

#End Region

#Region " DirectoryEntry "

    ''' <summary>
    ''' [Extensions] Return the Path without escaped characters
    ''' </summary>
    ''' <param name="TheDirectoryEntry"></param>
    ''' <returns>The path of the System.DirectoryServices.DirectoryEntry without escape characters</returns>
    ''' <remarks></remarks>
    <Runtime.CompilerServices.Extension()> _
    Public Function CleanPath(ByVal TheDirectoryEntry As DirectoryServices.DirectoryEntry) As String
        Return CleanDirectoryString(TheDirectoryEntry.Path)
    End Function

    ''' <summary>
    ''' [Extensions] Return the Path with escaped characters
    ''' </summary>
    ''' <param name="TheDirectoryEntry"></param>
    ''' <returns>The path of the System.DirectoryServices.DirectoryEntry with escape characters</returns>
    ''' <remarks></remarks>
    <Runtime.CompilerServices.Extension()> _
    Public Function EscapedPath(ByVal TheDirectoryEntry As DirectoryServices.DirectoryEntry) As String
        Return EscapedDirectoryString(TheDirectoryEntry.Path)
    End Function

    ''' <summary>
    ''' [Extensions] Return the Name without escaped characters
    ''' </summary>
    ''' <param name="TheDirectoryEntry"></param>
    ''' <returns>The Name of the System.DirectoryServices.DirectoryEntry without escape characters</returns>
    ''' <remarks></remarks>
    <Runtime.CompilerServices.Extension()> _
    Public Function CleanName(ByVal TheDirectoryEntry As DirectoryServices.DirectoryEntry) As String
        Return CleanDirectoryString(TheDirectoryEntry.Name)
    End Function

#End Region

#Region " SearchResult "

    ''' <summary>
    ''' [Extensions] Return the Path without escaped characters
    ''' </summary>
    ''' <param name="TheSearchResult"></param>
    ''' <returns>The path of the System.DirectoryServices.SearchResult without escape characters</returns>
    ''' <remarks></remarks>
    <Runtime.CompilerServices.Extension()> _
    Public Function CleanPath(ByVal TheSearchResult As DirectoryServices.SearchResult) As String
        Return CleanDirectoryString(TheSearchResult.Path)
    End Function

    ''' <summary>
    ''' [Extensions] Return the Path with escaped characters
    ''' </summary>
    ''' <param name="TheSearchResult"></param>
    ''' <returns>The path of the System.DirectoryServices.SearchResult with escape characters</returns>
    ''' <remarks></remarks>
    <Runtime.CompilerServices.Extension()> _
    Public Function EscapedPath(ByVal TheSearchResult As DirectoryServices.SearchResult) As String
        Return EscapedDirectoryString(TheSearchResult.Path)
    End Function

#End Region

#Region " <<- Helper methods ->> "

    Private Function EscapedDirectoryString(ByVal InputString As String) As String
        Dim ReturnPath As String = RemoveServerFromDN(InputString)

        ' All illegal characters must be prefixed by the escape character \
        ReturnPath = ReturnPath.Replace("\", "\\")
        ReturnPath = ReturnPath.Replace("/", "\/")
        ReturnPath = ReturnPath.Replace("+", "\+")
        ReturnPath = ReturnPath.Replace("<", "\<")
        ReturnPath = ReturnPath.Replace(">", "\>")
        ReturnPath = ReturnPath.Replace(";", "\;")
        ReturnPath = ReturnPath.Replace("#", "\#")
        ReturnPath = ReturnPath.Replace(",", "\,")
        ReturnPath = ReturnPath.Replace("=", "\=")
        ReturnPath = ReturnPath.Replace(Chr(34), "\" & Chr(34))

        If InputString.ToUpper.StartsWith("LDAP://") Then ReturnPath = "LDAP://" & _OriginalDirectoryServerAndPort & "/" & ReturnPath

        Return ReturnPath
    End Function

    Private Function CleanDirectoryString(ByVal InputString As String) As String
        Dim ReturnPath As String = InputString

        ' All illegal characters must be prefixed by the escape character \
        ReturnPath = ReturnPath.Replace("\\", "\")
        ReturnPath = ReturnPath.Replace("\/", "/")
        ReturnPath = ReturnPath.Replace("\+", "+")
        ReturnPath = ReturnPath.Replace("\<", "<")
        ReturnPath = ReturnPath.Replace("\>", ">")
        ReturnPath = ReturnPath.Replace("\;", ";")
        ReturnPath = ReturnPath.Replace("\#", "#")
        ReturnPath = ReturnPath.Replace("\,", ",")
        ReturnPath = ReturnPath.Replace("\=", "=")
        ReturnPath = ReturnPath.Replace("\" & Chr(34), Chr(34))

        Return ReturnPath
    End Function

    Private Function RemoveServerFromDN(ByVal DNPath As String) As String
        Dim ResultString As String = ""
        Dim ServerAndPort As String = ""

        DNPath &= ""

        If DNPath.ToUpper.StartsWith("LDAP://") Then
            ResultString = DNPath.Substring(7)
            Dim SlashPosition As Int32 = ResultString.IndexOf("/")

            ServerAndPort = ResultString.Substring(0, SlashPosition)
            ResultString = ResultString.Substring(SlashPosition + 1)
        Else
            ResultString = DNPath
        End If

        _OriginalDirectoryServerAndPort = ServerAndPort

        Return ResultString
    End Function

#End Region

End Module

