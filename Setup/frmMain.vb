Imports Core.Common
Imports Core.Common.Functions
Imports Core.Config
Imports Core.Config.Functions

Public Class frmSetup

#Region " Private variables "

    Private m_ConfigFilename As String = ""
    Private m_ChangesPerformed As Boolean = False

#End Region

#Region " GUI Methods "

    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        DoExit()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        DoLoad(m_ConfigFilename)
    End Sub

    Private Sub LoadToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadToolStripMenuItem.Click
        ofdSetup.Title = "Select application configuration file"
        ofdSetup.Filter = "Application configuration files (*.config)|*.config"
        ofdSetup.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.ProgramFiles
        ofdSetup.FileName = ""

        Dim Result As DialogResult = ofdSetup.ShowDialog()
        If Result = Windows.Forms.DialogResult.OK And ofdSetup.FileName.Length > 0 Then
            m_ConfigFilename = ofdSetup.FileName
            DoLoad(m_ConfigFilename)
        End If
    End Sub

    Private Sub SaveToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveToolStripMenuItem.Click
        DoSave()
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        DoExit()
    End Sub

    Private Sub btnApply_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnApply.Click
        DoSave()
    End Sub

    Private Sub frmSetup_DragDrop(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles MyBase.DragDrop
        Dim Filenames() As String = e.Data.GetData(DataFormats.FileDrop)

        If Filenames.Length > 0 Then
            m_ConfigFilename = Filenames(0)
            DoLoad(m_ConfigFilename)
        End If
    End Sub

    Private Sub frmSetup_DragEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles MyBase.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub tvwApplicationSettings_NodeMouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles tvwApplicationSettings.NodeMouseDoubleClick
        Dim ThisNode As TreeNode = e.Node

        If ThisNode.Level = 2 Then
            DoEdit(ThisNode)
        End If
    End Sub

    Private Sub tvwUserSettings_NodeMouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles tvwUserSettings.NodeMouseDoubleClick
        Dim ThisNode As TreeNode = e.Node

        If ThisNode.Level = 2 Then
            DoEdit(ThisNode)
        End If
    End Sub

    Private Sub DoEdit(ByRef Node As TreeNode)
        Select Case Node.Text
            Case "True", "False"
                Node.Text = CStr(Not (CBool(Node.Text)))
            Case Else
                Node.TreeView.LabelEdit = True
                Node.BeginEdit()
        End Select
    End Sub

    Private Sub frmSetup_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Command.StartsWith("-f:") Then
            Dim Filename As String = Command.Substring(3)
            If (Filename.Length > 0) And (IO.File.Exists(Filename)) Then DoLoad(Command.Substring(3))
        End If
    End Sub

    Private Sub btnExpandApplicationSettings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExpandApplicationSettings.Click
        tvwApplicationSettings.Nodes(0).ExpandAll()
    End Sub

    Private Sub btnContractApplicationSettings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnContractApplicationSettings.Click
        tvwApplicationSettings.Nodes(0).Collapse()
    End Sub

    Private Sub btnExpandUserSettings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExpandUserSettings.Click
        tvwUserSettings.Nodes(0).ExpandAll()
    End Sub

    Private Sub btnContractUserSettings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnContractUserSettings.Click
        tvwUserSettings.Nodes(0).Collapse()
    End Sub

    Private Sub tvwApplicationSettings_NodeMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles tvwApplicationSettings.NodeMouseClick
        tvwApplicationSettings.SelectedNode = e.Node
        tvwApplicationSettings.SelectedNode.EndEdit(True)
    End Sub

#End Region

#Region " Private Methods "

    Private Sub DoExit()
        Dim Reply As MsgBoxResult

        If m_ChangesPerformed Then
            Reply = MsgBox("Save configuration?", MsgBoxStyle.YesNoCancel + MsgBoxStyle.Question + MsgBoxStyle.DefaultButton1, "Input required")

            If Reply = MsgBoxResult.Yes Then DoSave()
            If Reply = MsgBoxResult.Yes Or Reply = MsgBoxResult.No Then Me.Close()
        Else
            Me.Close()
        End If
    End Sub

    Private Sub DoSave()
        MsgBox("TODO:  Save config file")
    End Sub

    Private Sub DoLoad(ByVal Filename As String)
        ' TODO:  Check this config file is well-formed

        ' Remove any existing nodes
        tvwUserSettings.Nodes.Clear()
        tvwApplicationSettings.Nodes.Clear()

        m_ChangesPerformed = False
        btnApply.Enabled = False

        ' Get the application name
        Dim ApplicationName As String = LoadApplicationConfigName(Filename)

        ' Set title
        Me.Text = "Configuration Setup - " & ApplicationName.Replace(".My.MySettings", "")

        ' Get the settings
        Dim UserConfigSettings As Collections.Specialized.OrderedDictionary = Core.Config.Functions.LoadApplicationConfigFileSettings(Filename, ApplicationConfigFileSectionType.User)
        Dim ApplicationConfigSettings As Collections.Specialized.OrderedDictionary = Core.Config.Functions.LoadApplicationConfigFileSettings(Filename, ApplicationConfigFileSectionType.Application)

        ' If there are no settings, show a label explaining it
        Me.lblNoUserSettings.Visible = (UserConfigSettings.Count = 0)
        Me.lblNoApplicationSettings.Visible = (ApplicationConfigSettings.Count = 0)

        ' Create a parent node for each treeview
        Dim UserRootNode As TreeNode = tvwUserSettings.Nodes.Add("userSettings")
        Dim ApplicationRootNode As TreeNode = tvwApplicationSettings.Nodes.Add("applicationSettings")

        ' Add the User settings
        For Each Entry As DictionaryEntry In UserConfigSettings
            Dim SettingNode As TreeNode = UserRootNode.Nodes.Add(Entry.Key)
            Dim ValueNode As TreeNode = SettingNode.Nodes.Add(Entry.Key & "Value", Entry.Value)
        Next

        ' Add the Application settings
        For Each Entry As DictionaryEntry In ApplicationConfigSettings
            Dim SettingNode As TreeNode = ApplicationRootNode.Nodes.Add(Entry.Key)
            Dim ValueNode As TreeNode = SettingNode.Nodes.Add(Entry.Key & "Value", Entry.Value)
        Next

        m_ConfigFilename = Filename

    End Sub

    Private Function CancelNodeEdit(ByVal Node As TreeNode) As Boolean
        If Node.Level = 2 Then
            m_ChangesPerformed = True
            btnApply.Enabled = True
            Return False
        Else
            Return True
        End If
    End Function

#End Region



End Class
