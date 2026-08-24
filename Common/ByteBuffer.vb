Imports System.Runtime.Serialization

<Serializable()> _
Public Class ByteBuffer

#Region " Private variables "

    Private _Contents As String() = {}
    Private _FixedLength As Boolean = False
    Private _MaxLength As Int32 = 0
    Private _ReadPosition As Int32 = 0
    Private _WritePosition As Int32 = 0
    Private _Length As Int32 = 0
    Private _RepeatingCharacter As Char = Nothing
    Private _RepeatingCharacterCount As Int64 = 0
    Private _RepeatWindow As Int64 = 512

#End Region

#Region " Properties "

    ''' <summary>
    ''' Gets or sets a value indicating whether the buffer is fixed-length.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FixedLength() As Boolean
        Get
            Return _FixedLength
        End Get
        Set(ByVal value As Boolean)
            _FixedLength = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets a 32 bit Integer that represents the maximum length of the buffer.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MaxLength() As Int32
        Get
            Return _MaxLength
        End Get
        Set(ByVal value As Int32)
            _MaxLength = value
        End Set
    End Property

    ''' <summary>
    ''' Gets a 32 bit Integer that represents the current length of the buffer.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Length() As Int32
        Get
            Return _Length
        End Get
    End Property

    ''' <summary>
    ''' Gets a System.String that represents the raw contents of the buffer.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Contents() As String
        Get
            Return String.Concat(_Contents)
        End Get
    End Property

    ''' <summary>
    ''' Gets a 32 bit Integer that represents the current write position within the buffer.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property WritePosition() As Int32
        Get
            Return _WritePosition
        End Get

    End Property

    ''' <summary>
    ''' Gets or sets a 64 bit integer to use for the RepeatWindow
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RepeatWindow() As Int64
        Get
            Return _RepeatWindow
        End Get
        Set(ByVal value As Int64)
            If value <= _MaxLength Then
                _RepeatWindow = value
            Else
                _RepeatWindow = _MaxLength
            End If
        End Set
    End Property

    Public ReadOnly Property RepeatingCharacterCount() As Int64
        Get
            Return _RepeatingCharacterCount
        End Get
    End Property

    Public ReadOnly Property RepeatingCharacter() As Char
        Get
            Return _RepeatingCharacter
        End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating if the content is composed only of repeating characters
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Repeating() As Boolean
        Get
            'If _MaxLength = _Length Then
            If _RepeatingCharacterCount >= _RepeatWindow Then
                Return True
            Else
                Return False
            End If
            'Else
            'Return False
            'End If
        End Get
    End Property

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Creates and returns a new ByteBuffer object.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        _FixedLength = False
    End Sub

    ''' <summary>
    ''' Creates and returns a new ByteBuffer object with a fixed length.
    ''' </summary>
    ''' <param name="MaxLength">The maximum length in bytes of the buffer.</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal MaxLength As Int32)
        _FixedLength = True
        _MaxLength = MaxLength
        Array.Resize(_Contents, MaxLength)
    End Sub

#End Region

#Region " Public methods "

    ''' <summary>
    ''' Adds a System.String to the end of the buffer.
    ''' </summary>
    ''' <param name="Bytes">The System.String to add to the end of the buffer</param>
    ''' <remarks></remarks>
    Public Sub Add(ByVal Bytes As String)
        If _FixedLength Then
            For ReadPointer As Int32 = 0 To Bytes.Length - 1
                AddToContents(Bytes(ReadPointer))
            Next
        Else
            Array.Resize(_Contents, _Contents.Length + Bytes.Length)
            For ReadPointer As Int32 = 0 To Bytes.Length - 1
                _Contents(ReadPointer + _WritePosition) = Bytes(ReadPointer)
            Next
            _WritePosition += Bytes.Length
        End If
    End Sub

    ''' <summary>
    ''' Converts this instance to its System.String representation.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function ToString() As String
        Dim Output As String = ""

        If _FixedLength Then
            _ReadPosition = _WritePosition
        Else
            _ReadPosition = 0
        End If

        Do
            Output &= GetFromContents()
        Loop Until Output.Length = _Length '_Contents.Length

        Return Output
    End Function

    ''' <summary>
    ''' Removes all bytes from the buffer.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Clear()
        Array.Clear(_Contents, 0, _Contents.Length)
        _ReadPosition = 0
        _WritePosition = 0
        _Length = 0
    End Sub

    ''' <summary>
    ''' Returns a value indicating whether the specified System.String object occurs in this buffer.
    ''' </summary>
    ''' <param name="value">The System.String to seek.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Contains(ByVal value As String) As Boolean
        Return ToString.Contains(value)
    End Function

    ''' <summary>
    ''' Reports the index of the first occurance of the specified System.String in this buffer.
    ''' </summary>
    ''' <param name="value">The System.String to seek.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IndexOf(ByVal value As String) As Integer
        Return ToString.IndexOf(value)
    End Function

    ''' <summary>
    ''' Determines whether the start of this buffer matches the specified System.String.
    ''' </summary>
    ''' <param name="value">A System.String to compare to.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function StartsWith(ByVal value As String) As Boolean
        Return ToString.StartsWith(value)
    End Function

    ''' <summary>
    ''' Determines whether the end of this buffer matches the specified System.String.
    ''' </summary>
    ''' <param name="value">A System.String to compare to.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function EndsWith(ByVal value As String) As Boolean
        Return ToString.EndsWith(value)
    End Function

    ''' <summary>
    ''' Retrieves a substring from this buffer.  The substring starts at a specified character position.
    ''' </summary>
    ''' <param name="Startindex">The zero-based starting character position of a substring in this buffer.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SubString(ByVal Startindex As Integer) As String
        Return ToString.Substring(Startindex)
    End Function

    ''' <summary>
    ''' Retrieves a substring from this buffer.  The substring starts at a specified character position and has a specified length.
    ''' </summary>
    ''' <param name="startIndex">The zero-based starting character position of a substring in this buffer.</param>
    ''' <param name="length">The number of characters in the substring.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SubString(ByVal startIndex As Integer, ByVal length As Integer) As String
        Return ToString.Substring(startIndex, length)
    End Function

    ''' <summary>
    ''' Reports the index position of the last occurance of a specified System.String within this instance.
    ''' </summary>
    ''' <param name="value">The System.String to seek.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LastIndexOf(ByVal value As String) As Integer
        Return ToString.LastIndexOf(value)
    End Function

    ''' <summary>
    ''' Reports the index position of the last occurance of a specified System.String within this instance.
    ''' The search starts at a specified character position.
    ''' </summary>
    ''' <param name="value">The System.String to seek.</param>
    ''' <param name="startIndex">The search starting position.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LastIndexOf(ByVal value As String, ByVal startIndex As Integer) As Integer
        Return ToString.LastIndexOf(value, startIndex)
    End Function

    ''' <summary>
    ''' Reports the index position of the last occurance of a specified System.String within this instance.
    ''' The search starts at a specified character position and examines a specified number of character positions.
    ''' </summary>
    ''' <param name="value">The System.String to seek.</param>
    ''' <param name="startIndex">The search starting position.</param>
    ''' <param name="count">The number of character positions to examine.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LastIndexOf(ByVal value As String, ByVal startIndex As Integer, ByVal count As Integer) As Integer
        Return ToString.LastIndexOf(value, startIndex, count)
    End Function

    ''' <summary>
    ''' Returns a copy of this buffer converted to uppercase, using the casing rules of the current culture.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ToUpper() As String
        Return ToString.ToUpper
    End Function

    ''' <summary>
    ''' Returns a copy of this buffer converted to lowercase, using the casing rules of the current culture.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ToLower() As String
        Return ToString.ToLower
    End Function

#End Region

#Region " Private methods "

    ' Data is always ONE CHARACTER only.
    Private Sub AddToContents(ByVal Data As Char)
        _Contents(_WritePosition) = Data
        _WritePosition += 1
        If _Length < MaxLength Then _Length += 1
        If _WritePosition >= _MaxLength Then _WritePosition = 0

        ' Repeating character detection
        If _RepeatingCharacter = Data Then
            _RepeatingCharacterCount += 1
            If _RepeatingCharacterCount > _Length Then _RepeatingCharacterCount = _Length
        Else
            _RepeatingCharacterCount = 0
        End If

        _RepeatingCharacter = Data
    End Sub

    Private Function GetFromContents() As String
        Dim Output As String = ""

        Output = _Contents(_ReadPosition)

        _ReadPosition += 1
        If _ReadPosition >= _Length Then _ReadPosition = 0 '_Contents.Length Then _ReadPosition = 0

        Return Output
    End Function

#End Region

End Class
