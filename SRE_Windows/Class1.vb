Imports System.Threading
Imports System.IO
Imports System.Text

Class Class1
    Private Shared m_thread As Thread = Nothing
    Private Shared m_stop As Boolean = False
    Private Shared m_pars As SearcherParams = Nothing
    Private Shared m_containingBytes As Byte() = Nothing
    Private Shared Sub Main(ByVal args As String())
        Dim fileNamesString As [String] = "*,*"
        Dim fileNames As [String]() = fileNamesString.Split(New [Char]() {";"c})
        Dim validFileNames As New List(Of [String])()
        For Each fileName As [String] In fileNames
            Dim trimmedFileName As [String] = fileName.Trim()
            If trimmedFileName <> "" Then
                validFileNames.Add(trimmedFileName)
            End If
        Next
        Dim encoding__1 As Encoding = Encoding.ASCII

        Dim pars As New SearcherParams("C:\", True, validFileNames, True, Convert.ToDateTime("4/2/2013"), False, _
         Convert.ToDateTime("4/2/2013"), True, "connectionString", encoding__1)
        Dim dir As New DirectoryInfo("C:\")
        SearchDirectory(dir)

    End Sub
    Private Shared Sub SearchDirectory(ByVal dirInfo As DirectoryInfo)
        If Not m_stop Then
            Try

                Dim infos As FileSystemInfo() = dirInfo.GetFileSystemInfos("*")

                For Each info As FileSystemInfo In infos
                    If m_stop Then
                        Exit For
                    End If

                    If MatchesRestrictions(info) Then
                        '   ListViewItem lvi = new ListViewItem();
                        Dim lvi As String = info.FullName
                        Dim lvsi As String
                        Console.WriteLine(info.FullName)

                        'ListViewItem.ListViewSubItem lvsi = new ListViewItem.ListViewSubItem();
                        If TypeOf info Is FileInfo Then
                            lvsi = GetBytesStringKB(DirectCast(info, FileInfo).Length)
                            Console.WriteLine(lvsi)
                        Else
                            lvsi = ""
                        End If
                        'lvi.SubItems.Add(lvsi);
                        '            lvsi = new ListViewItem.ListViewSubItem();
                        lvsi = info.LastWriteTime.ToShortDateString() & " " & info.LastWriteTime.ToShortTimeString()
                        Console.WriteLine(lvsi)
                        ' lvi.SubItems.Add(lvsi);
                        '  lvi.ToolTipText = info.FullName;
                        '   resultsList.Items.Add(lvi);

                        Console.WriteLine("*****************************************************************************")
                    End If
                Next

                If True Then
                    Dim subDirInfos As DirectoryInfo() = dirInfo.GetDirectories()
                    For Each subDirInfo As DirectoryInfo In subDirInfos
                        If m_stop Then
                            Exit For
                        End If

                        ' Recursion:
                        SearchDirectory(subDirInfo)
                    Next
                End If
            Catch generatedExceptionName As Exception
            End Try
        End If
    End Sub
    Private Shared Function MatchesRestrictions(ByVal info As FileSystemInfo) As [Boolean]
        Dim matches As [Boolean] = True

        If matches AndAlso False Then
            matches = (info.LastWriteTime >= m_pars.NewerThanDateTime)
        End If

        If matches AndAlso False Then
            matches = (info.LastWriteTime <= m_pars.OlderThanDateTime)
        End If

        If matches AndAlso True Then
            matches = False
            If TypeOf info Is FileInfo Then
                m_containingBytes = Encoding.ASCII.GetBytes("connectionString")
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
    Public Shared Function GetBytesStringKB(ByVal bytesCount As Int64) As [String]
        Dim bytesShow As Int64 = (bytesCount + 1023) >> 10
        Dim bytesString As [String] = Convert.ToString(GetPointString(bytesShow)) & " KB"
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


End Class
