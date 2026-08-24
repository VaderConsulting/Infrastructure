Imports System.Data.SqlClient

Public Class Functions

    Public Shared Function ConvertToSQLType(ByVal InputValue As Object, ByVal SQLValueType As String) As Object
        Dim ConvertedValue As Object = Nothing

        Select Case SQLValueType.ToLower & ""
            Case "boolean"
                Select Case InputValue.ToLower
                    Case "y", "1", "-1", "yes", "true", "checked", "selected", "on"
                        ConvertedValue = True
                    Case Else
                        ConvertedValue = False
                End Select
            Case "integer", "decimal"
                Select Case InputValue.ToLower
                    Case "y", "1", "-1", "yes", "true", "checked", "selected", "on"
                        ConvertedValue = 1
                    Case "n", "0", "no", "unchecked", "unselected", "false", "off"
                        ConvertedValue = 0
                    Case Else
                        If IsNumeric(InputValue) Then
                            ConvertedValue = CInt(InputValue)
                        Else
                            ConvertedValue = 0
                        End If
                End Select
            Case "short"
                If IsNumeric(InputValue) Then
                    ConvertedValue = CShort(InputValue)
                Else
                    ConvertedValue = 0
                End If
            Case "long"
                If IsNumeric(InputValue) Then
                    ConvertedValue = CLng(InputValue)
                Else
                    ConvertedValue = 0
                End If
            Case "date"
                If IsDate(InputValue) Then
                    ConvertedValue = CDate(InputValue)
                Else
                    ConvertedValue = "01/01/0001"
                End If
            Case "double"
                If IsNumeric(InputValue) Then
                    ConvertedValue = CDbl(InputValue)
                Else
                    ConvertedValue = 0
                End If
            Case "string", ""
                ConvertedValue = CStr(InputValue)
            Case Else ' String and unknown types
                ConvertedValue = CStr(InputValue)
        End Select

        Return ConvertedValue
    End Function

    ''' <summary>
    ''' Returns a List of Databases
    ''' </summary>
    ''' <param name="DatabaseServer"></param>
    ''' <param name="Username"></param>
    ''' <param name="Password"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetDatabases(ByVal DatabaseServer As String, ByVal Username As String, ByVal Password As String) As Collections.Specialized.OrderedDictionary
        Dim DatabaseList As New Collections.Specialized.OrderedDictionary
        Dim SelectQuery As String = ""
        Dim ConnectionString As String = ""
        Dim DataReader As SqlDataReader

        SelectQuery = "SELECT DISTINCT DatabaseName, ConnectionString"
        SelectQuery &= " FROM         DatabaseDetails"
        SelectQuery &= " WHERE     (EndDate IS NULL)"

        ConnectionString = "Server=" & DatabaseServer & ";Database=StratatelControl_Test; User ID=" & Username & ";Password=" & Password & ";Max Pool Size=5000;"

        ' ADO.NET Objects
        Dim DataConnection As New SqlConnection(ConnectionString)
        Dim DataCommand As New SqlCommand(SelectQuery, DataConnection)

        ' Open Connection and retrieve databases
        Try
            DataCommand.Connection = DataConnection
            Using DataConnection
                ' Execute query
                DataConnection.Open()
                DataReader = DataCommand.ExecuteReader()

                While DataReader.Read
                    DatabaseList.Add(DataReader("DatabaseName").ToString, DataReader("ConnectionString").ToString)
                End While
            End Using

            ' Close Reader
            DataReader.Close()
        Catch ex As SqlException
            'LogInfo.WriteError("Error: Error Connecting to Database (Unable to get Databases)")
        Catch ex As Exception
            ' _AppLogging.WriteError("Error: Error Connecting to Database (Unable to get Databases)")
        Finally
            ' Close Connection
            DataConnection.Close()
        End Try

        Return DatabaseList
    End Function

    ''' <summary>
    ''' Returns a List of Clients
    ''' </summary>
    ''' <param name="ConnectionString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetClients(ByVal ConnectionString As String) As Collections.Specialized.OrderedDictionary
        Dim ClientList As New Collections.Specialized.OrderedDictionary
        Dim DataReader As SqlDataReader = Nothing
        Dim SelectQuery As String

        SelectQuery = "SELECT     ClientID, ClientName"
        SelectQuery &= " FROM         Client"
        SelectQuery &= " WHERE     (EndDate IS NULL)"

        ' ADO.NET Objects
        Dim DataConnection As New SqlConnection(ConnectionString)
        Dim DataCommand As New SqlCommand(SelectQuery, DataConnection)

        ' Open Connection and retrieve databases
        Try
            DataCommand.Connection = DataConnection
            Using DataConnection
                ' Execute query
                DataConnection.Open()
                DataReader = DataCommand.ExecuteReader()

                While DataReader.Read
                    ClientList.Add(DataReader("ClientName").ToString, DataReader("ClientID").ToString)
                End While
            End Using

            ' Close Reader
            DataReader.Close()
        Catch ex As SqlException
            'LogInfo.WriteError("Error: Error Connecting to Database (Unable to get Clients)")
        Catch ex As Exception
            '_AppLogging.WriteError("Error: Error Connecting to Database (Unable to get Clients)")
        Finally
            ' Close Connection
            DataConnection.Close()
        End Try

        Return ClientList
    End Function

    ''' <summary>
    ''' Returns a List of Service Providers
    ''' </summary>
    ''' <param name="ConnectionString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetServiceProviders(ByVal ConnectionString As String) As Collections.Specialized.OrderedDictionary
        Dim ProviderList As New Collections.Specialized.OrderedDictionary
        Dim SelectQuery As String = ""
        Dim DataReader As SqlDataReader = Nothing

        SelectQuery = "SELECT     UID, ProviderName, ServiceType"
        SelectQuery &= " FROM         ServiceProvider"

        ' ADO.NET Objects
        Dim DataConnection As New SqlConnection(ConnectionString)
        Dim DataCommand As New SqlCommand(SelectQuery, DataConnection)

        ' Open Connection and retrieve databases
        Try
            DataCommand.Connection = DataConnection
            Using DataConnection
                ' Execute query
                DataConnection.Open()
                DataReader = DataCommand.ExecuteReader()

                While DataReader.Read
                    Dim ListBoxItem As New GenericListBoxItem

                    ListBoxItem.ItemName = DataReader("ProviderName").ToString
                    ListBoxItem.ItemID = DataReader("UID").ToString
                    ListBoxItem.Item1 = DataReader("ServiceType").ToString

                    ProviderList.Add(ProviderList.Count + 1, ListBoxItem)
                End While
            End Using

            ' Close Reader
            DataReader.Close()
        Catch ex As SqlException
            '_AppLogging.WriteError("Error: Error Connecting to Database (Unable to get ServiceProvider)")
        Catch ex As Exception
            ' _AppLogging.WriteError("Error: Error Connecting to Database (Unable to get ServiceProvider)")
        Finally
            ' Close Connection
            DataConnection.Close()
        End Try

        Return ProviderList
    End Function

    ''' <summary>
    ''' Gets the Site ID based on the database name selected
    ''' </summary>
    ''' <param name="ConnectionStr"></param>
    ''' <param name="DatabaseName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetSiteID(ByVal ConnectionStr As String, ByVal DatabaseName As String) As Integer
        Dim SiteID As Integer = -1

        ' Select Statement
        Dim SelectQuery As String
        SelectQuery = "SELECT     DatabaseDetails.UID, DatabaseDetails.DatabaseName, DatabaseDetails.StartDate, DatabaseDetails.EndDate, DatabaseDetails.ConnectionString, SiteDetails.SiteID"
        SelectQuery &= " FROM         DatabaseDetails INNER JOIN"
        SelectQuery &= "   SiteDetails ON DatabaseDetails.UID = SiteDetails.DatabaseID"
        SelectQuery &= " WHERE     (DatabaseDetails.DatabaseName = '" & DatabaseName & "')"

        ' ADO.NET Objects
        Dim DataConnection As New SqlConnection(ConnectionStr)
        Dim DataCommand As New SqlCommand(SelectQuery, DataConnection)
        Dim DataReader As SqlDataReader

        ' Open Connection and retrieve databases
        Try
            DataCommand.Connection = DataConnection
            Using DataConnection
                ' Execute query
                DataConnection.Open()
                DataReader = DataCommand.ExecuteReader()

                While DataReader.Read

                    SiteID = CInt(DataReader("SiteID"))

                End While

            End Using

            ' Close Reader
            DataReader.Close()

        Catch ex As SqlException
            '_AppLogging.WriteError("Error: Error Connecting to Database (Unable to get SiteID)")
        Catch ex As Exception
            ' _AppLogging.WriteError("Error: Error Connecting to Database (Unable to get SiteID)")
        Finally
            ' Close Connection
            DataConnection.Close()
        End Try

        Return SiteID

    End Function

End Class

' TODO:  Remove this class as it is a departure from standards
Public Class GenericListBoxItem

    Public ItemName As String
    Public ItemID As String
    Public Item1 As String
    Public Item2 As String

    Public Overrides Function ToString() As String
        Return ItemName
    End Function

End Class
