Imports System
Imports System.Runtime.InteropServices
Imports TaskScheduler.TaskSchedulerInterop

Namespace TaskScheduler
#Region " Enums "
    ''' <summary>
    ''' Valid types of triggers
    ''' </summary>
    Friend Enum TriggerType
        ''' <summary>
        ''' Trigger is set to run the task a single time. 
        ''' </summary>
        RunOnce = 0
        ''' <summary>
        ''' Trigger is set to run the task on a daily interval. 
        ''' </summary>
        RunDaily = 1
        ''' <summary>
        ''' Trigger is set to run the work item on specific days of a specific week of a specific month. 
        ''' </summary>
        RunWeekly = 2
        ''' <summary>
        ''' Trigger is set to run the task on a specific day(s) of the month.
        ''' </summary>
        RunMonthly = 3
        ''' <summary>
        ''' Trigger is set to run the task on specific days, weeks, and months.
        ''' </summary>
        RunMonthlyDOW = 4
        ''' <summary>
        ''' Trigger is set to run the task if the system remains idle for the amount of time specified by the idle wait time of the task.
        ''' </summary>
        OnIdle = 5
        ''' <summary>
        ''' Trigger is set to run the task at system startup.
        ''' </summary>
        OnSystemStart = 6
        ''' <summary>
        ''' Trigger is set to run the task when a user logs on. 
        ''' </summary>
        OnLogon = 7
    End Enum

    ''' <summary>
    ''' Values for days of the week (Monday, Tuesday, etc.)  These carry the Flags
    ''' attribute so DaysOfTheWeek and be combined with | (or).
    ''' </summary>
    <Flags()> _
    Public Enum DaysOfTheWeek As Short
        ''' <summary>
        ''' Sunday
        ''' </summary>
        Sunday = &H1
        ''' <summary>
        ''' Monday
        ''' </summary>
        Monday = &H2
        ''' <summary>
        ''' Tuesday
        ''' </summary>
        Tuesday = &H4
        ''' <summary>
        ''' Wednesday
        ''' </summary>
        Wednesday = &H8
        ''' <summary>
        ''' Thursday
        ''' </summary>
        Thursday = &H10
        ''' <summary>
        ''' Friday
        ''' </summary>
        Friday = &H20
        ''' <summary>
        ''' Saturday
        ''' </summary>
        Saturday = &H40
    End Enum

    ''' <summary>
    ''' Values for week of month (first, second, ..., last)
    ''' </summary>
    Public Enum WhichWeek As Short
        ''' <summary>
        ''' First week of the month
        ''' </summary>
        FirstWeek = 1
        ''' <summary>
        ''' Second week of the month
        ''' </summary>
        SecondWeek = 2
        ''' <summary>
        ''' Third week of the month
        ''' </summary>
        ThirdWeek = 3
        ''' <summary>
        ''' Fourth week of the month
        ''' </summary>
        FourthWeek = 4
        ''' <summary>
        ''' Last week of the month
        ''' </summary>
        LastWeek = 5
    End Enum

    ''' <summary>
    ''' Values for months of the year (January, February, etc.)  These carry the Flags
    ''' attribute so DaysOfTheWeek and be combined with | (or).
    ''' </summary>
    <Flags()> _
    Public Enum MonthsOfTheYear As Short
        ''' <summary>
        ''' January
        ''' </summary>
        January = &H1
        ''' <summary>
        ''' February
        ''' </summary>
        February = &H2
        ''' <summary>
        ''' March
        ''' </summary>
        March = &H4
        ''' <summary>
        ''' April
        ''' </summary>
        April = &H8
        ''' <summary>
        '''May 
        ''' </summary>
        May = &H10
        ''' <summary>
        ''' June
        ''' </summary>
        June = &H20
        ''' <summary>
        ''' July
        ''' </summary>
        July = &H40
        ''' <summary>
        ''' August
        ''' </summary>
        August = &H80
        ''' <summary>
        ''' September
        ''' </summary>
        September = &H100
        ''' <summary>
        ''' October
        ''' </summary>
        October = &H200
        ''' <summary>
        ''' November
        ''' </summary>
        November = &H400
        ''' <summary>
        ''' December
        ''' </summary>
        December = &H800
    End Enum
#End Region

    ''' <summary>
    ''' Trigger is a generalization of all the concrete trigger classes, and any actual
    ''' Trigger object is one of those types.  When included in the TriggerList of a
    ''' Task, a Trigger determines when a scheduled task will be run.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Create a concrete trigger for a specific start condition and then call TriggerList.Add
    ''' to include it in a task's TriggerList.</para>
    ''' <para>
    ''' A Trigger that is not yet in a Task's TriggerList is said to be unbound and it holds
    ''' no resources (i.e. COM interfaces).  Once it is added to a TriggerList, it is bound and
    ''' holds a COM interface that is only released when the Trigger is removed from the list or
    ''' the corresponding Task is closed.</para>  
    ''' <para>
    ''' A Trigger that is already bound cannot be added to a TriggerList.  To copy a Trigger from
    ''' one list to another, use <see cref="IList"/> to create an unbound copy and then add the
    ''' copy to the new list.  To move a Trigger from one list to another, use <see cref="TriggerList.Remove"/>
    ''' to extract the Trigger from the first list before adding it to the second.</para>
    ''' </remarks>
    Public MustInherit Class Trigger
        Implements ICloneable
#Region " Enums "
        ''' <summary>
        ''' Flags for triggers
        ''' </summary>
        <Flags()> _
        Private Enum TaskTriggerFlags
            HasEndDate = &H1
            KillAtDurationEnd = &H2
            Disabled = &H4
        End Enum
#End Region

#Region " Fields "
        Private iTaskTrigger As TaskSchedulerInterop.ITaskTrigger
        'null for an unbound Trigger
        Friend taskTrigger As TaskSchedulerInterop.TaskTrigger
#End Region

#Region " Constructors and Initializers "
        ''' <summary>
        ''' Internal base constructor for an unbound Trigger.
        ''' </summary>
        Friend Sub New()
            iTaskTrigger = Nothing
            taskTrigger = New TaskSchedulerInterop.TaskTrigger()
            taskTrigger.TriggerSize = DirectCast(Marshal.SizeOf(taskTrigger), UShort)
            taskTrigger.BeginYear = DirectCast(DateTime.Today.Year, UShort)
            taskTrigger.BeginMonth = DirectCast(DateTime.Today.Month, UShort)
            taskTrigger.BeginDay = DirectCast(DateTime.Today.Day, UShort)
        End Sub

        ''' <summary>
        ''' Internal constructor which initializes itself from
        ''' from an ITaskTrigger interface.
        ''' </summary>
        ''' <param name="iTrigger">Instance of ITaskTrigger from system task scheduler.</param>
        Friend Sub New(ByVal iTrigger As TaskSchedulerInterop.ITaskTrigger)
            If iTrigger Is Nothing Then
                Throw New ArgumentNullException("iTrigger", "ITaskTrigger instance cannot be null")
            End If
            taskTrigger = New TaskSchedulerInterop.TaskTrigger()
            taskTrigger.TriggerSize = DirectCast(Marshal.SizeOf(taskTrigger), UShort)
            iTrigger.GetTrigger(taskTrigger)
            iTaskTrigger = iTrigger
        End Sub
#End Region

#Region " Implement ICloneable "
        ''' <summary>
        ''' Clone returns an unbound copy of the Trigger object.  It can be use
        ''' on either bound or unbound original.
        ''' </summary>
        ''' <returns></returns>
        Public Function Clone() As Object Implements ICloneable.Clone
            Dim newTrigger As Trigger = DirectCast(Me.MemberwiseClone(), Trigger)
            newTrigger.iTaskTrigger = Nothing
            ' The clone is not bound
            Return newTrigger
        End Function
#End Region

#Region " Properties "

        ''' <summary>
        ''' Get whether the Trigger is currently bound
        ''' </summary>
        Friend ReadOnly Property Bound() As Boolean
            Get
                Return iTaskTrigger Is Nothing
            End Get
        End Property

        ''' <summary>
        ''' Gets/sets the beginning year, month, and day for the trigger.
        ''' </summary>
        Public Property BeginDate() As DateTime
            Get
                Return New DateTime(taskTrigger.BeginYear, taskTrigger.BeginMonth, taskTrigger.BeginDay)
            End Get
            Set(ByVal value As DateTime)
                taskTrigger.BeginYear = DirectCast(value.Year, UShort)
                taskTrigger.BeginMonth = DirectCast(value.Month, UShort)
                taskTrigger.BeginDay = DirectCast(value.Day, UShort)
                SyncTrigger()
            End Set
        End Property

        ''' <summary>
        ''' Gets/sets indication that the task uses an EndDate.  Returns true if a value has been
        ''' set for the EndDate property.  Set can only be used to turn indication off.  
        ''' </summary>
        ''' <exception cref="ArgumentException">Has EndDate becomes true only by setting the EndDate
        ''' property.</exception>
        Public Property HasEndDate() As Boolean
            Get
                Return ((taskTrigger.Flags And DirectCast(TaskTriggerFlags.HasEndDate, UInteger)) = DirectCast(TaskTriggerFlags.HasEndDate, UInteger))
            End Get
            Set(ByVal value As Boolean)
                If value Then
                    Throw New ArgumentException("HasEndDate can only be set false")
                End If
                taskTrigger.Flags = taskTrigger.Flags And Not DirectCast(TaskTriggerFlags.HasEndDate, UInteger)
                SyncTrigger()
            End Set
        End Property

        ''' <summary>
        ''' Gets/sets the ending year, month, and day for the trigger.  After a value has been set
        ''' with EndDate, HasEndDate becomes true.
        ''' </summary>
        Public Property EndDate() As DateTime
            Get
                If taskTrigger.EndYear = 0 Then
                    Return DateTime.MinValue
                End If
                Return New DateTime(taskTrigger.EndYear, taskTrigger.EndMonth, taskTrigger.EndDay)
            End Get
            Set(ByVal value As DateTime)
                taskTrigger.Flags = taskTrigger.Flags Or DirectCast(TaskTriggerFlags.HasEndDate, UInteger)
                taskTrigger.EndYear = DirectCast(value.Year, UShort)
                taskTrigger.EndMonth = DirectCast(value.Month, UShort)
                taskTrigger.EndDay = DirectCast(value.Day, UShort)
                SyncTrigger()
            End Set
        End Property

        ''' <summary>
        ''' Gets/sets the number of minutes after the trigger fires that it remains active.  Used
        ''' in conjunction with <see cref="IntervalMinutes"/> to run a task repeatedly for a period of time.
        ''' For example, if you want to start a task at 8:00 A.M. repeatedly restart it until 5:00 P.M.,
        ''' there would be 540 minutes (9 hours) in the duration.
        ''' Can also be used to terminate a task that is running when the DurationMinutes expire.  Use
        ''' <see cref="KillAtDurationEnd"/> to specify that task should be terminated at that time.
        ''' </summary>
        ''' <exception cref="ArgumentOutOfRangeException">Setting must be greater than or equal
        ''' to the IntervalMinutes setting.</exception>
        Public Property DurationMinutes() As Integer
            Get
                Return DirectCast(taskTrigger.MinutesDuration, Integer)
            End Get
            Set(ByVal value As Integer)
                If value < taskTrigger.MinutesInterval Then
                    Throw New ArgumentOutOfRangeException("DurationMinutes", value, "DurationMinutes must be greater than or equal the IntervalMinutes value")
                End If
                taskTrigger.MinutesDuration = DirectCast(value, UInteger)
                SyncTrigger()
            End Set
        End Property

        ''' <summary>
        ''' Gets/sets the number of minutes between executions for a task that is to be run repeatedly.
        ''' Repetition continues until the interval specified in <see cref="DurationMinutes"/> expires.
        ''' IntervalMinutes are counted from the start of the previous execution.
        ''' </summary>
        ''' <exception cref="ArgumentOutOfRangeException">Setting must be less than
        ''' to the DurationMinutes setting.</exception>
        Public Property IntervalMinutes() As Integer
            Get
                Return DirectCast(taskTrigger.MinutesInterval, Integer)
            End Get
            Set(ByVal value As Integer)
                If value > taskTrigger.MinutesDuration Then
                    Throw New ArgumentOutOfRangeException("IntervalMinutes", value, "IntervalMinutes must be less than or equal the DurationMinutes value")
                End If
                taskTrigger.MinutesInterval = DirectCast(value, UInteger)
                SyncTrigger()
            End Set
        End Property

        ''' <summary>
        ''' Gets/sets whether task will be killed (terminated) when DurationMinutes expires. 
        ''' See <see cref="Trigger.DurationMinutes"/>.
        ''' </summary>
        Public Property KillAtDurationEnd() As Boolean
            Get
                Return ((taskTrigger.Flags And DirectCast(TaskTriggerFlags.KillAtDurationEnd, UInteger)) = DirectCast(TaskTriggerFlags.KillAtDurationEnd, UInteger))
            End Get
            Set(ByVal value As Boolean)
                If value Then
                    taskTrigger.Flags = taskTrigger.Flags Or DirectCast(TaskTriggerFlags.KillAtDurationEnd, UInteger)
                Else
                    taskTrigger.Flags = taskTrigger.Flags And Not DirectCast(TaskTriggerFlags.KillAtDurationEnd, UInteger)
                End If
                SyncTrigger()
            End Set
        End Property

        ''' <summary>
        ''' Gets/sets whether trigger is disabled.
        ''' </summary>
        Public Property Disabled() As Boolean
            Get
                Return ((taskTrigger.Flags And DirectCast(TaskTriggerFlags.Disabled, UInteger)) = DirectCast(TaskTriggerFlags.Disabled, UInteger))
            End Get
            Set(ByVal value As Boolean)
                If value Then
                    taskTrigger.Flags = taskTrigger.Flags Or DirectCast(TaskTriggerFlags.Disabled, UInteger)
                Else
                    taskTrigger.Flags = taskTrigger.Flags And Not DirectCast(TaskTriggerFlags.Disabled, UInteger)
                End If
                SyncTrigger()
            End Set
        End Property
#End Region

#Region " Methods "
        ''' <summary>
        ''' Creates a new, bound Trigger object from an ITaskTrigger interface.  The type of the
        ''' concrete object created is determined by the type of ITaskTrigger.
        ''' </summary>
        ''' <param name="iTaskTrigger">Instance of ITaskTrigger.</param>
        ''' <returns>One of the concrete classes derived from Trigger.</returns>
        ''' <exception cref="ArgumentNullException"></exception>
        ''' <exception cref="ArgumentException">Unable to recognize trigger type.</exception>
        Friend Shared Function CreateTrigger(ByVal iTaskTrigger As TaskSchedulerInterop.ITaskTrigger) As Trigger
            If iTaskTrigger Is Nothing Then
                Throw New ArgumentNullException("iTaskTrigger", "Instance of ITaskTrigger cannot be null")
            End If
            Dim sTaskTrigger As New TaskSchedulerInterop.TaskTrigger()
            sTaskTrigger.TriggerSize = DirectCast(Marshal.SizeOf(sTaskTrigger), UShort)
            iTaskTrigger.GetTrigger(sTaskTrigger)
            Select Case DirectCast(sTaskTrigger.Type, TriggerType)
                Case TriggerType.RunOnce
                    Return New RunOnceTrigger(iTaskTrigger)
                Case TriggerType.RunDaily
                    Return New DailyTrigger(iTaskTrigger)
                Case TriggerType.RunWeekly
                    Return New WeeklyTrigger(iTaskTrigger)
                Case TriggerType.RunMonthlyDOW
                    Return New MonthlyDOWTrigger(iTaskTrigger)
                Case TriggerType.RunMonthly
                    Return New MonthlyTrigger(iTaskTrigger)
                Case TriggerType.OnIdle
                    Return New OnIdleTrigger(iTaskTrigger)
                Case TriggerType.OnSystemStart
                    Return New OnSystemStartTrigger(iTaskTrigger)
                Case TriggerType.OnLogon
                    Return New OnLogonTrigger(iTaskTrigger)
                Case Else
                    Throw New ArgumentException("Unable to recognize type of trigger referenced in iTaskTrigger", "iTaskTrigger")
            End Select
        End Function

        ''' <summary>
        ''' When a bound Trigger is changed, the corresponding trigger in the system
        ''' Task Scheduler is updated to stay in sync with the local structure.
        ''' </summary>
        Protected Sub SyncTrigger()
            If Not iTaskTrigger Is Nothing Then
                iTaskTrigger.SetTrigger(taskTrigger)
            End If
        End Sub

        ''' <summary>
        ''' Bind a Trigger object to an ITaskTrigger interface.  This causes the Trigger to
        ''' sync itself with the interface and remain in sync whenever it is modified in the future.
        ''' If the Trigger is already bound, an ArgumentException is thrown.
        ''' </summary>
        ''' <param name="iTaskTrigger">An interface representing a trigger in Task Scheduler.</param>
        ''' <exception cref="ArgumentException">Attempt to bind and already bound trigger.</exception>
        Friend Sub Bind(ByVal iTaskTrigger As TaskSchedulerInterop.ITaskTrigger)
            If Not iTaskTrigger Is Nothing Then
                Throw New ArgumentException("Attempt to bind an already bound trigger")
            End If
            Me.iTaskTrigger = iTaskTrigger
            iTaskTrigger.SetTrigger(taskTrigger)
        End Sub
        ''' <summary>
        ''' Bind a Trigger to the same interface the argument trigger is bound to.  
        ''' </summary>
        ''' <param name="trigger">A bound Trigger. </param>
        Friend Sub Bind(ByVal trigger As Trigger)
            Bind(trigger.iTaskTrigger)
        End Sub

        ''' <summary>
        ''' Break the connection between this Trigger and the system Task Scheduler.  This
        ''' releases COM resources used in bound Triggers.
        ''' </summary>
        Friend Sub Unbind()
            If Not iTaskTrigger Is Nothing Then
                Marshal.ReleaseComObject(iTaskTrigger)
                iTaskTrigger = Nothing
            End If
        End Sub

        ''' <summary>
        ''' Gets a string, supplied by the WindowsTask Scheduler, of a bound Trigger. 
        ''' For an unbound trigger, returns "Unbound Trigger".
        ''' </summary>
        ''' <returns>String representation of the trigger.</returns>
        Public Overloads Overrides Function ToString() As String
            If Not iTaskTrigger Is Nothing Then
                Dim lpwstr As IntPtr
                iTaskTrigger.GetTriggerString(lpwstr)
                Return TaskSchedulerInterop.CoTaskMem.LPWStrToString(lpwstr)
            Else
                Return "Unbound " + Me.[GetType]().ToString()
            End If
        End Function

        ''' <summary>
        ''' Determines if two triggers are internally equal.  Does not consider whether
        ''' the Triggers are bound or not.
        ''' </summary>
        ''' <param name="obj">Value of trigger to compare.</param>
        ''' <returns>true if triggers are equivalent.</returns>
        Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
            Return taskTrigger.Equals((DirectCast(obj, Trigger)).taskTrigger)
        End Function

        ''' <summary>
        ''' Gets a hash code for the current trigger.  A Trigger has the same hash
        ''' code whether it is bound or not.
        ''' </summary>
        ''' <returns>Hash code value.</returns>
        Public Overloads Overrides Function GetHashCode() As Integer
            Return taskTrigger.GetHashCode()
        End Function
#End Region

    End Class

    ''' <summary>
    ''' Generalization of all triggers that have a start time.
    ''' </summary>
    ''' <remarks>StartableTrigger serves as a base class for triggers with a
    ''' start time, but it has little use to clients.</remarks>
    Public MustInherit Class StartableTrigger
        Inherits Trigger
        ''' <summary>
        ''' Internal constructor, same as base.
        ''' </summary>
        Friend Sub New()
            MyBase.New()
        End Sub

        ''' <summary>
        ''' Internal constructor from ITaskTrigger interface.
        ''' </summary>
        ''' <param name="iTrigger">ITaskTrigger from system Task Scheduler.</param>
        Friend Sub New(ByVal iTrigger As TaskSchedulerInterop.ITaskTrigger)
            MyBase.New(iTrigger)
        End Sub

        ''' <summary>
        ''' Sets the start time of the trigger.
        ''' </summary>
        ''' <param name="hour">Hour of the day that the trigger will fire.</param>
        ''' <param name="minute">Minute of the hour.</param>
        ''' <exception cref="ArgumentOutOfRangeException">The hour is not between 0 and 23 or the minute is not between 0 and 59.</exception>
        Protected Sub SetStartTime(ByVal hour As UShort, ByVal minute As UShort)
            '			if (hour < 0 || hour > 23)
            '				throw new ArgumentOutOfRangeException("hour", hour, "hour must be between 0 and 23");
            '			if (minute < 0 || minute > 59)
            '				throw new ArgumentOutOfRangeException("minute", minute, "minute must be between 0 and 59");
            '			taskTrigger.StartHour = hour;
            '			taskTrigger.StartMinute = minute;
            '			base.SyncTrigger();
            StartHour = CShort(hour)
            StartMinute = CShort(minute)
        End Sub

        ''' <summary>
        ''' Gets/sets hour of the day that trigger will fire (24 hour clock).
        ''' </summary>
        Public Property StartHour() As Short
            Get
                Return CShort(taskTrigger.StartHour)
            End Get
            Set(ByVal value As Short)
                If value < 0 OrElse value > 23 Then
                    Throw New ArgumentOutOfRangeException("hour", value, "hour must be between 0 and 23")
                End If
                taskTrigger.StartHour = DirectCast(value, UShort)
                MyBase.SyncTrigger()
            End Set
        End Property

        ''' <summary>
        ''' Gets/sets minute of the hour (specified in <see cref="StartHour"/>) that trigger will fire.
        ''' </summary>
        Public Property StartMinute() As Short
            Get
                Return CShort(taskTrigger.StartMinute)
            End Get
            Set(ByVal value As Short)
                If value < 0 OrElse value > 59 Then
                    Throw New ArgumentOutOfRangeException("minute", value, "minute must be between 0 and 59")
                End If
                taskTrigger.StartMinute = DirectCast(value, UShort)
                MyBase.SyncTrigger()
            End Set
        End Property
    End Class

    ''' <summary>
    ''' Trigger that fires once only.
    ''' </summary>
    Public Class RunOnceTrigger
        Inherits StartableTrigger
        ''' <summary>
        ''' Create a RunOnceTrigger that fires when specified.
        ''' </summary>
        ''' <param name="runDateTime">Date and time to fire.</param>
        Public Sub New(ByVal runDateTime As DateTime)
            MyBase.New()
            taskTrigger.BeginYear = DirectCast(runDateTime.Year, UShort)
            taskTrigger.BeginMonth = DirectCast(runDateTime.Month, UShort)
            taskTrigger.BeginDay = DirectCast(runDateTime.Day, UShort)
            SetStartTime(DirectCast(runDateTime.Hour, UShort), DirectCast(runDateTime.Minute, UShort))
            taskTrigger.Type = TaskSchedulerInterop.TaskTriggerType.TIME_TRIGGER_ONCE
        End Sub

        ''' <summary>
        ''' Internal constructor to create from existing ITaskTrigger interface.
        ''' </summary>
        ''' <param name="iTrigger">ITaskTrigger from system Task Scheduler.</param>
        Friend Sub New(ByVal iTrigger As TaskSchedulerInterop.ITaskTrigger)
            MyBase.New(iTrigger)
        End Sub
    End Class

    ''' <summary>
    ''' Trigger that fires at a specified time, every so many days.
    ''' </summary>
    Public Class DailyTrigger
        Inherits StartableTrigger
        ''' <summary>
        ''' Creates a DailyTrigger that fires only at an interval of so many days.
        ''' </summary>
        ''' <param name="hour">Hour of day trigger will fire.</param>
        ''' <param name="minutes">Minutes of the hour trigger will fire.</param>
        ''' <param name="daysInterval">Number of days between task runs.</param>
        Public Sub New(ByVal hour As Short, ByVal minutes As Short, ByVal daysInterval As Short)
            MyBase.New()
            SetStartTime(DirectCast(hour, UShort), DirectCast(minutes, UShort))
            taskTrigger.Type = TaskSchedulerInterop.TaskTriggerType.TIME_TRIGGER_DAILY
            taskTrigger.Data.daily.DaysInterval = DirectCast(daysInterval, UShort)
        End Sub

        ''' <summary>
        ''' Creates DailyTrigger that fires every day.
        ''' </summary>
        ''' <param name="hour">Hour of day trigger will fire.</param>
        ''' <param name="minutes">Minutes of hour (specified in "hour") trigger will fire.</param>
        Public Sub New(ByVal hour As Short, ByVal minutes As Short)
            Me.New(hour, minutes, 1)
        End Sub

        ''' <summary>
        ''' Internal constructor to create from existing ITaskTrigger interface.
        ''' </summary>
        ''' <param name="iTrigger">ITaskTrigger from system Task Scheduler.</param>
        Friend Sub New(ByVal iTrigger As TaskSchedulerInterop.ITaskTrigger)
            MyBase.New(iTrigger)
        End Sub

        ''' <summary>
        ''' Gets/sets the number of days between successive firings.
        ''' </summary>
        Public Property DaysInterval() As Short
            Get
                Return DirectCast(taskTrigger.Data.daily.DaysInterval, Short)
            End Get
            Set(ByVal value As Short)
                taskTrigger.Data.daily.DaysInterval = DirectCast(value, UShort)
                MyBase.SyncTrigger()
            End Set
        End Property
    End Class

    ''' <summary>
    ''' Trigger that fires at a specified time, on specified days of the week,
    ''' every so many weeks.
    ''' </summary>
    Public Class WeeklyTrigger
        Inherits StartableTrigger
        ''' <summary>
        ''' Creates a WeeklyTrigger that is eligible to fire only during certain weeks.
        ''' </summary>
        ''' <param name="hour">Hour of day trigger will fire.</param>
        ''' <param name="minutes">Minutes of hour (specified in "hour") trigger will fire.</param>
        ''' <param name="daysOfTheWeek">Days of the week task will run.</param>
        ''' <param name="weeksInterval">Number of weeks between task runs.</param>
        Public Sub New(ByVal hour As Short, ByVal minutes As Short, ByVal daysOfTheWeek As DaysOfTheWeek, ByVal weeksInterval As Short)
            MyBase.New()
            SetStartTime(DirectCast(hour, UShort), DirectCast(minutes, UShort))
            taskTrigger.Type = TaskSchedulerInterop.TaskTriggerType.TIME_TRIGGER_WEEKLY
            taskTrigger.Data.weekly.WeeksInterval = DirectCast(weeksInterval, UShort)
            taskTrigger.Data.weekly.DaysOfTheWeek = DirectCast(daysOfTheWeek, UShort)
        End Sub

        ''' <summary>
        ''' Creates a WeeklyTrigger that is eligible to fire during any week.
        ''' </summary>
        ''' <param name="hour">Hour of day trigger will fire.</param>
        ''' <param name="minutes">Minutes of hour (specified in "hour") trigger will fire.</param>
        ''' <param name="daysOfTheWeek">Days of the week task will run.</param>
        Public Sub New(ByVal hour As Short, ByVal minutes As Short, ByVal daysOfTheWeek As DaysOfTheWeek)
            Me.New(hour, minutes, daysOfTheWeek, 1)
        End Sub

        ''' <summary>
        ''' Internal constructor to create from existing ITaskTrigger interface.
        ''' </summary>
        ''' <param name="iTrigger">ITaskTrigger interface from system Task Scheduler.</param>
        Friend Sub New(ByVal iTrigger As TaskSchedulerInterop.ITaskTrigger)
            MyBase.New(iTrigger)
        End Sub

        ''' <summary>
        ''' Gets/sets number of weeks from one eligible week to the next.
        ''' </summary>
        Public Property WeeksInterval() As Short
            Get
                Return CShort(taskTrigger.Data.weekly.WeeksInterval)
            End Get
            Set(ByVal value As Short)
                taskTrigger.Data.weekly.WeeksInterval = DirectCast(value, UShort)
                MyBase.SyncTrigger()
            End Set
        End Property

        ''' <summary>
        ''' Gets/sets the days of the week on which the trigger fires.
        ''' </summary>
        Public Property WeekDays() As DaysOfTheWeek
            Get
                Return DirectCast(taskTrigger.Data.weekly.DaysOfTheWeek, DaysOfTheWeek)
            End Get
            Set(ByVal value As DaysOfTheWeek)
                taskTrigger.Data.weekly.DaysOfTheWeek = DirectCast(value, UShort)
                MyBase.SyncTrigger()
            End Set
        End Property
    End Class

    ''' <summary>
    ''' Trigger that fires at a specified time, on specified days of the week, 
    ''' in specified weeks of the month, during specified months of the year.
    ''' </summary>
    Public Class MonthlyDOWTrigger
        Inherits StartableTrigger
        ''' <summary>
        ''' Creates a MonthlyDOWTrigger that fires during specified months only.
        ''' </summary>
        ''' <param name="hour">Hour of day trigger will fire.</param>
        ''' <param name="minutes">Minute of the hour trigger will fire.</param>
        ''' <param name="daysOfTheWeek">Days of the week trigger will fire.</param>
        ''' <param name="whichWeeks">Weeks of the month trigger will fire.</param>
        ''' <param name="months">Months of the year trigger will fire.</param>
        Public Sub New(ByVal hour As Short, ByVal minutes As Short, ByVal daysOfTheWeek As DaysOfTheWeek, ByVal whichWeeks As WhichWeek, ByVal months As MonthsOfTheYear)
            MyBase.New()
            SetStartTime(DirectCast(hour, UShort), DirectCast(minutes, UShort))
            taskTrigger.Type = TaskSchedulerInterop.TaskTriggerType.TIME_TRIGGER_MONTHLYDOW
            taskTrigger.Data.monthlyDOW.WhichWeek = DirectCast(whichWeeks, UShort)
            taskTrigger.Data.monthlyDOW.DaysOfTheWeek = DirectCast(daysOfTheWeek, UShort)
            taskTrigger.Data.monthlyDOW.Months = DirectCast(months, UShort)
        End Sub

        ''' <summary>
        ''' Creates a MonthlyDOWTrigger that fires every month.
        ''' </summary>
        ''' <param name="hour">Hour of day trigger will fire.</param>
        ''' <param name="minutes">Minute of the hour trigger will fire.</param>
        ''' <param name="daysOfTheWeek">Days of the week trigger will fire.</param>
        ''' <param name="whichWeeks">Weeks of the month trigger will fire.</param>
        Public Sub New(ByVal hour As Short, ByVal minutes As Short, ByVal daysOfTheWeek As DaysOfTheWeek, ByVal whichWeeks As WhichWeek)
            Me.New(hour, minutes, daysOfTheWeek, whichWeeks, MonthsOfTheYear.January Or MonthsOfTheYear.February Or MonthsOfTheYear.March Or MonthsOfTheYear.April Or MonthsOfTheYear.May Or MonthsOfTheYear.June Or MonthsOfTheYear.July Or MonthsOfTheYear.August Or MonthsOfTheYear.September Or MonthsOfTheYear.October Or MonthsOfTheYear.November Or MonthsOfTheYear.December)
        End Sub

        ''' <summary>
        ''' Internal constructor to create from existing ITaskTrigger interface.
        ''' </summary>
        ''' <param name="iTrigger">ITaskTrigger from the system Task Scheduler.</param>
        Friend Sub New(ByVal iTrigger As TaskSchedulerInterop.ITaskTrigger)
            MyBase.New(iTrigger)
        End Sub

        ''' <summary>
        ''' Gets/sets weeks of the month in which trigger will fire.
        ''' </summary>
        Public Property WhichWeeks() As Short
            Get
                Return CShort(taskTrigger.Data.monthlyDOW.WhichWeek)
            End Get
            Set(ByVal value As Short)
                taskTrigger.Data.monthlyDOW.WhichWeek = DirectCast(value, UShort)
                MyBase.SyncTrigger()
            End Set
        End Property

        ''' <summary>
        ''' Gets/sets days of the week on which trigger will fire.
        ''' </summary>
        Public Property WeekDays() As DaysOfTheWeek
            Get
                Return DirectCast(taskTrigger.Data.monthlyDOW.DaysOfTheWeek, DaysOfTheWeek)
            End Get
            Set(ByVal value As DaysOfTheWeek)
                taskTrigger.Data.monthlyDOW.DaysOfTheWeek = DirectCast(value, UShort)
                MyBase.SyncTrigger()
            End Set
        End Property

        ''' <summary>
        ''' Gets/sets months of the year in which trigger will fire.
        ''' </summary>
        Public Property Months() As MonthsOfTheYear
            Get
                Return DirectCast(taskTrigger.Data.monthlyDOW.Months, MonthsOfTheYear)
            End Get
            Set(ByVal value As MonthsOfTheYear)
                taskTrigger.Data.monthlyDOW.Months = DirectCast(value, UShort)
                MyBase.SyncTrigger()
            End Set
        End Property
    End Class

    ''' <summary>
    ''' Trigger that fires at a specified time, on specified days of themonth,
    ''' on specified months of the year.
    ''' </summary>
    Public Class MonthlyTrigger
        Inherits StartableTrigger
        ''' <summary>
        ''' Creates a MonthlyTrigger that fires only during specified months of the year.
        ''' </summary>
        ''' <param name="hour">Hour of day trigger will fire.</param>
        ''' <param name="minutes">Minutes of hour (specified in "hour") trigger will fire.</param>
        ''' <param name="daysOfMonth">Days of the month trigger will fire.  (See <see cref="Days"/> property.</param>
        ''' <param name="months">Months of the year trigger will fire.</param>
        Public Sub New(ByVal hour As Short, ByVal minutes As Short, ByVal daysOfMonth As Integer(), ByVal months As MonthsOfTheYear)
            MyBase.New()
            SetStartTime(DirectCast(hour, UShort), DirectCast(minutes, UShort))
            taskTrigger.Type = TaskSchedulerInterop.TaskTriggerType.TIME_TRIGGER_MONTHLYDATE
            taskTrigger.Data.monthlyDate.Months = DirectCast(months, UShort)
            taskTrigger.Data.monthlyDate.Days = DirectCast(IndicesToMask(daysOfMonth), UInteger)
        End Sub

        ''' <summary>
        ''' Creates a MonthlyTrigger that fires during any month.
        ''' </summary>
        ''' <param name="hour">Hour of day trigger will fire.</param>
        ''' <param name="minutes">Minutes of hour (specified in "hour") trigger will fire.</param>
        ''' <param name="daysOfMonth">Days of the month trigger will fire.  (See <see cref="Days"/> property.</param>
        Public Sub New(ByVal hour As Short, ByVal minutes As Short, ByVal daysOfMonth As Integer())
            Me.New(hour, minutes, daysOfMonth, MonthsOfTheYear.January Or MonthsOfTheYear.February Or MonthsOfTheYear.March Or MonthsOfTheYear.April Or MonthsOfTheYear.May Or MonthsOfTheYear.June Or MonthsOfTheYear.July Or MonthsOfTheYear.August Or MonthsOfTheYear.September Or MonthsOfTheYear.October Or MonthsOfTheYear.November Or MonthsOfTheYear.December)
        End Sub

        ''' <summary>
        ''' Internal constructor to create from existing ITaskTrigger interface.
        ''' </summary>
        ''' <param name="iTrigger">ITaskTrigger from system Task Scheduler.</param>
        Friend Sub New(ByVal iTrigger As TaskSchedulerInterop.ITaskTrigger)
            MyBase.New(iTrigger)
        End Sub

        ''' <summary>
        ''' Gets/sets months of the year trigger will fire.
        ''' </summary>
        Public Property Months() As MonthsOfTheYear
            Get
                Return DirectCast(taskTrigger.Data.monthlyDate.Months, MonthsOfTheYear)
            End Get
            Set(ByVal value As MonthsOfTheYear)
                taskTrigger.Data.monthlyDOW.Months = DirectCast(value, UShort)
                MyBase.SyncTrigger()
            End Set
        End Property

        ''' <summary>
        ''' Convert an integer representing a mask to an array where each element contains the index
        ''' of a bit that is ON in the mask.  Bits are considered to number from 1 to 32.
        ''' </summary>
        ''' <param name="mask">An interger to be interpreted as a mask.</param>
        ''' <returns>An array with an element for each bit of the mask which is ON.</returns>
        Private Shared Function MaskToIndices(ByVal mask As Integer) As Integer()
            'count bits in mask
            Dim cnt As Integer = 0
            Dim i As Integer = 0
            While (mask >> i) > 0
                cnt = cnt + (1 And (mask >> i))
                System.Math.Max(System.Threading.Interlocked.Increment(i), i - 1)
            End While
            'allocate return array with one entry for each bit
            Dim indices As Integer() = New Integer(cnt) {}
            'fill array with bit indices
            cnt = 0
            Dim i As Integer = 0
            While (mask >> i) > 0
                If (1 And (mask >> i)) = 1 Then
                    indices(System.Math.Max(System.Threading.Interlocked.Increment(cnt), cnt - 1)) = i + 1
                End If
                System.Math.Max(System.Threading.Interlocked.Increment(i), i - 1)
            End While
            Return indices
        End Function
        ''' <summary>
        ''' Converts an array of bit indices into a mask with bits  turned ON at every index
        ''' contained in the array.  Indices must be from 1 to 32 and bits are numbered the same.
        ''' </summary>
        ''' <param name="indices">An array with an element for each bit of the mask which is ON.</param>
        ''' <returns>An interger to be interpreted as a mask.</returns>
        Private Shared Function IndicesToMask(ByVal indices As Integer()) As Integer
            Dim mask As Integer = 0
            For Each index As Integer In indices
                If index < 1 OrElse index > 31 Then
                    Throw New ArgumentException("Days must be in the range 1..31")
                End If
                mask = mask Or 1 << (index - 1)
            Next
            Return mask
        End Function

        ''' <summary>
        ''' Gets/sets days of the month trigger will fire.
        ''' </summary>
        ''' <value>An array with one element for each day that the trigger will fire.
        ''' The value of the element is the number of the day, in the range 1..31.</value>
        Public Property Days() As Integer()
            Get
                Return MaskToIndices(CInt(taskTrigger.Data.monthlyDate.Days))
            End Get
            Set(ByVal value As Integer())
                taskTrigger.Data.monthlyDate.Days = DirectCast(IndicesToMask(value), UInteger)
                MyBase.SyncTrigger()
            End Set
        End Property
    End Class

    ''' <summary>
    ''' Trigger that fires when the system is idle for a period of time.
    ''' Length of period set by <see cref="Task.IdleWaitMinutes"/>.
    ''' </summary>
    Public Class OnIdleTrigger
        Inherits Trigger
        ''' <summary>
        ''' Creates an OnIdleTrigger.  Idle period set separately.
        ''' See <see cref="Task.IdleWaitMinutes"/> inherited property.
        ''' </summary>
        Public Sub New()
            MyBase.New()
            taskTrigger.Type = TaskSchedulerInterop.TaskTriggerType.EVENT_TRIGGER_ON_IDLE
        End Sub

        ''' <summary>
        ''' Internal constructor to create from existing ITaskTrigger interface.
        ''' </summary>
        ''' <param name="iTrigger">Current base Trigger.</param>
        Friend Sub New(ByVal iTrigger As TaskSchedulerInterop.ITaskTrigger)
            MyBase.New(iTrigger)
        End Sub
    End Class

    ''' <summary>
    ''' Trigger that fires when the system starts.
    ''' </summary>
    Public Class OnSystemStartTrigger
        Inherits Trigger
        ''' <summary>
        ''' Creates an OnSystemStartTrigger.
        ''' </summary>
        Public Sub New()
            MyBase.New()
            taskTrigger.Type = TaskSchedulerInterop.TaskTriggerType.EVENT_TRIGGER_AT_SYSTEMSTART
        End Sub

        ''' <summary>
        ''' Internal constructor to create from existing ITaskTrigger interface.
        ''' </summary>
        ''' <param name="iTrigger">ITaskTrigger interface from system Task Scheduler.</param>
        Friend Sub New(ByVal iTrigger As TaskSchedulerInterop.ITaskTrigger)
            MyBase.New(iTrigger)
        End Sub
    End Class

    ''' <summary>
    ''' Trigger that fires when a user logs on.
    ''' </summary>
    ''' <remarks>Triggers of this type fire when any user logs on, not just the
    ''' user identified in the account information.</remarks>
    Public Class OnLogonTrigger
        Inherits Trigger
        ''' <summary>
        ''' Creates an OnLogonTrigger.
        ''' </summary>
        Public Sub New()
            MyBase.New()
            taskTrigger.Type = TaskSchedulerInterop.TaskTriggerType.EVENT_TRIGGER_AT_LOGON
        End Sub
        ''' <summary>
        ''' Internal constructor to create from existing ITaskTrigger interface.
        ''' </summary>
        ''' <param name="iTrigger">ITaskTrigger from system Task Scheduler.</param>
        Friend Sub New(ByVal iTrigger As TaskSchedulerInterop.ITaskTrigger)
            MyBase.New(iTrigger)
        End Sub
    End Class

End Namespace