Public Class Reader

#Region " Private variables "

    'Private WithEvents m_AttachedPort As Serial.COM
    'Private m_EOLCharacter As String = vbCrLf
    'Private m_RawData As String = ""
    'Private m_FormattedData As String = ""

#End Region

#Region " Events "

    'Public Event ReceivedData(ByVal Data As String)

#End Region

#Region " Event Handlers "

    '    Private Sub m_AttachedPort_ReceivedDataEvent(ByVal Data As String, ByVal ReceivedByteCount As Int64) Handles m_AttachedPort.ReceivedDataEvent
    '        m_RawData &= Data
    '        While m_RawData.Contains(m_EOLCharacter)
    '            Dim Buffer As String()
    '            Buffer = m_RawData.Split(m_EOLCharacter)
    '            For i As Int64 = 0 To Buffer.LongCount - 2
    '                m_FormattedData = Buffer(i)
    '                RaiseEvent ReceivedData(m_FormattedData)
    '            Next
    '            If Data.EndsWith(Buffer(Buffer.LongCount - 1)) Then
    '                RaiseEvent ReceivedData(m_FormattedData)
    '                m_RawData = ""
    '            Else
    '                m_RawData = m_FormattedData
    '            End If
    '        End While

    '    End Sub

#End Region

End Class
