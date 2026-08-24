Public Class Functions

    Public Enum ScheduleFrequency
        'Once = 0
        'DayOfWeek = 1
        'DayOfMonth = 2
        Second = 3
        Minute = 4
        Hour = 5
        Day = 6
        Week = 7
        Month = 8
        Year = 9
    End Enum

    ''' <summary>
    ''' Check if the application is scheduled to run
    ''' </summary>
    ''' <returns>True if the application schedule is outstanding</returns>
    ''' <remarks></remarks>
    Public Shared Function IsScheduled(ByVal ScheduleInstance As Schedule) As Boolean

        ' TODO:  Implement stringent checking
        Dim NextRuntime As DateTime = Date.MaxValue

        Select Case ScheduleInstance.Frequency
            Case ScheduleFrequency.Day
                NextRuntime = DateAdd(DateInterval.Day, CDbl(ScheduleInstance.FrequencyValue), ScheduleInstance.LastTime)
                'Case ScheduleFrequency.DayOfMonth
                'Case ScheduleFrequency.DayOfWeek
            Case ScheduleFrequency.Hour
                NextRuntime = DateAdd(DateInterval.Hour, CDbl(ScheduleInstance.FrequencyValue), ScheduleInstance.LastTime)
            Case ScheduleFrequency.Minute
                NextRuntime = DateAdd(DateInterval.Minute, CDbl(ScheduleInstance.FrequencyValue), ScheduleInstance.LastTime)
            Case ScheduleFrequency.Month
                NextRuntime = DateAdd(DateInterval.Month, CDbl(ScheduleInstance.FrequencyValue), ScheduleInstance.LastTime)
                'Case ScheduleFrequency.Once
            Case ScheduleFrequency.Second
                NextRuntime = DateAdd(DateInterval.Second, CDbl(ScheduleInstance.FrequencyValue), ScheduleInstance.LastTime)
            Case ScheduleFrequency.Week
                NextRuntime = DateAdd(DateInterval.Day, CDbl(ScheduleInstance.FrequencyValue) * 7, ScheduleInstance.LastTime)
            Case ScheduleFrequency.Year
                NextRuntime = DateAdd(DateInterval.Year, CDbl(ScheduleInstance.FrequencyValue), ScheduleInstance.LastTime)
        End Select

        If DateTime.Now >= NextRuntime Then
            Return True
        Else
            Return False
        End If

    End Function

End Class
