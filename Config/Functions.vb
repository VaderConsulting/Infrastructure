#Region " Imports "

Imports System
Imports System.Xml.XPath
Imports System.Xml.XmlDocument
Imports System.Collections.Specialized
Imports Core.Common.Functions
Imports System.Configuration ' For ConfigXmlDocument

#End Region

Partial Public Class Functions

#Region " Enums "

    Public Enum ApplicationConfigFileSectionType
        User = 1
        Application = 2
    End Enum

#End Region

#Region " Public methods "

    ''' <summary>
    ''' Returns an OrderedDictionary collection of all Settings in the Config Section
    ''' </summary>
    ''' <param name="ConfigurationFilename">The filename to load</param>
    ''' <param name="SectionName">The section to load</param>
    ''' <param name="NameFilter">Optional filter of names to load</param>
    ''' <returns>OrderedDictionary collection of the results</returns>
    ''' <remarks></remarks>
    Public Shared Function LoadConfigSection(ByVal ConfigurationFilename As String, ByVal SectionName As String, Optional ByVal NameFilter As String = "*") As Collections.Specialized.OrderedDictionary
        Dim ConfigNode As XPathNavigator = Nothing
        Dim SettingComplete As Boolean = False
        Dim MoveResult As Boolean = False
        Dim ConfigSection As New Collections.Specialized.OrderedDictionary
        Dim ConfigDocument As New ConfigXmlDocument

        If OpenConfigurationFile(ConfigDocument, ConfigNode, ConfigurationFilename) Then

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

    ''' <summary>
    ''' Opens the File Configuration list and updates DuplicateNameFound if there was a duplicate name
    ''' </summary>
    ''' <param name="ConfigurationFilename">The filename to load</param>
    ''' <param name="SectionName">The section to load</param>
    ''' <param name="DuplicateNameFound">ByRef variable indicating if a duplicate node was found</param>
    ''' <param name="NameFilter">Optional filter of names to load</param>
    ''' <returns>Collection of settings</returns>
    ''' <remarks></remarks>
    Public Shared Function LoadConfigSection(ByVal ConfigurationFilename As String, ByVal SectionName As String, ByRef DuplicateNameFound As Boolean, Optional ByVal NameFilter As String = "*") As Collections.Specialized.OrderedDictionary
        Dim ConfigNode As XPathNavigator = Nothing
        Dim SettingComplete As Boolean = False
        Dim MoveResult As Boolean = False
        Dim ConfigSection As New Collections.Specialized.OrderedDictionary
        Dim ConfigDocument As New ConfigXmlDocument

        If OpenConfigurationFile(ConfigDocument, ConfigNode, ConfigurationFilename) Then

            If ConfigNode.HasChildren Then
                ConfigNode.MoveToFirstChild()
                Do
                    If ConfigNode.Name Like SectionName Then
                        MoveResult = ConfigNode.MoveToFirstChild()
                        If MoveResult And Not ConfigNode.NodeType = XPathNodeType.Comment Then
                            Do
                                If (ConfigNode.Name Like (NameFilter)) And (ConfigNode.Name <> "") Then
                                    If ConfigSection.Contains(ConfigNode.Name) Then
                                        DuplicateNameFound = True
                                    Else
                                        ConfigSection.Add(ConfigNode.Name, ConfigNode.Value)
                                    End If
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

    ''' <summary>
    ''' Load all section names from the given file
    ''' </summary>
    ''' <param name="ConfigurationFilename">The filename to load</param>
    ''' <param name="SectionNameFilter">Optional filter of names to load</param>
    ''' <returns>Collection of section names</returns>
    ''' <remarks></remarks>
    Public Shared Function LoadConfigSectionNames(ByVal ConfigurationFilename As String, Optional ByVal SectionNameFilter As String = "*") As Collections.Specialized.OrderedDictionary
        Dim ConfigNode As XPathNavigator = Nothing
        Dim ConfigSectionNames As New Collections.Specialized.OrderedDictionary
        Dim ConfigDocument As New ConfigXmlDocument

        If OpenConfigurationFile(ConfigDocument, ConfigNode, ConfigurationFilename) Then

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
    ''' Adds the given section
    ''' </summary>
    ''' <param name="SectionName">The section to add</param>
    ''' <remarks>V2.0  D. Robinson</remarks>
    Public Shared Function AddConfigSection(ByVal ConfigurationFilename As String, ByVal SectionName As String) As Boolean
        Dim Result As Boolean = False
        Dim ConfigDocument As New ConfigXmlDocument

        Try
            Dim ConfigNode As XPathNavigator = Nothing

            If OpenConfigurationFile(ConfigDocument, ConfigNode, ConfigurationFilename) Then
                ConfigNode.AppendChildElement("", SectionName, "", "")

                Result = True

                ConfigNode = Nothing
            End If

        Catch ex As Exception
            WriteTraceMessage(ex.StackTrace)
        End Try

    End Function

    ''' <summary>
    ''' Adds the given setting
    ''' </summary>
    ''' <param name="SectionName">The setting to add</param>
    ''' <param name="SettingName">The setting to add</param>
    ''' <param name="SettingValue">The default value to write</param>
    ''' <remarks>V2.0  D. Robinson</remarks>
    Public Shared Function AddConfigSetting(ByVal ConfigurationFilename As String, ByVal SectionName As String, ByVal SettingName As String, ByVal SettingValue As String) As Boolean
        Dim SettingComplete As Boolean = False
        Dim ConfigDocument As New ConfigXmlDocument
        Dim Result As Boolean = False

        Try
            Dim ConfigNode As XPathNavigator = Nothing

            If OpenConfigurationFile(ConfigDocument, ConfigNode, ConfigurationFilename) Then
                ConfigNode.MoveToFirstChild()
                Do
                    If ConfigNode.Name Like SectionName Then
                        ConfigNode.AppendChildElement("", SettingName, Nothing, SettingValue)
                        ConfigDocument.Save(ConfigurationFilename)

                        SettingComplete = True
                        Result = True
                    End If
                Loop Until (ConfigNode.MoveToNext = False Or SettingComplete = True)

                ConfigNode = Nothing
            End If
        Catch ex As Exception
            WriteTraceMessage(ex.StackTrace)
        End Try

        Return Result

    End Function

    ''' <summary>
    ''' Adds the given attribute to a setting
    ''' </summary>
    ''' <param name="SectionName">The section to add to</param>
    ''' <param name="SettingName">The setting to add</param>
    ''' <param name="AttributeName"></param>
    ''' <param name="AttributeValue"></param>
    ''' <remarks>V2.0  D. Robinson</remarks>
    Public Shared Function AddConfigSettingAttribute(ByVal ConfigurationFilename As String, ByVal SectionName As String, ByVal SettingName As String, ByVal AttributeName As String, ByVal AttributeValue As String) As Boolean
        Dim SettingComplete As Boolean = False
        Dim ConfigDocument As New ConfigXmlDocument
        Dim Result As Boolean = False

        Try
            Dim ConfigNode As XPathNavigator = Nothing
            Dim ResultOfMove As Boolean = False

            If OpenConfigurationFile(ConfigDocument, ConfigNode, ConfigurationFilename) Then
                ResultOfMove = ConfigNode.MoveToChild(SectionName, "")
                If ResultOfMove = True Then
                    ResultOfMove = ConfigNode.MoveToChild(SettingName, "")
                    If ResultOfMove = True Then
                        ConfigNode.CreateAttribute("", AttributeName, Nothing, AttributeValue)

                        ConfigDocument.Save(ConfigurationFilename)

                        SettingComplete = True
                        Result = True
                    End If
                End If
                ConfigNode = Nothing
            End If
        Catch ex As Exception
            WriteTraceMessage(ex.StackTrace)
        End Try

        Return Result

    End Function

    ''' <summary>
    ''' Adds the given setting and attribute
    ''' </summary>
    ''' <param name="SectionName">The section to add to</param>
    ''' <param name="SettingName">The setting to add</param>
    ''' <param name="SettingValue">The default value to write</param>
    ''' <param name="AttributeName"></param>
    ''' <param name="AttributeValue"></param>
    ''' <remarks>V2.0  D. Robinson</remarks>
    Public Shared Function AddConfigSettingAndAttribute(ByVal ConfigurationFilename As String, ByVal SectionName As String, ByVal SettingName As String, ByVal SettingValue As String, ByVal AttributeName As String, ByVal AttributeValue As String) As Boolean
        Dim SettingComplete As Boolean = False
        Dim ConfigDocument As New ConfigXmlDocument
        Dim Result As Boolean = False

        Try
            Dim ConfigNode As XPathNavigator = Nothing

            If OpenConfigurationFile(ConfigDocument, ConfigNode, ConfigurationFilename) Then
                ConfigNode.MoveToFirstChild()
                Do
                    If ConfigNode.Name Like SectionName Then
                        ConfigNode.AppendChildElement("", SettingName, Nothing, SettingValue)
                        ConfigNode.MoveToChild(SettingName, "")
                        ConfigNode.CreateAttribute("", AttributeName, Nothing, AttributeValue)

                        ConfigDocument.Save(ConfigurationFilename)

                        SettingComplete = True
                        Result = True
                    End If
                Loop Until (ConfigNode.MoveToNext = False Or SettingComplete = True)

                ConfigNode = Nothing
            End If
        Catch ex As Exception
            WriteTraceMessage(ex.StackTrace)
        End Try

    End Function

    ''' <summary>
    ''' Reads the given String value
    ''' </summary>
    ''' <param name="SettingName">The Setting name</param>
    ''' <param name="DefaultValue">The default value to use</param>
    ''' <returns>The value</returns>
    ''' <remarks>V2.0  D. Robinson</remarks>
    Public Shared Function LoadConfigSetting(ByVal ConfigurationFilename As String, ByVal SectionName As String, ByVal SettingName As String, Optional ByVal DefaultValue As String = "") As String
        Dim ConfigNode As XPathNavigator = Nothing
        Dim ConfigDocument As New ConfigXmlDocument

        Try
            If OpenConfigurationFile(ConfigDocument, ConfigNode, ConfigurationFilename) Then
                Return LoadConfigValue(ConfigNode, SectionName, SettingName)
            Else
                Return DefaultValue ' Error opening External Configuration file
            End If
        Catch ex As Exception
            WriteTraceMessage(ex.StackTrace)
            Return DefaultValue
        End Try

    End Function

    Public Shared Function LoadConfigSettingAttribute(ByVal ConfigurationFilename As String, ByVal SectionName As String, ByVal SettingName As String, ByVal AttributeName As String, Optional ByVal DefaultValue As String = "") As String
        Try
            Dim ConfigNode As XPathNavigator = Nothing
            Dim ConfigDocument As New ConfigXmlDocument

            If OpenConfigurationFile(ConfigDocument, ConfigNode, ConfigurationFilename) Then
                Return LoadConfigAttribute(ConfigNode, SectionName, SettingName, AttributeName)
            Else
                Return DefaultValue
            End If
        Catch ex As Exception
            WriteTraceMessage(ex.StackTrace)
            Return DefaultValue
        End Try

    End Function

    Public Shared Function LoadConfigSettingAttributes(ByVal ConfigurationFilename As String, ByVal SectionName As String, ByVal SettingName As String, Optional ByVal NameFilter As String = "*") As Collections.Specialized.OrderedDictionary
        Dim ConfigNode As XPathNavigator = Nothing
        Dim SettingComplete As Boolean = False
        Dim ConfigContents As New Collections.Specialized.OrderedDictionary
        Dim ConfigDocument As New ConfigXmlDocument

        '' TODO:  Test this works!!!
        If OpenConfigurationFile(ConfigDocument, ConfigNode, ConfigurationFilename) Then
            If ConfigNode.HasChildren Then
                ConfigNode.MoveToFirstChild()
                Do
                    If ConfigNode.Name Like SectionName Then
                        ConfigNode.MoveToFirstChild()
                        Do
                            If ConfigNode.Name Like (NameFilter) Then
                                Dim AttributeValue As String = ""
                                Dim MoveResult As Boolean = False

                                MoveResult = ConfigNode.MoveToFirstAttribute()
                                Do Until MoveResult = False
                                    AttributeValue = ConfigNode.Value
                                    ConfigContents.Add(ConfigNode.Name, AttributeValue)
                                    MoveResult = ConfigNode.MoveToNextAttribute
                                Loop
                            End If
                        Loop Until ConfigNode.MoveToNext = False
                        SettingComplete = True
                    End If
                Loop Until (ConfigNode.MoveToNext = False Or SettingComplete = True)
            End If
        End If

        Return ConfigContents
    End Function

    Public Shared Function LoadConfigSectionAttribute(ByVal ConfigurationFilename As String, ByVal SectionName As String, ByVal AttributeName As String, Optional ByVal DefaultValue As String = "") As String
        Dim ConfigDocument As New ConfigXmlDocument
        Try
            Dim ConfigNode As XPathNavigator = Nothing

            If OpenConfigurationFile(ConfigDocument, ConfigNode, ConfigurationFilename) Then
                Return LoadConfigAttribute(ConfigNode, SectionName, AttributeName)
            Else
                Return DefaultValue ' Error opening External Configuration file
            End If
        Catch ex As Exception
            WriteTraceMessage(ex.StackTrace)
            Return DefaultValue
        End Try
    End Function

    Public Shared Function LoadConfigSectionAttributeNames(ByVal ConfigurationFilename As String, ByVal SectionName As String, ByVal SettingName As String, Optional ByVal NameFilter As String = "*") As Collections.Specialized.OrderedDictionary
        Dim ConfigNode As XPathNavigator = Nothing
        Dim SettingComplete As Boolean = False
        Dim MoveResult As Boolean = False
        Dim AttributeNames As New Collections.Specialized.OrderedDictionary
        Dim ConfigDocument As New ConfigXmlDocument

        '' TODO:  Test this works!!!
        If OpenConfigurationFile(ConfigDocument, ConfigNode, ConfigurationFilename) Then
            If ConfigNode.HasChildren Then
                ConfigNode.MoveToFirstChild()
                Do
                    If ConfigNode.Name Like SectionName Then
                        ConfigNode.MoveToFirstChild()
                        Do
                            If ConfigNode.Name Like (NameFilter) Then
                                Dim AttributeValue As String = ""

                                MoveResult = ConfigNode.MoveToFirstAttribute()
                                Do Until MoveResult = False
                                    AttributeValue = ConfigNode.Value
                                    AttributeNames.Add(ConfigNode.Name, AttributeValue)
                                    MoveResult = ConfigNode.MoveToNextAttribute
                                Loop
                            End If
                        Loop Until ConfigNode.MoveToNext = False
                        SettingComplete = True
                    End If
                Loop Until (ConfigNode.MoveToNext = False Or SettingComplete = True)
            End If
        End If

        Return AttributeNames
    End Function

    '''' <summary>
    '''' Writes the given String value
    '''' </summary>
    '''' <param name="SettingName">The Setting name</param>
    '''' <remarks>V2.0  D. Robinson</remarks>
    Public Shared Function SaveConfigSetting(ByVal ConfigurationFilename As String, ByVal SectionName As String, ByVal SettingName As String, ByVal Value As String) As Boolean
        Dim ConfigDocument As New ConfigXmlDocument

        Try
            Dim ConfigNode As XPathNavigator = Nothing

            If OpenConfigurationFile(ConfigDocument, ConfigNode, ConfigurationFilename) Then
                Return SaveConfigValue(ConfigurationFilename, ConfigDocument, ConfigNode, SectionName, SettingName, Value)
            End If
        Catch ex As Exception
            WriteTraceMessage(ex.StackTrace)
            Return False
        End Try

    End Function

    ''' <summary>
    ''' Writes the given String value
    ''' </summary>
    ''' <param name="SettingName">The Setting name</param>
    ''' <remarks>V2.0  D. Robinson</remarks>
    Public Shared Function SaveConfigSettingAttribute(ByVal ConfigurationFilename As String, ByVal SectionName As String, ByVal SettingName As String, ByVal AttributeName As String, ByVal AttributeValue As String) As Boolean
        Dim ConfigDocument As New ConfigXmlDocument

        Try
            Dim ConfigNode As XPathNavigator = Nothing

            If OpenConfigurationFile(ConfigDocument, ConfigNode, ConfigurationFilename) Then
                Return SaveConfigAttribute(ConfigNode, ConfigDocument, ConfigurationFilename, SectionName, SettingName, AttributeName, AttributeValue)
            Else
                Return False
            End If
        Catch ex As Exception
            WriteTraceMessage(ex.StackTrace)
            Return False
        End Try

    End Function

    ''' <summary>
    ''' Deletes the given setting
    ''' </summary>
    ''' <param name="SettingName">The Setting name</param>
    ''' <remarks>V2.0  D. Robinson</remarks>
    Public Shared Function DeleteConfigSetting(ByVal ConfigurationFilename As String, ByVal SectionName As String, ByVal SettingName As String) As Boolean
        Try
            Dim ConfigNode As XPathNavigator = Nothing
            Dim ConfigDocument As New ConfigXmlDocument
            Dim ResultOfMove As Boolean

            If OpenConfigurationFile(ConfigDocument, ConfigNode, ConfigurationFilename) Then
                ResultOfMove = ConfigNode.MoveToChild(SectionName, "")
                If ResultOfMove = True Then
                    ResultOfMove = ConfigNode.MoveToChild(SettingName, "")
                    If ResultOfMove = True Then
                        ConfigNode.DeleteSelf()
                        ConfigDocument.Save(ConfigurationFilename)
                        Return True
                    Else
                        ' Setting not found
                        Return False
                    End If
                Else
                    ' Section not found
                    Return False
                End If
            Else
                Return False ' Error opening External Configuration file
            End If
        Catch ex As Exception
            WriteTraceMessage(ex.StackTrace)
        End Try

    End Function

    ''' <summary>
    ''' Deletes the given setting
    ''' </summary>
    ''' <param name="SettingName">The Setting name</param>
    ''' <remarks>V2.0  D. Robinson</remarks>
    Public Shared Function DeleteConfigSettingAttribute(ByVal ConfigurationFilename As String, ByVal SectionName As String, ByVal SettingName As String, ByVal AttributeName As String) As Boolean
        Try
            Dim ConfigNode As XPathNavigator = Nothing
            Dim ConfigDocument As New ConfigXmlDocument
            Dim ResultOfMove As Boolean

            If OpenConfigurationFile(ConfigDocument, ConfigNode, ConfigurationFilename) Then
                ResultOfMove = ConfigNode.MoveToChild(SectionName, "")
                If ResultOfMove = True Then
                    ResultOfMove = ConfigNode.MoveToChild(SettingName, "")
                    If ResultOfMove = True Then
                        ResultOfMove = ConfigNode.MoveToAttribute(AttributeName, "")
                        If ResultOfMove = True Then
                            ConfigNode.DeleteSelf()

                            ConfigDocument.Save(ConfigurationFilename)
                            Return True
                        Else
                            Return False
                        End If
                    Else
                        ' Setting not found
                        Return False
                    End If
                Else
                    ' Section not found
                    Return False
                End If
            Else
                Return False ' Error opening External Configuration file
            End If
        Catch ex As Exception
            WriteTraceMessage(ex.StackTrace)
        End Try

    End Function

    ''' <summary>
    ''' Load configuration settings from app.config
    ''' </summary>
    ''' <param name="ConfigurationFilename"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function LoadApplicationConfigName(ByVal ConfigurationFilename As String) As String
        Dim ConfigNode As XPathNavigator = Nothing
        Dim ApplicationConfigName As String = "Unknown"
        Dim SettingsRootName As String = ""
        Dim ResultOfMove As Boolean = False
        Dim Counter As Int32 = 0
        Dim ConfigDocument As New ConfigXmlDocument

        Do
            Counter += 1

            Select Case Counter
                Case 1
                    SettingsRootName = "userSettings"
                Case 2
                    SettingsRootName = "applicationSettings"
            End Select

            If OpenConfigurationFile(ConfigDocument, ConfigNode, ConfigurationFilename) Then

                If ConfigNode.HasChildren Then
                    ResultOfMove = ConfigNode.MoveToChild(SettingsRootName, "")

                    ConfigNode.MoveToFirstChild() ' eg <userSettings>                      -->     <OperatorConsole.My.MySettings>
                    If ResultOfMove Then ApplicationConfigName = ConfigNode.Name
                End If
            End If
        Loop Until (ApplicationConfigName.Length > 0) Or Counter = 2

        Return ApplicationConfigName
    End Function

    ''' <summary>
    ''' Load configuration settings from [app].config
    ''' </summary>
    ''' <param name="ConfigurationFilename"></param>
    ''' <param name="ConfigSectionType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function LoadApplicationConfigFileSettings(ByVal ConfigurationFilename As String, ByVal ConfigSectionType As ApplicationConfigFileSectionType) As Collections.Specialized.OrderedDictionary
        Dim ConfigNode As XPathNavigator = Nothing
        Dim ConfigSettings As New Collections.Specialized.OrderedDictionary
        Dim SettingsRootName As String = ""
        Dim ConfigDocument As New ConfigXmlDocument

        Select Case ConfigSectionType
            Case ApplicationConfigFileSectionType.User
                SettingsRootName = "userSettings"
            Case ApplicationConfigFileSectionType.Application
                SettingsRootName = "applicationSettings"
        End Select

        If OpenConfigurationFile(ConfigDocument, ConfigNode, ConfigurationFilename) Then

            If ConfigNode.HasChildren Then
                ConfigNode.MoveToChild(SettingsRootName, "")

                ConfigNode.MoveToFirstChild()
                ConfigNode.MoveToFirstChild()
                Do
                    If (ConfigNode.Name = "setting") And (Not ConfigNode.NodeType = XPathNodeType.Comment) Then
                        Dim SettingName As String = ""
                        Dim SettingValue As String = ""

                        ConfigNode.MoveToFirstAttribute()
                        If ConfigNode.Name = "name" Then
                            SettingName = ConfigNode.Value
                            ConfigNode.MoveToParent()
                            ConfigNode.MoveToFirstChild()
                            If ConfigNode.Name = "value" Then
                                SettingValue = ConfigNode.Value
                            End If
                        End If

                        ConfigSettings.Add(SettingName, SettingValue)
                        ConfigNode.MoveToParent()
                    End If
                Loop Until (ConfigNode.MoveToNext = False)
            End If
        End If

        Return ConfigSettings
    End Function

    ''' <summary>
    ''' Save configuration settings to [app].config
    ''' </summary>
    ''' <param name="ConfigurationFilename"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function SaveApplicationConfigFileSettings(ByVal ConfigurationFilename As String) As Boolean
        Dim ConfigNode As XPathNavigator = Nothing
        Dim Result As Boolean
        Dim SettingsRootName As String = ""
        Dim ResultOfMove As Boolean = False
        Dim ConfigDocument As New ConfigXmlDocument

        If OpenConfigurationFile(ConfigDocument, ConfigNode, ConfigurationFilename) Then

            If ConfigNode.HasChildren Then
                ResultOfMove = ConfigNode.MoveToChild(SettingsRootName, "")

                ConfigNode.MoveToFirstChild()
                ConfigNode.MoveToFirstChild()
                Do
                    If (ConfigNode.Name = "setting") And (Not ConfigNode.NodeType = XPathNodeType.Comment) Then
                        Dim SettingName As String = ""
                        Dim SettingValue As String = ""

                        ResultOfMove = ConfigNode.MoveToFirstAttribute()
                        If ConfigNode.Name = "name" Then
                            SettingName = ConfigNode.Value
                            ConfigNode.MoveToParent()
                            ConfigNode.MoveToFirstChild()
                            If ConfigNode.Name = "value" Then
                                SettingValue = ConfigNode.Value
                            End If
                        End If

                        ConfigNode.MoveToParent()
                    End If
                Loop Until (ConfigNode.MoveToNext = False)
            End If
        End If

        Return Result
    End Function

#End Region

#Region " Private methods "

    ''' <summary>
    ''' Opens the External Configuration file
    ''' </summary>
    ''' <param name="Navigator">XPathNavigator</param>
    ''' <remarks>V2.0  D. Robinson</remarks>
    Private Shared Function OpenConfigurationFile(ByRef ConfigDocument As ConfigXmlDocument, ByRef Navigator As XPathNavigator, ByVal ConfigurationFilename As String) As Boolean
        Dim Result As Boolean = False
        'Dim ConfigDocument As New ConfigXmlDocument

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
                WriteTraceMessage(ex.StackTrace)
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

    ''' <summary>
    ''' Writes the value to the configuration file
    ''' </summary>
    ''' <param name="Node">XPathNavigator</param>
    ''' <param name="SectionName">The Section name</param>
    ''' <param name="SettingName">The Setting name</param>
    ''' <param name="SettingValue">The value</param>
    ''' <remarks>V2.0  D. Robinson</remarks>
    Private Shared Function SaveConfigValue(ByVal ConfigurationFilename As String, ByRef ConfigDocument As ConfigXmlDocument, ByVal Node As XPathNavigator, ByVal SectionName As String, ByVal SettingName As String, ByVal SettingValue As String) As Boolean
        Dim ResultOfMove As Boolean = False
        Dim ReadComplete As Boolean = False
        'Dim ConfigDocument As New ConfigXmlDocument

        'ConfigDocument.Load(ConfigurationFilename)

        If Node.HasChildren Then
            Node.MoveToFirstChild()
            Do
                If Node.Name Like SectionName Then
                    Do
                        Node.SelectSingleNode(SettingName)
                        ResultOfMove = Node.MoveToChild(SettingName, "")
                        If ResultOfMove = True Then
                            ' Save the value to this node
                            Try
                                Node.SetValue(SettingValue)
                                ConfigDocument.Save(ConfigurationFilename)
                                Return True
                            Catch ex As Exception
                                WriteTraceMessage(ex.StackTrace)
                                Return False
                            End Try
                            ReadComplete = True
                        Else  ' Unsuccessful
                            Try
                                Node.AppendChild(SettingName)
                                Try
                                    ResultOfMove = Node.MoveToChild(SettingName, "")
                                    Node.SetValue(SettingValue)
                                Catch ex As Exception
                                    WriteTraceMessage(ex.StackTrace)
                                    Return False
                                End Try
                            Catch ex As Exception
                                WriteTraceMessage(ex.StackTrace)
                                Return False
                            End Try
                            ReadComplete = True
                        End If
                    Loop Until Node.MoveToNext = False Or ReadComplete = True
                End If
            Loop Until (Node.MoveToNext = False Or ReadComplete = True)
        End If

        Return False
    End Function

    ''' <summary>
    ''' Writes the attribute to the configuration file
    ''' </summary>
    ''' <param name="Node">XPathNavigator</param>
    ''' <param name="SectionName">The Section name</param>
    ''' <param name="SettingName">The Setting name</param>
    ''' <param name="AttributeName">The attribute</param>
    ''' <param name="AttributeValue">The attribute value</param>
    ''' <remarks>V2.0  D. Robinson</remarks>
    Private Shared Function SaveConfigAttribute(ByVal Node As XPathNavigator, ByRef ConfigDocument As ConfigXmlDocument, ByVal ConfigurationFilename As String, ByVal SectionName As String, ByVal SettingName As String, ByVal AttributeName As String, ByVal AttributeValue As String) As Boolean
        Dim ResultOfMove As Boolean = False
        Dim ReadComplete As Boolean = False

        'ConfigDocument.Load(ConfigurationFilename)

        If Node.HasChildren Then
            Node.MoveToFirstChild()
            Do
                If Node.Name Like SectionName Then
                    Do
                        Node.SelectSingleNode(SettingName)
                        ResultOfMove = Node.MoveToChild(SettingName, "")
                        If ResultOfMove = True Then
                            ' Save the value to this attribute
                            Try
                                Node.MoveToAttribute(AttributeName, "")

                                Node.SetValue(AttributeValue)

                                ConfigDocument.Save(ConfigurationFilename)
                                ConfigDocument = Nothing
                            Catch ex As Exception
                                WriteTraceMessage(ex.StackTrace)
                                ConfigDocument = Nothing
                                Return False
                            End Try
                            ReadComplete = True
                        Else
                            ' Unsuccessful
                            ConfigDocument = Nothing
                            Return False
                        End If
                    Loop Until Node.MoveToNext = False Or ReadComplete = True
                End If
            Loop Until (Node.MoveToNext = False Or ReadComplete = True)
        End If

        Return True
    End Function

#End Region

End Class
