Imports System
Imports System.IO
Imports System.Runtime.InteropServices
Imports TaskScheduler.TaskSchedulerInterop

Namespace TaskScheduler
    ''' <summary>
    ''' ScheduledTasks represents the a computer's Scheduled Tasks folder.  Using a ScheduledTasks
    ''' object, you can discover the names of the tasks in the folder and you can open a task
    ''' to work with it.  You can also create or delete tasks.
    ''' </summary>
    ''' <remarks>
    ''' A ScheduledTasks object holds a COM interface that can be released by calling <see cref="Dispose"/>.
    ''' </remarks>
    Public Class ScheduledTasks
        Implements IDisposable

        ''' <summary>
        ''' Underlying COM interface.
        ''' </summary>
        Private its As ITaskScheduler = Nothing

        ' --- Contructors ---

        ''' <summary>
        ''' Constructor to use Scheduled tasks of a remote computer identified by a UNC
        ''' name.  The calling process must have administrative privileges on the remote machine. 
        ''' May throw exception if the computer's task scheduler cannot be reached, and may
        ''' give strange results if the argument is not in UNC format.
        ''' </summary>
        ''' <param name="computer">The remote computer's UNC name, e.g. "\\DALLAS".</param>
        ''' <exception cref="ArgumentException">The Task Scheduler could not be accessed.</exception>
        Public Sub New(ByVal computer As String)
            Me.New()
            its.SetTargetComputer(computer)
        End Sub

        ''' <summary>
        ''' Constructor to use Scheduled Tasks of the local computer.
        ''' </summary>
        Public Sub New()
            Dim cts As New CTaskScheduler()
            its = DirectCast(cts, ITaskScheduler)
        End Sub

        ' --- Methods ---

        Private Function GrowStringArray(ByVal s As String(), ByVal n As UInteger) As String()
            Dim sl As String() = New String(s.Length + n) {}
            Dim i As Integer = 0
            While i < s.Length
                sl(i) = s(i)
                System.Math.Max(System.Threading.Interlocked.Increment(i), i - 1)
            End While
            Return sl
        End Function

        ''' <summary>
        ''' Return the names of all scheduled tasks.  The names returned include the file extension ".job";
        ''' methods requiring a task name can take the name with or without the extension.
        ''' </summary>
        ''' <returns>The names in a string array.</returns>
        Public Function GetTaskNames() As String()
            Const TASKS_TO_FETCH As Integer = 10
            Dim taskNames As String() = {}
            Dim nTaskNames As Integer = 0

            Dim ienum As IEnumWorkItems
            its.[Enum](ienum)

            Dim nFetchedTasks As UInteger
            Dim pNames As IntPtr

            While ienum.[Next](TASKS_TO_FETCH, pNames, nFetchedTasks) >= 0 AndAlso nFetchedTasks > 0
                taskNames = GrowStringArray(taskNames, nFetchedTasks)
                While nFetchedTasks > 0
                    Dim name As IntPtr = Marshal.ReadIntPtr(pNames, CInt(System.Threading.Interlocked.Decrement(nFetchedTasks)) * IntPtr.Size)
                    taskNames(System.Math.Max(System.Threading.Interlocked.Increment(nTaskNames), nTaskNames - 1)) = Marshal.PtrToStringUni(name)
                    Marshal.FreeCoTaskMem(name)
                End While
                Marshal.FreeCoTaskMem(pNames)
            End While
            Return taskNames

        End Function

        ''' <summary>
        ''' Creates a new task on the system with the given <paramref name="name" />.
        ''' </summary>
        ''' <remarks>Task names follow normal filename character restrictions.  The name
        ''' will be come the name of the file used to store the task (with .job added).</remarks>
        ''' <param name="name">Name for the new task.</param>
        ''' <returns>Instance of new task.</returns>
        ''' <exception cref="ArgumentException">There is an existing task with the given name.</exception>
        Public Function CreateTask(ByVal name As String) As Task
            Dim tester As Task = OpenTask(name)
            If Not tester Is Nothing Then
                tester.Close()
                Throw New ArgumentException("The task """ + name + """ already exists.")
            End If
            Try
                Dim o As Object = Nothing
                its.NewWorkItem(name, CTaskGuid, ITaskGuid, o)
                Dim iTask As ITask = DirectCast(o, ITask)
                Return New Task(iTask, name)
            Catch
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Deletes the task of the given <paramref name="name" />.
        ''' </summary>
        ''' <remarks>If you delete a task that is open, a subsequent save will throw an
        ''' exception.  You can save to a filename, however, to create a new task.</remarks>
        ''' <param name="name">Name of task to delete.</param>
        ''' <returns>True if the task was deleted, false if the task wasn't found.</returns>
        Public Function DeleteTask(ByVal name As String) As Boolean
            Try
                its.Delete(name)
                Return True
            Catch
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Opens the task with the given <paramref name="name" />.  An open task holds COM interfaces
        ''' which are released by the Task's Close() method.
        ''' </summary>
        ''' <remarks>If the task does not exist, null is returned.</remarks>
        ''' <param name="name">Name of task to open.</param>
        ''' <returns>An instance of a Task, or null if the task name couldn't be found.</returns>
        Public Function OpenTask(ByVal name As String) As Task
            Try
                Dim o As Object = Nothing
                its.Activate(name, ITaskGuid, o)
                Dim iTask As ITask = DirectCast(o, ITask)
                Return New Task(iTask, name)
            Catch
                Return Nothing
            End Try
        End Function


#Region " Implementation of IDisposable "
        ''' <summary>
        ''' The internal COM interface is released.  Further access to the
        ''' object will throw null reference exceptions.
        ''' </summary>
        Public Sub Dispose() Implements System.IDisposable.Dispose
            Marshal.ReleaseComObject(its)
            its = Nothing
        End Sub
#End Region

        ' Two Guids for calls to ITaskScheduler methods Activate(), NewWorkItem(), and IsOfType()
        Friend Shared ITaskGuid As Guid
        Friend Shared CTaskGuid As Guid
        Shared Sub New()
            ITaskGuid = Marshal.GenerateGuidForType(GetType(ITask))
            CTaskGuid = Marshal.GenerateGuidForType(GetType(CTask))
        End Sub

    End Class

End Namespace
