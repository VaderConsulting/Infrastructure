Imports System.Drawing

Public Class DirectoryBranch
    Implements IDisposable

    Public Enum SchemaClass As Int32
        Other = 0
        Domain = 1
        Site = 2
        SiteLink = 3
        DomainComponent = 4
        OrganisationUnit = 5
        Container = 6
        Server = 10
        Computer = 11
        Desktop = 12
        Laptop = 13
        Printer = 14
        Group = 20
        User = 30
        Person = 31
        Phone = 32
        Car = 33
        Asset = 34
    End Enum

    Private _DisposedValue As Boolean = False        ' To detect redundant calls
    Private _Path As String = ""
    Private _Name As String = ""
    Private _Description As String = ""
    Private _Level As Int32 = -1
    Private _Class As SchemaClass
    Private _ParentPath As String = ""
    Private _DisplayName As String = ""
    Private _DisplayPath As String = ""
    Private _PathCollection As New Collections.Specialized.OrderedDictionary
    Private _Image As Image = Nothing
    Private _Children As New Collections.ObjectModel.Collection(Of DirectoryBranch)
    Private _HasChildren As Boolean = False
    Private _SourceImageList As Windows.Forms.ImageList = Nothing
    Private _ImageIndex As Int32 = -1

    Public Sub New()

    End Sub

#Region " Properties "

    Public Property Path() As String
        Get
            Return _Path
        End Get
        Set(ByVal value As String)
            _Path = value
        End Set
    End Property

    Public Property Name() As String
        Get
            Return _Name
        End Get
        Set(ByVal value As String)
            _Name = value
        End Set
    End Property

    Public Property Description() As String
        Get
            Return _Description
        End Get
        Set(ByVal value As String)
            _Description = value
        End Set
    End Property

    Public Property Level() As Int32
        Get
            Return _Level
        End Get
        Set(ByVal value As Int32)
            _Level = value
        End Set
    End Property

    Public Property [Class]() As SchemaClass
        Get
            Return _Class
        End Get
        Set(ByVal value As SchemaClass)
            _Class = value

            Select Case _Class
                Case SchemaClass.Asset
                    _ImageIndex = 18
                Case SchemaClass.Car
                    _ImageIndex = 17
                Case SchemaClass.Computer
                    _ImageIndex = 9
                Case SchemaClass.Container
                    _ImageIndex = 7
                Case SchemaClass.Desktop
                    _ImageIndex = 10
                Case SchemaClass.Domain
                    _ImageIndex = 2
                Case SchemaClass.DomainComponent
                    _ImageIndex = 3
                Case SchemaClass.Group
                    _ImageIndex = 13
                Case SchemaClass.Laptop
                    _ImageIndex = 11
                Case SchemaClass.OrganisationUnit
                    _ImageIndex = 6
                Case SchemaClass.Other
                    _ImageIndex = 1
                Case SchemaClass.Person
                    _ImageIndex = 15
                Case SchemaClass.Phone
                    _ImageIndex = 16
                Case SchemaClass.Printer
                    _ImageIndex = 12
                Case SchemaClass.Server
                    _ImageIndex = 8
                Case SchemaClass.Site
                    _ImageIndex = 3
                Case SchemaClass.SiteLink
                    _ImageIndex = 4
                Case SchemaClass.User
                    _ImageIndex = 14
                Case Else
                    _ImageIndex = 1
            End Select

            If _Path.Contains("recycle bin") Then
                _ImageIndex = 0
            End If

        End Set
    End Property

    Public Property ParentPath() As String
        Get
            Return _ParentPath
        End Get
        Set(ByVal value As String)
            _ParentPath = value
        End Set
    End Property

    Public Property DisplayName() As String
        Get
            Return _DisplayName
        End Get
        Set(ByVal value As String)
            _DisplayName = value
        End Set
    End Property

    Public Property DisplayPath() As String
        Get
            Return _DisplayPath
        End Get
        Set(ByVal value As String)
            _DisplayPath = value
        End Set
    End Property

    Public ReadOnly Property Image() As Image
        Get
            Return _Image
        End Get
    End Property

    Public ReadOnly Property ImageIndex() As Int32
        Get
            Return _ImageIndex
        End Get
    End Property

    Public Property Children() As Collections.ObjectModel.Collection(Of DirectoryBranch)
        Get
            Return _Children
        End Get
        Set(ByVal value As Collections.ObjectModel.Collection(Of DirectoryBranch))
            _Children = value
            If Not value Is Nothing AndAlso value.Count > 0 Then
                _HasChildren = True
            End If
        End Set
    End Property

    Public ReadOnly Property NameAndDescription() As String
        Get
            If _Description <> "" Then
                Return _DisplayName & " (" & _Description & ")"
            Else
                Return _DisplayName
            End If
        End Get
    End Property

    Public Property SourceImageList() As Windows.Forms.ImageList
        Get
            Return _SourceImageList
        End Get
        Set(ByVal value As Windows.Forms.ImageList)
            _SourceImageList = value
        End Set
    End Property

    Public Property PathCollection() As Collections.Specialized.OrderedDictionary
        Get
            Return _PathCollection
        End Get
        Set(ByVal value As Collections.Specialized.OrderedDictionary)
            _PathCollection = value
        End Set
    End Property

#End Region

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me._DisposedValue Then
            If disposing Then
                ' TODO: free other state (managed objects).
            End If

            ' TODO: free your own state (unmanaged objects).
            ' TODO: set large fields to null.
        End If
        Me._DisposedValue = True
    End Sub

#Region " IDisposable Support "
    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
