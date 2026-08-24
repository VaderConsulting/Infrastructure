Imports System.Reflection
Imports System.Reflection.Assembly

Public Class ActivationCode

#Region " Private Variables "

    Private m_ProductCode As New Code
    Private m_Feature As New Code
    Private m_ExpiryDate As New Code
    Private m_Value1 As New Code
    Private m_Value2 As New Code
    Private m_Value3 As New Code
    Private m_ClientID As New Code
    Private m_CRC As String = ""
    Private m_EncodedValue As String = ""
    Private m_DecodedValue As String = ""
    Private m_IsValid As Boolean?

#End Region

#Region " Properties "

    Public ReadOnly Property IsValid() As Boolean?
        Get
            Return m_IsValid
        End Get
    End Property

    Public ReadOnly Property EncodedValue() As String
        Get
            Return m_EncodedValue
        End Get
    End Property

    Public ReadOnly Property DecodedValue() As String
        Get
            Return m_DecodedValue
        End Get
    End Property

    Public Property ProductCode() As Code
        Get
            Return m_ProductCode
        End Get
        Set(ByVal value As Code)
            m_ProductCode = value
        End Set
    End Property

    Public Property Feature() As Code
        Get
            Return m_Feature
        End Get
        Set(ByVal value As Code)
            m_Feature = value
        End Set
    End Property

    Public Property ExpiryDate() As Code
        Get
            Return m_ExpiryDate
        End Get
        Set(ByVal value As Code)
            m_ExpiryDate = value
        End Set
    End Property

    Public Property Value1() As Code
        Get
            Return m_Value1
        End Get
        Set(ByVal value As Code)
            m_Value1 = value
        End Set
    End Property

    Public Property Value2() As Code
        Get
            Return m_Value2
        End Get
        Set(ByVal value As Code)
            m_Value2 = value
        End Set
    End Property

    Public Property Value3() As Code
        Get
            Return m_Value3
        End Get
        Set(ByVal value As Code)
            m_Value3 = value
        End Set
    End Property

    Public Property ClientID() As Code
        Get
            Return m_ClientID
        End Get
        Set(ByVal value As Code)
            m_ClientID = value
        End Set
    End Property

    Public ReadOnly Property CRC() As String
        Get
            m_CRC = CreateCRC(m_ProductCode.Cleartext & "-" & _
                          m_Feature.Cleartext & "-" & _
                          m_ExpiryDate.Cleartext & "-" & _
                          m_Value1.Cleartext & "-" & _
                          m_Value2.Cleartext & "-" & _
                          m_Value3.Cleartext & "-" & _
                          m_ClientID.Cleartext _
                         )
            Return m_CRC
        End Get
    End Property

#End Region

#Region " Methods "

    Public Sub Encode()
        m_ProductCode.EncodedValue = ConvertDecToBaseN(m_ProductCode.Cleartext, 36)
        m_Feature.EncodedValue = ConvertDecToBaseN(m_Feature.Cleartext, 36)
        m_ExpiryDate.EncodedValue = ConvertDecToBaseN(Convert.ToDateTime(m_ExpiryDate.Cleartext).ToOADate, 36)
        m_Value1.EncodedValue = ConvertDecToBaseN(m_Value1.Cleartext, 36)
        m_Value2.EncodedValue = ConvertDecToBaseN(m_Value2.Cleartext, 36)
        m_Value3.EncodedValue = ConvertDecToBaseN(m_Value3.Cleartext, 36)
        m_ClientID.EncodedValue = ConvertDecToBaseN(m_ClientID.Cleartext, 36)

        m_CRC = CreateCRC(m_ProductCode.Cleartext & "-" & _
                          m_Feature.Cleartext & "-" & _
                          m_ExpiryDate.Cleartext & "-" & _
                          m_Value1.Cleartext & "-" & _
                          m_Value2.Cleartext & "-" & _
                          m_Value3.Cleartext & "-" & _
                          m_ClientID.Cleartext _
                         )

        Trace.WriteLine("Original values: " & m_ProductCode.Cleartext & "-" & _
                          m_Feature.Cleartext & "-" & _
                          m_ExpiryDate.Cleartext & "-" & _
                          m_Value1.Cleartext & "-" & _
                          m_Value2.Cleartext & "-" & _
                          m_Value3.Cleartext & "-" & _
                          m_ClientID.Cleartext)

        Trace.WriteLine("Computed CRC from source: " & m_CRC)

        m_EncodedValue = m_ProductCode.EncodedValue & "-" & _
                          m_Feature.EncodedValue & "-" & _
                          m_ExpiryDate.EncodedValue & "-" & _
                          m_Value1.EncodedValue & "-" & _
                          m_Value2.EncodedValue & "-" & _
                          m_Value3.EncodedValue & "-" & _
                          m_ClientID.EncodedValue & "-" & _
                          m_CRC

        m_IsValid = True

    End Sub

    Public Sub Decode()
        m_ProductCode.Cleartext = ConvertBaseNToDec(m_ProductCode.EncodedValue, 36)
        m_Feature.Cleartext = ConvertBaseNToDec(m_Feature.EncodedValue, 36)
        m_ExpiryDate.Cleartext = DateTime.FromOADate(ConvertBaseNToDec(m_ExpiryDate.EncodedValue, 36))
        m_Value1.Cleartext = ConvertBaseNToDec(m_Value1.EncodedValue, 36)
        m_Value2.Cleartext = ConvertBaseNToDec(m_Value2.EncodedValue, 36)
        m_Value3.Cleartext = ConvertBaseNToDec(m_Value3.EncodedValue, 36)
        m_ClientID.Cleartext = ConvertBaseNToDec(m_ClientID.EncodedValue, 36)

        Dim ComputedCRC As String = ""

        ComputedCRC = CreateCRC(m_ProductCode.Cleartext & "-" & _
                          m_Feature.Cleartext & "-" & _
                          Convert.ToDateTime(m_ExpiryDate.Cleartext) & "-" & _
                          m_Value1.Cleartext & "-" & _
                          m_Value2.Cleartext & "-" & _
                          m_Value3.Cleartext & "-" & _
                          m_ClientID.Cleartext _
                         )

        If ComputedCRC <> m_CRC Then
            m_IsValid = False
            Err.Raise(vbObjectError + 600, GetExecutingAssembly.GetName.Name, "Supplied product code is invalid.")
        End If

        m_IsValid = True
    End Sub

    Public Function Decode(ByVal Value As String) As String
        'Dim ReturnString As String = ""
        Dim InterimValues() As String = Split(Value, "-")

        If InterimValues.Length <> 8 Then
            Err.Raise(vbObjectError + 600, GetExecutingAssembly.GetName.Name, "Supplied product code is invalid.")
        End If

        m_ProductCode.EncodedValue = InterimValues(0)
        m_Feature.EncodedValue = InterimValues(1)
        m_ExpiryDate.EncodedValue = InterimValues(2)
        m_Value1.EncodedValue = InterimValues(3)
        m_Value2.EncodedValue = InterimValues(4)
        m_Value3.EncodedValue = InterimValues(5)
        m_ClientID.EncodedValue = InterimValues(6)

        m_CRC = InterimValues(7)

        Decode()

        Return m_ProductCode.Cleartext & "-" & _
                          m_Feature.Cleartext & "-" & _
                          Convert.ToDateTime(m_ExpiryDate.Cleartext) & "-" & _
                          m_Value1.Cleartext & "-" & _
                          m_Value2.Cleartext & "-" & _
                          m_Value3.Cleartext & "-" & _
                          m_ClientID.Cleartext

        m_IsValid = True
    End Function

    Private Function ConvertDecToBaseN(ByVal dValue As Double, Optional ByVal byBase As Byte = 16) As String
        Const BASENUMBERS As String = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        Dim Result As String
        Dim Remainder As Double

        Result = ""

        If (byBase < 2) Or (byBase > 36) Then
            Return ""
        End If

        dValue = System.Math.Abs(dValue)

        Do
            Remainder = dValue - (byBase * Int(dValue / byBase))
            Result = Mid(BASENUMBERS, Remainder + 1, 1) & Result
            dValue = Int(dValue / byBase)

        Loop While (dValue > 0)

        Return Result
    End Function

    Private Function ConvertBaseNToDec(ByVal dValue As String, Optional ByVal byBase As Byte = 16) As String
        Const BASENUMBERS As String = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        Dim ReturnNumber As Double
        Dim n As Short

        If (byBase < 2) Or (byBase > 36) Then
            Return ""
        End If

        n = 0

        Do
            ReturnNumber = ((InStr(1, BASENUMBERS, Mid(dValue, Len(dValue) - n, 1)) - 1) * (byBase ^ n)) + ReturnNumber
            n = n + 1
        Loop Until n = Len(dValue)

        Return CStr(ReturnNumber)
    End Function

    Private Function CreateCRC(ByVal InputString As String) As String
        Dim OutputString As String = ""
        Dim CRC As New CRC32

        OutputString = Hex(CRC.Calculate(InputString))

        CRC = Nothing

        Return OutputString
    End Function

    Public Sub Clear()
        m_ProductCode.Cleartext = ""
        m_Feature.Cleartext = ""
        m_ExpiryDate.Cleartext = ""
        m_Value1.Cleartext = ""
        m_Value2.Cleartext = ""
        m_Value3.Cleartext = ""
        m_ClientID.Cleartext = ""

        m_ProductCode.EncodedValue = ""
        m_Feature.EncodedValue = ""
        m_ExpiryDate.EncodedValue = ""
        m_Value1.EncodedValue = ""
        m_Value2.EncodedValue = ""
        m_Value3.EncodedValue = ""
        m_ClientID.EncodedValue = ""

        m_CRC = ""
        m_IsValid = Nothing

        m_EncodedValue = ""
        m_DecodedValue = ""
    End Sub

#End Region

End Class