Imports System
Imports System.Collections

Namespace TaskScheduler
    ''' <summary>
    ''' Deprecated.  Provided for V1 compatibility only. 
    ''' </summary>
    ''' <remarks>
    ''' <p>Presents the Scheduled Tasks folder as a Task Collection. </p> 
    ''' 
    ''' <p>A TaskList is indexed by name rather than position.
    ''' You can't add, remove, or assign tasks in TaskList.  Accessing
    ''' a Task in the list by indexing, or by enumeration, is equivalent to opening a task
    ''' by calling the ScheduledTasks Open() method.</p> 	
    ''' <p><i>Provided for compatibility with version one of the library.  Use of Scheduler
    ''' and TaskList will normally result in COM memory leaks.</i></p>
    ''' </remarks>
    Public Class TaskList
        Implements IEnumerable
        Implements IDisposable
        ''' <summary>
        ''' Scheduled Tasks folder supporting this TaskList.
        ''' </summary>
        Private st As ScheduledTasks = Nothing

        ''' <summary>
        ''' Name of the target computer whose Scheduled Tasks are to be accessed.
        ''' </summary>
        Private nameComputer As String

        ''' <summary>
        ''' Constructors - marked internal so you have to create using Scheduler class.
        ''' </summary>
        Friend Sub New()
            st = New ScheduledTasks()
        End Sub

        Friend Sub New(ByVal computer As String)
            st = New ScheduledTasks(computer)
        End Sub

        ''' <summary>
        ''' Enumerator for <c>TaskList</c>
        ''' </summary>
        Private Class Enumerator
            Implements IEnumerator
            Private outer As ScheduledTasks
            Private nameTask As String()
            Private curIndex As Integer
            Private curTask As Task

            ''' <summary>
            ''' Internal constructor - Only accessable through <see cref="IEnumerable.GetEnumerator"/>
            ''' </summary>
            ''' <param name="st">ScheduledTasks object</param>
            Friend Sub New(ByVal st As ScheduledTasks)
                outer = st
                nameTask = st.GetTaskNames()
                Reset()
            End Sub

            ''' <summary>
            ''' Moves to the next task. See <see cref="IEnumerator.MoveNext"/> for more information.
            ''' </summary>
            ''' <returns>true if next task found, false if no more tasks.</returns>
            Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
                Dim ok As Boolean = System.Threading.Interlocked.Increment(curIndex) < nameTask.Length
                If ok Then
                    curTask = outer.OpenTask(nameTask(curIndex))
                End If
                Return ok
            End Function

            ''' <summary>
            ''' Reset task enumeration. See <see cref="IEnumerator.Reset"/> for more information.
            ''' </summary>
            Public Sub Reset() Implements IEnumerator.Reset
                curIndex = -1
                curTask = Nothing
            End Sub

            ''' <summary>
            ''' Retrieves the current task.  See <see cref="IEnumerator.Current"/> for more information.
            ''' </summary>
            Public ReadOnly Property Current() As Object Implements IEnumerator.Current
                Get
                    Return curTask
                End Get
            End Property
        End Class

        ''' <summary>
        ''' Name of target computer
        ''' </summary>
        Friend Property TargetComputer() As String
            Get
                Return nameComputer
            End Get
            Set(ByVal value As String)
                st.Dispose()
                st = New ScheduledTasks(value)
                nameComputer = value
            End Set
        End Property

        ''' <summary>
        ''' Creates a new task on the system with the supplied <paramref name="name" />.
        ''' </summary>
        ''' <param name="name">Unique display name for the task. If not unique, an ArgumentException will be thrown.</param>
        ''' <returns>Instance of new task</returns>
        ''' <exception cref="ArgumentException">There is already a task of the same name as the one supplied for the new task.</exception>
        Public Function NewTask(ByVal name As String) As Task
            Return st.CreateTask(name)
        End Function

        ''' <summary>
        ''' Deletes the task of the given <paramref name="name" />.
        ''' </summary>
        ''' <param name="name">Name of task to delete</param>
        Public Sub Delete(ByVal name As String)
            st.DeleteTask(name)
        End Sub

        ''' <summary>
        ''' Indexer which retrieves task of given <paramref name="name" />.
        ''' </summary>
        ''' <param name="name">Name of task to retrieve</param>
        Default Public ReadOnly Property Item(ByVal name As String) As Task
            Get
                Return st.OpenTask(name)
            End Get
        End Property

#Region " Implementation of IEnumerable "
        ''' <summary>
        ''' Gets a TaskList enumerator
        ''' </summary>
        ''' <returns>Enumerator for TaskList</returns>
        Public Function GetEnumerator() As System.Collections.IEnumerator Implements IEnumerable.GetEnumerator
            Return New Enumerator(st)
        End Function
#End Region

#Region " Implementation of IDisposable "
        ''' <summary>
        ''' Disposes TaskList
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            st.Dispose()
        End Sub
#End Region

    End Class

End Namespace

