Imports System
Imports System.Runtime.InteropServices

Namespace TaskSchedulerInterop
#Region " class HRESULT -- Values peculiar to the task scheduler. "
    Friend Class HResult
        ' The task is ready to run at its next scheduled time.
        Public Const SCHED_S_TASK_READY As Integer = &H41300
        ' The task is currently running.
        Public Const SCHED_S_TASK_RUNNING As Integer = &H41301
        ' The task will not run at the scheduled times because it has been disabled.
        Public Const SCHED_S_TASK_DISABLED As Integer = &H41302
        ' The task has not yet run.
        Public Const SCHED_S_TASK_HAS_NOT_RUN As Integer = &H41303
        ' There are no more runs scheduled for this task.
        Public Const SCHED_S_TASK_NO_MORE_RUNS As Integer = &H41304
        ' One or more of the properties that are needed to run this task on a schedule have not been set.
        Public Const SCHED_S_TASK_NOT_SCHEDULED As Integer = &H41305
        ' The last run of the task was terminated by the user.
        Public Const SCHED_S_TASK_TERMINATED As Integer = &H41306
        ' Either the task has no triggers or the existing triggers are disabled or not set.
        Public Const SCHED_S_TASK_NO_VALID_TRIGGERS As Integer = &H41307
        ' Event triggers don't have set run times.
        Public Const SCHED_S_EVENT_TRIGGER As Integer = &H41308
        ' Trigger not found.
        Public Const SCHED_E_TRIGGER_NOT_FOUND As Integer = DirectCast(&H80041309, Integer)
        ' One or more of the properties that are needed to run this task have not been set.
        Public Const SCHED_E_TASK_NOT_READY As Integer = DirectCast(&H8004130A, Integer)
        ' There is no running instance of the task to terminate.
        Public Const SCHED_E_TASK_NOT_RUNNING As Integer = DirectCast(&H8004130B, Integer)
        ' The Task Scheduler Service is not installed on this computer.
        Public Const SCHED_E_SERVICE_NOT_INSTALLED As Integer = DirectCast(&H8004130C, Integer)
        ' The task object could not be opened.
        Public Const SCHED_E_CANNOT_OPEN_TASK As Integer = DirectCast(&H8004130D, Integer)
        ' The object is either an invalid task object or is not a task object.
        Public Const SCHED_E_INVALID_TASK As Integer = DirectCast(&H8004130E, Integer)
        ' No account information could be found in the Task Scheduler security database for the task indicated.
        Public Const SCHED_E_ACCOUNT_INFORMATION_NOT_SET As Integer = DirectCast(&H8004130F, Integer)
        ' Unable to establish existence of the account specified.
        Public Const SCHED_E_ACCOUNT_NAME_NOT_FOUND As Integer = DirectCast(&H80041310, Integer)
        ' Corruption was detected in the Task Scheduler security database; the database has been reset.
        Public Const SCHED_E_ACCOUNT_DBASE_CORRUPT As Integer = DirectCast(&H80041311, Integer)
        ' Task Scheduler security services are available only on Windows NT.
        Public Const SCHED_E_NO_SECURITY_SERVICES As Integer = DirectCast(&H80041312, Integer)
        ' The task object version is either unsupported or invalid.
        Public Const SCHED_E_UNKNOWN_OBJECT_VERSION As Integer = DirectCast(&H80041313, Integer)
        ' The task has been configured with an unsupported combination of account settings and run time options.
        Public Const SCHED_E_UNSUPPORTED_ACCOUNT_OPTION As Integer = DirectCast(&H80041314, Integer)
        ' The Task Scheduler Service is not running.
        Public Const SCHED_E_SERVICE_NOT_RUNNING As Integer = DirectCast(&H80041315, Integer)
        ' The Task Scheduler service must be configured to run in the System account to function properly.  Individual tasks may be configured to run in other accounts.
        Public Const SCHED_E_SERVICE_NOT_LOCALSYSTEM As Integer = DirectCast(&H80041316, Integer)
    End Class
#End Region

    ' ------ Types used in in the Task Scheduler Interfaces ------
    Friend Enum TaskTriggerType
        TIME_TRIGGER_ONCE = 0
        ' Ignore the Type field.
        TIME_TRIGGER_DAILY = 1
        ' Use DAILY
        TIME_TRIGGER_WEEKLY = 2
        ' Use WEEKLY
        TIME_TRIGGER_MONTHLYDATE = 3
        ' Use MONTHLYDATE
        TIME_TRIGGER_MONTHLYDOW = 4
        ' Use MONTHLYDOW
        EVENT_TRIGGER_ON_IDLE = 5
        ' Ignore the Type field.
        EVENT_TRIGGER_AT_SYSTEMSTART = 6
        ' Ignore the Type field.
        EVENT_TRIGGER_AT_LOGON = 7
        ' Ignore the Type field.
    End Enum

    <StructLayout(LayoutKind.Sequential)> _
    Friend Structure Daily
        Public DaysInterval As UShort
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Friend Structure Weekly
        Public WeeksInterval As UShort
        Public DaysOfTheWeek As UShort
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Friend Structure MonthlyDate
        Public Days As UInteger
        Public Months As UShort
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Friend Structure MonthlyDOW
        Public WhichWeek As UShort
        Public DaysOfTheWeek As UShort
        Public Months As UShort
    End Structure

    <StructLayout(LayoutKind.Explicit)> _
    Friend Structure TriggerTypeData
        <FieldOffset(0)> _
        Public daily As Daily
        <FieldOffset(0)> _
        Public weekly As Weekly
        <FieldOffset(0)> _
        Public monthlyDate As MonthlyDate
        <FieldOffset(0)> _
        Public monthlyDOW As MonthlyDOW
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Friend Structure TaskTrigger
        Public TriggerSize As UShort
        ' Structure size.
        Public Reserved1 As UShort
        ' Reserved. Must be zero.
        Public BeginYear As UShort
        ' Trigger beginning date year.
        Public BeginMonth As UShort
        ' Trigger beginning date month.
        Public BeginDay As UShort
        ' Trigger beginning date day.
        Public EndYear As UShort
        ' Optional trigger ending date year.
        Public EndMonth As UShort
        ' Optional trigger ending date month.
        Public EndDay As UShort
        ' Optional trigger ending date day.
        Public StartHour As UShort
        ' Run bracket start time hour.
        Public StartMinute As UShort
        ' Run bracket start time minute.
        Public MinutesDuration As UInteger
        ' Duration of run bracket.
        Public MinutesInterval As UInteger
        ' Run bracket repetition interval.
        Public Flags As UInteger
        ' Trigger flags.
        Public Type As TaskTriggerType
        ' Trigger type.
        Public Data As TriggerTypeData
        ' Trigger data peculiar to this type (union).
        Public Reserved2 As UShort
        ' Reserved. Must be zero.
        Public RandomMinutesInterval As UShort
        ' Maximum number of random minutes after start time.
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Friend Structure SystemTime
        Public Year As UShort
        Public Month As UShort
        Public DayOfWeek As UShort
        Public Day As UShort
        Public Hour As UShort
        Public Minute As UShort
        Public Second As UShort
        Public Milliseconds As UShort
    End Structure

    ' ------ Types for calling PropertySheet (comctl32) through PInvoke ------
    <StructLayout(LayoutKind.Sequential)> _
    Friend Structure PropSheetHeader
        Public dwSize As UInt32
        Public dwFlags As UInt32
        Public hwndParent As IntPtr
        Public hInstance As IntPtr
        Public hIcon As IntPtr
        Public pszCaption As [String]
        Public nPages As UInt32
        Public nStartPage As UInt32
        Public phpage As IntPtr
        Public pfnCallback As IntPtr
        Public hbmWatermark As IntPtr
        Public hplWatermark As IntPtr
        Public hbmHeader As IntPtr
    End Structure

    <Flags()> _
    Friend Enum PropSheetFlags As UInteger
        PSH_DEFAULT = &H0
        PSH_PROPTITLE = &H1
        PSH_USEHICON = &H2
        PSH_USEICONID = &H4
        PSH_PROPSHEETPAGE = &H8
        PSH_WIZARDHASFINISH = &H10
        PSH_WIZARD = &H20
        PSH_USEPSTARTPAGE = &H40
        PSH_NOAPPLYNOW = &H80
        PSH_USECALLBACK = &H100
        PSH_HASHELP = &H200
        PSH_MODELESS = &H400
        PSH_RTLREADING = &H800
        PSH_WIZARDCONTEXTHELP = &H1000
        PSH_WIZARD97 = &H1000000
        PSH_WATERMARK = &H8000
        PSH_USEHBMWATERMARK = &H10000
        ' user pass in a hbmWatermark instead of pszbmWatermark
        PSH_USEHPLWATERMARK = &H20000
        '
        PSH_STRETCHWATERMARK = &H40000
        ' stretchwatermark also applies for the header
        PSH_HEADER = &H80000
        PSH_USEHBMHEADER = &H100000
        PSH_USEPAGELANG = &H200000
        ' use frame dialog template matched to page
    End Enum

    Friend Class PropertySheetDisplay
        'Display a property sheet
        <DllImport("comctl32.dll")> _
        Public Shared Function PropertySheet(<[In](), MarshalAs(UnmanagedType.Struct)> ByRef psh As PropSheetHeader) As Integer
        End Function
    End Class

    ' ----- Interfaces -----
    <Guid("148BD527-A2AB-11CE-B11F-00AA00530503"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Friend Interface ITaskScheduler
        Sub SetTargetComputer(<[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal Computer As String)
        Sub GetTargetComputer(ByRef Computer As System.IntPtr)
        Sub [Enum](<Out(), MarshalAs(UnmanagedType.[Interface])> ByRef EnumWorkItems As IEnumWorkItems)
        Sub Activate(<[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal Name As String, <[In]()> ByRef riid As System.Guid, <Out(), MarshalAs(UnmanagedType.IUnknown)> ByRef obj As Object)
        Sub Delete(<[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal Name As String)
        Sub NewWorkItem(<[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal TaskName As String, <[In]()> ByRef rclsid As System.Guid, <[In]()> ByRef riid As System.Guid, <Out(), MarshalAs(UnmanagedType.IUnknown)> ByRef obj As Object)
        Sub AddWorkItem(<[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal TaskName As String, <[In](), MarshalAs(UnmanagedType.[Interface])> ByVal WorkItem As ITask)
        Sub IsOfType(<[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal TaskName As String, <[In]()> ByRef riid As System.Guid)
    End Interface

    <Guid("148BD528-A2AB-11CE-B11F-00AA00530503"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Friend Interface IEnumWorkItems
        <PreserveSig()> _
        Function [Next](<[In]()> ByVal RequestCount As UInteger, <Out()> ByRef Names As System.IntPtr, <Out()> ByRef Fetched As UInteger) As Integer
        Sub Skip(<[In]()> ByVal Count As UInteger)
        Sub Reset()
        Sub Clone(<Out(), MarshalAs(UnmanagedType.[Interface])> ByRef EnumWorkItems As IEnumWorkItems)
    End Interface

#If WorkItem Then
	' The IScheduledWorkItem interface is actually never used because ITask inherits all of its
	' methods.  As ITask is the only kind of WorkItem (in 2002) it is the only interface we need.
	<Guid("a6b952f0-a4b1-11d0-997d-00aa006887ec"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
	Friend Interface IScheduledWorkItem
		Sub CreateTrigger(<Out> ByRef NewTriggerIndex As UShort, <Out, MarshalAs(UnmanagedType.[Interface])> ByRef Trigger As ITaskTrigger)
		Sub DeleteTrigger(<[In]> TriggerIndex As UShort)
		Sub GetTriggerCount(<Out> ByRef Count As UShort)
		Sub GetTrigger(<[In]> TriggerIndex As UShort, <Out, MarshalAs(UnmanagedType.[Interface])> ByRef Trigger As ITaskTrigger)
		Sub GetTriggerString(<[In]> TriggerIndex As UShort, ByRef TriggerString As System.IntPtr)
		Sub GetRunTimes(<[In], MarshalAs(UnmanagedType.Struct)> ByRef Begin As SystemTime, <[In], MarshalAs(UnmanagedType.Struct)> ByRef [End] As SystemTime, ByRef Count As UShort, <Out> ByRef TaskTimes As System.IntPtr)
		Sub GetNextRunTime(<[In], Out, MarshalAs(UnmanagedType.Struct)> ByRef NextRun As SystemTime)
		Sub SetIdleWait(<[In]> IdleMinutes As UShort, <[In]> DeadlineMinutes As UShort)
		Sub GetIdleWait(<Out> ByRef IdleMinutes As UShort, <Out> ByRef DeadlineMinutes As UShort)
		Sub Run()
		Sub Terminate()
		Sub EditWorkItem(<[In]> hParent As UInteger, <[In]> dwReserved As UInteger)
		Sub GetMostRecentRunTime(<[In], Out, MarshalAs(UnmanagedType.Struct)> ByRef LastRun As SystemTime)
		Sub GetStatus(<Out, MarshalAs(UnmanagedType.[Error])> ByRef Status As Integer)
		Sub GetExitCode(<Out> ByRef ExitCode As UInteger)
		Sub SetComment(<[In], MarshalAs(UnmanagedType.LPWStr)> Comment As String)
		Sub GetComment(ByRef Comment As System.IntPtr)
		Sub SetCreator(<[In], MarshalAs(UnmanagedType.LPWStr)> Creator As String)
		Sub GetCreator(ByRef Creator As System.IntPtr)
		Sub SetWorkItemData(<[In]> DataLen As UShort, <[In], MarshalAs(UnmanagedType.LPArray, SizeParamIndex := 0, ArraySubType := UnmanagedType.U1)> Data As Byte())
		Sub GetWorkItemData(<Out> ByRef DataLen As UShort, <Out> ByRef Data As System.IntPtr)
		Sub SetErrorRetryCount(<[In]> RetryCount As UShort)
		Sub GetErrorRetryCount(<Out> ByRef RetryCount As UShort)
		Sub SetErrorRetryInterval(<[In]> RetryInterval As UShort)
		Sub GetErrorRetryInterval(<Out> ByRef RetryInterval As UShort)
		Sub SetFlags(<[In]> Flags As UInteger)
		Sub GetFlags(<Out> ByRef Flags As UInteger)
		Sub SetAccountInformation(<[In], MarshalAs(UnmanagedType.LPWStr)> AccountName As String, <[In], MarshalAs(UnmanagedType.LPWStr)> Password As String)
		Sub GetAccountInformation(ByRef AccountName As System.IntPtr)
	End Interface
#End If

    <Guid("148BD524-A2AB-11CE-B11F-00AA00530503"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Friend Interface ITask
        Sub CreateTrigger(<Out()> ByRef NewTriggerIndex As UShort, <Out(), MarshalAs(UnmanagedType.[Interface])> ByRef Trigger As ITaskTrigger)
        Sub DeleteTrigger(<[In]()> ByVal TriggerIndex As UShort)
        Sub GetTriggerCount(<Out()> ByRef Count As UShort)
        Sub GetTrigger(<[In]()> ByVal TriggerIndex As UShort, <Out(), MarshalAs(UnmanagedType.[Interface])> ByRef Trigger As ITaskTrigger)
        Sub GetTriggerString(<[In]()> ByVal TriggerIndex As UShort, ByRef TriggerString As System.IntPtr)
        Sub GetRunTimes(<[In](), MarshalAs(UnmanagedType.Struct)> ByRef Begin As SystemTime, <[In](), MarshalAs(UnmanagedType.Struct)> ByRef [End] As SystemTime, ByRef Count As UShort, <Out()> ByRef TaskTimes As System.IntPtr)
        Sub GetNextRunTime(<[In](), Out(), MarshalAs(UnmanagedType.Struct)> ByRef NextRun As SystemTime)
        Sub SetIdleWait(<[In]()> ByVal IdleMinutes As UShort, <[In]()> ByVal DeadlineMinutes As UShort)
        Sub GetIdleWait(<Out()> ByRef IdleMinutes As UShort, <Out()> ByRef DeadlineMinutes As UShort)
        Sub Run()
        Sub Terminate()
        Sub EditWorkItem(<[In]()> ByVal hParent As UInteger, <[In]()> ByVal dwReserved As UInteger)
        Sub GetMostRecentRunTime(<[In](), Out(), MarshalAs(UnmanagedType.Struct)> ByRef LastRun As SystemTime)
        Sub GetStatus(<Out(), MarshalAs(UnmanagedType.[Error])> ByRef Status As Integer)
        Sub GetExitCode(<Out()> ByRef ExitCode As UInteger)
        Sub SetComment(<[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal Comment As String)
        Sub GetComment(ByRef Comment As System.IntPtr)
        Sub SetCreator(<[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal Creator As String)
        Sub GetCreator(ByRef Creator As System.IntPtr)
        Sub SetWorkItemData(<[In]()> ByVal DataLen As UShort, <[In](), MarshalAs(UnmanagedType.LPArray, SizeParamIndex:=0, ArraySubType:=UnmanagedType.U1)> ByVal Data As Byte())
        Sub GetWorkItemData(<Out()> ByRef DataLen As UShort, <Out()> ByRef Data As System.IntPtr)
        Sub SetErrorRetryCount(<[In]()> ByVal RetryCount As UShort)
        Sub GetErrorRetryCount(<Out()> ByRef RetryCount As UShort)
        Sub SetErrorRetryInterval(<[In]()> ByVal RetryInterval As UShort)
        Sub GetErrorRetryInterval(<Out()> ByRef RetryInterval As UShort)
        Sub SetFlags(<[In]()> ByVal Flags As UInteger)
        Sub GetFlags(<Out()> ByRef Flags As UInteger)
        Sub SetAccountInformation(<[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal AccountName As String, <[In]()> ByVal Password As IntPtr)
        Sub GetAccountInformation(ByRef AccountName As System.IntPtr)
        Sub SetApplicationName(<[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal ApplicationName As String)
        Sub GetApplicationName(ByRef ApplicationName As System.IntPtr)
        Sub SetParameters(<[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal Parameters As String)
        Sub GetParameters(ByRef Parameters As System.IntPtr)
        Sub SetWorkingDirectory(<[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal WorkingDirectory As String)
        Sub GetWorkingDirectory(ByRef WorkingDirectory As System.IntPtr)
        Sub SetPriority(<[In]()> ByVal Priority As UInteger)
        Sub GetPriority(<Out()> ByRef Priority As UInteger)
        Sub SetTaskFlags(<[In]()> ByVal Flags As UInteger)
        Sub GetTaskFlags(<Out()> ByRef Flags As UInteger)
        Sub SetMaxRunTime(<[In]()> ByVal MaxRunTimeMS As UInteger)
        Sub GetMaxRunTime(<Out()> ByRef MaxRunTimeMS As UInteger)
    End Interface

    <Guid("148BD52B-A2AB-11CE-B11F-00AA00530503"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Friend Interface ITaskTrigger
        Sub SetTrigger(<[In](), Out(), MarshalAs(UnmanagedType.Struct)> ByRef Trigger As TaskTrigger)
        Sub GetTrigger(<[In](), Out(), MarshalAs(UnmanagedType.Struct)> ByRef Trigger As TaskTrigger)
        Sub GetTriggerString(ByRef TriggerString As System.IntPtr)
    End Interface
    <Guid("4086658a-cbbb-11cf-b604-00c04fd8d565"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Friend Interface IProvideTaskPage
        Sub GetPage(<[In]()> ByVal tpType As Integer, <[In]()> ByVal fPersistChanges As Boolean, <Out()> ByRef phPage As IntPtr)
    End Interface

    ' ------ Classes ------
    <ComImport(), Guid("148BD52A-A2AB-11CE-B11F-00AA00530503")> _
    Friend Class CTaskScheduler
    End Class

    <ComImport(), Guid("148BD520-A2AB-11CE-B11F-00AA00530503")> _
    Friend Class CTask
    End Class

    Friend Class CoTaskMem
        ''' <summary>
        ''' Many COM methods in ITask, ITaskTrigger, and ITaskScheduler return an LPWStr which should
        ''' should be freed after the string is accessed.  The "out" pointer could be converted  
        ''' to a string during marshalling, but then the memory wouldn't be freed.  Instead
        ''' these entries return an IntPtr--call this method to convert it to a string.
        ''' </summary>
        ''' <param name="lpwstr">A pointer to a unicode string in COM Task Memory, invalid at exit.</param>
        ''' <returns>String value.</returns>
        Public Shared Function LPWStrToString(ByVal lpwstr As System.IntPtr) As String
            Dim ret As String = Marshal.PtrToStringUni(lpwstr)
            Marshal.FreeCoTaskMem(lpwstr)
            Return ret
        End Function
    End Class

End Namespace