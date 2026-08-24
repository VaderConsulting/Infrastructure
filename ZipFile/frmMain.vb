Public Class frmMain

    Private m_CommandLineInitiated As Boolean = False
    Private m_Common As Core.Common.Singleton

    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        m_Common = Core.Common.Singleton.GetSingleton

        If Core.Common.Functions.CommandLineInitiated Then
            m_CommandLineInitiated = True
            Dim Arguments() As String = Split(Command, ":")



        End If

    End Sub
End Class
