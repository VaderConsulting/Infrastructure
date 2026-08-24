Public Class TraceForm
    Implements ITrace

    Private Delegate Sub AddToListboxCallback(ByVal Text As String)

    Public Sub LogMessage(ByVal Message As String) Implements ITrace.LogMessage
        ' We have to make this thread-safe, so use Callbacks and Invoke
        If Me.lstTrace.InvokeRequired Then
            Dim Callback As New AddToListboxCallback(AddressOf LogMessage)
            Me.Invoke(Callback, New Object() {Message})
        Else
            Dim FullMessage As String = Date.Now.ToString("yyyy-MM-dd HH:mm:ss tt") & ": " & Message

            If lstTrace.Items.Count = 0 Then
                lstTrace.Items.Add(FullMessage)
            Else
                Dim LastMessage As String = lstTrace.Items(lstTrace.Items.Count - 1).ToString
                If LastMessage <> FullMessage Then
                    ' Add the log message.
                    lstTrace.Items.Add(FullMessage)

                    If lstTrace.Items.Count > 1000 Then
                        lstTrace.Items.RemoveAt(0)
                    End If
                End If
            End If

            ' Scroll to the bottom of the list.
            lstTrace.SelectedIndex = lstTrace.Items.Count - 1
        End If
    End Sub

    Private Sub TraceForm_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason = Windows.Forms.CloseReason.UserClosing Then
            e.Cancel = True
        End If
    End Sub

    Private Sub TraceForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Text = My.Application.Info.AssemblyName & " " & My.Application.Info.Version.ToString & " Trace Window"
        LogMessage("Tracing started")
    End Sub

    Protected Overrides Sub Finalize()
        LogMessage("Tracing ended")
        MyBase.Finalize()
    End Sub

    Private Sub btnClipboard_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClipboard.Click
        Dim Text As String = ""

        For ItemCounter As Int16 = 0 To lstTrace.Items.Count - 1
            Text &= lstTrace.Items(ItemCounter).ToString & vbCrLf
        Next

        My.Computer.Clipboard.SetText(Text)

        MsgBox("Debug text copied to clipboard", MsgBoxStyle.OkOnly)

    End Sub

End Class