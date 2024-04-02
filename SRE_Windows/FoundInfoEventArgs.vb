Imports System.IO

Public Class FoundInfoEventArgs
    ' ----- Variables -----

    Private m_info As FileSystemInfo


    ' ----- Constructor -----

    Public Sub New(ByVal info As FileSystemInfo)
        m_info = info
    End Sub


    ' ----- Public Properties -----

    Public ReadOnly Property Info() As FileSystemInfo
        Get
            Return m_info
        End Get
    End Property
End Class
