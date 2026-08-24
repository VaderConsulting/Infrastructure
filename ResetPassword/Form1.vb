Imports System
Imports System.Xml.XPath
Imports System.Xml.XmlDocument
Imports System.Configuration ' For ConfigXmlDocument
Imports System.Collections.Specialized
Imports System.Security.Cryptography
Imports System.IO
Imports System.DirectoryServices

Public Class frmMain

#Region " Private Variables "

    Private m_DataPath As String = My.Computer.FileSystem.SpecialDirectories.AllUsersApplicationData
    Private m_LocalSystemConfigPath As String = m_DataPath & "\cads.xml"
    Private m_CentralSystemConfigPath As String = m_DataPath & "\cads.xml"
    Private m_InitialLoadComplete As Boolean = False
    Private m_ConfigContents As New OrderedDictionary
    Private m_DirectoryServers() As String = {}
    Private m_TargetDirectoryConnectionUsername As String = ""
    Private m_TargetDirectoryConnectionPassword As String = ""
    Private m_CurrentDirectoryServer As String = ""
    Private m_DirectoryAuthenticationType As AuthenticationTypes = AuthenticationTypes.ServerBind
    Private m_DecryptedDirectoryPassword As String
    Private m_ConfigurationServerName As String = ""

#End Region

#Region " Public Methods "

#End Region

#Region " Form methods "

    Private Sub radLocationLocal_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radLocationLocal.CheckedChanged
        txtCN.Text = ""
        txtPassword.Text = ""
        btnReset.Enabled = False
        If radLocationLocal.Checked Then DoLocationChange(1)
    End Sub

    Private Sub radLocationCentral_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radLocationCentral.CheckedChanged
        txtCN.Text = ""
        txtPassword.Text = ""
        btnReset.Enabled = False
        If radLocationCentral.Checked Then DoLocationChange(2)
    End Sub

    Private Sub btnConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConnect.Click
        m_TargetDirectoryConnectionUsername = ""
        m_TargetDirectoryConnectionPassword = ""

        Dim UserAndSystemConfig As OrderedDictionary = MySettings(cmbAccounts.Text)

        If m_TargetDirectoryConnectionUsername.Length > 0 Then
            Me.txtCN.Text = m_TargetDirectoryConnectionUsername
            Me.txtPassword.Text = m_TargetDirectoryConnectionPassword

            btnReset.Enabled = True
        End If
    End Sub

    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click
        Dim Result As DialogResult

        Result = MsgBox("This will reset the Connection account Password.  Are you sure?", MsgBoxStyle.YesNo + MsgBoxStyle.Question + MsgBoxStyle.DefaultButton2, "Question")

        If Result = Windows.Forms.DialogResult.Yes Then
            Try
                Dim ADUser As DirectoryEntry = GetDirectoryObject("LDAP://" & m_CurrentDirectoryServer & "/" & txtCN.Text, False)

                ADUser.InvokeSet("msDS-UserAccountDisabled", "FALSE")
                ADUser.CommitChanges()

                ADUser.Password = txtPassword.Text
                ADUser.CommitChanges()

                MsgBox("Password for " & txtCN.Text & " has been reset." & vbCrLf & "msDS-UserAccountDisabled has been set to FALSE.", MsgBoxStyle.Information + MsgBoxStyle.OkOnly, "Information")
            Catch ex As Exception
                MsgBox("Password reset and\or setting of msDS-UserAccountDisabled for " & txtCN.Text & " has failed." & vbCrLf & vbCrLf & "The error was:" & ex.Message & vbCrLf & ex.StackTrace.ToString, MsgBoxStyle.Exclamation + MsgBoxStyle.OkOnly, "Error")
                WriteTraceMessage(ex.Message & vbCrLf & ex.StackTrace.ToString)
            End Try
        End If
    End Sub

    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Show(Me)
        Try
            txtInstance.Text = m_LocalSystemConfigPath
            radLocationLocal.Checked = True

            m_CentralSystemConfigPath = LoadConfigSetting(txtInstance.Text, "SystemSettings", "ConfigFileLocation") & "\cads.xml"

            m_InitialLoadComplete = True
        Catch ex As Exception
            WriteTraceMessage(ex.Message & vbCrLf & ex.StackTrace.ToString)
        End Try
    End Sub

    Private Sub cmbAccounts_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbAccounts.SelectedIndexChanged
        txtCN.Text = ""
        txtPassword.Text = ""
        btnReset.Enabled = False
    End Sub

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Simple Decryption routine.  Do not use for sensitive information
    ''' </summary>
    ''' <param name="InputText">Input string to en\decrypt</param>
    ''' <returns>Output string en\decrypted</returns>
    ''' <remarks>V1.0  D. Robinson</remarks>
    Private Function SimpleCrypt(ByVal InputText As String) As String
        Dim OutputText As String

        ' Encrypts/decrypts the passed string using a simple ASCII value-swapping algorithm
        Try
            Dim strTempChar As String = "", i As Integer
            For i = 1 To Len(InputText)
                If Asc(Mid$(InputText, i, 1)) < 128 Then
                    strTempChar = _
              CType(Asc(Mid$(InputText, i, 1)) + 128, String)
                ElseIf Asc(Mid$(InputText, i, 1)) > 128 Then
                    strTempChar = _
              CType(Asc(Mid$(InputText, i, 1)) - 128, String)
                End If
                Mid$(InputText, i, 1) = _
                    Chr(CType(strTempChar, Integer))
            Next i
            OutputText = InputText

            Return OutputText
        Catch ex As Exception
            WriteTraceMessage(ex.Message & vbCrLf & ex.StackTrace.ToString)
            Return ""
        End Try
    End Function

    Private Sub DoLocationChange(ByVal LocationType As Int16)
        'If Not m_InitialLoadComplete Then Exit Sub
        Select Case LocationType
            Case 1 ' Local
                txtInstance.Text = m_LocalSystemConfigPath
                m_DataPath = My.Computer.FileSystem.SpecialDirectories.AllUsersApplicationData
            Case 2 ' Remote
                txtInstance.Text = m_CentralSystemConfigPath
                m_DataPath = m_CentralSystemConfigPath.Substring(0, m_CentralSystemConfigPath.LastIndexOf("\"))
        End Select

        cmbAccounts.Items.Clear()

        For Each Key As String In SystemAccountNames.Keys
            cmbAccounts.Items.Add(Key.ToString)
        Next

        cmbAccounts.SelectedIndex = 0

    End Sub

    Private Function CollectionValue(ByVal SettingName As String, ByVal SettingsCollection As OrderedDictionary) As String
        Dim Keys As ICollection = SettingsCollection.Keys
        Dim Values As ICollection = SettingsCollection.Values

        For i As Int16 = 0 To Keys.Count - 1
            If Keys(i).ToString = SettingName Then
                Return Values(i).ToString
            End If
        Next

        Return ""
    End Function

    ''' <summary>
    ''' Decrypts the given string from a format suitable for output to XML files
    ''' </summary>
    ''' <param name="stringToDecrypt">Input string to decrypt</param>
    ''' <param name="EncryptionKey">The Key used to decrypt the string</param>
    ''' <returns>Decrypted String</returns>
    ''' <remarks>V1.0 D. Robinson</remarks>
    Private Function XMLDecrypt(ByVal stringToDecrypt As String, ByVal EncryptionKey As String) As String
        Dim inputByteArray(stringToDecrypt.Length) As Byte
        Dim RSAkey() As Byte = {}
        Dim IV() As Byte = {&H12, &H34, &H56, &H78, &H90, &HAB, &HCD, &HEF}
        Dim DecryptedString As String
        ' Note: The DES CryptoService only accepts certain key byte lengths
        ' We are going to make things easy by insisting on an 8 byte legal key length
        If stringToDecrypt.Trim.Length = 0 Then Return ""
        Try
            RSAkey = System.Text.Encoding.UTF8.GetBytes(Microsoft.VisualBasic.Left(EncryptionKey, 8))
            Dim des As New DESCryptoServiceProvider()
            ' we have a base 64 encoded string so first must decode to regular unencoded (encrypted) string
            inputByteArray = Convert.FromBase64String(stringToDecrypt)
            ' now decrypt the regular string
            Dim ms As New MemoryStream()
            Dim cs As New CryptoStream(ms, des.CreateDecryptor(RSAkey, IV), CryptoStreamMode.Write)
            cs.Write(inputByteArray, 0, inputByteArray.Length)
            cs.FlushFinalBlock()
            Dim encoding As System.Text.Encoding = System.Text.Encoding.UTF8
            DecryptedString = encoding.GetString(ms.ToArray())

            Return DecryptedString
        Catch ex As Exception
            WriteTraceMessage(ex.Message & vbCrLf & ex.StackTrace.ToString)
            Return ""
        End Try
    End Function

    Private Function SystemAccountNames() As OrderedDictionary
        Dim Accounts As New OrderedDictionary

        Accounts = LoadConfigSection(txtInstance.Text, "SystemSecurityLevels")

        Return Accounts

    End Function

    Private Function MySettings(ByVal Username As String) As System.Collections.Specialized.OrderedDictionary
        Dim FinalSettings As New OrderedDictionary
        Dim SystemSettings As New OrderedDictionary
        Dim DirectorySettings As New OrderedDictionary
        Dim SystemSecurityLevels As New OrderedDictionary
        Dim ConsoleUserSettings As New OrderedDictionary
        Dim Servers As New OrderedDictionary
        Dim SecurityLevels As New OrderedDictionary
        Dim MySecuritylevel As UInt64

        Dim UserSettings As New OrderedDictionary
        Dim LevelSettings As New OrderedDictionary
        Dim ConsoleSearchFields As New OrderedDictionary
        Dim DirectoryServer As New OrderedDictionary
        Dim ConsoleDetailFields As New OrderedDictionary
        Dim ConfigFilename As String = m_LocalSystemConfigPath  ' Save this because we have to switch between files
        Dim UserConfigFilename As String = ""
        Dim UserConfigExists As Boolean = False
        Dim UsernameToCheck As String = Replace(Username, "\", "_")
        Dim UserDefaultLocation As String = ""

        ' ------------------------------------------------------------------------------------------
        ' Load all System settings
        m_ConfigContents = LoadConfigSection(txtInstance.Text, "SystemSettings") ' Should only contain TemporaryAccounts and EncryptionKey
        CloneCollection(m_ConfigContents, SystemSettings)
        ' ------------------------------------------------------------------------------------------
        ' Load system Security Levels
        m_ConfigContents = LoadConfigSection(txtInstance.Text, "SystemSecurityLevels") ' Contains Security levels
        CloneCollection(m_ConfigContents, SystemSecurityLevels)
        ' ------------------------------------------------------------------------------------------
        ' Load User Console settings
        m_ConfigContents = LoadConfigSection(txtInstance.Text, "ConsoleUserSettings") ' Contains System settings that are customisable, and are not in CADS.xml
        CloneCollection(m_ConfigContents, ConsoleUserSettings)
        ' ------------------------------------------------------------------------------------------
        ' Load the default Directory servers
        m_ConfigContents = LoadConfigSection(txtInstance.Text, "DirectoryServer") ' Contains basic config settings
        CloneCollection(m_ConfigContents, DirectoryServer)
        ' ------------------------------------------------------------------------------------------
        ' Determine what level names this user has
        MySecuritylevel = GetUserAccessLevel(Username)

        ' Switch to the User-specific config
        UserConfigFilename = m_DataPath & "\Users\" & UsernameToCheck & ".xml"

        If IO.File.Exists(UserConfigFilename) Then
            ' Load User settings
            UserSettings = LoadConfigSection(UserConfigFilename, "ConsoleUserSettings")
            CloneCollection(m_ConfigContents, UserSettings)
            ' If the user config exists, it will over-ride the level config
            DirectorySettings.Clear()
            ConsoleSearchFields.Clear()
            ConsoleDetailFields.Clear()
            DirectorySettings = LoadConfigSection(UserConfigFilename, "DirectorySettings")
            CloneCollection(m_ConfigContents, DirectorySettings)

            ' Get Location to determine servers
            UserDefaultLocation = CollectionValue("Location", DirectorySettings)
            ' Now retrieve the list of servers for this location from ADAM

            Try
                Dim EncryptedEncryptionKey As String = CollectionValue("EncryptionKey", SystemSettings)
                Dim DecryptedEncryptionKey As String = SimpleCrypt(EncryptedEncryptionKey)
                Dim DirectoryUsername As String = CollectionValue("DirectoryUserUsername", DirectorySettings)
                Dim EncryptedDirectoryPassword As String = CollectionValue("DirectoryUserPassword", DirectorySettings)
                Dim DecryptedDirectoryPassword As String = XMLDecrypt(EncryptedDirectoryPassword, DecryptedEncryptionKey)
                Dim ConfigurationServerName As String = CollectionValue("Default", DirectoryServer)
                Dim DirectoryConfigurationPath As String = CollectionValue("ConfigurationRoot", DirectoryServer)

                m_TargetDirectoryConnectionUsername = DirectoryUsername
                m_TargetDirectoryConnectionPassword = DecryptedDirectoryPassword
                m_ConfigurationServerName = ConfigurationServerName

                Servers = GetLookupCollection(ConfigurationServerName, "Locations", False, DirectoryConfigurationPath, DirectoryUsername, DecryptedDirectoryPassword)

                ' Finally, get the actual server names
                Dim TempDirectoryServers() As String
                TempDirectoryServers = Split(CollectionValue(UserDefaultLocation, Servers), "|")
                For i As Integer = 1 To TempDirectoryServers.Count - 1
                    ReDim Preserve m_DirectoryServers(i - 1)
                    m_DirectoryServers(i - 1) = TempDirectoryServers(i)
                Next

                If m_DirectoryServers.Length = 0 Then
                    ReDim m_DirectoryServers(0)
                    m_DirectoryServers(0) = ConfigurationServerName
                End If

                m_CurrentDirectoryServer = DetermineTargetServerName(m_DirectoryServers)

            Catch ex As Exception

                WriteTraceMessage(ex.Message & vbCrLf & ex.StackTrace.ToString)

            End Try
            ConsoleSearchFields = LoadConfigSection(UserConfigFilename, "ConsoleSearchFields")
            CloneCollection(m_ConfigContents, ConsoleSearchFields)
            ConsoleDetailFields = LoadConfigSection(UserConfigFilename, "ConsoleDetailFields")
            CloneCollection(m_ConfigContents, ConsoleDetailFields)

            UserConfigExists = True
        Else
            MsgBox("There is no local configuration file for this account", MsgBoxStyle.Exclamation + MsgBoxStyle.OkOnly, "Information")
            Return Nothing
        End If

        ' Combine all of these settings into one collection
        ' SystemSettings
        CloneCollection(SystemSettings, FinalSettings)
        ' SystemSecurityLevels
        CloneCollection(SystemSecurityLevels, FinalSettings)
        ' DirectoryServers
        CloneCollection(Servers, FinalSettings)
        ' DirectorySettings
        CloneCollection(DirectorySettings, FinalSettings)
        ' ConsoleSearchFields
        CloneCollection(ConsoleSearchFields, FinalSettings)
        ' ConsoleDetailFields 
        CloneCollection(ConsoleDetailFields, FinalSettings)
        ' DirectoryServer
        CloneCollection(DirectoryServer, FinalSettings)
        MergeCollections(UserSettings, LevelSettings, FinalSettings)

        Return FinalSettings
    End Function

    Private Sub CloneCollection(ByVal SourceCollection As OrderedDictionary, ByRef DestinationCollection As OrderedDictionary)
        Dim CollectionItem As DictionaryEntry

        For Each CollectionItem In SourceCollection
            Try
                If Not DestinationCollection.Contains(CollectionItem.Key) Then
                    DestinationCollection.Add(CollectionItem.Key, CollectionItem.Value)
                End If
            Catch ex As Exception
                WriteTraceMessage(ex.Message & vbCrLf & ex.StackTrace.ToString)
            End Try
        Next

    End Sub

    Private Sub MergeCollections(ByVal SourceCollection1 As OrderedDictionary, ByVal SourceCollection2 As OrderedDictionary, ByRef DestinationCollection As OrderedDictionary, Optional ByVal UseEnforcedValue As Boolean = False)
        Dim SourceValueToUse As String

        For Each CollectionItem In SourceCollection1
            SourceValueToUse = GetOverridingSetting(CollectionItem.Key, SourceCollection1, SourceCollection2)
            If Not DestinationCollection.Contains(CollectionItem.Key) Then
                DestinationCollection.Add(CollectionItem.Key, SourceValueToUse)
            End If
        Next
    End Sub

    Private Function GetOverridingSetting(ByVal SettingName As String, ByVal LevelSettings As OrderedDictionary, ByVal UserSettings As OrderedDictionary) As String
        Dim LevelSetting As String = ""
        Dim UserSetting As String = ""

        Dim LevelSettingEnforced As Boolean = False
        Dim UserSettingEnforced As Boolean = False
        Dim Enforced As Boolean = False

        Dim FinalSetting As String = ""

        If UserSettings.Contains(SettingName) Then
            UserSetting = UserSettings(SettingName)
        End If
        If LevelSettings.Contains(SettingName) Then
            LevelSetting = LevelSettings(SettingName)
        End If

        LevelSettingEnforced = CheckEnforced(LevelSetting)
        UserSettingEnforced = CheckEnforced(UserSetting)

        If UserSettingEnforced Then
            FinalSetting = UserSetting
            Enforced = True
        ElseIf LevelSettingEnforced Then
            FinalSetting = LevelSetting
            Enforced = True
        End If

        If Not Enforced Then
            If UserSetting <> "" Then
                FinalSetting = UserSetting
            ElseIf LevelSetting <> "" Then
                FinalSetting = LevelSetting
            End If
        End If

        Return FinalSetting
    End Function

    Private Function CheckEnforced(ByVal SettingValue As String) As Boolean
        Dim Values() As String
        Dim Value As String
        Dim Enforced As Boolean

        Try
            Values = Split(SettingValue, "|")

            If Values.Length = 2 Then
                Value = Values(0).ToLower
                If Values(1).ToLower = "true" Or Values(1).ToLower = "false" Then
                    Enforced = CBool(Values(1))
                    Return Enforced
                Else
                    Return False
                End If

            Else
                Return False
            End If
        Catch ex As Exception
            WriteTraceMessage(ex.Message & vbCrLf & ex.StackTrace.ToString)
            Return False
        End Try
    End Function

    Private Function GetUserAccessLevel(ByVal Username As String) As String
        Dim UsernameToCheck As String = Replace(Username, "\", "_")
        Dim UserAccessLevel As String = ""

        ' Check Username is specified
        If Username.Trim.Length = 0 Then
            Return ""
        End If

        ' Look for the user in our security configuration
        UserAccessLevel = LoadConfigSettingAttribute(txtInstance.Text, "SystemSecurityLevels", UsernameToCheck, "SecurityValue")

        Return UserAccessLevel

    End Function

    Private Function GetLookupCollection(ByVal LDAPServerAndPort As String, _
                                    ByVal ListName As String, _
                                    ByVal ReturnDescriptionOnly As Boolean, _
                                    ByVal DirectoryConfigurationRoot As String, _
                                    ByVal DirectoryConnectionUsername As String, _
                                    ByVal DirectoryConnectionPassword As String) As Collections.Specialized.OrderedDictionary
        Dim SiteCollection = New Collections.Specialized.OrderedDictionary

        If Not LDAPServerAndPort.StartsWith("LDAP://") Then
            LDAPServerAndPort = "LDAP://" & LDAPServerAndPort
        End If

        If Not LDAPServerAndPort.EndsWith("/") Then
            LDAPServerAndPort &= "/"
        End If

        If m_CurrentDirectoryServer & "" <> "" Then
            If DirectoryConfigurationRoot.StartsWith("LDAP://") Then DirectoryConfigurationRoot = Replace(DirectoryConfigurationRoot, "LDAP://", "")
            If DirectoryConfigurationRoot.StartsWith(m_CurrentDirectoryServer) Then DirectoryConfigurationRoot = Replace(DirectoryConfigurationRoot, m_CurrentDirectoryServer, "")
        End If

        ' Create full directory path
        Dim FullListPath As String = LDAPServerAndPort & "OU=" & ListName & "," & DirectoryConfigurationRoot

        Try
            'Bind to the directory
            If LDAPObjectExists(FullListPath) Then
                Dim de As DirectoryEntry = GetDirectoryObject(FullListPath)

                'Create search object
                Dim Search As DirectorySearcher = New DirectorySearcher(de)

                'Create search criteria
                Search.Filter = "(objectClass=User)"
                Search.SearchScope = SearchScope.OneLevel
                Search.PageSize = 200000
                Search.PropertiesToLoad.Add("description")
                Search.PropertiesToLoad.Add("cn")
                Search.PropertiesToLoad.Add("wbemPath")

                'Search the directory and return paths of matching users
                Dim Results As SearchResultCollection = Search.FindAll()
                Dim Description As String = ""
                Dim cn As String

                Dim ObjectCount As Integer = 1

                For Each Result As SearchResult In Results
                    Try
                        Description = GetADAttribute(Result.GetDirectoryEntry, "description") 'Result.Properties("description")(0).ToString
                    Catch
                        Try
                            Description = Result.Properties("description").Item(0).ToString()
                        Catch ex As Exception
                            Description = ""
                        End Try
                    End Try

                    cn = GetADAttribute(Result.GetDirectoryEntry, "cn") ' Result.Properties("cn")(0).ToString
                    If ReturnDescriptionOnly Then
                        SiteCollection.Add(cn, Description)
                    Else
                        Dim wbemPath As String = ""
                        For Each Value As String In Result.Properties("wbemPath")
                            wbemPath &= Value & "|"
                        Next
                        If wbemPath.EndsWith("|") Then wbemPath = wbemPath.Substring(0, wbemPath.Length - 1)
                        SiteCollection.Add(cn, Description & "|" & wbemPath)
                    End If
                    ObjectCount += 1
                Next

                de.Close()
                de = Nothing
            Else

            End If
        Catch ex As Exception

        End Try

        Return SiteCollection

    End Function

    ''' <summary>
    ''' Check if the given Directory path exists
    ''' </summary>
    ''' <param name="DirectoryPath">Fully qualified LDAP Path in the form 'LDAP://[Server:Port/]dc=Name,dc=COM'</param>
    ''' <returns>Boolean value indicating the result of the search</returns>
    ''' <remarks>V1.0 D. Robinson</remarks>
    Private Function LDAPObjectExists(ByVal DirectoryPath As String) As Boolean
        Dim DirectoryRoot As DirectoryEntry = GetDirectoryObject(DirectoryPath)
        Dim oSearcher As New DirectorySearcher(DirectoryPath)

        Try
            Return DirectoryRoot.Name <> ""
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function DetermineTargetServerName(ByVal ServerNames() As String) As String
        Dim Server As String = ""
        Dim Index As Integer = 0
        Dim ServerUp As Boolean = False
        Dim PingAttempt As Integer = 0

        If ServerNames.Length = 0 Then
            Return ""
        End If

        Do
            Server = ServerNames(Index)

            If Server.Contains(":") Then
                Server = Server.Substring(0, Server.IndexOf(":"))
            End If

            Do
                PingAttempt += 1

                Try
                    If My.Computer.Network.Ping(Server) Then
                        ServerUp = True
                    End If
                Catch ex As Exception
                    ' Could not resolve this name - just as good as a ping failure

                End Try
            Loop Until PingAttempt = 3 Or ServerUp

            PingAttempt = 0

            If Not ServerUp Then
                Index += 1
            End If
        Loop Until ServerUp Or Index = ServerNames.Length

        If ServerUp Then
            Return ServerNames(Index)
        Else
            Return m_ConfigurationServerName
            'Return Constants.DEFAULTSTRINGVALUE
        End If
    End Function

    Private Function GetADAttribute(ByVal DirectoryObject As DirectoryEntry, ByVal AttributeName As String, Optional ByVal ValueIndex As Integer = -1) As String
        Dim AttributeValue As String = ""

        Try
            If DirectoryObject.Properties.Contains(AttributeName) Then
                If TypeOf DirectoryObject.Properties(AttributeName).Value Is Array Then
                    If ValueIndex > -1 Then
                        If DirectoryObject.Properties(AttributeName).Count >= ValueIndex Then
                            AttributeValue = DirectoryObject.Properties(AttributeName)(ValueIndex).ToString
                        Else
                            AttributeValue = ""
                        End If
                    Else
                        AttributeValue = ConvertGUIDToString(DirectoryObject.Properties(AttributeName)(0))
                    End If
                Else
                    AttributeValue = DirectoryObject.Properties(AttributeName).Value.ToString
                End If
            Else
                AttributeValue = ""
            End If
        Catch
            'Err.Raise(vbObjectError + 528, GetExecutingAssembly.GetName.Name, "Failed to retrieve Attribute (" & AttributeName & ") of Directory Object " & DirectoryObject.Path)
        End Try

        Return AttributeValue
    End Function

    Private Function ConvertGUIDToString(ByRef GUIDString() As Byte) As String
        Dim ReturnString As String = ""

        Try
            For Count = 0 To GUIDString.Length - 1
                ReturnString = ReturnString + Hex(GUIDString(Count))
            Next
        Catch ex As Exception
            ' Hopefully we don't ever get here.  If so, we don't want to die!
        End Try
        Return ReturnString
    End Function

    Private Function GetDirectoryObject(ByVal FullObjectPath As String, Optional ByVal ConnectAnonymously As Boolean = False) As DirectoryEntry
        Dim ReturnObject As DirectoryEntry = Nothing

        If ConnectAnonymously Then
            ReturnObject = New DirectoryEntry(FullObjectPath)
        Else
            ReturnObject = New DirectoryEntry(FullObjectPath, m_TargetDirectoryConnectionUsername, m_TargetDirectoryConnectionPassword, m_DirectoryAuthenticationType)
        End If

        Return ReturnObject
    End Function

#End Region

#Region " Private Shared Methods "

    ''' <summary>
    ''' Reads the given String value
    ''' </summary>
    ''' <param name="SettingName">The Setting name</param>
    ''' <param name="DefaultValue">The default value to use</param>
    ''' <returns>The value</returns>
    ''' <remarks>V2.0  D. Robinson</remarks>
    Private Shared Function LoadConfigSetting(ByVal ConfigurationFilename As String, ByVal SectionName As String, ByVal SettingName As String, Optional ByVal DefaultValue As String = "") As String
        Dim ConfigNode As XPathNavigator = Nothing

        Try
            If OpenConfigurationFile(ConfigNode, ConfigurationFilename) Then
                Return LoadConfigValue(ConfigNode, SectionName, SettingName)
            Else
                Return DefaultValue ' Error opening External Configuration file
            End If
        Catch ex As Exception
            WriteTraceMessage(ex.Message & vbCrLf & ex.StackTrace.ToString)
            Return DefaultValue
        End Try

    End Function

    ''' <summary>
    ''' Opens the External Configuration file
    ''' </summary>
    ''' <param name="Navigator">XPathNavigator</param>
    ''' <remarks>V2.0  D. Robinson</remarks>
    Private Shared Function OpenConfigurationFile(ByRef Navigator As XPathNavigator, ByVal ConfigurationFilename As String) As Boolean
        Dim Result As Boolean = False
        Dim ConfigDocument As New ConfigXmlDocument

        If ConfigurationFilename.Length = 0 Then
            Result = False
        End If
        If IO.File.Exists(ConfigurationFilename) Then
            Try
                ConfigDocument.Load(ConfigurationFilename)

                Dim Nav As XPathNavigator = ConfigDocument.CreateNavigator()
                Dim ConfigNode As XPathNavigator = Nav.SelectSingleNode("/configuration")

                Navigator = ConfigNode.Clone

                ConfigNode = Nothing
                Nav = Nothing

                Result = True
            Catch ex As Exception
                WriteTraceMessage(ex.Message & vbCrLf & ex.StackTrace.ToString)
            End Try
        End If

        Return Result
    End Function

    ''' <summary>
    ''' Reads the value from the configuration file
    ''' </summary>
    ''' <param name="Node">XPathNavigator</param>
    ''' <param name="SectionName">The Section name</param>
    ''' <param name="SettingName">The Setting name</param>
    ''' <returns>The Value</returns>
    ''' <remarks>V2.0  D. Robinson</remarks>
    Private Shared Function LoadConfigValue(ByVal Node As XPathNavigator, ByVal SectionName As String, ByVal SettingName As String) As String
        Dim ResultOfMove As Boolean = False
        Dim Result As String = ""
        Dim ReadComplete As Boolean = False

        If Node.HasChildren Then
            Node.MoveToFirstChild()
            Do
                If Node.Name Like SectionName Then
                    'Node.MoveToFirstChild()
                    Do
                        Node.SelectSingleNode(SettingName)
                        ResultOfMove = Node.MoveToChild(SettingName, "")
                        If ResultOfMove = True Then
                            Result = Node.Value
                            ReadComplete = True
                        Else  ' Unsuccessful
                            Result = ""
                        End If
                    Loop Until Node.MoveToNext = False Or ReadComplete = True
                End If
            Loop Until (Node.MoveToNext = False Or ReadComplete = True)
        End If
        Return Result
    End Function

    Private Shared Sub WriteTraceMessage(ByVal Message As String)
        Trace.WriteLine("Error!: " & Message)
    End Sub

    ''' <summary>
    ''' Returns an OrderedDictionary collection of all Settings in the Config Section
    ''' </summary>
    ''' <param name="ConfigurationFilename"></param>
    ''' <param name="SectionName"></param>
    ''' <param name="NameFilter"></param>
    ''' <returns>OrderedDictionary collection of the results</returns>
    ''' <remarks></remarks>
    Private Shared Function LoadConfigSection(ByVal ConfigurationFilename As String, ByVal SectionName As String, Optional ByVal NameFilter As String = "*") As Collections.Specialized.OrderedDictionary
        Dim ConfigNode As XPathNavigator = Nothing
        Dim SettingComplete As Boolean = False
        Dim MoveResult As Boolean = False
        Dim ConfigSection As New Collections.Specialized.OrderedDictionary

        If OpenConfigurationFile(ConfigNode, ConfigurationFilename) Then

            If ConfigNode.HasChildren Then
                ConfigNode.MoveToFirstChild()
                Do
                    If ConfigNode.Name Like SectionName Then
                        MoveResult = ConfigNode.MoveToFirstChild()
                        If MoveResult And Not ConfigNode.NodeType = XPathNodeType.Comment Then
                            Do
                                If (ConfigNode.Name Like (NameFilter)) And (ConfigNode.Name <> "") Then
                                    ConfigSection.Add(ConfigNode.Name, ConfigNode.Value)
                                End If
                            Loop Until ConfigNode.MoveToNext = False
                            SettingComplete = True
                        Else
                            ' No children
                            SettingComplete = True
                        End If
                    End If
                Loop Until (ConfigNode.MoveToNext = False Or SettingComplete = True)
            End If
        End If

        Return ConfigSection

    End Function

    Private Shared Function LoadConfigSectionNames(ByVal ConfigurationFilename As String, Optional ByVal SectionNameFilter As String = "*") As Collections.Specialized.OrderedDictionary
        Dim ConfigNode As XPathNavigator = Nothing
        Dim ConfigSectionNames As New Collections.Specialized.OrderedDictionary

        If OpenConfigurationFile(ConfigNode, ConfigurationFilename) Then

            If ConfigNode.HasChildren Then
                ConfigNode.MoveToFirstChild()
                Do
                    If (ConfigNode.Name Like SectionNameFilter) And (Not ConfigNode.NodeType = XPathNodeType.Comment) Then
                        ConfigSectionNames.Add(ConfigNode.Name, ConfigNode.Value)
                    End If
                Loop Until (ConfigNode.MoveToNext = False)
            End If
        End If

        Return ConfigSectionNames

    End Function

    ''' <summary>
    ''' Reads the attribute from the configuration file
    ''' </summary>
    ''' <param name="Node">XPathNavigator</param>
    ''' <param name="SectionName">The Section name</param>
    ''' <param name="AttributeName">The attribute</param>
    ''' <returns>The Attribute</returns>
    ''' <remarks>V2.0  D. Robinson</remarks>
    Private Shared Function LoadConfigAttribute(ByVal Node As XPathNavigator, ByVal SectionName As String, ByVal AttributeName As String) As String
        Dim Result As String = ""
        Dim ReadComplete As Boolean = False

        If Node.HasChildren Then
            Node.MoveToFirstChild()
            Do
                If Node.Name Like SectionName Then
                    Do Until Node.MoveToNext = False Or ReadComplete = True
                        Result = Node.GetAttribute(AttributeName, "")
                        ReadComplete = True
                    Loop
                End If
            Loop Until (Node.MoveToNext = False Or ReadComplete = True)
        End If
        Return Result
    End Function

    Private Shared Function LoadConfigSettingAttribute(ByVal ConfigurationFilename As String, ByVal SectionName As String, ByVal SettingName As String, ByVal AttributeName As String, Optional ByVal DefaultValue As String = "") As String
        Try
            Dim ConfigNode As XPathNavigator = Nothing

            If OpenConfigurationFile(ConfigNode, ConfigurationFilename) Then
                Return LoadConfigAttribute(ConfigNode, SectionName, SettingName, AttributeName)
            Else
                Return DefaultValue
            End If
        Catch ex As Exception
            WriteTraceMessage(ex.Message & vbCrLf & ex.StackTrace.ToString)
            Return DefaultValue
        End Try

    End Function

    ''' <summary>
    ''' Reads the attribute from the configuration file
    ''' </summary>
    ''' <param name="Node">XPathNavigator</param>
    ''' <param name="SectionName">The Section name</param>
    ''' <param name="SettingName">The Setting name</param>
    ''' <param name="AttributeName">The attribute</param>
    ''' <returns>The Attribute</returns>
    ''' <remarks>V2.0  D. Robinson</remarks>
    Private Shared Function LoadConfigAttribute(ByVal Node As XPathNavigator, ByVal SectionName As String, ByVal SettingName As String, ByVal AttributeName As String) As String
        Dim ResultOfMove As Boolean = False
        Dim Result As String = ""
        Dim ReadComplete As Boolean = False

        If Node.HasChildren Then
            Node.MoveToFirstChild()
            Do
                If Node.Name Like SectionName Then
                    Do
                        Node.SelectSingleNode(SettingName)
                        ResultOfMove = Node.MoveToChild(SettingName, "")
                        If ResultOfMove = True Then
                            Result = Node.GetAttribute(AttributeName, "")
                            ReadComplete = True
                        Else  ' Unsuccessful
                            Result = ""
                        End If
                    Loop Until Node.MoveToNext = False Or ReadComplete = True
                End If
            Loop Until (Node.MoveToNext = False Or ReadComplete = True)
        End If
        Return Result
    End Function

#End Region

End Class
