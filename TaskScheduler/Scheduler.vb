Imports System
Imports System.Collections
Imports System.Runtime.InteropServices
Imports TaskScheduler.TaskSchedulerInterop

Namespace TaskScheduler
    ''' <summary>
    ''' Deprecated.  For V1 compatibility only. 
    ''' </summary>
    ''' <remarks>
    ''' <p>Scheduler is just a wrapper around the TaskList class.</p>
    ''' <p><i>Provided for compatibility with version one of the library.  Use of Scheduler
    ''' and TaskList will normally result in COM memory leaks.</i></p>
    ''' </remarks>
    Public Class Scheduler
        ''' <summary>
        ''' Internal field which holds TaskList instance
        ''' </summary>
        Private ReadOnly m_Tasks As TaskList = Nothing

        ''' <summary>
        ''' Creates instance of task scheduler on local machine
        ''' </summary>
        Public Sub New()
            m_Tasks = New TaskList()
        End Sub

        ''' <summary>
        ''' Creates instance of task scheduler on remote machine
        ''' </summary>
        ''' <param name="computer">Name of remote machine</param>
        Public Sub New(ByVal Computer As String)
            m_Tasks = New TaskList()
            TargetComputer = Computer
        End Sub

        ''' <summary>
        ''' Gets/sets name of target computer. Null or emptry string specifies local computer.
        ''' </summary>
        Public Property TargetComputer() As String
            Get
                Return m_Tasks.TargetComputer
            End Get
            Set(ByVal value As String)
                m_Tasks.TargetComputer = value
            End Set
        End Property

        ''' <summary>
        ''' Gets collection of system tasks
        ''' </summary>
        Public ReadOnly Property Tasks() As TaskList
            Get
                Return m_Tasks
            End Get
        End Property

    End Class
End Namespace

