Imports System.Collections.Generic
Imports System.Text
Imports System.Threading
Imports System.IO
Imports EPocalipse.IFilter



Public Class Searcher
    ' ----- Asynchronous Events -----

    Public Delegate Sub FoundInfoEventHandler(ByVal e As FoundInfoEventArgs)
    Public Shared Event FoundInfo As FoundInfoEventHandler

    Public Delegate Sub ThreadEndedEventHandler(ByVal e As ThreadEndedEventArgs)
    Public Shared Event ThreadEnded As ThreadEndedEventHandler
    Private Shared searchnew As Boolean = False
    Private Shared dtsearch As DataTable
    Private Shared startno As Integer
    Private Shared endno As Integer
    Private Shared countrecord As Integer
    ' ----- Variables -----

    Private Shared m_thread As Thread = Nothing
    Private Shared m_stop As [Boolean] = False
    Private Shared m_pars As SearcherParams = Nothing
    Private Shared m_containingBytes As [Byte]() = Nothing

    ' ----- Public Methods -----

    Public Function Start(ByVal pars As SearcherParams, ByVal start1 As Integer, ByVal end1 As Integer) As DataTable
        Dim success As [Boolean] = False
        Dim dt As New DataTable
        startno = start1
        endno = end1
        If m_thread Is Nothing Then
            ' Perform a reset of all variables,
            ' to ensure that the state of the searcher is the same on every new start:
            ResetVariables()
            ' Remember the parameters:
            m_pars = pars
            ' Start searching for FileSystemInfos that match the parameters:
            dt = Search(start1, end1)
            ' m_thread = New Thread(New ThreadStart(AddressOf SearchThread))

            ' m_thread.Start()
            success = True
        End If

        Return dt
    End Function

    Public Shared Sub [Stop]()
        ' Stop the thread by setting a flag:
        m_stop = True
    End Sub


    ' ----- Private Methods -----

    Private Shared Sub ResetVariables()
        m_thread = Nothing
        m_stop = False
        m_pars = Nothing
        m_containingBytes = Nothing
    End Sub

    Private Shared Sub SearchThread()
        Dim success As [Boolean] = True
        Dim errorMsg As [String] = ""
        searchnew = True
        ' Search for FileSystemInfos that match the parameters:
        If (m_pars.SearchDir.Length >= 3) AndAlso (Directory.Exists(m_pars.SearchDir)) Then
            If m_pars.FileNames.Count > 0 Then
                ' Convert the string to search for into bytes if necessary:
                If m_pars.ContainingChecked Then
                    If m_pars.ContainingText <> "" Then
                        Try
                            m_containingBytes = m_pars.Encoding.GetBytes(m_pars.ContainingText)
                        Catch generatedExceptionName As Exception
                            success = False
                            errorMsg = "The string" & vbCr & vbLf & Convert.ToString(m_pars.ContainingText) & vbCr & vbLf & "cannot be converted into bytes."
                        End Try
                    Else
                        success = False
                        errorMsg = "The string to search for must not be empty."
                    End If
                End If

                If success Then
                    ' Get the directory info for the search directory:
                    Dim dirInfo As DirectoryInfo = Nothing
                    Try
                        dirInfo = New DirectoryInfo(m_pars.SearchDir)
                    Catch ex As Exception
                        success = False
                        errorMsg = ex.Message
                    End Try

                    If success Then
                        ' Search the directory (maybe recursively),
                        ' and raise events if something was found:
                        '  Dim dt As DataTable = SearchDirectory(dirInfo)
                        '  Dim a111 As Integer = dt.Rows.Count
                    End If
                End If
            Else
                success = False
                errorMsg = "Please enter one or more filenames to search for."
            End If
        Else
            success = False
            errorMsg = "The directory" & vbCr & vbLf & Convert.ToString(m_pars.SearchDir) & vbCr & vbLf & "does not exist."
        End If

        ' Remember the thread has ended:
        m_thread = Nothing

        ' Raise an event:
        RaiseEvent ThreadEnded(New ThreadEndedEventArgs(success, errorMsg))
    End Sub
    Private Shared Function Search(ByVal start1 As Integer, ByVal end1 As Integer) As DataTable
        Dim dt As New DataTable
        Dim success As [Boolean] = True
        Dim errorMsg As [String] = ""
        searchnew = True
        ' Search for FileSystemInfos that match the parameters:
        If (m_pars.SearchDir.Length >= 3) AndAlso (Directory.Exists(m_pars.SearchDir)) Then
            If m_pars.FileNames.Count > 0 Then
                ' Convert the string to search for into bytes if necessary:
                If m_pars.ContainingChecked Then
                    If m_pars.ContainingText <> "" Then
                        Try
                            m_containingBytes = m_pars.Encoding.GetBytes(m_pars.ContainingText)
                        Catch generatedExceptionName As Exception
                            success = False
                            errorMsg = "The string" & vbCr & vbLf & Convert.ToString(m_pars.ContainingText) & vbCr & vbLf & "cannot be converted into bytes."
                        End Try
                    Else
                        success = False
                        errorMsg = "The string to search for must not be empty."
                    End If
                End If

                If success Then
                    ' Get the directory info for the search directory:
                    Dim dirInfo As DirectoryInfo = Nothing
                    Try
                        dirInfo = New DirectoryInfo(m_pars.SearchDir)
                    Catch ex As Exception
                        success = False
                        errorMsg = ex.Message
                    End Try

                    If success Then
                        ' Search the directory (maybe recursively),
                        ' and raise events if something was found:
                        countrecord = -1
                        dt = SearchDirectory(dirInfo, start1, end1)
                        Dim a111 As Integer = dt.Rows.Count
                    End If
                End If
            Else
                success = False
                errorMsg = "Please enter one or more filenames to search for."
            End If
        Else
            success = False
            errorMsg = "The directory" & vbCr & vbLf & Convert.ToString(m_pars.SearchDir) & vbCr & vbLf & "does not exist."
        End If

        ' Remember the thread has ended:
        m_thread = Nothing
        Return dt
    End Function
    Private Shared Function SearchDirectory(ByVal dirInfo As DirectoryInfo, ByVal start1 As Integer, ByVal end1 As Integer) As DataTable
        If searchnew = True Then
            searchnew = False
            dtsearch = New DataTable()
            dtsearch.Columns.Add("fileName")
            dtsearch.Columns.Add("fullname")
            dtsearch.Columns.Add("filesize")
            dtsearch.Columns.Add("createdate")
            dtsearch.Columns.Add("modifydate")
            dtsearch.Columns.Add("topicname")
        End If
        If Not m_stop Then
            Dim directorypath As String = dirInfo.FullName
            Try
                Dim infos As FileSystemInfo() = dirInfo.GetFileSystemInfos("*")
                For Each info As FileSystemInfo In infos
                    If m_stop Then
                        Exit For
                    End If
                    Try
                        Dim reader As TextReader = New FilterReader(info.FullName)
                        Using reader
                            Dim text As String = reader.ReadToEnd()
                            'label1.Text = "Text loaded from " + openFileDialog1.FileName;
                            Dim s1 As String = text.ToLower().ToString()
                            Dim positionOfDream As Integer = 0
                            positionOfDream = s1.IndexOf(m_pars.ContainingText)
                            If positionOfDream > -1 Then
                                If info.LastAccessTime >= m_pars.NewerThanDateTime Then
                                    countrecord = countrecord + 1
                                    If countrecord >= start1 And countrecord <= end1 Then
                                        ' RaiseEvent FoundInfo(New FoundInfoEventArgs(info))
                                        Dim drow As DataRow = dtsearch.NewRow()
                                        drow(0) = info.Name
                                        drow(1) = info.FullName                            'file full name
                                        drow(2) = GetBytesStringKB(DirectCast(info, FileInfo).Length)  'file full name
                                        drow(3) = info.LastWriteTime.ToString()            'modify date
                                        drow(4) = info.CreationTime.ToString()             'create date
                                        drow(5) = m_pars.ContainingText                                'search topic name
                                        dtsearch.Rows.Add(drow)

                                    End If
                                    If countrecord = end1 Then
                                        Return dtsearch
                                    End If
                                End If
                            End If
                        End Using
                    Catch ex As Exception
                    End Try
                Next
                If True Then
                    Dim subDirInfos As DirectoryInfo() = dirInfo.GetDirectories()
                    For Each subDirInfo As DirectoryInfo In subDirInfos
                        If m_stop Then
                            Exit For
                        End If
                        ' Recursion:
                        SearchDirectory(subDirInfo, start1, end1)

                    Next
                End If
            Catch generatedExceptionName As Exception
            End Try
        End If
        Return dtsearch
    End Function

    Public Shared Function GetBytesStringKB(ByVal bytesCount As Int64) As [String]
        Dim bytesShow As Int64 = (bytesCount + 1023) >> 10
        Dim bytesString As [String] = GetPointString(bytesShow) & " KB"
        Return bytesString
    End Function
    Public Shared Function GetPointString(ByVal value As Int64) As [String]
        Dim pointString As [String] = value.ToString()

        Dim i As Int32 = 3
        While pointString.Length > i
            pointString = pointString.Substring(0, pointString.Length - i) & "." & pointString.Substring(pointString.Length - i, i)
            i += 4
        End While

        Return pointString
    End Function
    Private Shared Function MatchesRestrictions(ByVal info As FileSystemInfo) As [Boolean]
        Dim matches As [Boolean] = True
        If matches AndAlso m_pars.NewerThanChecked Then
            matches = (info.LastWriteTime >= m_pars.NewerThanDateTime)
        End If
        If matches AndAlso m_pars.OlderThanChecked Then
            matches = (info.LastWriteTime <= m_pars.OlderThanDateTime)
        End If
        If matches AndAlso m_pars.ContainingChecked Then
            matches = False
            If TypeOf info Is FileInfo Then
                matches = FileContainsBytes(info.FullName, m_containingBytes)
            End If
        End If
        Return matches
    End Function

    Private Shared Function FileContainsBytes(ByVal path As [String], ByVal compare As [Byte]()) As [Boolean]
        Dim contains As [Boolean] = False

        Dim blockSize As Int32 = 4096
        If (compare.Length >= 1) AndAlso (compare.Length <= blockSize) Then
            Dim block As [Byte]() = New [Byte](compare.Length - 1 + (blockSize - 1)) {}

            Try
                Dim fs As New FileStream(path, FileMode.Open, FileAccess.Read)

                ' Read the first bytes from the file into "block":
                Dim bytesRead As Int32 = fs.Read(block, 0, block.Length)

                Do
                    ' Search "block" for the sequence "compare":
                    Dim endPos As Int32 = bytesRead - compare.Length + 1
                    For i As Int32 = 0 To endPos - 1
                        ' Read "compare.Length" bytes at position "i" from the buffer,
                        ' and compare them with "compare":
                        Dim j As Int32
                        For j = 0 To compare.Length - 1
                            If block(i + j) <> compare(j) Then
                                Exit For
                            End If
                        Next

                        If j = compare.Length Then
                            ' "block" contains the sequence "compare":
                            contains = True
                            Exit For
                        End If
                    Next

                    ' Search completed?
                    If contains OrElse (fs.Position >= fs.Length) Then
                        Exit Do
                    Else
                        ' Copy the last "compare.Length - 1" bytes to the beginning of "block":
                        For i As Int32 = 0 To (compare.Length - 1) - 1
                            block(i) = block(blockSize + i)
                        Next

                        ' Read the next "blockSize" bytes into "block":
                        bytesRead = compare.Length - 1 + fs.Read(block, compare.Length - 1, blockSize)
                    End If
                Loop While Not m_stop

                fs.Close()
            Catch generatedExceptionName As Exception
            End Try
        End If

        Return contains
    End Function
End Class

