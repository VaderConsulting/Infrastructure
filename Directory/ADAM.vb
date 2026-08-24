Imports System.DirectoryServices
Imports System.Security.Principal

Public Class DirectoryConnectionProperties

    Public Structure Connection
        Dim Name As String
        Dim Value As String
    End Structure

    Private _Username As String = ""
    Private _Password As String = ""
    Private _DirectoryRoot As String = ""
    Private _PersonRoot As String = ""
    Private _DirectoryPaths As Collections.ObjectModel.Collection(Of Connection)

    Private Sub Main()
        Dim x As New Connection
        x.Name = "Name"
        x.Value = "Value"

        _DirectoryPaths.Add(x)

    End Sub

End Class

Public Class ADAM

    Public Shared Function SetADAMPassword(ByVal UserCN As String, ByVal Password As String) As Boolean

    End Function

    'Public Function GetDirectoryBranch(ByVal ObjectPath As String) As Collections.Specialized.OrderedDictionary
    '    Dim ReturnObject As New Collections.Specialized.OrderedDictionary
    '    Dim NewObjectPath As String = ObjectPath
    '    Dim DirectoryObject As DirectoryEntry = GetDirectoryObject(NewObjectPath, False)
    '    Dim DirectoryRoot As String = LoadConfigSetting("SystemSettings", "DirectoryRoot", CONFIG_TYPE.System)
    '    Dim PhoneRoot As String = LoadConfigSetting("DirectorySettings", "DirectoryPhoneRoot", CONFIG_TYPE.User)
    '    Dim PersonRoot As String = LoadConfigSetting("DirectorySettings", "DirectoryUserRoot", CONFIG_TYPE.User)

    '    If DirectoryRoot = "" Then
    '        Dim ConfigRoot As String = LoadConfigSetting("DirectoryServer", "ConfigurationRoot", CONFIG_TYPE.System)
    '        DirectoryRoot = ConfigRoot.Substring(DirectoryRoot.IndexOf(",") + 1, CInt(ConfigRoot.Length - CInt(ConfigRoot.Substring(DirectoryRoot.IndexOf(",")))))
    '    End If

    '    Dim Path As String() = {}
    '    Dim CommonPath As String = DirectoryRoot
    '    Dim ParentOUPaths As New Collections.Specialized.NameValueCollection

    '    NewObjectPath = Replace(NewObjectPath.ToUpper, CommonPath.ToUpper, "")

    '    If NewObjectPath.EndsWith(",") Then NewObjectPath = NewObjectPath.Substring(0, Len(NewObjectPath) - 1)

    '    Path = Split(NewObjectPath, ",")

    '    Dim ParentEntry As DirectoryEntry = DirectoryObject.Parent
    '    'Dim ParentObject As DirectoryObject = New DirectoryObject(ParentEntry.Path)
    '    Dim OULevelCount As Integer = 0

    '    For i As Integer = 0 To ParentEntry.Path.ToString.Length - 1
    '        If ParentEntry.Path.Substring(i, 1) = "," Then OULevelCount += 1
    '    Next
    '    ' OULevelCount now contains the number of hierarchical levels deep the Object is.

    '    ' Retrieve the path of each OU above the Object, until we get to the Root
    '    Dim Index As Integer = 0
    '    Try
    '        Do Until (ParentEntry.Path.ToLower = ("ldap://" & CurrentDirectoryServer & PhoneRoot).ToLower) Or _
    '                 (ParentEntry.Path.ToLower = ("ldap://" & CurrentDirectoryServer & PersonRoot).ToLower) Or _
    '                 (ParentEntry.Path.ToLower = ("ldap://" & CurrentDirectoryServer & DirectoryRoot).ToLower) ' For other objects
    '            ParentOUPaths.Add(Index.ToString, ParentEntry.Path)

    '            ParentEntry = ParentEntry.Parent

    '            'ParentEntry = New DirectoryObject(ParentEntry.Path)
    '            Index += 1
    '        Loop
    '    Catch ex As Exception
    '        'FileLoggingObject.WriteErrorEvent("Error in GetDirectoryBranch().")
    '        'FileLoggingObject.WriteErrorEvent("The line(s) between the asterisks contain the Exception message.")
    '        'FileLoggingObject.WriteErrorEvent("************************************************************************")
    '        'FileLoggingObject.WriteErrorEvent(ex.ToString)
    '        'FileLoggingObject.WriteErrorEvent("************************************************************************")
    '        'LastStatus.LastException = ex
    '    End Try

    '    ' Now to reorder the Parent Paths
    '    Dim OULevelCountBelowUserRoot = Index
    '    Dim ParentOUPath(ParentOUPaths.Count - 1) As String

    '    For i As Integer = 0 To ParentOUPaths.Count - 1
    '        Dim Branch As New DirectoryBranch

    '        Branch.Level = i
    '        Branch.ParentPath = ParentOUPaths((OULevelCountBelowUserRoot - i - 1))

    '        ReturnObject.Add(i, Branch)
    '        'ParentOUPath(i) = DirectoryDisplayTextforOUS(ParentOUPaths(OULevelCountBelowUserRoot - i - 1))
    '        ParentOUPath(i) = ParentOUPaths(OULevelCountBelowUserRoot - i - 1)
    '    Next

    '    ' Retrieve the other object information suitable for filling our DirectoryBranch properties.
    '    Dim OUCode(ParentOUPaths.Count - 1) As String
    '    Dim OUName(ParentOUPaths.Count - 1) As String
    '    For OULevelCounter As Integer = 0 To ParentOUPaths.Count - 1
    '        Dim OU As DirectoryEntry = GetDirectoryObject(ParentOUPath(OULevelCounter))
    '        Dim ThisBranch As DirectoryBranch = CType(ReturnObject.Item(OULevelCounter), DirectoryBranch)

    '        Try
    '            ' Is it MultiValued?
    '            ThisBranch.Path = RemoveServerFromDN(OU.Path)
    '            ThisBranch.DisplayPath = RemoveServerFromDN(OU.Path)
    '            Try
    '                ThisBranch.ParentPath = RemoveServerFromDN(OU.Parent.Path)
    '            Catch
    '            End Try
    '            ThisBranch.Level = OULevelCounter
    '            ThisBranch.Name = OU.Name.Substring(3)
    '            ThisBranch.DisplayName = OU.Name.Substring(3)

    '            Select Case OU.SchemaClassName.ToLower
    '                Case "container"
    '                    ThisBranch.Class = DirectoryBranch.SchemaClass.Container
    '                Case "computer"
    '                    ThisBranch.Class = DirectoryBranch.SchemaClass.Computer
    '                Case "domain"
    '                    ThisBranch.Class = DirectoryBranch.SchemaClass.Domain
    '                Case "organizationalunit"
    '                    ThisBranch.Class = DirectoryBranch.SchemaClass.OrganisationUnit
    '                Case "group"
    '                    ThisBranch.Class = DirectoryBranch.SchemaClass.Group
    '                Case "user"
    '                    ThisBranch.Class = DirectoryBranch.SchemaClass.User
    '                Case Else
    '                    ThisBranch.Class = DirectoryBranch.SchemaClass.Other
    '            End Select

    '            If OU.Properties.Contains("description") Then
    '                OUCode(OULevelCounter) = GetADAttribute(OU, "description")
    '                ThisBranch.Description = OUCode(OULevelCounter)
    '            Else
    '                OUCode(OULevelCounter) = ""
    '                ThisBranch.Description = ""
    '            End If

    '            OUName(OULevelCounter) = GetADAttribute(OU, "name")
    '            ThisBranch.PathCollection.Add(OULevelCounter, OUName(OULevelCounter))

    '            'Console.WriteLine(Space(OULevelCounter * 3) & OUName(OULevelCounter))
    '        Catch ex As Exception
    '            ' No description (code)
    '            OUCode(OULevelCounter) = ""
    '            OUName(OULevelCounter) = ""
    '        End Try

    '        ReturnObject.Item(OULevelCounter) = ThisBranch

    '        OU.Close()
    '    Next

    '    ' If this object is a container (OU etc), then we have to add it's own path
    '    If (DirectoryObject.SchemaClassName = "organizationalUnit" Or DirectoryObject.SchemaClassName = "container") And ReturnObject.Count = 0 Then
    '        'Dim FinalBranch As DirectoryBranch = CType(ReturnObject.Item(0), DirectoryBranch)
    '        Dim FinalBranch As DirectoryBranch = New DirectoryBranch

    '        FinalBranch.Path = RemoveServerFromDN(ObjectPath)
    '        'FinalBranch.DisplayPath = RemoveServerFromDN(DirectoryDisplayTextforOUS(ObjectPath))
    '        FinalBranch.DisplayPath = RemoveServerFromDN(ObjectPath)

    '        FinalBranch.PathCollection.Add(0, GetADAttribute(DirectoryObject, "name"))

    '        FinalBranch.Level = 0
    '        FinalBranch.Name = DirectoryObject.Name.Substring(3)
    '        'FinalBranch.DisplayName = DirectoryDisplayTextforOUS(DirectoryObject.Name).Substring(3)
    '        FinalBranch.DisplayName = DirectoryObject.Name.Substring(3)

    '        Select Case DirectoryObject.SchemaClassName.ToLower
    '            Case "container"
    '                FinalBranch.Class = DirectoryBranch.SchemaClass.Container
    '            Case "computer"
    '                FinalBranch.Class = DirectoryBranch.SchemaClass.Computer
    '            Case "domain"
    '                FinalBranch.Class = DirectoryBranch.SchemaClass.Domain
    '            Case "organizationalunit"
    '                FinalBranch.Class = DirectoryBranch.SchemaClass.OrganisationUnit
    '            Case "group"
    '                FinalBranch.Class = DirectoryBranch.SchemaClass.Group
    '            Case "user"
    '                FinalBranch.Class = DirectoryBranch.SchemaClass.User
    '            Case Else
    '                FinalBranch.Class = DirectoryBranch.SchemaClass.Other
    '        End Select

    '        If DirectoryObject.Properties.Contains("description") Then
    '            'OUCode(0) = GetADAttribute(DirectoryObject, "description")
    '            FinalBranch.Description = GetADAttribute(DirectoryObject, "description")
    '        Else
    '            'OUCode(0) = ""
    '            FinalBranch.Description = ""
    '        End If

    '        ReturnObject.Add(0, FinalBranch)
    '        'ReturnObject.Item(ParentOUPaths) = FinalBranch
    '    End If

    '    'If ReturnObject.Count > 0 Then
    '    '    Dim FinalBranch As DirectoryBranch = CType(ReturnObject.Item(0), DirectoryBranch)

    '    '    FinalBranch.PathCollection.Add(ParentOUPaths.Count, GetADAttribute(DirectoryObject, "name"))
    '    '    ReturnObject.Item(ParentOUPaths) = FinalBranch
    '    'Else
    '    '    ' Single level
    '    '    Console.WriteLine("Single level branch")
    '    'End If

    '    DirectoryObject.Close()
    '    DirectoryObject = Nothing
    '    ' ----------------------

    '    Return ReturnObject
    'End Function

    'Public Function GetDirectoryObject(ByVal FullObjectPath As String, ByVal UseAnonymousConnection As Boolean) As DirectoryEntry ' DirectoryEntry
    '    Dim ReturnObject As DirectoryEntry = Nothing
    '    Dim ShortPath As String = RemoveServerFromDN(FullObjectPath)

    '    If CurrentDirectoryServer & "" <> "" Then
    '        ShortPath = "LDAP://" & CurrentDirectoryServer & ShortPath 'DirectorySaveTextforOUS(ShortPath)
    '    Else
    '        ShortPath = FullObjectPath
    '    End If

    '    'Debug.Print("Directory Safe Path: " & DirectorySafePath)

    '    Try
    '        If UseAnonymousConnection Then
    '            ReturnObject = New DirectoryEntry(ShortPath) ' New DirectoryEntry(DirectorySafePath)
    '        Else
    '            ReturnObject = New DirectoryEntry(ShortPath, TargetDirectoryConnectionUsername, TargetDirectoryConnectionPassword, _DirectoryAuthenticationType) ' New DirectoryEntry(DirectorySafePath, TargetDirectoryConnectionUsername, TargetDirectoryConnectionPassword, m_DirectoryAuthenticationType)
    '        End If
    '    Catch ex As Exception
    '        ' If there was an error, we will just return the empty ReturnObject
    '        Debug.Print("Error in GetDirectoryObject!!")
    '    End Try
    '    Return ReturnObject
    'End Function

    'Public Function RemoveServerFromDN(ByVal DNPath As String) As String
    '    Dim ResultString As String = ""
    '    DNPath &= ""

    '    If DNPath.ToUpper.StartsWith("LDAP://") Then
    '        ResultString = DNPath.Substring(7)
    '        ResultString = ResultString.Substring(ResultString.IndexOf("/") + 1)
    '    Else
    '        ResultString = DNPath
    '    End If

    '    Return ResultString
    'End Function

End Class
