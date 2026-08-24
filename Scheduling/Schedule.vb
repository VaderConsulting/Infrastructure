Imports System.Reflection

Public Class Schedule

#Region " Private variables "

    Private m_GUIDString As String = ""
    Private m_Path As String = ""
    Private m_Arguments As String = ""
    Private m_StartPath As String = ""
    Private m_MaximumRunTimeMinutes As Int32 = -1
    Private m_AllowConcurrent As Boolean = True
    Private m_LastTime As DateTime = New DateTime(1, 1, 1)
    Private m_LastResult As String = ""
    Private m_Frequency As Scheduling.Functions.ScheduleFrequency = Nothing
    Private m_FrequencyValue As String = ""
    Private m_Username As String = ""
    Private m_Password As String = ""
    Private m_ScheduleSettingsFilename As String = IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.AllUsersApplicationData, "schedule.xml")

#End Region

#Region " Properties "

    Public Property GUIDString() As String
        Get
            Return m_GUIDString
        End Get
        Set(ByVal value As String)
            m_GUIDString = value
        End Set
    End Property

    Public Property Path() As String
        Get
            Return m_Path
        End Get
        Set(ByVal value As String)
            m_Path = value
        End Set
    End Property

    Public Property Arguments() As String
        Get
            Return m_Arguments
        End Get
        Set(ByVal value As String)
            m_Arguments = value
        End Set
    End Property

    Public Property StartPath() As String
        Get
            Return m_StartPath
        End Get
        Set(ByVal value As String)
            m_StartPath = value
        End Set
    End Property

    Public Property MaximumRunTimeMinutes() As Int32
        Get
            Return m_MaximumRunTimeMinutes
        End Get
        Set(ByVal value As Int32)
            m_MaximumRunTimeMinutes = value
        End Set
    End Property

    Public Property AllowConcurrent() As Boolean
        Get
            Return m_AllowConcurrent
        End Get
        Set(ByVal value As Boolean)
            m_AllowConcurrent = value
        End Set
    End Property

    Public Property LastTime() As DateTime
        Get
            Return m_LastTime
        End Get
        Set(ByVal value As DateTime)
            m_LastTime = value
        End Set
    End Property

    Public Property LastResult() As String
        Get
            Return m_LastResult
        End Get
        Set(ByVal value As String)
            m_LastResult = value
        End Set
    End Property

    Public Property Frequency() As Scheduling.Functions.ScheduleFrequency
        Get
            Return m_Frequency
        End Get
        Set(ByVal value As Scheduling.Functions.ScheduleFrequency)
            m_Frequency = value
        End Set
    End Property

    Public Property FrequencyValue() As String
        Get
            Return m_FrequencyValue
        End Get
        Set(ByVal value As String)
            m_FrequencyValue = value
        End Set
    End Property

    Public Property Username() As String
        Get
            Return m_Username
        End Get
        Set(ByVal value As String)
            m_Username = value
        End Set
    End Property

    Public Property Password() As String
        Get
            Return m_Password
        End Get
        Set(ByVal value As String)
            m_Password = value
        End Set
    End Property

    Public Property ScheduleSettingsFilename() As String
        Get
            Return m_ScheduleSettingsFilename
        End Get
        Set(ByVal value As String)
            m_ScheduleSettingsFilename = value
        End Set
    End Property

#End Region

#Region " Methods "

    ''' <summary>
    ''' Create a new instance of the Schedule class.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

    End Sub

    ''' <summary>
    ''' Create a new instance of the Schedule class, loading the specified schedule filename.
    ''' </summary>
    ''' <param name="ScheduleSettingsFilename"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal ScheduleSettingsFilename As String)
        m_ScheduleSettingsFilename = ScheduleSettingsFilename
    End Sub

    ''' <summary>
    ''' Loads the schedule.
    ''' </summary>
    ''' <returns></returns>
    Public Function Load() As Boolean
        Dim Result As Boolean = True

        If IO.File.Exists(m_ScheduleSettingsFilename) Then
            m_Path = Config.Functions.LoadConfigSettingAttribute(m_ScheduleSettingsFilename, "Tasks", "GUID-" & m_GUIDString, "Path")
            m_Arguments = Config.Functions.LoadConfigSettingAttribute(m_ScheduleSettingsFilename, "Tasks", "GUID-" & m_GUIDString, "Arguments")
            m_StartPath = Config.Functions.LoadConfigSettingAttribute(m_ScheduleSettingsFilename, "Tasks", "GUID-" & m_GUIDString, "StartPath")
            m_AllowConcurrent = CBool(Config.Functions.LoadConfigSettingAttribute(m_ScheduleSettingsFilename, "Tasks", "GUID-" & m_GUIDString, "AllowConcurrent"))
            m_LastResult = Config.Functions.LoadConfigSettingAttribute(m_ScheduleSettingsFilename, "Tasks", "GUID-" & m_GUIDString, "LastResult")
            m_Username = Config.Functions.LoadConfigSettingAttribute(m_ScheduleSettingsFilename, "Tasks", "GUID-" & m_GUIDString, "Username")
            m_Password = Config.Functions.LoadConfigSettingAttribute(m_ScheduleSettingsFilename, "Tasks", "GUID-" & m_GUIDString, "Password")
            m_LastTime = CDate(Config.Functions.LoadConfigSettingAttribute(m_ScheduleSettingsFilename, "Tasks", "GUID-" & m_GUIDString, "LastTime"))
            m_FrequencyValue = Config.Functions.LoadConfigSettingAttribute(m_ScheduleSettingsFilename, "Tasks", "GUID-" & m_GUIDString, "FrequencyValue")

            Dim MaximumRuntimeMinutes As String = Config.Functions.LoadConfigSettingAttribute(m_ScheduleSettingsFilename, "Tasks", "GUID-" & m_GUIDString, "MaximumRuntimeMinutes")
            If MaximumRuntimeMinutes = "" Then
                m_MaximumRunTimeMinutes = -1
            Else
                m_MaximumRunTimeMinutes = CInt(MaximumRuntimeMinutes)
            End If

            Dim Frequency As String = Config.Functions.LoadConfigSettingAttribute(m_ScheduleSettingsFilename, "Tasks", "GUID-" & m_GUIDString, "Frequency")

            Select Case Frequency
                Case "s", "S"
                    m_Frequency = Functions.ScheduleFrequency.Second
                Case "m"
                    m_Frequency = Functions.ScheduleFrequency.Minute
                Case "h", "H"
                    m_Frequency = Functions.ScheduleFrequency.Hour
                Case "d", "D"
                    m_Frequency = Functions.ScheduleFrequency.Day
                Case "w", "W"
                    m_Frequency = Functions.ScheduleFrequency.Week
                Case "M"
                    m_Frequency = Functions.ScheduleFrequency.Month
                Case "y", "Y"
                    m_Frequency = Functions.ScheduleFrequency.Year
            End Select
            Result = True
        Else
            Result = False
        End If

        Return Result
    End Function

    ''' <summary>
    ''' Save the schedule.
    ''' </summary>
    ''' <param name="ScheduleInstance"></param>
    ''' <returns>True is the save succeeded, False if it failed</returns>
    ''' <remarks></remarks>
    Public Shared Function Save(ByVal ScheduleInstance As Schedule) As Boolean
        Dim SaveResult As Boolean = False

        If IO.File.Exists(ScheduleInstance.ScheduleSettingsFilename) Then
            Config.Functions.SaveConfigSettingAttribute(ScheduleInstance.ScheduleSettingsFilename, "Tasks", "GUID-" & ScheduleInstance.GUIDString, "Path", ScheduleInstance.Path)
            Config.Functions.SaveConfigSettingAttribute(ScheduleInstance.ScheduleSettingsFilename, "Tasks", "GUID-" & ScheduleInstance.GUIDString, "Arguments", ScheduleInstance.Arguments)
            Config.Functions.SaveConfigSettingAttribute(ScheduleInstance.ScheduleSettingsFilename, "Tasks", "GUID-" & ScheduleInstance.GUIDString, "StartPath", ScheduleInstance.StartPath)
            Config.Functions.SaveConfigSettingAttribute(ScheduleInstance.ScheduleSettingsFilename, "Tasks", "GUID-" & ScheduleInstance.GUIDString, "MaximumRuntimeMinutes", ScheduleInstance.MaximumRunTimeMinutes)
            Config.Functions.SaveConfigSettingAttribute(ScheduleInstance.ScheduleSettingsFilename, "Tasks", "GUID-" & ScheduleInstance.GUIDString, "AllowConcurrent", ScheduleInstance.AllowConcurrent.ToString)
            Config.Functions.SaveConfigSettingAttribute(ScheduleInstance.ScheduleSettingsFilename, "Tasks", "GUID-" & ScheduleInstance.GUIDString, "LastResult", ScheduleInstance.LastResult)
            Select Case ScheduleInstance.Frequency
                Case Functions.ScheduleFrequency.Day
                    Config.Functions.SaveConfigSettingAttribute(ScheduleInstance.ScheduleSettingsFilename, "Tasks", "GUID-" & ScheduleInstance.GUIDString, "Frequency", "d")
                Case Functions.ScheduleFrequency.Hour
                    Config.Functions.SaveConfigSettingAttribute(ScheduleInstance.ScheduleSettingsFilename, "Tasks", "GUID-" & ScheduleInstance.GUIDString, "Frequency", "h")
                Case Functions.ScheduleFrequency.Minute
                    Config.Functions.SaveConfigSettingAttribute(ScheduleInstance.ScheduleSettingsFilename, "Tasks", "GUID-" & ScheduleInstance.GUIDString, "Frequency", "m")
                Case Functions.ScheduleFrequency.Month
                    Config.Functions.SaveConfigSettingAttribute(ScheduleInstance.ScheduleSettingsFilename, "Tasks", "GUID-" & ScheduleInstance.GUIDString, "Frequency", "M")
                Case Functions.ScheduleFrequency.Second
                    Config.Functions.SaveConfigSettingAttribute(ScheduleInstance.ScheduleSettingsFilename, "Tasks", "GUID-" & ScheduleInstance.GUIDString, "Frequency", "s")
                Case Functions.ScheduleFrequency.Week
                    Config.Functions.SaveConfigSettingAttribute(ScheduleInstance.ScheduleSettingsFilename, "Tasks", "GUID-" & ScheduleInstance.GUIDString, "Frequency", "w")
                Case Functions.ScheduleFrequency.Year
                    Config.Functions.SaveConfigSettingAttribute(ScheduleInstance.ScheduleSettingsFilename, "Tasks", "GUID-" & ScheduleInstance.GUIDString, "Frequency", "y")
            End Select

            Config.Functions.SaveConfigSettingAttribute(ScheduleInstance.ScheduleSettingsFilename, "Tasks", "GUID-" & ScheduleInstance.GUIDString, "FrequencyValue", ScheduleInstance.FrequencyValue)
            Config.Functions.SaveConfigSettingAttribute(ScheduleInstance.ScheduleSettingsFilename, "Tasks", "GUID-" & ScheduleInstance.GUIDString, "Username", ScheduleInstance.Username)
            Config.Functions.SaveConfigSettingAttribute(ScheduleInstance.ScheduleSettingsFilename, "Tasks", "GUID-" & ScheduleInstance.GUIDString, "Password", ScheduleInstance.Password)

            SaveResult = Config.Functions.SaveConfigSettingAttribute(ScheduleInstance.ScheduleSettingsFilename, "Tasks", "GUID-" & ScheduleInstance.GUIDString, "LastTime", ScheduleInstance.LastTime.ToString)
        End If

        Return SaveResult

    End Function

#End Region

End Class
