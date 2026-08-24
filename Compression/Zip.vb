#Region " Imports "

Imports System.IO
Imports System.IO.Packaging
Imports System.IO.Packaging.Package
Imports System.IO.Packaging.ZipPackage
Imports ICSharpCode.SharpZipLib
Imports ICSharpCode.SharpZipLib.Zip
Imports Core.Common

#End Region

''' <summary>
''' Compression libraries
''' </summary>
''' <remarks></remarks>
Public Class Zip

    ''' <summary>
    ''' Decompress the given file into the same directory
    ''' </summary>
    ''' <param name="ZipFilename"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function DecompressFile(ByVal ZipFilename As String) As Boolean
        Dim fi As New FileInfo(ZipFilename)
        Dim SourceDirectory As String = fi.DirectoryName

        Return DecompressFile(ZipFilename, SourceDirectory)
    End Function

    ''' <summary>
    ''' Decompress the given file into the provided directory
    ''' </summary>
    ''' <param name="ZipFilename"></param>
    ''' <param name="DestinationDirectory"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function DecompressFile(ByVal ZipFilename As String, ByVal DestinationDirectory As String) As Boolean
        Dim Result As Boolean = False

        Try
            ' FastZip does not implement IDisposable, so no 'Using'
            Dim ZipFile As New FastZip
            ZipFile.ExtractZip(ZipFilename, DestinationDirectory, "")

            ZipFile = Nothing
            Result = True
        Catch ex As Exception
            Result = False
        End Try

        Return Result
    End Function

    ''' <summary>
    ''' Compress the given file, storing the result with a .zip extension
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CompressFile(ByVal Filename As String, Optional ByVal DeleteOriginalFile As Boolean = False) As Boolean
        Dim fi As New FileInfo(Filename)
        Dim ShortFilename As String = fi.Name
        Dim DestinationFilename As String = ShortFilename.Substring(0, ShortFilename.LastIndexOf(".")) & ".zip"

        Return CompressFile(Filename, DestinationFilename, True, False, DeleteOriginalFile)
    End Function

    ''' <summary>
    ''' Compress multiple files, storing the result with a .zip extension
    ''' </summary>
    ''' <returns>False if error occured</returns>
    ''' <remarks></remarks>
    Public Shared Function CompressFiles(ByVal FileNames As ArrayList, ByVal ZipFileName As String, ByVal OverwriteFile As Boolean, Optional ByVal ReturnErrorOnFileNotFound As Boolean = False, Optional ByVal DeleteOriginalFile As Boolean = False) As Boolean

        If ZipFileName.Trim.Length = 0 Then
            ZipFileName = Common.Functions.ConvertGUIDToString(Guid.NewGuid.ToByteArray)
        End If

        Try
            Dim DestinationFilename As String = ZipFileName
            If Not DestinationFilename.ToLower.Trim.EndsWith(".zip") Then
                DestinationFilename &= ".zip"
            End If

            Dim strmZipOutputStream As ZipOutputStream = Nothing

            If Not File.Exists(ZipFileName) Or (File.Exists(ZipFileName) And OverwriteFile) Then
                'If File.Exists(DestinationFilename) = False And (Not OverwriteFile) Then
                strmZipOutputStream = New ZipOutputStream(File.Create(DestinationFilename))

                ' Compression Level: 0 (none) - 9 (max)
                strmZipOutputStream.SetLevel(9)

                For Each Filename As String In FileNames
                    Dim strmFile As FileStream

                    If File.Exists(Filename) Then
                        strmFile = File.OpenRead(Filename)

                        Dim ByteBuffer(strmFile.Length - 1) As Byte
                        strmFile.Read(ByteBuffer, 0, ByteBuffer.Length)

                        Dim fi As New FileInfo(Filename)

                        Dim objZipEntry As ZipEntry = New ZipEntry(fi.Name)
                        objZipEntry.DateTime = DateTime.Now
                        objZipEntry.Size = strmFile.Length
                        strmFile.Close()

                        strmZipOutputStream.PutNextEntry(objZipEntry)
                        strmZipOutputStream.Write(ByteBuffer, 0, ByteBuffer.Length)

                        ' Deletes the original file
                        If DeleteOriginalFile Then
                            File.Delete(Filename)
                        End If
                    Else
                        ' The file could not be found or does not exist.
                        If ReturnErrorOnFileNotFound Then
                            strmZipOutputStream.Finish()
                            strmZipOutputStream.Close()
                            strmZipOutputStream = Nothing
                            Throw New FileNotFoundException("CompressFiles cannot find file " & Filename)
                            Return False
                        End If
                    End If
                Next
                strmZipOutputStream.Finish()
                strmZipOutputStream.Close()
                strmZipOutputStream = Nothing
            End If

            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

    ''' <summary>
    ''' Compress a single file, storing the result with a .zip extension
    ''' </summary>
    ''' <returns>False if error occured</returns>
    ''' <remarks></remarks>
    Public Shared Function CompressFile(ByVal FileName As String, ByVal ZipFileName As String, ByVal OverwriteFile As Boolean, Optional ByVal ReturnErrorOnFileNotFound As Boolean = False, Optional ByVal DeleteOriginalFile As Boolean = False) As Boolean

        If ZipFileName.Trim.Length = 0 Then
            ZipFileName = Common.Functions.ConvertGUIDToString(Guid.NewGuid.ToByteArray)
        End If

        Try
            Dim DestinationFilename As String = ZipFileName
            If Not DestinationFilename.ToLower.Trim.EndsWith(".zip") Then
                DestinationFilename &= ".zip"
            End If

            Dim strmZipOutputStream As ZipOutputStream = Nothing

            If Not File.Exists(ZipFileName) Or (File.Exists(ZipFileName) And OverwriteFile) Then

                ' Firstly, if the file DOES exist, delete it.
                If File.Exists(ZipFileName) Then
                    File.Delete(ZipFileName)
                End If
                strmZipOutputStream = New ZipOutputStream(File.Create(DestinationFilename))

                ' Compression Level: 0 (none) - 9 (max)
                strmZipOutputStream.SetLevel(9)

                Dim strmFile As FileStream

                If File.Exists(FileName) Then
                    strmFile = File.OpenRead(FileName)

                    Dim ByteBuffer(strmFile.Length - 1) As Byte
                    strmFile.Read(ByteBuffer, 0, ByteBuffer.Length)

                    Dim fi As New FileInfo(FileName)

                    Dim objZipEntry As ZipEntry = New ZipEntry(fi.Name)
                    objZipEntry.DateTime = DateTime.Now
                    objZipEntry.Size = strmFile.Length
                    strmFile.Close()

                    strmZipOutputStream.PutNextEntry(objZipEntry)
                    strmZipOutputStream.Write(ByteBuffer, 0, ByteBuffer.Length)

                    strmFile.close()

                    If DeleteOriginalFile Then
                        File.Delete(FileName)
                    End If
                Else
                    ' The file could not be found or does not exist.
                    If ReturnErrorOnFileNotFound Then
                        strmZipOutputStream.Finish()
                        strmZipOutputStream.Close()
                        strmZipOutputStream = Nothing
                        Throw New FileNotFoundException("CompressFile cannot find file " & FileName)
                        Return False
                    End If
                End If
                strmZipOutputStream.Finish()
                strmZipOutputStream.Close()
                strmZipOutputStream = Nothing
            End If

            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

    '''' <summary>
    '''' Compress the given file, storing the result with the given filename
    '''' </summary>
    '''' <param name="Filename"></param>
    '''' <param name="DestinationFilename"></param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Public Shared Function CompressFile(ByVal Filename As String, ByVal DestinationFilename As String) As Boolean
    '    Dim Result As Boolean = False
    '    Dim fi As New FileInfo(Filename)
    '    Dim SourceDirectory As String = fi.DirectoryName

    '    If Not SourceDirectory.EndsWith("\") Then SourceDirectory &= "\"

    '    Try
    '        Dim oZip As New FastZip

    '        oZip.CreateZip(DestinationFilename, SourceDirectory, False, fi.Name)

    '        Result = True
    '    Catch ex As Exception
    '        Result = False
    '    End Try

    '    Return Result
    'End Function

    ''' <summary>
    ''' Create or open the given filename as a Windows Package (zipfile)
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <param name="Mode"></param>
    ''' <param name="Access"></param>
    ''' <param name="Share"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetPackage(ByVal Filename As String, ByVal Mode As FileMode, ByVal Access As FileAccess, ByVal Share As FileShare) As Packaging.Package
        Dim Zip As Packaging.Package = Packaging.ZipPackage.Open(Filename, Mode, Access, Share)

        Return Zip
    End Function

    ''' <summary>
    ''' Add the given file to the provided Windows Package (zipfile)
    ''' </summary>
    ''' <param name="PackageFile"></param>
    ''' <param name="FilenameToAdd"></param>
    ''' <remarks></remarks>
    Public Shared Sub AddFileToPackage(ByVal PackageFile As Packaging.Package, ByVal FilenameToAdd As String)
        AddFileToPackage(PackageFile, FilenameToAdd, CompressionOption.Maximum)
    End Sub

    ''' <summary>
    ''' Add the given file to the provided Windows Package (zipfile) with the specified compression option
    ''' </summary>
    ''' <param name="ZipPackage"></param>
    ''' <param name="FilenameToAdd"></param>
    ''' <param name="Compression"></param>
    ''' <remarks></remarks>
    Public Shared Sub AddFileToPackage(ByVal ZipPackage As Package, ByVal FilenameToAdd As String, ByVal Compression As CompressionOption)
        'Validate if the file exists
        If Not File.Exists(FilenameToAdd) Then
            Throw New FileNotFoundException("AddFileToPackage cannot process file " & FilenameToAdd)
        End If

        'Create a URI from the filename to zip (to ensure the name is valid)
        Dim partURI As New Uri(FilenameToAdd, UriKind.Relative)
        'Create a Package Part
        Dim pkgPart As PackagePart = ZipPackage.CreatePart(partURI, Net.Mime.MediaTypeNames.Application.Zip, Compression)
        'Read the file into a byte array
        Dim arrBuffer As Byte() = File.ReadAllBytes(FilenameToAdd)
        'Add the array of byte to the Package
        pkgPart.GetStream().Write(arrBuffer, 0, arrBuffer.Length)
    End Sub

    ''' <summary>
    ''' Decompress the given Windows Package (zipfile) to the specified directory
    ''' </summary>
    ''' <param name="PackageFilename"></param>
    ''' <param name="OutputPath"></param>
    ''' <remarks></remarks>
    Public Shared Sub DecompressPackage(ByVal PackageFilename As String, ByVal OutputPath As String)
        Using pkgMain As Package = Package.Open(PackageFilename, FileMode.Open, FileAccess.Read)
            For Each pkgPart As PackagePart In pkgMain.GetParts()
                Dim strTarget As String = Path.Combine(OutputPath, pkgPart.Uri.ToString)

                Using stmSource As Stream = pkgPart.GetStream(FileMode.Open, FileAccess.Read)
                    Using stmDestination As Stream = File.OpenWrite(strTarget)
                        Dim arrBuffer(10000) As Byte
                        Dim intRead As Integer = 0

                        intRead = stmSource.Read(arrBuffer, 0, arrBuffer.Length)
                        While intRead > 0
                            stmDestination.Write(arrBuffer, 0, intRead)
                            intRead = stmSource.Read(arrBuffer, 0, arrBuffer.Length)
                        End While

                    End Using
                End Using
            Next
        End Using
    End Sub

End Class
