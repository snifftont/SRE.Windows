Imports System.Text
Imports System.IO
Imports System.Threading
Imports System.Xml
Imports System.Data.OleDb
Imports EPocalipse.IFilter

Public Class SRE_Windows
    Private Shared findText1 As String = ""
    Private Shared searchnew As Boolean = False
    Private Shared modifyDate1 As DateTime
    Private Shared m_thread As Thread = Nothing
    Private Shared m_stop As Boolean = False
    Private Shared m_pars As SearcherParams = Nothing
    Private Shared m_containingBytes As Byte() = Nothing
    ' Dim dtsearch As New DataTable()
    Shared dtsearch As DataTable
    ''' <summary>
    ''' Function to search topics in Windows and return result as DataTable
    ''' </summary>
    ''' <param name="path1"></param>
    ''' <param name="modifydate"></param>
    ''' <param name="reqID"></param>
    ''' <param name="findText"></param>
    ''' <param name="newseach"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function searchResult(ByVal path1 As String, ByVal modifydate As String, ByVal reqID As String, ByVal findText As String, ByVal newseach As Boolean) As DataTable
        Dim dt As New DataTable()
        modifyDate1 = Convert.ToDateTime(modifydate)
        searchnew = newseach
        findText1 = findText
        Dim fileNamesString As [String] = "*,*"
        Dim fileNames As [String]() = fileNamesString.Split(New [Char]() {";"})
        Dim validFileNames As New List(Of [String])()
        For Each fileName As [String] In fileNames
            Dim trimmedFileName As [String] = fileName.Trim()
            If trimmedFileName <> "" Then
                validFileNames.Add(trimmedFileName)
            End If
        Next
        Dim encoding__1 As Encoding = Encoding.ASCII

        Dim pars As New SearcherParams(path1, True, validFileNames, True, Convert.ToDateTime(modifydate), False, _
         Convert.ToDateTime(modifydate), True, findText, encoding__1)
        Dim dir As New DirectoryInfo(path1)
        Dim dt1 As DataTable = SearchDirectory(dir)
        Return dt1
    End Function

    Public Function SearchDirectory(ByVal dirInfo As DirectoryInfo) As DataTable

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
        If dtsearch Is Nothing Then
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
                            positionOfDream = s1.IndexOf(findText1.ToLower().ToString())
                            If positionOfDream > -1 Then
                                If info.LastAccessTime >= modifyDate1 Then
                                    ' Dim drow As DataRow = dtsearch.NewRow()
                                    Dim drow As DataRow = dtsearch.NewRow()
                                    drow(0) = info.Name
                                    '   ListViewItem lvi = new ListViewItem();
                                    Dim lvi As String = info.FullName
                                    Dim lvsi As String
                                    Console.WriteLine(info.FullName)

                                    'ListViewItem.ListViewSubItem lvsi = new ListViewItem.ListViewSubItem();
                                    If TypeOf info Is FileInfo Then
                                        lvsi = GetBytesStringKB(DirectCast(info, FileInfo).Length)
                                        drow(2) = lvsi     'File size
                                    Else
                                        lvsi = ""
                                    End If
                                    'lvi.SubItems.Add(lvsi);
                                    '            lvsi = new ListViewItem.ListViewSubItem();
                                    lvsi = info.LastWriteTime.ToShortDateString() & " " & info.LastWriteTime.ToShortTimeString()
                                    drow(1) = info.FullName                            'file full name
                                    ' drow(2) = info.FullName                            'file full name
                                    drow(3) = info.LastWriteTime.ToString()            'modify date
                                    drow(4) = info.CreationTime.ToString()             'create date
                                    drow(5) = findText1                                'search topic name
                                    dtsearch.Rows.Add(drow)
                                    ' Return dtsearch
                                End If
                                Return dtsearch
                            End If
                        End Using
                    Catch ex As Exception

                    End Try

                    'If MatchesRestrictions(info) Then


                    'End If
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

        Return dtsearch
    End Function

    Private Function SearchDirectory1(ByVal dirInfo As DirectoryInfo) As DataTable

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
        If dtsearch Is Nothing Then
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
                            Dim checking As Boolean = False
                            Dim test As String() = findText1.Split(","c)
                            Dim drow As DataRow = dtsearch.NewRow()
                            Dim findtextres As String = ""
                            Dim text As String = reader.ReadToEnd()
                            'label1.Text = "Text loaded from " + openFileDialog1.FileName;
                            Dim s1 As String = text.ToLower().ToString()
                            For check As Integer = 0 To test.Length - 2
                                If Not test(check) = "" Then




                                    Dim positionOfDream As Integer = 0
                                    positionOfDream = s1.IndexOf(test(check))
                                    If positionOfDream > -1 Then
                                        If info.LastAccessTime >= modifyDate1 Then
                                            ' Dim drow As DataRow = dtsearch.NewRow()

                                            drow(0) = info.Name
                                            '   ListViewItem lvi = new ListViewItem();
                                            Dim lvi As String = info.FullName
                                            Dim lvsi As String
                                            Console.WriteLine(info.FullName)

                                            'ListViewItem.ListViewSubItem lvsi = new ListViewItem.ListViewSubItem();
                                            If TypeOf info Is FileInfo Then
                                                lvsi = GetBytesStringKB(DirectCast(info, FileInfo).Length)
                                                drow(2) = lvsi     'File size
                                            Else
                                                lvsi = ""
                                            End If
                                            'lvi.SubItems.Add(lvsi);
                                            '            lvsi = new ListViewItem.ListViewSubItem();
                                            lvsi = info.LastWriteTime.ToShortDateString() & " " & info.LastWriteTime.ToShortTimeString()
                                            drow(1) = info.FullName                            'file full name
                                            ' drow(2) = info.FullName                            'file full name
                                            drow(3) = info.LastWriteTime.ToString()            'modify date
                                            drow(4) = info.CreationTime.ToString()             'create date
                                            findtextres = findtextres + test(check) + ","
                                            drow(5) = findtextres                                'search topic name
                                            checking = True
                                            ' Return dtsearch
                                        End If
                                    End If
                                End If
                            Next
                            If checking = True Then
                                dtsearch.Rows.Add(drow)
                                checking = False
                            End If

                        End Using
                    Catch ex As Exception

                    End Try

                    'If MatchesRestrictions(info) Then


                    'End If
                Next

                If True Then
                    Dim subDirInfos As DirectoryInfo() = dirInfo.GetDirectories()
                    For Each subDirInfo As DirectoryInfo In subDirInfos
                        If m_stop Then
                            Exit For
                        End If

                        ' Recursion:
                        SearchDirectory1(subDirInfo)
                    Next
                End If
            Catch generatedExceptionName As Exception
            End Try
        End If

        Return dtsearch
    End Function

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
                m_containingBytes = Encoding.ASCII.GetBytes(findText1)
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

    ''' <summary>
    ''' Function to search topics in Windows and return result as XMLDOCUMENT
    ''' </summary>
    ''' <param name="path2"></param>
    ''' <param name="modifydate"></param>
    ''' <param name="reqID"></param>
    ''' <param name="findText"></param>
    ''' <param name="newseach"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function searchResult_XML(ByVal xdocs As String, ByVal path2 As String, ByVal modifydate As String, ByVal reqID As String, ByVal findText As String, ByVal newseach As Boolean) As String
        Dim dt As New XmlDocument
        Dim xmlDoc As New XmlDocument
        Dim path1 As String = ""
        searchnew = True
        findText1 = ""
        xmlDoc.LoadXml(xdocs.ToString())
        For Each xmlNode As XmlNode In xmlDoc.SelectNodes("SRE_Request")
            Dim a11 As String = xmlNode.Attributes("ID").Value
        Next
        For Each xmlNode1 As XmlNode In xmlDoc.DocumentElement.SelectNodes("ModifyDate")
            Dim a11 As String = xmlNode1.InnerText.ToString()
            modifyDate1 = Convert.ToDateTime(xmlNode1.InnerText.ToString())
        Next
        For Each xmlNode2 As XmlNode In xmlDoc.DocumentElement.SelectNodes("Scope")
            Dim a11 As String = xmlNode2.InnerText.ToString()
            path1 = xmlNode2.InnerText.ToString()
        Next

        For Each xmlNode3 As XmlNode In xmlDoc.DocumentElement.LastChild.SelectNodes("Topic")
            Dim a11 As String = xmlNode3.Attributes("ID").Value
            Dim a12 As String = xmlNode3.Attributes("Text").Value
            findText1 = findText1 + a12 + ","
        Next

        ' modifyDate1 = Convert.ToDateTime(modifydate)
        'findText1 = findText
        Dim fileNamesString As [String] = "*,*"
        Dim fileNames As [String]() = fileNamesString.Split(New [Char]() {";"})
        Dim validFileNames As New List(Of [String])()
        For Each fileName As [String] In fileNames
            Dim trimmedFileName As [String] = fileName.Trim()
            If trimmedFileName <> "" Then
                validFileNames.Add(trimmedFileName)
            End If
        Next
        Dim encoding__1 As Encoding = Encoding.ASCII

        Dim pars As New SearcherParams(path1, True, validFileNames, True, Convert.ToDateTime(modifydate), False, _
         Convert.ToDateTime(modifydate), True, findText, encoding__1)
        Dim dir As New DirectoryInfo(path1)
        Dim dt1 As DataTable = SearchDirectory1(dir)

        'MessageBox.Show(dt1.Rows.Count)
        Dim xmlString As String = Nothing
        'CREATING XML RESPONSE FROM RESULTS
        Using sw As New StringWriter()
            Dim writer As New XmlTextWriter(sw)
            writer.Formatting = Formatting.Indented
            ' if you want it indented
            writer.WriteStartDocument()
            ' <?xml version="1.0" encoding="utf-16"?>
            writer.WriteStartElement("SRE_Response")
            '<SRE_Request>
            writer.WriteStartAttribute("RequestID")
            writer.WriteString("1")
            ' <SRE_Type name=className />
            writer.WriteStartElement("SRE_Type")
            writer.WriteStartAttribute("name")
            writer.WriteString("SRE_Windows")
            writer.WriteEndElement()

            '<Items>
            writer.WriteStartElement("Items")
            ' <Item>
            '      <Properties>
            '          <ID></ID>
            '          <Type></Type>
            '          <Size></Size>
            '          <UniqueUrl></UniqueUrl>
            '          <Created></Created>
            '          <Modified></Modified>
            '      </Properties>
            Dim id1 As Integer = 0
            Dim id22 As Integer = 1
            For i11 As Integer = 0 To dt1.Rows.Count - 1
                writer.WriteStartElement("Item")
                writer.WriteStartElement("Properties")
                writer.WriteStartElement("ID")
                writer.WriteString(id22.ToString())
                writer.WriteEndElement()


                writer.WriteStartElement("Type")
                writer.WriteString(Path.GetExtension(dt1.Rows(i11)(0).ToString()) & "File")
                writer.WriteEndElement()

                writer.WriteStartElement("UniqueUrl")
                writer.WriteString(dt1.Rows(i11)(1).ToString())
                writer.WriteEndElement()

                writer.WriteStartElement("Size")
                writer.WriteString(dt1.Rows(i11)(2).ToString())
                writer.WriteEndElement()

                writer.WriteStartElement("Modified")
                writer.WriteString(dt1.Rows(i11)(4).ToString())
                writer.WriteEndElement()

                writer.WriteStartElement("Created")
                writer.WriteString(dt1.Rows(i11)(3).ToString())
                writer.WriteEndElement()
                writer.WriteEndElement()
                '      </Properties>  closed here
                writer.WriteStartElement("Topic")
                writer.WriteStartElement("ID")
                writer.WriteString("1")
                writer.WriteEndElement()
                writer.WriteStartElement("Text")
                writer.WriteString(dt1.Rows(i11)(5).ToString())
                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteEndElement()

                id22 = id22 + 1
            Next
            writer.WriteEndDocument()
            '</SRE_Response>
            xmlString = sw.ToString()
            ' dt.Save(sw)
        End Using

        Return xmlString
    End Function

End Class
