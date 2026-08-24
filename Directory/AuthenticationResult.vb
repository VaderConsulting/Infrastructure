Imports System.Security.Principal

Public Class AuthenticationResult

    Public Enum LogonType As Int32
        ConfigurationFile = 0
        ADAM = 1
        ActiveDirectory = 2
        SQLServer = 3
    End Enum

    Private _AccountLocked As Boolean
    Private _AccountDisabled As Boolean
    Private _AccountExpired As Boolean
    Private _PasswordExpired As Boolean
    Private _Success As Boolean
    Private _Cancelled As Boolean
    Private _MultipleAccountsFound As Boolean
    Private _MemberOf As System.DirectoryServices.ActiveDirectory.AdamRoleCollection ' Collections.Specialized.OrderedDictionary
    Private _AuthenticationType As LogonType
    Private _AccountNotFound As Boolean
    Private _DirectoryRootPath As String
    Private _AssociatedAccount As System.DirectoryServices.DirectoryEntry
    Private _Context As System.DirectoryServices.AccountManagement.PrincipalContext


End Class
