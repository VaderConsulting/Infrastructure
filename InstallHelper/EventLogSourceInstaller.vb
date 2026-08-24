Imports System.Diagnostics
Imports System.Configuration.Install
Imports System.ComponentModel

''' <summary>
''' Installs the EventLog Source, thus permitting your application to write to the event log.
''' Not required for reading from the Event log.
''' </summary>
''' <remarks>To use:  Run "InstallUtil.exe InstallHelper.dll"</remarks>
<RunInstaller(True)> _
Public Class EventLogSourceInstaller
    Inherits Installer

    Private m_EventLogInstaller As EventLogInstaller

    Public Sub New(Optional ByVal Source As String = "", Optional ByVal LogName As String = "Application")
        If Source = "" Then
            Source = My.Application.Info.CompanyName.ToString & " " & My.Application.Info.ProductName.ToString
        End If

        ' Create an instance of 'EventLogInstaller'.
        m_EventLogInstaller = New EventLogInstaller()

        ' Set the 'Source' of the event log to be created.
        m_EventLogInstaller.Source = Source

        ' Set the 'Log' that the source is created in.
        m_EventLogInstaller.Log = LogName

        ' Add to 'InstallerCollection'.
        Installers.Add(m_EventLogInstaller)

    End Sub

End Class
