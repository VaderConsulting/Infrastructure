Imports Core
Imports System.Xml

Public Class Functions

    Private Shared _Common As Common.Singleton

    Public Sub New()
        _Common = Core.Common.Singleton.GetSingleton
    End Sub

    ''' <summary>
    ''' Write an Entry into an XML file on the Innertext 
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <param name="ElementXPath"></param>
    ''' <param name="WriteValue"></param>
    ''' <remarks></remarks>
    Public Sub WriteElement(ByVal Filename As String, ByVal ElementXPath As String, ByVal WriteValue As String)

        Dim TempNode As XmlNode

        Try
            Dim Doc As New XmlDocument

            ' Checks to see the file exists
            If IO.File.Exists(Filename) Then

                ' Opens an XML document
                Doc.Load(Filename)

                ' Gets the root XML node
                Dim rootNode As XmlElement = Doc.DocumentElement
                ' Returns an XMLNode of the selected XPath
                TempNode = rootNode.SelectSingleNode(ElementXPath)

                TempNode.InnerText = WriteValue

                Doc.Save(Filename)

            Else
                ' Error Opening File
                _Common.Logging.WriteEvent("File (" & Filename & ") does not exist", Logging.Functions.LOGGING_LEVEL.ERRORS)
            End If
        Catch ex As XmlException
            ' XmlException
            _Common.Logging.WriteEvent("Error reading XML file (" & Filename & ")", Logging.Functions.LOGGING_LEVEL.ERRORS)
        Catch ex As Exception
            'Unknown Exception
            _Common.Logging.WriteEvent("Unknown error: " & ex.ToString, Logging.Functions.LOGGING_LEVEL.ERRORS)
        End Try

    End Sub

    ''' <summary>
    ''' Write an Entry into an XML file on the Innertext 
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <param name="ElementXPath"></param>
    ''' <remarks></remarks>
    Public Function ReadElement(ByVal Filename As String, ByVal ElementXPath As String) As String

        Dim TempNode As XmlNode
        Dim OutString As String = ""

        Try
            Dim Doc As New XmlDocument

            ' Checks to see the file exists
            If IO.File.Exists(Filename) Then

                ' Opens an XML document
                Doc.Load(Filename)

                ' Gets the root XML node
                Dim rootNode As XmlElement = Doc.DocumentElement
                ' Returns an XMLNode of the selected XPath
                TempNode = rootNode.SelectSingleNode(ElementXPath)

                OutString = TempNode.InnerText

            Else
                ' Error Opening File
                _Common.Logging.WriteEvent("File (" & Filename & ") does not exist", Logging.Functions.LOGGING_LEVEL.ERRORS)
            End If
        Catch ex As XmlException
            ' XmlException
            _Common.Logging.WriteEvent("Error reading XML file (" & Filename & ")", Logging.Functions.LOGGING_LEVEL.ERRORS)
        Catch ex As Exception
            'Unknown Exception
            _Common.Logging.WriteEvent("Unknown error: " & ex.ToString, Logging.Functions.LOGGING_LEVEL.ERRORS)
        End Try

        Return OutString

    End Function

    ''' <summary>
    ''' Finds an Element from a Specific Xpath and returns and XMLNodeList
    ''' </summary>
    ''' <param name="FileName"></param>
    ''' <param name="ElementXPath"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FindElement(ByVal Filename As String, ByVal ElementXPath As String) As XmlNodeList

        Dim XMLNodeListReturn As XmlNodeList = Nothing

        Try
            Dim Doc As New XmlDocument

            ' Checks to see the file exists
            If IO.File.Exists(Filename) Then

                ' Opens an XML document
                Doc.Load(Filename)

                ' Gets the root XML node
                Dim rootNode As XmlElement = Doc.DocumentElement
                ' Returns an XMLNodeList of the selected XPath
                XMLNodeListReturn = rootNode.SelectNodes(ElementXPath)

            Else
                ' Error Opening File
                _Common.Logging.WriteEvent("File (" & Filename & ") does not exist", Logging.Functions.LOGGING_LEVEL.ERRORS)
            End If
        Catch ex As XmlException
            ' XmlException
            _Common.Logging.WriteEvent("Error reading XML file (" & Filename & ")", Logging.Functions.LOGGING_LEVEL.ERRORS)
        Catch ex As Exception
            'Unknown Exception
            _Common.Logging.WriteEvent("Unknown error: " & ex.ToString, Logging.Functions.LOGGING_LEVEL.ERRORS)
        End Try

        Return XMLNodeListReturn

    End Function

End Class
