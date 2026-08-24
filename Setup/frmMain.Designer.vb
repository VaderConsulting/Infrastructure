<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSetup
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
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSetup))
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.lblNoUserSettings = New System.Windows.Forms.Label
        Me.btnContractUserSettings = New System.Windows.Forms.Button
        Me.btnExpandUserSettings = New System.Windows.Forms.Button
        Me.tvwUserSettings = New System.Windows.Forms.TreeView
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.lblNoApplicationSettings = New System.Windows.Forms.Label
        Me.btnContractApplicationSettings = New System.Windows.Forms.Button
        Me.btnExpandApplicationSettings = New System.Windows.Forms.Button
        Me.tvwApplicationSettings = New System.Windows.Forms.TreeView
        Me.btnOK = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.mnuMain = New System.Windows.Forms.MenuStrip
        Me.mnuToolstripMain = New System.Windows.Forms.ToolStripMenuItem
        Me.LoadToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.btnApply = New System.Windows.Forms.Button
        Me.ofdSetup = New System.Windows.Forms.OpenFileDialog
        Me.imlTree = New System.Windows.Forms.ImageList(Me.components)
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.mnuMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.lblNoUserSettings)
        Me.GroupBox1.Controls.Add(Me.btnContractUserSettings)
        Me.GroupBox1.Controls.Add(Me.btnExpandUserSettings)
        Me.GroupBox1.Controls.Add(Me.tvwUserSettings)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 27)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(290, 374)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "User Settings"
        '
        'lblNoUserSettings
        '
        Me.lblNoUserSettings.BackColor = System.Drawing.Color.Gold
        Me.lblNoUserSettings.Location = New System.Drawing.Point(6, 17)
        Me.lblNoUserSettings.Name = "lblNoUserSettings"
        Me.lblNoUserSettings.Size = New System.Drawing.Size(278, 53)
        Me.lblNoUserSettings.TabIndex = 7
        Me.lblNoUserSettings.Text = "No User Settings"
        Me.lblNoUserSettings.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnContractUserSettings
        '
        Me.btnContractUserSettings.Location = New System.Drawing.Point(232, 18)
        Me.btnContractUserSettings.Name = "btnContractUserSettings"
        Me.btnContractUserSettings.Size = New System.Drawing.Size(23, 23)
        Me.btnContractUserSettings.TabIndex = 8
        Me.btnContractUserSettings.Text = "-"
        Me.btnContractUserSettings.UseVisualStyleBackColor = True
        '
        'btnExpandUserSettings
        '
        Me.btnExpandUserSettings.Location = New System.Drawing.Point(261, 18)
        Me.btnExpandUserSettings.Name = "btnExpandUserSettings"
        Me.btnExpandUserSettings.Size = New System.Drawing.Size(23, 23)
        Me.btnExpandUserSettings.TabIndex = 8
        Me.btnExpandUserSettings.Text = "+"
        Me.btnExpandUserSettings.UseVisualStyleBackColor = True
        '
        'tvwUserSettings
        '
        Me.tvwUserSettings.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tvwUserSettings.Location = New System.Drawing.Point(6, 44)
        Me.tvwUserSettings.Name = "tvwUserSettings"
        Me.tvwUserSettings.Size = New System.Drawing.Size(278, 324)
        Me.tvwUserSettings.TabIndex = 0
        '
        'GroupBox2
        '
        Me.GroupBox2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox2.Controls.Add(Me.lblNoApplicationSettings)
        Me.GroupBox2.Controls.Add(Me.btnContractApplicationSettings)
        Me.GroupBox2.Controls.Add(Me.btnExpandApplicationSettings)
        Me.GroupBox2.Controls.Add(Me.tvwApplicationSettings)
        Me.GroupBox2.Location = New System.Drawing.Point(322, 27)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(290, 374)
        Me.GroupBox2.TabIndex = 2
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Application Settings"
        '
        'lblNoApplicationSettings
        '
        Me.lblNoApplicationSettings.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblNoApplicationSettings.BackColor = System.Drawing.Color.Gold
        Me.lblNoApplicationSettings.Location = New System.Drawing.Point(6, 17)
        Me.lblNoApplicationSettings.Name = "lblNoApplicationSettings"
        Me.lblNoApplicationSettings.Size = New System.Drawing.Size(278, 53)
        Me.lblNoApplicationSettings.TabIndex = 7
        Me.lblNoApplicationSettings.Text = "No Application Settings"
        Me.lblNoApplicationSettings.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnContractApplicationSettings
        '
        Me.btnContractApplicationSettings.Location = New System.Drawing.Point(232, 18)
        Me.btnContractApplicationSettings.Name = "btnContractApplicationSettings"
        Me.btnContractApplicationSettings.Size = New System.Drawing.Size(23, 23)
        Me.btnContractApplicationSettings.TabIndex = 8
        Me.btnContractApplicationSettings.Text = "-"
        Me.btnContractApplicationSettings.UseVisualStyleBackColor = True
        '
        'btnExpandApplicationSettings
        '
        Me.btnExpandApplicationSettings.Location = New System.Drawing.Point(261, 18)
        Me.btnExpandApplicationSettings.Name = "btnExpandApplicationSettings"
        Me.btnExpandApplicationSettings.Size = New System.Drawing.Size(23, 23)
        Me.btnExpandApplicationSettings.TabIndex = 8
        Me.btnExpandApplicationSettings.Text = "+"
        Me.btnExpandApplicationSettings.UseVisualStyleBackColor = True
        '
        'tvwApplicationSettings
        '
        Me.tvwApplicationSettings.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tvwApplicationSettings.Location = New System.Drawing.Point(6, 44)
        Me.tvwApplicationSettings.Name = "tvwApplicationSettings"
        Me.tvwApplicationSettings.Size = New System.Drawing.Size(278, 324)
        Me.tvwApplicationSettings.TabIndex = 0
        '
        'btnOK
        '
        Me.btnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOK.Location = New System.Drawing.Point(375, 410)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(75, 23)
        Me.btnOK.TabIndex = 3
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCancel.Location = New System.Drawing.Point(456, 410)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 4
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'mnuMain
        '
        Me.mnuMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuToolstripMain})
        Me.mnuMain.Location = New System.Drawing.Point(0, 0)
        Me.mnuMain.Name = "mnuMain"
        Me.mnuMain.Size = New System.Drawing.Size(624, 24)
        Me.mnuMain.TabIndex = 5
        Me.mnuMain.Text = "MenuStrip1"
        '
        'mnuToolstripMain
        '
        Me.mnuToolstripMain.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LoadToolStripMenuItem, Me.SaveToolStripMenuItem, Me.ToolStripSeparator1, Me.ExitToolStripMenuItem})
        Me.mnuToolstripMain.Name = "mnuToolstripMain"
        Me.mnuToolstripMain.Size = New System.Drawing.Size(37, 20)
        Me.mnuToolstripMain.Text = "File"
        '
        'LoadToolStripMenuItem
        '
        Me.LoadToolStripMenuItem.Image = Global.Setup.My.Resources.Resources.Folder_Open
        Me.LoadToolStripMenuItem.Name = "LoadToolStripMenuItem"
        Me.LoadToolStripMenuItem.Size = New System.Drawing.Size(100, 22)
        Me.LoadToolStripMenuItem.Text = "Load"
        '
        'SaveToolStripMenuItem
        '
        Me.SaveToolStripMenuItem.Image = Global.Setup.My.Resources.Resources.Disk_Small
        Me.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem"
        Me.SaveToolStripMenuItem.Size = New System.Drawing.Size(100, 22)
        Me.SaveToolStripMenuItem.Text = "Save"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(97, 6)
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Image = Global.Setup.My.Resources.Resources.Power_small
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(100, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'btnApply
        '
        Me.btnApply.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnApply.Enabled = False
        Me.btnApply.Location = New System.Drawing.Point(537, 410)
        Me.btnApply.Name = "btnApply"
        Me.btnApply.Size = New System.Drawing.Size(75, 23)
        Me.btnApply.TabIndex = 6
        Me.btnApply.Text = "Apply"
        Me.btnApply.UseVisualStyleBackColor = True
        '
        'ofdSetup
        '
        Me.ofdSetup.FileName = "OpenFileDialog1"
        '
        'imlTree
        '
        Me.imlTree.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit
        Me.imlTree.ImageSize = New System.Drawing.Size(16, 16)
        Me.imlTree.TransparentColor = System.Drawing.Color.Transparent
        '
        'frmSetup
        '
        Me.AllowDrop = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(624, 444)
        Me.Controls.Add(Me.btnApply)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.mnuMain)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.mnuMain
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(640, 1280)
        Me.MinimumSize = New System.Drawing.Size(640, 480)
        Me.Name = "frmSetup"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Configuration Setup - [no file]"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.mnuMain.ResumeLayout(False)
        Me.mnuMain.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents tvwUserSettings As System.Windows.Forms.TreeView
    Friend WithEvents tvwApplicationSettings As System.Windows.Forms.TreeView
    Friend WithEvents mnuMain As System.Windows.Forms.MenuStrip
    Friend WithEvents mnuToolstripMain As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LoadToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnApply As System.Windows.Forms.Button
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ofdSetup As System.Windows.Forms.OpenFileDialog
    Friend WithEvents lblNoUserSettings As System.Windows.Forms.Label
    Friend WithEvents lblNoApplicationSettings As System.Windows.Forms.Label
    Friend WithEvents btnContractUserSettings As System.Windows.Forms.Button
    Friend WithEvents btnExpandUserSettings As System.Windows.Forms.Button
    Friend WithEvents btnContractApplicationSettings As System.Windows.Forms.Button
    Friend WithEvents btnExpandApplicationSettings As System.Windows.Forms.Button
    Friend WithEvents imlTree As System.Windows.Forms.ImageList

End Class
