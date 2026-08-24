Imports System
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Runtime.InteropServices.ComTypes
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Security
Imports TaskScheduler.TaskSchedulerInterop

Namespace TaskScheduler
#Region " Enums "

    ''' <summary>
    ''' Options for a task, used for the Flags property of a Task. Uses the
    ''' "Flags" attribute, so these values are combined with |. 
    ''' Some flags are documented as Windows 95 only, but they have a
    ''' user interface in Windows XP so that may not be true.
    ''' </summary>
    <Flags()> _
    Public Enum TaskFlags
        ''' <summary>
        ''' The precise meaning of this flag is elusive.  The MSDN documentation describes it
        ''' only for use in converting jobs from the Windows NT "AT" service to the newer
        ''' Task Scheduler.  No other use for the flag is documented.
        ''' </summary>
        Interactive = &H1
        ''' <summary>
        ''' The task will be deleted when there are no more scheduled run times.
        ''' </summary>
        DeleteWhenDone = &H2
        ''' <summary>
        ''' The task is disabled.  Used to temporarily prevent a task from being triggered normally.
        ''' </summary>
        Disabled = &H4
        ''' <summary>
        ''' The task begins only if the computer is idle at the scheduled start time. 
        ''' The computer is not considered idle until the task's <see cref="Task.IdleWaitMinutes"/> time
        ''' elapses with no user input.
        ''' </summary>
        StartOnlyIfIdle = &H10
        ''' <summary>
        ''' The task terminates if the computer makes an idle to non-idle transition while the task is running.
        ''' For information regarding idle triggers, see <see cref="OnIdleTrigger"/>.
        ''' </summary>
        KillOnIdleEnd = &H20
        ''' <summary>
        ''' The task does not start if the computer is running on battery power.
        ''' </summary>
        DontStartIfOnBatteries = &H40
        ''' <summary>
        ''' The task ends, and the associated application quits if the computer switches
        ''' to battery power.
        ''' </summary>
        KillIfGoingOnBatteries = &H80
        ''' <summary>
        ''' The task runs only if the system is docked.  
        ''' (Not mentioned in current MSDN documentation; probably obsolete.)
        ''' </summary>
        RunOnlyIfDocked = &H100
        ''' <summary>
        ''' The task item is hidden.  
        ''' 
        ''' This is implemented by setting the job file's hidden attribute.  Testing revealed that clearing
        ''' this flag doesn't clear the file attribute, so the library sets the file attribute directly.  This
        ''' flag is kept in sync with the task's Hidden property, so they function equivalently.
        ''' </summary>
        Hidden = &H200
        ''' <summary>
        ''' The task runs only if there is currently a valid Internet connection.
        ''' Not currently implemented. (Check current MSDN documentation for updates.)
        ''' </summary>
        RunIfConnectedToInternet = &H400
        ''' <summary>
        ''' The task starts again if the computer makes a non-idle to idle transition before all the
        ''' task's task_triggers elapse. (Use this flag in conjunction with KillOnIdleEnd.)
        ''' </summary>
        RestartOnIdleResume = &H800
        ''' <summary>
        ''' Wake the computer to run this task.  Seems to be misnamed, but the name is taken from
        ''' the low-level interface.
        ''' 
        ''' </summary>
        SystemRequired = &H1000
        ''' <summary>
        ''' The task runs only if the user specified in SetAccountInformation() is
        ''' logged on interactively.  This flag has no effect on tasks set to run in
        ''' the local SYSTEM account.
        ''' </summary>
        RunOnlyIfLoggedOn = &H2000
    End Enum

    ''' <summary>
    ''' Status values returned for a task.  Some values have been determined to occur although
    ''' they do no appear in the Task Scheduler system documentation.
    ''' </summary>
    Public Enum TaskStatus
        ''' <summary>
        ''' The task is ready to run at its next scheduled time.
        ''' </summary>
        Ready = HResult.SCHED_S_TASK_READY
        ''' <summary>
        ''' The task is currently running.
        ''' </summary>
        Running = HResult.SCHED_S_TASK_RUNNING
        ''' <summary>
        ''' One or more of the properties that are needed to run this task on a schedule have not been set. 
        ''' </summary>
        NotScheduled = HResult.SCHED_S_TASK_NOT_SCHEDULED
        ''' <summary>
        ''' The task has not yet run.
        ''' </summary>
        NeverRun = HResult.SCHED_S_TASK_HAS_NOT_RUN
        ''' <summary>
        ''' The task will not run at the scheduled times because it has been disabled.
        ''' </summary>
        Disabled = HResult.SCHED_S_TASK_DISABLED
        ''' <summary>
        ''' There are no more runs scheduled for this task.
        ''' </summary>
        NoMoreRuns = HResult.SCHED_S_TASK_NO_MORE_RUNS
        ''' <summary>
        ''' The last run of the task was terminated by the user.
        ''' </summary>
        Terminated = HResult.SCHED_S_TASK_TERMINATED
        ''' <summary>
        ''' Either the task has no triggers or the existing triggers are disabled or not set.
        ''' </summary>
        NoTriggers = HResult.SCHED_S_TASK_NO_VALID_TRIGGERS
        ''' <summary>
        ''' Event triggers don't have set run times.
        ''' </summary>
        NoTriggerTime = HResult.SCHED_S_EVENT_TRIGGER
    End Enum

#End Region

    ''' <summary>
    ''' Represents an item in the Scheduled Tasks folder.  There are no public constructors for Task.
    ''' New instances are generated by a <see cref="ScheduledTasks"/> object using Open or Create methods.
    ''' A task object holds COM interfaces;  call its <see cref="Close"/> method to release them.
    ''' </summary>
    Public Class Task
        Implements IDisposable

#Region " Fields "

        ''' <summary>
        ''' Internal COM interface
        ''' </summary>
        Private m_ITask As ITask
        ''' <summary>
        ''' Name of this task (with no .job extension)
        ''' </summary>
        Private m_Name As String
        ''' <summary>
        ''' List of triggers for this task
        ''' </summary>
        Private m_Triggers As TriggerList

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Internal constructor for a task, used by <see cref="ScheduledTasks"/>.
        ''' </summary>
        ''' <param name="iTask">Instance of an ITask.</param>
        ''' <param name="taskName">Name of the task.</param>
        Friend Sub New(ByVal iTask As ITask, ByVal taskName As String)
            Me.m_ITask = iTask
            If taskName.EndsWith(".job") Then
                m_Name = taskName.Substring(0, taskName.Length - 4)
            Else
                m_Name = taskName
            End If
            m_Triggers = Nothing
            Me.Hidden = GetHiddenFileAttr()
        End Sub

#End Region

#Region " Properties "

        ''' <summary>
        ''' Gets the name of the task.  The name is also the filename (plus a .job extension)
        ''' the Task Scheduler uses to store the task information.  To change the name of a
        ''' task, use <see cref="Save"/> to save it as a new name and then delete
        ''' the old task.
        ''' </summary>
        Public ReadOnly Property Name() As String
            Get
                Return m_Name
            End Get
        End Property

        ''' <summary>
        ''' Gets the list of triggers associated with the task.
        ''' </summary>
        Public ReadOnly Property Triggers() As TriggerList
            Get
                If Triggers Is Nothing Then
                    ' Trigger list has not been requested before; create it
                    Triggers = New TriggerList(m_ITask)
                End If
                Return Triggers
            End Get
        End Property

        ''' <summary>
        ''' Gets/sets the application filename that task is to run.  Get returns 
        ''' an absolute pathname.  A name searched with the PATH environment variable can
        ''' be assigned, and the path search is done when the task is saved.
        ''' </summary>
        Public Property ApplicationName() As String
            Get
                Dim lpwstr As IntPtr
                m_ITask.GetApplicationName(lpwstr)
                Return CoTaskMem.LPWStrToString(lpwstr)
            End Get
            Set(ByVal value As String)
                m_ITask.SetApplicationName(value)
            End Set
        End Property

        ''' <summary>
        ''' Gets the name of the account under which the task process will run.
        ''' </summary>
        Public ReadOnly Property AccountName() As String
            Get
                Dim lpwstr As IntPtr = IntPtr.Zero
                m_ITask.GetAccountInformation(lpwstr)
                Return CoTaskMem.LPWStrToString(lpwstr)
            End Get
        End Property

        ''' <summary>
        ''' Gets/sets the comment associated with the task.  The comment appears in the 
        ''' Scheduled Tasks user interface.
        ''' </summary>
        Public Property Comment() As String
            Get
                Dim lpwstr As IntPtr
                m_ITask.GetComment(lpwstr)
                Return CoTaskMem.LPWStrToString(lpwstr)
            End Get
            Set(ByVal value As String)
                m_ITask.SetComment(value)
            End Set
        End Property

        ''' <summary>
        ''' Gets/sets the creator of the task.  If no value is supplied, the system
        ''' fills in the account name of the caller when the task is saved.
        ''' </summary>
        Public Property Creator() As String
            Get
                Dim lpwstr As IntPtr
                m_ITask.GetCreator(lpwstr)
                Return CoTaskMem.LPWStrToString(lpwstr)
            End Get
            Set(ByVal value As String)
                m_ITask.SetCreator(value)
            End Set
        End Property

        '''' <summary>
        '''' Gets/sets the number of times to retry task execution after failure. (Not implemented.)
        '''' </summary>
        'Private Property ErrorRetryCount() As Short
        '    Get
        '        Dim ret As UShort
        '        m_ITask.GetErrorRetryCount(ret)
        '        Return CShort(ret)
        '    End Get
        '    Set(ByVal value As Short)
        '        ITask.SetErrorRetryCount(CShort(value))
        '    End Set
        'End Property

        '''' <summary>
        '''' Gets/sets the time interval, in minutes, to delay between error retries. (Not implemented.)
        '''' </summary>
        'Private Property ErrorRetryInterval() As Short
        '    Get
        '        Dim ret As UShort
        '        m_ITask.GetErrorRetryInterval(ret)
        '        Return DirectCast(ret, Short)
        '    End Get
        '    Set(ByVal value As Short)
        '        iTask.SetErrorRetryInterval(DirectCast(value, UShort))
        '    End Set
        'End Property

        ''' <summary>
        ''' Gets the Win32 exit code from the last execution of the task.  If the task failed
        ''' to start on its last run, the reason is returned as an exception.  Not updated while
        ''' in an open task;  the property does not change unless the task is closed and re-opened.
        ''' <exception>Various exceptions for a task that couldn't be run.</exception>
        ''' </summary>
        Public ReadOnly Property ExitCode() As Integer
            Get
                Dim ret As UInteger = 0
                m_ITask.GetExitCode(ret)
                Return CInt(ret)
            End Get
        End Property

        ''' <summary>
        ''' Gets/sets the <see cref="TaskFlags"/> associated with the current task. 
        ''' </summary>
        Public Property Flags() As TaskFlags
            Get
                Dim ret As UInteger
                m_ITask.GetFlags(ret)
                Return DirectCast(ret, TaskFlags)
            End Get
            Set(ByVal value As TaskFlags)
                iTask.SetFlags(DirectCast(value, UInteger))
            End Set
        End Property

        ''' <summary>
        ''' Gets/sets how long the system must remain idle, even after the trigger
        ''' would normally fire, before the task will run. 
        ''' </summary>
        Public Property IdleWaitMinutes() As Short
            Get
                Dim ret As UShort, [nothing] As UShort
                m_ITask.GetIdleWait(ret, [nothing])
                Return CShort(ret)
            End Get
            Set(ByVal value As Short)
                Dim m As UShort = CUShort(IdleWaitDeadlineMinutes)
                m_ITask.SetIdleWait(CUShort(value), m)
            End Set
        End Property

        ''' <summary>
        ''' Gets/sets the maximum number of minutes that Task Scheduler will wait for a 
        ''' required idle period to occur. 
        ''' </summary>
        Public Property IdleWaitDeadlineMinutes() As Short
            Get
                Dim ret As UShort, [nothing] As UShort
                m_ITask.GetIdleWait([nothing], ret)
                Return CShort(ret)
            End Get
            Set(ByVal value As Short)
                Dim m As UShort = CUShort(IdleWaitMinutes)
                m_ITask.SetIdleWait(m, CUShort(value))
            End Set
        End Property

        ''' <summary>
        ''' <p>Gets/sets the maximum length of time the task is permitted to run.
        ''' Setting MaxRunTime also affects the value of <see cref="Task.MaxRunTimeLimited"/>.
        ''' </p>
        ''' <p>The longest MaxRunTime implemented is 0xFFFFFFFE milliseconds, or 
        ''' about 50 days.  If you set a TimeSpan longer than that, the
        ''' MaxRunTime will be unlimited.</p>
        ''' </summary>
        ''' <Remarks>
        ''' </Remarks>
        Public Property MaxRunTime() As TimeSpan
            Get
                Dim ret As UInteger
                m_ITask.GetMaxRunTime(ret)
                Return New TimeSpan(CLng(ret) * TimeSpan.TicksPerMillisecond)
            End Get
            Set(ByVal value As TimeSpan)
                Dim proposed As Double = (DirectCast(value, TimeSpan)).TotalMilliseconds
                If proposed >= UInteger.MaxValue Then
                    m_ITask.SetMaxRunTime(UInteger.MaxValue)
                Else
                    iTask.SetMaxRunTime(DirectCast(proposed, UInteger))

                    'iTask.SetMaxRunTime((uint)((TimeSpan)value).TotalMilliseconds);
                End If
            End Set
        End Property

        ''' <summary>
        ''' <p>If the maximum run time is limited, the task will be terminated after 
        ''' <see cref="Task.MaxRunTime"/> expires.  Setting the value to FALSE, i.e. unlimited,
        ''' invalidates MaxRunTime.</p> 
        ''' <p>The Task Scheduler service will try to send a WM_CLOSE message when it needs to terminate
        ''' a task.  If the message can't be sent, or the task does not respond with three minutes,
        ''' the task will be terminated using TerminateProcess.</p> 
        ''' </summary>
        Public Property MaxRunTimeLimited() As Boolean
            Get
                Dim ret As UInteger
                m_ITask.GetMaxRunTime(ret)
                Return (ret = UInteger.MaxValue)
            End Get
            Set(ByVal value As Boolean)
                If value Then
                    Dim ret As UInteger
                    m_ITask.GetMaxRunTime(ret)
                    If ret = UInteger.MaxValue Then
                        '72 hours.  Thats what Explorer sets.
                        m_ITask.SetMaxRunTime(72 * 360 * 1000)
                    End If
                Else
                    m_ITask.SetMaxRunTime(UInteger.MaxValue)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets the most recent time the task began running.  <see cref="DateTime.MinValue"/> 
        ''' returned if the task has not run.
        ''' </summary>
        Public ReadOnly Property MostRecentRunTime() As DateTime
            Get
                Dim st As New SystemTime()
                m_ITask.GetMostRecentRunTime(st)
                If st.Year = 0 Then
                    Return DateTime.MinValue
                End If
                Return New DateTime(DirectCast(st.Year, Integer), DirectCast(st.Month, Integer), DirectCast(st.Day, Integer), DirectCast(st.Hour, Integer), DirectCast(st.Minute, Integer), DirectCast(st.Second, Integer), _
                 DirectCast(st.Milliseconds, Integer))
            End Get
        End Property

        ''' <summary>
        ''' Gets the next time the task will run. Returns <see cref="DateTime.MinValue"/> 
        ''' if the task is not scheduled to run.
        ''' </summary>
        Public ReadOnly Property NextRunTime() As DateTime
            Get
                Dim st As New SystemTime()
                m_ITask.GetNextRunTime(st)
                If st.Year = 0 Then
                    Return DateTime.MinValue
                End If
                Return New DateTime(DirectCast(st.Year, Integer), DirectCast(st.Month, Integer), DirectCast(st.Day, Integer), DirectCast(st.Hour, Integer), DirectCast(st.Minute, Integer), DirectCast(st.Second, Integer), _
                 DirectCast(st.Milliseconds, Integer))
            End Get
        End Property

        ''' <summary>
        ''' Gets/sets the command-line parameters for the task.
        ''' </summary>
        Public Property Parameters() As String
            Get
                Dim lpwstr As IntPtr
                m_ITask.GetParameters(lpwstr)
                Return CoTaskMem.LPWStrToString(lpwstr)
            End Get
            Set(ByVal value As String)
                m_ITask.SetParameters(value)
            End Set
        End Property

        ''' <summary>
        ''' Gets/sets the priority for the task process.  
        ''' Note:  ProcessPriorityClass defines two levels (AboveNormal and BelowNormal) that are
        ''' not documented in the task scheduler interface and can't be use on Win 98 platforms.
        ''' </summary>
        Public Property Priority() As System.Diagnostics.ProcessPriorityClass
            Get
                Dim ret As UInteger
                m_ITask.GetPriority(ret)
                Return DirectCast(ret, System.Diagnostics.ProcessPriorityClass)
            End Get
            Set(ByVal value As System.Diagnostics.ProcessPriorityClass)
                If value = System.Diagnostics.ProcessPriorityClass.AboveNormal OrElse value = System.Diagnostics.ProcessPriorityClass.BelowNormal Then
                    Throw New ArgumentException("Unsupported Priority Level")
                End If
                iTask.SetPriority(DirectCast(value, UInteger))
            End Set
        End Property

        ''' <summary>
        ''' Gets the status of the task.  Returns <see cref="TaskStatus"/>.
        ''' Not updated while a task is open.
        ''' </summary>
        Public ReadOnly Property Status() As TaskStatus
            Get
                Dim ret As Integer
                m_ITask.GetStatus(ret)
                Return DirectCast(ret, TaskStatus)
            End Get
        End Property

        ''' <summary>
        ''' Extended Flags associated with a task. These are associated with the ITask com interface
        ''' and none are currently defined.
        ''' </summary>
        Private Property FlagsEx() As Integer
            Get
                Dim ret As UInteger
                m_ITask.GetTaskFlags(ret)
                Return DirectCast(ret, Integer)
            End Get
            Set(ByVal value As Integer)
                iTask.SetTaskFlags(DirectCast(value, UInteger))
            End Set
        End Property

        ''' <summary>
        ''' Gets/sets the initial working directory for the task.
        ''' </summary>
        Public Property WorkingDirectory() As String
            Get
                Dim lpwstr As IntPtr
                m_ITask.GetWorkingDirectory(lpwstr)
                Return CoTaskMem.LPWStrToString(lpwstr)
            End Get
            Set(ByVal value As String)
                m_ITask.SetWorkingDirectory(value)
            End Set
        End Property

        ''' <summary>
        ''' Hidden tasks are stored in files with
        ''' the hidden file attribute so they don't appear in the Explorer user interface.
        ''' Because there is a special interface for Scheduled Tasks, they don't appear
        ''' even if Explorer is set to show hidden files.
        ''' Functionally equivalent to TaskFlags.Hidden.
        ''' </summary>
        Public Property Hidden() As Boolean
            Get
                Return (Me.Flags And TaskFlags.Hidden) <> 0
            End Get
            Set(ByVal value As Boolean)
                If value Then
                    Me.Flags = Me.Flags Or TaskFlags.Hidden
                Else
                    Me.Flags = Me.Flags And Not TaskFlags.Hidden
                End If
            End Set
        End Property
        ''' <summary>
        ''' Gets/sets arbitrary data associated with the task.  The tag can be used for any purpose
        ''' by the client, and is not used by the Task Scheduler.  Known as WorkItemData in the
        ''' IWorkItem com interface.
        ''' </summary>
        Public Property Tag() As Object
            Get
                Dim DataLen As UShort
                Dim Data As IntPtr
                m_ITask.GetWorkItemData(DataLen, Data)
                Dim bytes As Byte() = New Byte(DataLen) {}
                Marshal.Copy(Data, bytes, 0, DataLen)
                Dim stream As New MemoryStream(bytes, False)
                Dim b As New BinaryFormatter()
                Return b.Deserialize(stream)
            End Get
            Set(ByVal value As Object)
                If Not value.[GetType]().IsSerializable Then
                    Throw New ArgumentException("Objects set as Data for Tasks must be serializable", "value")
                End If
                Dim b As New BinaryFormatter()
                Dim stream As New MemoryStream()
                b.Serialize(stream, value)
                m_ITask.SetWorkItemData(CUShort(stream.Length), stream.GetBuffer())
            End Set
        End Property
#End Region

#Region " Methods "
        ''' <summary>
        ''' Set the hidden attribute on the file corresponding to this task.
        ''' </summary>
        ''' <param name="set">Set the attribute accordingly.</param>
        Private Sub SetHiddenFileAttr(ByVal [set] As Boolean)
            Dim iFile As IPersistFile = DirectCast(m_ITask, IPersistFile)
            Dim fileName As String = ""
            iFile.GetCurFile(fileName)
            Dim attr As System.IO.FileAttributes
            attr = System.IO.File.GetAttributes(fileName)
            If [set] Then
                attr = attr Or System.IO.FileAttributes.Hidden
            Else
                attr = attr And Not System.IO.FileAttributes.Hidden
            End If
            System.IO.File.SetAttributes(fileName, attr)
        End Sub
        ''' <summary>
        ''' Get the hidden attribute from the file corresponding to this task.
        ''' </summary>
        ''' <returns>The value of the attribute.</returns>
        Private Function GetHiddenFileAttr() As Boolean
            Dim iFile As IPersistFile = DirectCast(m_ITask, IPersistFile)
            Dim fileName As String = ""
            iFile.GetCurFile(fileName)
            Dim attr As System.IO.FileAttributes
            Try
                attr = System.IO.File.GetAttributes(fileName)
                Return (attr And System.IO.FileAttributes.Hidden) <> 0
            Catch
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Calculate the next time the task would be scheduled
        ''' to run after a given arbitrary time.  If the task will not run
        ''' (perhaps disabled) then returns <see cref="DateTime.MinValue"/>.
        ''' </summary>
        ''' <param name="after">The time to calculate from.</param>
        ''' <returns>The next time the task would run.</returns>
        Public Function NextRunTimeAfter(ByVal after As DateTime) As DateTime
            'Add one second to get a run time strictly greater than the specified time.
            after = after.AddSeconds(1)
            'Convert to a valid SystemTime
            Dim stAfter As New SystemTime()
            stAfter.Year = CUShort(after.Year)
            stAfter.Month = CUShort(after.Month)
            stAfter.Day = CUShort(after.Day)
            stAfter.DayOfWeek = CUShort(after.DayOfWeek)
            stAfter.Hour = CUShort(after.Hour)
            stAfter.Minute = CUShort(after.Minute)
            stAfter.Second = CUShort(after.Second)
            Dim stLimit As New SystemTime()
            ' Would like to pass null as the second parameter to GetRunTimes, indicating that
            ' the interval is unlimited.  Can't figure out how to do that, so use a big time value.
            stLimit = stAfter
            stLimit.Year = CUShort(DateTime.MaxValue.Year)
            stLimit.Month = 1
            'Just in case stAfter date was Feb 29, but MaxValue.Year is not a leap year!
            Dim pTimes As IntPtr
            Dim nFetch As UShort = 1
            m_ITask.GetRunTimes(stAfter, stLimit, nFetch, pTimes)
            If nFetch = 1 Then
                Dim stNext As New SystemTime()
                stNext = DirectCast(Marshal.PtrToStructure(pTimes, GetType(SystemTime)), SystemTime)
                Marshal.FreeCoTaskMem(pTimes)
                Return New DateTime(stNext.Year, stNext.Month, stNext.Day, stNext.Hour, stNext.Minute, stNext.Second)
            Else
                Return DateTime.MinValue
            End If
        End Function

        ''' <summary>
        ''' Schedules the task for immediate execution.  
        ''' The system works from the saved version of the task, so call <see cref="Save"/> before running.
        ''' If the task has never been saved, it throws an argument exception.  Problems starting
        ''' the task are reported by the <see cref="ExitCode"/> property, not by exceptions on Run.
        ''' </summary>
        ''' <remarks>The system never updates an open task, so you don't get current results for
        ''' the <see cref="Status"/> or the <see cref="ExitCode"/> properties until you close
        ''' and reopen the task.
        ''' </remarks>
        ''' <exception cref="ArgumentException"></exception>
        Public Sub Run()
            m_ITask.Run()
        End Sub

        ''' <summary>
        ''' Saves changes to the established task name.
        ''' </summary>
        ''' <overloads>Saves changes that have been made to this Task.</overloads>
        ''' <remarks>The account name is checked for validity
        ''' when a Task is saved.  The password is not checked, but the account name
        ''' must be valid (or empty).
        ''' </remarks>
        ''' <exception cref="COMException">Unable to establish existence of the account specified.</exception>
        Public Sub Save()
            Dim iFile As IPersistFile = DirectCast(m_ITask, IPersistFile)
            iFile.Save(Nothing, False)
            SetHiddenFileAttr(Hidden)
            'Do the Task Scheduler's work for it because it doesn't reset properly
        End Sub

        ''' <summary>
        ''' Saves the Task with a new name.  The task with the old name continues to 
        ''' exist in whatever state it was last saved.  It is no longer open, because  
        ''' the Task object is associated with the new name from now on. 
        ''' If there is already a task using the new name, it is overwritten.
        ''' </summary>
        ''' <remarks>See the <see cref="Save"/>() overload.</remarks>
        ''' <param name="name">The new name to be used for this task.</param>
        ''' <exception cref="COMException">Unable to establish existence of the account specified.</exception>
        Public Sub Save(ByVal Name As String)
            Dim iFile As IPersistFile = DirectCast(m_ITask, IPersistFile)
            Dim CurrentPath As IO.Path

            iFile.GetCurFile(CurrentPath.ToString)
            Dim NewPath As IO.Path ' String

            NewPath = Path.GetDirectoryName(CurrentPath.ToString) + Path.DirectorySeparatorChar + Name + Path.GetExtension(CurrentPath.ToString)
            iFile.Save(NewPath.ToString, True)
            iFile.SaveCompleted(NewPath.ToString)
            ' probably unnecessary 
            m_Name = Name
            SetHiddenFileAttr(Hidden)
            'Do the Task Scheduler's work for it because it doesn't reset properly
        End Sub

        ''' <summary>
        ''' Release COM interfaces for this Task.  After a Task is closed, accessing its
        ''' members throws a null reference exception.
        ''' </summary>
        Public Sub Close()
            If Not Triggers Is Nothing Then
                m_Triggers.Dispose()
            End If
            Marshal.ReleaseComObject(m_ITask)
            m_ITask = Nothing
        End Sub

        ''' <summary>
        ''' For compatibility with earlier versions.  New clients should use <see cref="DisplayPropertySheet"/>.
        ''' </summary>
        ''' <remarks>
        ''' Display the property pages of this task for user editing.  If the user clicks OK, the
        ''' task's properties are updated and the task is also automatically saved.
        ''' </remarks>
        Public Sub DisplayForEdit()
            m_ITask.EditWorkItem(0, 0)
        End Sub

        ''' <summary>
        ''' Argument for DisplayForEdit to determine which property pages to display.
        ''' </summary>
        <Flags()> _
        Public Enum PropPages
            ''' <summary>
            ''' The task property page
            ''' </summary>
            Task = &H1
            ''' <summary>
            ''' The schedule property page
            ''' </summary>
            Schedule = &H2
            ''' <summary>
            ''' The setting property page
            ''' </summary>
            Settings = &H4
        End Enum
        ''' 
        ''' <summary>
        ''' Display all property pages.
        ''' </summary>
        ''' <remarks>  
        ''' The method does not return until the user has dismissed the dialog box.
        ''' If the dialog box is dismissed with the OK button, returns true and
        ''' updates properties in the task.
        ''' The changes are not made permanent, however, until the task is saved.  (Save() method.)
        ''' </remarks>
        ''' <returns><c>true</c> if dialog box was dismissed with OK, otherwise <c>false</c>.</returns>
        ''' <overloads>Display the property pages of this task for user editing.</overloads>
        Public Function DisplayPropertySheet() As Boolean
            'iTask.EditWorkItem(0, 0);  //This implementation saves automatically, so we don't use it.
            Return DisplayPropertySheet(PropPages.Task Or PropPages.Schedule Or PropPages.Settings)
        End Function

        ''' <summary>
        ''' Display only the specified property pages.  
        ''' </summary>
        ''' <remarks>  
        ''' See the <see cref="DisplayPropertySheet"/>() overload.
        ''' </remarks>
        ''' <param name="pages">Controls which pages are presented</param>
        ''' <returns><c>true</c> if dialog box was dismissed with OK, otherwise <c>false</c>.</returns>
        Public Function DisplayPropertySheet(ByVal pages As PropPages) As Boolean
            Dim hdr As New PropSheetHeader()
            Dim iProvideTaskPage As IProvideTaskPage = DirectCast(m_ITask, IProvideTaskPage)
            Dim hPages As IntPtr() = New IntPtr(3) {}
            Dim hPage As IntPtr
            Dim nPages As Integer = 0
            If (pages And PropPages.Task) <> 0 Then
                'get task page
                iProvideTaskPage.GetPage(0, False, hPage)
                hPages(System.Math.Max(System.Threading.Interlocked.Increment(nPages), nPages - 1)) = hPage
            End If
            If (pages And PropPages.Schedule) <> 0 Then
                'get task page
                iProvideTaskPage.GetPage(1, False, hPage)
                hPages(System.Math.Max(System.Threading.Interlocked.Increment(nPages), nPages - 1)) = hPage
            End If
            If (pages And PropPages.Settings) <> 0 Then
                'get task page
                iProvideTaskPage.GetPage(2, False, hPage)
                hPages(System.Math.Max(System.Threading.Interlocked.Increment(nPages), nPages - 1)) = hPage
            End If
            If nPages = 0 Then
                Throw (New ArgumentException("No Property Pages to display"))
            End If
            hdr.dwSize = DirectCast(Marshal.SizeOf(hdr), UInteger)
            hdr.dwFlags = DirectCast((PropSheetFlags.PSH_DEFAULT Or PropSheetFlags.PSH_NOAPPLYNOW), UInteger)
            hdr.pszCaption = Me.m_Name
            hdr.nPages = DirectCast(nPages, UInteger)
            Dim gch As GCHandle = GCHandle.Alloc(hPages, GCHandleType.Pinned)
            hdr.phpage = gch.AddrOfPinnedObject()
            Dim res As Integer = PropertySheetDisplay.PropertySheet(hdr)
            gch.Free()
            If res < 0 Then
                Throw (New Exception("Property Sheet failed to display"))
            End If
            Return res > 0
        End Function


        ''' <summary>
        ''' Sets the account under which the task will run.  Supply the account name and 
        ''' password as parameters.  For the localsystem account, pass an empty string for
        ''' the account name and null for the password.  See Remarks.
        ''' </summary>
        ''' <param name="accountName">Full account name.</param>
        ''' <param name="password">Password for the account.</param>
        ''' <remarks>
        ''' <p>To have the task to run under the local system account, pass the empty string ("")
        ''' as accountName and null as the password.  The caller must be running in
        ''' an administrator account or in the local system account.
        ''' </p> 
        ''' <p>
        ''' You can also specify a null password if the task has the flag RunOnlyIfLoggedOn set.
        ''' This allows you to schedule a task for an account for which you don't know the password,
        ''' but the account must be logged on interactively at the time the task runs.</p>
        ''' </remarks>
        Public Sub SetAccountInformation(ByVal accountName As String, ByVal password As String)
            Dim pwd As IntPtr = Marshal.StringToCoTaskMemUni(password)
            m_ITask.SetAccountInformation(accountName, pwd)
            Marshal.FreeCoTaskMem(pwd)
        End Sub
        ''' <summary>
        ''' Overload for SetAccountInformation which permits use of a SecureString for the
        ''' password parameter.  The decoded password will remain in memory only as long as
        ''' needed to be passed to the TaskScheduler service.
        ''' </summary>
        ''' <param name="accountName">Full account name.</param>
        ''' <param name="password">Password for the account.</param>
        Public Sub SetAccountInformation(ByVal accountName As String, ByVal password As SecureString)
            Dim pwd As IntPtr = Marshal.SecureStringToCoTaskMemUnicode(password)
            m_ITask.SetAccountInformation(accountName, pwd)
            Marshal.ZeroFreeCoTaskMemUnicode(pwd)
        End Sub

        ''' <summary>
        ''' Request that the task be terminated if it is currently running.  The call returns
        ''' immediately, although the task may continue briefly.  For Windows programs, a WM_CLOSE
        ''' message is sent first and the task is given three minutes to shut down voluntarily.
        ''' Should it not, or if the task is not a Windows program, TerminateProcess is used.
        ''' </summary>
        ''' <exception cref="COMException">The task is not running.</exception>
        Public Sub Terminate()
            m_ITask.Terminate()
        End Sub

        ''' <summary>
        ''' Overridden. Outputs the name of the task, the application and parameters.
        ''' </summary>
        ''' <returns>String representing task.</returns>
        Public Overloads Overrides Function ToString() As String
            Return String.Format("{0} (""{1}"" {2})", m_Name, ApplicationName, Parameters)
        End Function
#End Region

#Region " Implementation of IDisposable "
        ''' <summary>
        ''' A synonym for Close.
        ''' </summary>
        Public Sub Dispose() Implements System.IDisposable.Dispose
            Me.Close()
        End Sub
#End Region

    End Class
End Namespace

