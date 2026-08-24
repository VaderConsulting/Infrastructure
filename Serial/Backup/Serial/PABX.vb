''' <summary>
''' Unfinished.
''' </summary>
''' <remarks>Nothing to see here - move along</remarks>
Public Class PABX

#Region " Enums "

    ''' <summary>
    ''' PABX Types emulated by this class
    ''' </summary>
    ''' <remarks>Will eventually contain a list of common PABX types</remarks>
    Public Enum PABXTypes
        Test = 0
        Erricsson = 1
    End Enum

#End Region

#Region " Private variables "

    Private WithEvents m_SerialPort As New System.IO.Ports.SerialPort
    Private m_ReceivedData As String = ""
    Private m_PortName As String = "COM1"
    Private m_PortBaudRate As Int16 = 9600
    Private m_PortParity As IO.Ports.Parity = IO.Ports.Parity.None
    Private m_PortDatabits As Int16 = 8
    Private m_PortStopBits As IO.Ports.StopBits = IO.Ports.StopBits.One
    Private m_PortHandshake As IO.Ports.Handshake = IO.Ports.Handshake.RequestToSend
    Private m_PortRTSEnable As Boolean = True
    Private m_PortDTREnable As Boolean = True

    Private m_PABXType As PABXTypes = PABXTypes.Test

#End Region

End Class
