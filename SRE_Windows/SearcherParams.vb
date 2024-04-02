Imports System.Text

Public Class SearcherParams
    ' ----- Variables -----

    Private m_searchDir As String
    Private m_includeSubDirsChecked As Boolean
    Private m_fileNames As List(Of String)
    Private m_newerThanChecked As Boolean
    Private m_newerThanDateTime As DateTime
    Private m_olderThanChecked As Boolean
    Private m_olderThanDateTime As DateTime
    Private m_containingChecked As Boolean
    Private m_containingText As String
    Private m_encoding As Encoding


    ' ----- Constructor -----

    Public Sub New(ByVal searchDir As [String], ByVal includeSubDirsChecked As [Boolean], ByVal fileNames As List(Of [String]), ByVal newerThanChecked As [Boolean], ByVal newerThanDateTime As DateTime, ByVal olderThanChecked As [Boolean], _
     ByVal olderThanDateTime As DateTime, ByVal containingChecked As [Boolean], ByVal containingText As [String], ByVal encoding As Encoding)
        m_searchDir = searchDir
        m_includeSubDirsChecked = includeSubDirsChecked
        m_fileNames = fileNames
        m_newerThanChecked = newerThanChecked
        m_newerThanDateTime = newerThanDateTime
        m_olderThanChecked = olderThanChecked
        m_olderThanDateTime = olderThanDateTime
        m_containingChecked = containingChecked
        m_containingText = containingText
        m_encoding = encoding
    End Sub


    ' ----- Public Properties -----

    Public ReadOnly Property SearchDir() As String
        Get
            Return m_searchDir
        End Get
    End Property

    Public ReadOnly Property IncludeSubDirsChecked() As Boolean
        Get
            Return m_includeSubDirsChecked
        End Get
    End Property

    Public ReadOnly Property FileNames() As List(Of String)
        Get
            Return m_fileNames
        End Get
    End Property

    Public ReadOnly Property NewerThanChecked() As Boolean
        Get
            Return m_newerThanChecked
        End Get
    End Property

    Public ReadOnly Property NewerThanDateTime() As DateTime
        Get
            Return m_newerThanDateTime
        End Get
    End Property

    Public ReadOnly Property OlderThanChecked() As Boolean
        Get
            Return m_olderThanChecked
        End Get
    End Property

    Public ReadOnly Property OlderThanDateTime() As DateTime
        Get
            Return m_olderThanDateTime
        End Get
    End Property

    Public ReadOnly Property ContainingChecked() As Boolean
        Get
            Return m_containingChecked
        End Get
    End Property

    Public ReadOnly Property ContainingText() As String
        Get
            Return m_containingText
        End Get
    End Property

    Public ReadOnly Property Encoding() As Encoding
        Get
            Return m_encoding
        End Get
    End Property
End Class
