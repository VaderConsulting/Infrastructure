Imports System.Diagnostics

Public Class TraceListener
    Inherits DefaultTraceListener

    Public View As TraceForm

    Public Sub New()
        MyBase.New()
        Me.View = New TraceForm
    End Sub

    Public Sub New(ByVal TraceForm As ITrace)
        MyBase.New()

        If Not TypeOf TraceForm Is FormatException Then
            Throw New InvalidCastException("TraceListener must be used on a Form instance.")
        End If

        Me.View = TraceForm
    End Sub

    Public Overloads Overrides Sub Write(ByVal message As String)
        If NeedIndent Then
            WriteIndent()
            'NeedIndent = True
        End If
        View.LogMessage(message)
    End Sub

    Public Overloads Overrides Sub WriteLine(ByVal message As String)
        If NeedIndent Then
            WriteIndent()
            'NeedIndent = True
        End If
        Write(message)
    End Sub

End Class
