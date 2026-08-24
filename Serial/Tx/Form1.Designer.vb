<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTransmit
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmTransmit))
        Me.btnSend = New System.Windows.Forms.Button
        Me.lblFileCount = New System.Windows.Forms.Label
        Me.pbrFile = New System.Windows.Forms.ProgressBar
        Me.WorkerThread = New System.ComponentModel.BackgroundWorker
        Me.cmbPorts = New System.Windows.Forms.ComboBox
        Me.SuspendLayout()
        '
        'btnSend
        '
        Me.btnSend.Location = New System.Drawing.Point(139, 10)
        Me.btnSend.Name = "btnSend"
        Me.btnSend.Size = New System.Drawing.Size(74, 22)
        Me.btnSend.TabIndex = 0
        Me.btnSend.Text = "Send"
        Me.btnSend.UseVisualStyleBackColor = True
        '
        'lblFileCount
        '
        Me.lblFileCount.AutoSize = True
        Me.lblFileCount.Location = New System.Drawing.Point(12, 56)
        Me.lblFileCount.Name = "lblFileCount"
        Me.lblFileCount.Size = New System.Drawing.Size(91, 13)
        Me.lblFileCount.TabIndex = 1
        Me.lblFileCount.Text = "File send count: 0"
        '
        'pbrFile
        '
        Me.pbrFile.Location = New System.Drawing.Point(12, 39)
        Me.pbrFile.Name = "pbrFile"
        Me.pbrFile.Size = New System.Drawing.Size(201, 14)
        Me.pbrFile.TabIndex = 2
        '
        'WorkerThread
        '
        Me.WorkerThread.WorkerReportsProgress = True
        Me.WorkerThread.WorkerSupportsCancellation = True
        '
        'cmbPorts
        '
        Me.cmbPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbPorts.FormattingEnabled = True
        Me.cmbPorts.Location = New System.Drawing.Point(12, 12)
        Me.cmbPorts.Name = "cmbPorts"
        Me.cmbPorts.Size = New System.Drawing.Size(121, 21)
        Me.cmbPorts.Sorted = True
        Me.cmbPorts.TabIndex = 3
        '
        'frmTransmit
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(220, 76)
        Me.Controls.Add(Me.cmbPorts)
        Me.Controls.Add(Me.pbrFile)
        Me.Controls.Add(Me.lblFileCount)
        Me.Controls.Add(Me.btnSend)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmTransmit"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "PABX Emulator"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnSend As System.Windows.Forms.Button
    Friend WithEvents lblFileCount As System.Windows.Forms.Label
    Friend WithEvents pbrFile As System.Windows.Forms.ProgressBar
    Friend WithEvents WorkerThread As System.ComponentModel.BackgroundWorker
    Friend WithEvents cmbPorts As System.Windows.Forms.ComboBox

End Class
