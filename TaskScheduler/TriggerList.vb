Imports System
Imports System.Collections
Imports TaskScheduler.TaskSchedulerInterop

Namespace TaskScheduler

    ''' <summary>
    ''' TriggerList is a collection of Triggers.  Every Task has a TriggerList that is
    ''' created and destroyed automatically along with the Task.  There are no public constructors. 
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' A TriggerList can be empty, and indeed a newly created Task has an empty list. 
    ''' It's not clear how the system handles a task with no triggers, however.</para>
    ''' <para>
    ''' TriggerList implements IList and behaves like other indexable collections with one limitation:   
    ''' You can't insert a Trigger at a position.  <c>Insert()</c> throws NotImplementedException.  This
    ''' restriction is based on the underlying API. </para>
    ''' </remarks>
    Public Class TriggerList
        Implements IList
        Implements IDisposable
        ' Internal COM interface to access task that this list is associated with.
        Private iTask As TaskSchedulerInterop.ITask
        ' Trigger objects store in an ArrayList
        Private oTriggers As ArrayList

        ''' <summary>
        ''' Internal constructor creates TriggerList using an ITask interface to initialize.
        ''' </summary>
        ''' <param name="iTask">Instance of an ITask.</param>
        Friend Sub New(ByVal iTask As TaskSchedulerInterop.ITask)
            Me.iTask = iTask
            Dim cnt As UShort = 0
            iTask.GetTriggerCount(cnt)
            oTriggers = New ArrayList(cnt + 5)
            'Allow for five additional entries without growing base array
            Dim i As Integer = 0
            While i < cnt
                Dim iTaskTrigger As TaskSchedulerInterop.ITaskTrigger = Nothing
                iTask.GetTrigger(CShort(i), iTaskTrigger)
                oTriggers.Add(Trigger.CreateTrigger(iTaskTrigger))
                System.Math.Max(System.Threading.Interlocked.Increment(i), i - 1)
            End While
        End Sub

        ''' <summary>
        ''' Enumerator for TriggerList; implements IEnumerator interface.
        ''' </summary>
        Private Class Enumerator
            Implements IEnumerator
            Private outer As TriggerList
            Private currentIndex As Integer

            ''' <summary>
            ''' Internal constructor - Only accessible through <see cref="IEnumerator"/>.
            ''' </summary>
            ''' <param name="outer">Instance of a TriggerList.</param>
            Friend Sub New(ByVal outer As TriggerList)
                Me.outer = outer
                Reset()
            End Sub

            ''' <summary>
            ''' Moves to the next trigger. See <see cref="IEnumerator.MoveNext"/> for more information.
            ''' </summary>
            ''' <returns>False if there is no next trigger.</returns>
            Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
                Return System.Threading.Interlocked.Increment(currentIndex) < outer.oTriggers.Count
            End Function

            ''' <summary>
            ''' Reset trigger enumeration. See <see cref="IEnumerator.Reset"/> for more information.
            ''' </summary>
            Public Sub Reset() Implements IEnumerator.Reset
                currentIndex = -1
            End Sub

            ''' <summary>
            ''' Retrieves the current trigger.  See <see cref="IEnumerator.Current"/> for more information.
            ''' </summary>
            Public ReadOnly Property Current() As Object Implements IEnumerator.Current
                Get
                    Return outer.oTriggers(currentIndex)
                End Get
            End Property
        End Class

#Region " Implementation of IList "
        ''' <summary>
        ''' Removes the trigger at a specified index.
        ''' </summary>
        ''' <param name="index">Index of trigger to remove.</param>
        ''' <exception cref="ArgumentOutOfRangeException">Index out of range.</exception>
        Public Sub RemoveAt(ByVal index As Integer) Implements IList.RemoveAt
            If index >= Count Then
                Throw New ArgumentOutOfRangeException("index", index, "Failed to remove Trigger. Index out of range.")
            End If
            DirectCast(oTriggers(index), Trigger).Unbind()
            'releases resources in the trigger
            oTriggers.RemoveAt(index)
            'Remove the Trigger object from the array representing the list
            iTask.DeleteTrigger(DirectCast(index, UShort))
            'Remove the trigger from the Task Scheduler
        End Sub

        ''' <summary>
        ''' Not implemented; throws NotImplementedException.
        ''' If implemented, would insert a trigger at a specified index. 
        ''' </summary>
        ''' <param name="index">Index to insert trigger.</param>
        ''' <param name="value">Value of trigger to insert.</param>
        Sub Insert(ByVal index As Integer, ByVal value As Object) Implements IList.Insert
            Throw New NotImplementedException("TriggerList does not support Insert().")
        End Sub

        ''' <summary>
        ''' Removes the trigger from the collection.  If the trigger is not in
        ''' the collection, nothing happens.  (No exception.)
        ''' </summary>
        ''' <param name="trigger">Trigger to remove.</param>
        Public Sub Remove(ByVal trigger As Trigger)
            Dim i As Integer = IndexOf(trigger)
            If i <> -1 Then
                RemoveAt(i)
            End If
        End Sub

        ''' <summary>
        ''' IList.Remove implementation.
        ''' </summary>
        Sub Remove(ByVal value As Object) Implements IList.Remove
            Remove(TryCast(value, Trigger))
        End Sub

        ''' <summary>
        ''' Test to see if trigger is part of the collection.
        ''' </summary>
        ''' <param name="trigger">Trigger to find.</param>
        ''' <returns>true if trigger found in collection.</returns>
        Public Function Contains(ByVal trigger As Trigger) As Boolean
            Return (IndexOf(trigger) <> -1)
        End Function

        ''' <summary>
        ''' IList.Contains implementation.
        ''' </summary>
        Function Contains(ByVal value As Object) As Boolean Implements IList.Contains
            Return Contains(TryCast(value, Trigger))
        End Function

        ''' <summary>
        ''' Remove all triggers from collection.
        ''' </summary>
        Public Sub Clear() Implements IList.Clear
            Dim i As Integer = Count - 1
            While i >= 0
                RemoveAt(i)
                System.Math.Max(System.Threading.Interlocked.Decrement(i), i + 1)
            End While
        End Sub

        ''' <summary>
        ''' Returns the index of the supplied Trigger.
        ''' </summary>
        ''' <param name="trigger">Trigger to find.</param>
        ''' <returns>Zero based index in collection, -1 if not a member.</returns>
        Public Function IndexOf(ByVal trigger As Trigger) As Integer
            Dim i As Integer = 0
            While i < Count
                If Me(i).Equals(trigger) Then
                    Return i
                End If
                System.Math.Max(System.Threading.Interlocked.Increment(i), i - 1)
            End While
            Return -1
        End Function

        ''' <summary>
        ''' IList.IndexOf implementation.
        ''' </summary>
        Function IndexOf(ByVal value As Object) As Integer Implements IList.IndexOf
            Return IndexOf(TryCast(value, Trigger))
        End Function

        ''' <summary>
        ''' Add the supplied Trigger to the collection.  The Trigger to be added must be unbound,
        ''' i.e. it must not be a current member of a TriggerList--this or any other.
        ''' </summary>
        ''' <param name="trigger">Trigger to add.</param>
        ''' <returns>Index of added trigger.</returns>
        ''' <exception cref="ArgumentException">Trigger being added is already bound.</exception>
        Public Function Add(ByVal trigger As Trigger) As Integer
            ' if trigger is already bound a list throw an exception
            If trigger.Bound Then
                Throw New ArgumentException("A Trigger cannot be added if it is already in a list.")
            End If
            ' Add a trigger to the task for this TaskList
            Dim iTrigger As TaskSchedulerInterop.ITaskTrigger = Nothing
            Dim index As UShort
            iTask.CreateTrigger(index, iTrigger)
            ' Add the Trigger to the TaskList
            trigger.Bind(iTrigger)
            Dim index2 As Integer = oTriggers.Add(trigger)
            ' Verify index is the same in task and in list
            If index2 <> CInt(index) Then
                Throw New ApplicationException("Assertion Failure")
            End If
            Return CInt(index)
        End Function

        ''' <summary>
        ''' IList.Add implementation.
        ''' </summary>
        Function Add(ByVal value As Object) As Integer Implements IList.Add
            Return Add(TryCast(value, Trigger))
        End Function

        ''' <summary>
        ''' Gets read-only state of collection. Always false for TriggerLists.
        ''' </summary>
        Public ReadOnly Property IsReadOnly() As Boolean Implements IList.IsReadOnly
            Get
                Return False
            End Get
        End Property

        ' TODO:  FIX THIS
        '''' <summary>
        '''' Access the Trigger at a specified index.  Assigning to a TriggerList element requires
        '''' the value to unbound.  The previous list element becomes unbound and lost,
        '''' while the newly assigned Trigger becomes bound in its place.
        '''' </summary>
        '''' <exception cref="ArgumentOutOfRangeException">Collection index out of range.</exception>
        'Default Public Property Item(ByVal index As Integer) As Object Implements IList.Item
        '    Get
        '        If index >= Count Then
        '            Throw New ArgumentOutOfRangeException("index", index, "TriggerList collection")
        '        End If
        '        Return DirectCast(oTriggers(index), Trigger)
        '    End Get
        '    Set(ByVal value As Trigger)
        '        If index >= Count Then
        '            Throw New ArgumentOutOfRangeException("index", index, "TriggerList collection")
        '        End If
        '        Dim previous As Trigger = DirectCast(oTriggers(index), Trigger)
        '        value.Bind(previous)
        '        oTriggers(index) = value
        '    End Set
        'End Property

        ''' <summary>
        ''' IList.this[int] implementation.
        ''' </summary>
        Default Property Item(ByVal index As Integer) As Object Implements IList.Item
            Get
                Return Me(index)
            End Get
            Set(ByVal value As Object)
                Me(index) = (TryCast(value, Trigger))
            End Set
        End Property

        ''' <summary>
        ''' Returns whether collection is a fixed size. Always returns false for TriggerLists.
        ''' </summary>
        Public ReadOnly Property IsFixedSize() As Boolean Implements IList.IsFixedSize
            Get
                Return False
            End Get
        End Property
#End Region

#Region " Implementation of ICollection "
        ''' <summary>
        ''' Gets the number of Triggers in the collection.
        ''' </summary>
        Public ReadOnly Property Count() As Integer Implements IList.Count
            Get
                Return oTriggers.Count
            End Get
        End Property

        ''' <summary>
        ''' Copies all the Triggers in the collection to an array, beginning at the given index. 
        ''' The Triggers assigned to the array are cloned from the originals, implying they are
        ''' unbound copies.  (Can't tell if cloning is the intended semantics for this ICollection method,
        ''' but it seems a good choice for TriggerLists.) 
        ''' </summary>
        ''' <param name="array">Array to copy triggers into.</param>
        ''' <param name="index">Index at which to start copying.</param>
        Public Sub CopyTo(ByVal array As System.Array, ByVal index As Integer) Implements IList.CopyTo
            If oTriggers.Count > array.Length - index Then
                Throw New ArgumentException("Array has insufficient space to copy the collection.")
            End If
            Dim i As Integer = 0
            While i < oTriggers.Count
                array.SetValue((DirectCast(oTriggers(i), Trigger)).Clone(), index + i)
                System.Math.Max(System.Threading.Interlocked.Increment(i), i - 1)
            End While
        End Sub

        ''' <summary>
        ''' Returns synchronizable state. Always false since the Task Scheduler is not
        ''' thread safe.
        ''' </summary>
        Public ReadOnly Property IsSynchronized() As Boolean Implements IList.IsSynchronized
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Gets the root object for synchronization. Always null since TriggerLists aren't synchronized.
        ''' </summary>
        Public ReadOnly Property SyncRoot() As Object Implements IList.SyncRoot
            Get
                Return Nothing
            End Get
        End Property
#End Region

#Region " Implementation of IEnumerable "
        ''' <summary>
        ''' Gets a TriggerList enumerator.
        ''' </summary>
        ''' <returns>Enumerator for TriggerList.</returns>
        Public Function GetEnumerator() As System.Collections.IEnumerator Implements IList.GetEnumerator
            Return New Enumerator(Me)
        End Function
#End Region

#Region " Implementation of IDisposable "
        ''' <summary>
        ''' Unbinds and Disposes all the Triggers in the collection, releasing the com interfaces they hold.
        ''' Destroys the internal private pointer to the ITask com interface, but does not 
        ''' specifically release the interface because it is also in the containing task.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            For Each o As Object In oTriggers
                DirectCast(o, Trigger).Unbind()
            Next
            oTriggers = Nothing
            iTask = Nothing
        End Sub
#End Region
    End Class

End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Built and maintained by Todd Anglin and Telerik
'=======================================================
