Imports System.Security.Cryptography
Imports System.Text
Imports System.IO

''' <summary>
''' Encryption routines
''' </summary>
''' <remarks></remarks>
Public Class Functions

    ''' <summary>
    ''' Decrypts the given string from a format suitable for output to XML files
    ''' </summary>
    ''' <param name="stringToDecrypt">Input string to decrypt</param>
    ''' <param name="EncryptionKey">The Key used to decrypt the string</param>
    ''' <returns>Decrypted String</returns>
    ''' <remarks>V1.0 D. Robinson</remarks>
    Public Shared Function XMLDecrypt(ByVal stringToDecrypt As String, ByVal EncryptionKey As String) As String
        Dim inputByteArray(stringToDecrypt.Length) As Byte
        Dim RSAkey() As Byte = {}
        Dim IV() As Byte = {&H12, &H34, &H56, &H78, &H90, &HAB, &HCD, &HEF}
        Dim DecryptedString As String = ""
        ' Note: The DES CryptoService only accepts certain key byte lengths
        ' We are going to make things easy by insisting on an 8 byte legal key length
        If EncryptionKey.Length < 8 Then
            Throw New Exception("The provided EncryptionKey must be 8 or more Characters")
        End If
        'If stringToDecrypt.Trim.Length = 0 Then Return ""
        RSAkey = System.Text.Encoding.UTF8.GetBytes(Microsoft.VisualBasic.Left(EncryptionKey, 8))
        Dim des As New DESCryptoServiceProvider()
        ' we have a base 64 encoded string so first must decode to regular unencoded (encrypted) string
        inputByteArray = Convert.FromBase64String(stringToDecrypt)
        ' now decrypt the regular string
        Dim ms As New MemoryStream()
        Dim cs As New CryptoStream(ms, des.CreateDecryptor(RSAkey, IV), CryptoStreamMode.Write)
        cs.Write(inputByteArray, 0, inputByteArray.Length)
        cs.FlushFinalBlock()
        Dim encoding As System.Text.Encoding = System.Text.Encoding.UTF8
        DecryptedString = encoding.GetString(ms.ToArray())

        Return DecryptedString

    End Function

    ''' <summary>
    ''' Encrypts the given string to a format suitable for XML files
    ''' </summary>
    ''' <param name="stringToEncrypt">Input string to encrypt</param>
    ''' <param name="EncryptionKey">The Key used to decrypt the string</param>
    ''' <returns>The encrypted string</returns>
    ''' <remarks>V1.0 D. Robinson</remarks>
    Public Shared Function XMLEncrypt(ByVal StringToEncrypt As String, ByVal EncryptionKey As String) As String
        Dim RSAkey() As Byte = {}
        Dim IV() As Byte = {&H12, &H34, &H56, &H78, &H90, &HAB, &HCD, &HEF}
        Dim EncryptedString As String = ""

        If EncryptionKey.Length < 8 Then
            Throw New Exception("The provided EncryptionKey must be 8 or more Characters")
        End If

        RSAkey = System.Text.Encoding.UTF8.GetBytes(Microsoft.VisualBasic.Left(EncryptionKey, 8))
        Dim des As New DESCryptoServiceProvider()
        ' convert our input string to a byte array
        Dim inputByteArray() As Byte = Encoding.UTF8.GetBytes(StringToEncrypt)
        'now encrypt the bytearray
        Dim ms As New MemoryStream()
        Dim cs As New CryptoStream(ms, des.CreateEncryptor(RSAkey, IV), CryptoStreamMode.Write)
        cs.Write(inputByteArray, 0, inputByteArray.Length)
        cs.FlushFinalBlock()
        ' now return the byte array as a "safe for XMLDOM" Base64 String
        EncryptedString = Convert.ToBase64String(ms.ToArray())

        Return EncryptedString
    End Function

    ''' <summary>
    ''' Simple Decryption routine.  Do not use for sensitive information
    ''' </summary>
    ''' <param name="InputText">Input string to en\decrypt</param>
    ''' <returns>Output string en\decrypted</returns>
    ''' <remarks>V1.0  D. Robinson</remarks>
    Public Shared Function SimpleCrypt(ByVal InputText As String) As String
        Dim OutputText As String

        ' Encrypts/decrypts the passed string using a simple ASCII value-swapping algorithm
        Try
            Dim strTempChar As String = "", i As Integer
            For i = 1 To Len(InputText)
                If Asc(Mid$(InputText, i, 1)) < 128 Then
                    strTempChar = _
              CType(Asc(Mid$(InputText, i, 1)) + 128, String)
                ElseIf Asc(Mid$(InputText, i, 1)) > 128 Then
                    strTempChar = _
              CType(Asc(Mid$(InputText, i, 1)) - 128, String)
                End If
                Mid$(InputText, i, 1) = _
                    Chr(CType(strTempChar, Integer))
            Next i
            OutputText = InputText

            Return OutputText
        Catch ex As Exception

        End Try

        Return ""
    End Function

    ''' <summary>
    ''' 3DES Decryption
    ''' </summary>
    ''' <param name="InputString">String to decrypt</param>
    ''' <param name="CryptoKey">The Key used to decrypt the string</param>
    ''' <returns>The Decrypted string</returns>
    ''' <remarks>V1.0 D. Robinson</remarks>
    Public Shared Function Decrypt3DES(ByVal InputString As String, ByVal CryptoKey As String) As String
        Dim buffer() As Byte
        Dim CryptoClass As New TripleDESCryptoServiceProvider
        Dim CryptoProvider As New MD5CryptoServiceProvider
        Dim Vector() As Byte = {240, 3, 45, 29, 0, 76, 173, 59}
        Dim DecryptedString As String

        Try
            buffer = Convert.FromBase64String(InputString)
            CryptoClass.Key = CryptoProvider.ComputeHash(ASCIIEncoding.ASCII.GetBytes(CryptoKey))
            CryptoClass.IV = Vector
            DecryptedString = Encoding.ASCII.GetString(CryptoClass.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length()))

            Return DecryptedString
        Catch ex As Exception

        Finally
            CryptoClass.Clear()
            CryptoProvider.Clear()
            CryptoClass = Nothing
            CryptoProvider = Nothing
        End Try

        Return ""

    End Function

    ''' <summary>
    ''' 3DES Encryption
    ''' </summary>
    ''' <param name="InputString">String to encrypt</param>
    ''' <param name="CryptoKey">The Key used to encrypt the string</param>
    ''' <returns>The encrypted string</returns>
    ''' <remarks>V1.0 D. Robinson</remarks>
    Public Shared Function Encrypt3DES(ByVal InputString As String, ByVal CryptoKey As String) As String
        Dim CryptoClass As New TripleDESCryptoServiceProvider
        Dim CryptoProvider As New MD5CryptoServiceProvider
        Dim Buffer() As Byte
        Dim Vector() As Byte = {240, 3, 45, 29, 0, 76, 173, 59}
        Dim EncryptedString As String

        Try
            Buffer = System.Text.Encoding.ASCII.GetBytes(InputString)
            CryptoClass.Key = CryptoProvider.ComputeHash(ASCIIEncoding.ASCII.GetBytes(CryptoKey))
            CryptoClass.IV = Vector
            InputString = Convert.ToBase64String(CryptoClass.CreateEncryptor().TransformFinalBlock(Buffer, 0, Buffer.Length()))
            EncryptedString = InputString

            Return EncryptedString
        Catch ex As Exception

        Finally
            CryptoClass.Clear()
            CryptoProvider.Clear()
            CryptoClass = Nothing
            CryptoProvider = Nothing
        End Try

        Return ""
    End Function

    ''' <summary>
    ''' Rijndael 128 bit encryption
    ''' </summary>
    ''' <param name="StringToBeEncrypted">String To Be Encrypted</param>
    ''' <param name="EncryptionKey">Encryption Key</param>
    ''' <returns>EncryptedResult</returns>
    ''' <remarks>V1.0 D. Robinson</remarks>
    Public Shared Function EncryptString128Bit(ByVal StringToBeEncrypted As String, ByVal EncryptionKey As String) As String
        Dim bytValue() As Byte
        Dim bytKey() As Byte
        Dim bytEncoded() As Byte = {}
        Dim bytIV() As Byte = {121, 241, 10, 1, 132, 74, 11, 39, 255, 91, 45, 78, 14, 211, 22, 62}
        Dim intLength As Integer
        Dim intRemaining As Integer
        Dim objMemoryStream As New MemoryStream()
        Dim objCryptoStream As CryptoStream
        Dim objRijndaelManaged As RijndaelManaged
        Dim EncryptedResult As String

        '   **********************************************************************
        '   ******  Strip any null character from string to be encrypted    ******
        '   **********************************************************************

        StringToBeEncrypted = StripNullCharacters(StringToBeEncrypted)

        '   **********************************************************************
        '   ******  Value must be within ASCII range (i.e., no DBCS chars)  ******
        '   **********************************************************************

        bytValue = Encoding.ASCII.GetBytes(StringToBeEncrypted.ToCharArray)

        intLength = Len(EncryptionKey)

        '   ********************************************************************
        '   ******   Encryption Key must be 256 bits long (32 bytes)      ******
        '   ******   If it is longer than 32 bytes it will be truncated.  ******
        '   ******   If it is shorter than 32 bytes it will be padded     ******
        '   ******   with upper-case Xs.                                  ****** 
        '   ********************************************************************

        If intLength >= 32 Then
            EncryptionKey = Strings.Left(EncryptionKey, 32)
        Else
            intLength = Len(EncryptionKey)
            intRemaining = 32 - intLength
            EncryptionKey = EncryptionKey & Strings.StrDup(intRemaining, "X")
        End If

        bytKey = Encoding.ASCII.GetBytes(EncryptionKey.ToCharArray)

        objRijndaelManaged = New RijndaelManaged()

        '   ***********************************************************************
        '   ******  Create the encryptor and write value to it after it is   ******
        '   ******  converted into a byte array                              ******
        '   ***********************************************************************

        Try

            objCryptoStream = New CryptoStream(objMemoryStream, _
              objRijndaelManaged.CreateEncryptor(bytKey, bytIV), _
              CryptoStreamMode.Write)
            objCryptoStream.Write(bytValue, 0, bytValue.Length)

            objCryptoStream.FlushFinalBlock()

            bytEncoded = objMemoryStream.ToArray
            objMemoryStream.Close()
            objCryptoStream.Close()
        Catch ex As Exception
        End Try

        '   ***********************************************************************
        '   ******   Return encryptes value (converted from  byte Array to   ******
        '   ******   a base64 string).  Base64 is MIME encoding)             ******
        '   ***********************************************************************

        EncryptedResult = Convert.ToBase64String(bytEncoded)


        Return EncryptedResult
    End Function

    ''' <summary>
    ''' Rijndael 128 bit decryption
    ''' </summary>
    ''' <param name="StringToBeDecrypted">String To Be Decrypted</param>
    ''' <param name="DecryptionKey">Decryption Key</param>
    ''' <returns>DecryptedResult</returns>
    ''' <remarks>V1.0 D. Robinson</remarks>
    Public Shared Function DecryptString128Bit(ByVal StringToBeDecrypted As String, ByVal DecryptionKey As String) As String
        Dim bytDataToBeDecrypted() As Byte
        Dim bytTemp() As Byte
        Dim bytIV() As Byte = {121, 241, 10, 1, 132, 74, 11, 39, 255, 91, 45, 78, 14, 211, 22, 62}
        Dim objRijndaelManaged As New RijndaelManaged()
        Dim objMemoryStream As MemoryStream
        Dim objCryptoStream As CryptoStream
        Dim bytDecryptionKey() As Byte

        Dim intLength As Integer
        Dim intRemaining As Integer
        Dim strReturnString As String = String.Empty
        Dim DecryptedResult As String

        '   *****************************************************************
        '   ******   Convert base64 encrypted value to byte array      ******
        '   *****************************************************************

        bytDataToBeDecrypted = Convert.FromBase64String(StringToBeDecrypted)

        '   ********************************************************************
        '   ******   Encryption Key must be 256 bits long (32 bytes)      ******
        '   ******   If it is longer than 32 bytes it will be truncated.  ******
        '   ******   If it is shorter than 32 bytes it will be padded     ******
        '   ******   with upper-case Xs.                                  ****** 
        '   ********************************************************************

        intLength = Len(DecryptionKey)

        If intLength >= 32 Then
            DecryptionKey = Strings.Left(DecryptionKey, 32)
        Else
            intLength = Len(DecryptionKey)
            intRemaining = 32 - intLength
            DecryptionKey = DecryptionKey & Strings.StrDup(intRemaining, "X")
        End If

        bytDecryptionKey = Encoding.ASCII.GetBytes(DecryptionKey.ToCharArray)

        ReDim bytTemp(bytDataToBeDecrypted.Length)

        objMemoryStream = New MemoryStream(bytDataToBeDecrypted)

        '   ***********************************************************************
        '   ******  Create the decryptor and write value to it after it is   ******
        '   ******  converted into a byte array                              ******
        '   ***********************************************************************

        Try

            objCryptoStream = New CryptoStream(objMemoryStream, _
               objRijndaelManaged.CreateDecryptor(bytDecryptionKey, bytIV), _
               CryptoStreamMode.Read)

            objCryptoStream.Read(bytTemp, 0, bytTemp.Length)

            objCryptoStream.FlushFinalBlock()
            objMemoryStream.Close()
            objCryptoStream.Close()

        Catch ex As Exception

        End Try

        '   *****************************************
        '   ******   Return decypted value     ******
        '   *****************************************

        DecryptedResult = StripNullCharacters(Encoding.ASCII.GetString(bytTemp))

        Return DecryptedResult
    End Function

    ''' <summary>
    ''' Strips NULL characters
    ''' </summary>
    ''' <param name="StringWithNulls"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function StripNullCharacters(ByVal StringWithNulls As String) As String
        Dim Position As Integer = 0
        Dim StringWithoutNulls As String = ""

        Position = 1
        StringWithoutNulls = StringWithNulls

        Do While Position > 0
            Position = InStr(Position, StringWithNulls, vbNullChar)

            If Position > 0 Then
                StringWithoutNulls = Microsoft.VisualBasic.Left(StringWithoutNulls, Position - 1) & _
                                     Microsoft.VisualBasic.Right(StringWithoutNulls, Len(StringWithoutNulls) - Position)
            End If

            If Position > StringWithoutNulls.Length Then
                Exit Do
            End If
        Loop

        Return StringWithoutNulls

    End Function

End Class
