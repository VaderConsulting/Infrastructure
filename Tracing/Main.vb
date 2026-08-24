Imports System.Windows
Imports System.Windows.Forms

Public Class Main

    ''' <summary>
    ''' Enable or Disable Debug tracing
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum TracingSwitch
        [On] = 1
        [Off] = 2
    End Enum

    Private m_TraceListener As TraceListener

    Public Function StartTracing() As Boolean

        ' Set up Error handling.
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf UnhandledErrorHandler
        AddHandler Application.ThreadException, AddressOf UnhandledFormErrorHandler

        m_TraceListener = New TraceListener
        Trace.Listeners.Add(m_TraceListener)

        Dim TraceForm As Form = m_TraceListener.View

        TraceForm.Show()

        Return True

    End Function

    Private Sub UnhandledErrorHandler(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)
        Dim Ex As Exception

        Ex = e.ExceptionObject
        Trace.WriteLine("Unhandled exception:")
        Trace.WriteLine(Ex.StackTrace)
    End Sub

    Private Sub UnhandledFormErrorHandler(ByVal sender As Object, ByVal e As Threading.ThreadExceptionEventArgs)
        Trace.WriteLine("Unhandled Form exception:")
        Trace.WriteLine(e.Exception.StackTrace)
    End Sub



End Class
