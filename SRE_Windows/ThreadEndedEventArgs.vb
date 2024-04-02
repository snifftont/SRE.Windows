Public Class ThreadEndedEventArgs
    ' ----- Variables -----

    Private m_success As Boolean
    Private m_errorMsg As String


    ' ----- Constructor -----

    Public Sub New(ByVal success As [Boolean], ByVal errorMsg As String)
        m_success = success
        m_errorMsg = errorMsg
    End Sub


    ' ----- Public Properties -----

    Public ReadOnly Property Success() As Boolean
        Get
            Return m_success
        End Get
    End Property

    Public ReadOnly Property ErrorMsg() As String
        Get
            Return m_errorMsg
        End Get
    End Property
End Class
