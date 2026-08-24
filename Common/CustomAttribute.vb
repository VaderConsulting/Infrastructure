Public Class CustomAttribute

    Dim _Name As String = ""
    Dim _Value As New Collections.Specialized.StringCollection

    Public Property Name() As String
        Get
            Return _Name
        End Get
        Set(ByVal value As String)
            _Name = value
        End Set
    End Property

    Public Property [Value]() As Collections.Specialized.StringCollection
        Get
            Return _Value
        End Get
        Set(ByVal value As Collections.Specialized.StringCollection)
            _Value = value
        End Set
    End Property

    Public Sub New()

    End Sub

    Public Sub New(ByVal Name As String)
        _Name = Name
    End Sub

    Public Sub New(ByVal Name As String, ByVal Value As Collections.Specialized.StringCollection)
        _Name = Name
        _Value = Value
    End Sub

    Public Overrides Function ToString() As String
        Return _Name
        'Return MyBase.ToString()
    End Function

End Class
