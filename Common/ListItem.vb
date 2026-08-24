Public Class ListItem

    Private _Attributes As New Collections.Generic.List(Of CustomAttribute)

    Public Property Attributes() As Collections.Generic.List(Of CustomAttribute)
        Get
            Return _Attributes
        End Get
        Set(ByVal value As Collections.Generic.List(Of CustomAttribute))
            _Attributes = value
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return _Attributes.Item(0).Value(0)
    End Function

    Public Overloads Function ToString(ByVal WithName As Boolean, ByVal WithDescription As Boolean) As String
        Dim ReturnValue As String = ""

        If Not WithName And Not WithDescription Then ' Return nothing
            ReturnValue = Nothing
        End If

        If WithName And Not WithDescription Then ' Return Name only
            ReturnValue = _Attributes.Item(0).Value(0)
        End If

        If Not WithName And WithDescription Then ' Return Description only
            ReturnValue = _Attributes.Item(1).Value(0)
        End If

        If WithName And WithDescription Then ' Return Name (Description)
            If _Attributes.Item(1).Value.Count = 0 Then ' No description value
                ReturnValue = _Attributes.Item(0).Value(0)
            Else
                ReturnValue = _Attributes.Item(0).Value(0) & " (" & _Attributes.Item(1).Value(0) & ")"
            End If
        End If

        Return ReturnValue
    End Function

End Class
