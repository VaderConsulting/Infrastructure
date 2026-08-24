Public Class Code
    Private m_Cleartext As String = ""
    Private m_Encoded As String = ""

    Public Property Cleartext() As String
        Get
            Return m_Cleartext
        End Get
        Set(ByVal value As String)
            m_Cleartext = value
        End Set
    End Property

    Public Property EncodedValue() As String
        Get
            Return m_Encoded
        End Get
        Set(ByVal value As String)
            m_Encoded = value
        End Set
    End Property

End Class

