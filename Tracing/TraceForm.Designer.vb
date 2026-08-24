<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TraceForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TraceForm))
        Me.btnClipboard = New System.Windows.Forms.Button
        Me.lblLast = New System.Windows.Forms.Label
        Me.lstTrace = New System.Windows.Forms.ListBox
        Me.SuspendLayout()
        '
        'btnClipboard
        '
        Me.btnClipboard.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClipboard.Location = New System.Drawing.Point(367, 151)
        Me.btnClipboard.Name = "btnClipboard"
        Me.btnClipboard.Size = New System.Drawing.Size(105, 23)
        Me.btnClipboard.TabIndex = 7
        Me.btnClipboard.Text = "Copy to Clipboard"
        Me.btnClipboard.UseVisualStyleBackColor = True
        '
        'lblLast
        '
        Me.lblLast.AutoSize = True
        Me.lblLast.Location = New System.Drawing.Point(12, 6)
        Me.lblLast.Name = "lblLast"
        Me.lblLast.Size = New System.Drawing.Size(92, 13)
        Me.lblLast.TabIndex = 6
        Me.lblLast.Text = "Last 1000 events:"
        '
        'lstTrace
        '
        Me.lstTrace.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstTrace.FormattingEnabled = True
        Me.lstTrace.Location = New System.Drawing.Point(12, 22)
        Me.lstTrace.Name = "lstTrace"
        Me.lstTrace.Size = New System.Drawing.Size(460, 121)
        Me.lstTrace.TabIndex = 5
        '
        'TraceForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(484, 180)
        Me.Controls.Add(Me.btnClipboard)
        Me.Controls.Add(Me.lblLast)
        Me.Controls.Add(Me.lstTrace)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "TraceForm"
        Me.Text = "Trace Output"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnClipboard As System.Windows.Forms.Button
    Friend WithEvents lblLast As System.Windows.Forms.Label
    Friend WithEvents lstTrace As System.Windows.Forms.ListBox
End Class
