<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
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
        Me.Label1 = New System.Windows.Forms.Label
        Me.lblReceivedByteCount = New System.Windows.Forms.Label
        Me.lstMessages = New System.Windows.Forms.ListBox
        Me.cmbPorts = New System.Windows.Forms.ComboBox
        Me.btnDial = New System.Windows.Forms.Button
        Me.btnHangup = New System.Windows.Forms.Button
        Me.lblState = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 35)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(107, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Received Byte count"
        '
        'lblReceivedByteCount
        '
        Me.lblReceivedByteCount.AutoSize = True
        Me.lblReceivedByteCount.Location = New System.Drawing.Point(132, 35)
        Me.lblReceivedByteCount.Name = "lblReceivedByteCount"
        Me.lblReceivedByteCount.Size = New System.Drawing.Size(13, 13)
        Me.lblReceivedByteCount.TabIndex = 1
        Me.lblReceivedByteCount.Text = "0"
        '
        'lstMessages
        '
        Me.lstMessages.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstMessages.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lstMessages.FormattingEnabled = True
        Me.lstMessages.ItemHeight = 14
        Me.lstMessages.Location = New System.Drawing.Point(12, 51)
        Me.lstMessages.Name = "lstMessages"
        Me.lstMessages.Size = New System.Drawing.Size(490, 186)
        Me.lstMessages.TabIndex = 2
        '
        'cmbPorts
        '
        Me.cmbPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbPorts.FormattingEnabled = True
        Me.cmbPorts.Location = New System.Drawing.Point(12, 11)
        Me.cmbPorts.Name = "cmbPorts"
        Me.cmbPorts.Size = New System.Drawing.Size(99, 21)
        Me.cmbPorts.TabIndex = 3
        '
        'btnDial
        '
        Me.btnDial.Location = New System.Drawing.Point(135, 9)
        Me.btnDial.Name = "btnDial"
        Me.btnDial.Size = New System.Drawing.Size(75, 23)
        Me.btnDial.TabIndex = 4
        Me.btnDial.Text = "Dial"
        Me.btnDial.UseVisualStyleBackColor = True
        '
        'btnHangup
        '
        Me.btnHangup.Location = New System.Drawing.Point(216, 9)
        Me.btnHangup.Name = "btnHangup"
        Me.btnHangup.Size = New System.Drawing.Size(75, 23)
        Me.btnHangup.TabIndex = 5
        Me.btnHangup.Text = "Hangup"
        Me.btnHangup.UseVisualStyleBackColor = True
        '
        'lblState
        '
        Me.lblState.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblState.AutoSize = True
        Me.lblState.Location = New System.Drawing.Point(12, 242)
        Me.lblState.Name = "lblState"
        Me.lblState.Size = New System.Drawing.Size(73, 13)
        Me.lblState.TabIndex = 6
        Me.lblState.Text = "Disconnected"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(514, 264)
        Me.Controls.Add(Me.lblState)
        Me.Controls.Add(Me.btnHangup)
        Me.Controls.Add(Me.btnDial)
        Me.Controls.Add(Me.cmbPorts)
        Me.Controls.Add(Me.lstMessages)
        Me.Controls.Add(Me.lblReceivedByteCount)
        Me.Controls.Add(Me.Label1)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblReceivedByteCount As System.Windows.Forms.Label
    Friend WithEvents lstMessages As System.Windows.Forms.ListBox
    Friend WithEvents cmbPorts As System.Windows.Forms.ComboBox
    Friend WithEvents btnDial As System.Windows.Forms.Button
    Friend WithEvents btnHangup As System.Windows.Forms.Button
    Friend WithEvents lblState As System.Windows.Forms.Label

End Class
