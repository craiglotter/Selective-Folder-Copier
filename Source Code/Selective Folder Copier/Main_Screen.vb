Imports System.IO
Imports System.Web.Mail
Imports System.Security.Cryptography
Imports System.Text


Public Class Main_Screen

    Private busyworking As Boolean = False
    Private cancelled As Boolean = False
    Private AutoUpdate As Boolean = False
    Dim shownminimizetip As Boolean = False

    Private sourcefolder As String = ""
    Private backupfolder As String = ""

    Dim foldersprocessed As Long
    Dim filesprocessed As Long
    Dim filesignored As Long
    Dim datacopied As Long
    Dim precountfolders As Long
    Dim precountfiles As Long



    Private Sub Error_Handler(ByVal ex As Exception, Optional ByVal identifier_msg As String = "")
        Try
            If ex.Message.IndexOf("Thread was being aborted") < 0 Then
                Dim Display_Message1 As New Display_Message()
                Display_Message1.Message_Textbox.Text = "The Application encountered the following problem: " & vbCrLf & identifier_msg & ": " & ex.ToString
                Display_Message1.Timer1.Interval = 1000
                Display_Message1.ShowDialog()
                Dim dir As System.IO.DirectoryInfo = New System.IO.DirectoryInfo((Application.StartupPath & "\").Replace("\\", "\") & "Error Logs")
                If dir.Exists = False Then
                    dir.Create()
                End If
                dir = Nothing
                Dim filewriter As System.IO.StreamWriter = New System.IO.StreamWriter((Application.StartupPath & "\").Replace("\\", "\") & "Error Logs\" & Format(Now(), "yyyyMMdd") & "_Error_Log.txt", True)
                filewriter.WriteLine("#" & Format(Now(), "dd/MM/yyyy hh:mm:ss tt") & " - " & identifier_msg & ": " & ex.ToString)
                filewriter.WriteLine("")
                filewriter.Flush()
                filewriter.Close()
                filewriter = Nothing
            End If
            StatusLabel.Text = "Error Reported"
        Catch exc As Exception
            MsgBox("An error occurred in the application's error handling routine. The application will try to recover from this serious error." & vbCrLf & vbCrLf & exc.ToString, MsgBoxStyle.Critical, "Critical Error Encountered")
        End Try
    End Sub

    Private Sub Activity_Handler(ByVal message As String)
        Try
            Dim dir As System.IO.DirectoryInfo = New System.IO.DirectoryInfo((Application.StartupPath & "\").Replace("\\", "\") & "Activity Logs")
            If dir.Exists = False Then
                dir.Create()
            End If
            dir = Nothing
            Dim filewriter As System.IO.StreamWriter = New System.IO.StreamWriter((Application.StartupPath & "\").Replace("\\", "\") & "Activity Logs\" & Format(Now(), "yyyyMMdd") & "_Activity_Log.txt", True)
            filewriter.WriteLine("#" & Format(Now(), "dd/MM/yyyy hh:mm:ss tt") & " - " & message)
            filewriter.WriteLine("")
            filewriter.Flush()
            filewriter.Close()
            filewriter = Nothing
            StatusLabel.Text = "Activity Logged"
        Catch ex As Exception
            Error_Handler(ex, "Activity Handler")
        End Try
    End Sub

    Private Sub Main_Screen_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            Control.CheckForIllegalCrossThreadCalls = False
            Me.Text = My.Application.Info.ProductName & " (" & Format(My.Application.Info.Version.Major, "0000") & Format(My.Application.Info.Version.Minor, "00") & Format(My.Application.Info.Version.Build, "00") & "." & Format(My.Application.Info.Version.Revision, "00") & ")"
            NotifyIcon1.BalloonTipText = "You have chosen to hide " & My.Application.Info.ProductName & ". To bring it back up, simply click here."
            NotifyIcon1.BalloonTipTitle = My.Application.Info.ProductName
            NotifyIcon1.Text = "Click to bring up " & My.Application.Info.ProductName
            Label1.Text = "0 Files"
            Label2.Text = "0 Files"
            Label3.Text = "0 Files"
            ProgressBar1.Value = 0
            loadSettings()
            StatusLabel.Text = "Application Loaded"
            Control_Enabler(True)
        Catch ex As Exception
            Error_Handler(ex, "Application Loading")
        End Try
    End Sub

    Private Sub loadSettings()
        Try
            Dim configfile As String = (Application.StartupPath & "\config.sav").Replace("\\", "\")
            If My.Computer.FileSystem.FileExists(configfile) Then
                Dim reader As StreamReader = New StreamReader(configfile)
                Dim lineread As String
                Dim variablevalue As String
                While reader.Peek <> -1
                    lineread = reader.ReadLine
                    If lineread.IndexOf("=") <> -1 Then
                        variablevalue = lineread.Remove(0, lineread.IndexOf("=") + 1)
                        If lineread.StartsWith("FilesToIgnore=") Then
                            FilesToIgnore_TextBox.Text = variablevalue.Trim
                        End If
                        If lineread.StartsWith("SourceFolder=") Then
                            sourcefolder = variablevalue.Trim
                        End If
                        If lineread.StartsWith("BackupFolder=") Then
                            backupfolder = variablevalue.Trim
                        End If
                    End If
                End While
                reader.Close()
                reader = Nothing
            End If
            StatusLabel.Text = "Application Settings Loaded"
        Catch ex As Exception
            Error_Handler(ex, "Load Settings")
        End Try
    End Sub

    Private Sub SaveSettings()
        Try
            Dim configfile As String = (Application.StartupPath & "\config.sav").Replace("\\", "\")
            Dim writer As StreamWriter = New StreamWriter(configfile, False)
            writer.WriteLine("FilesToIgnore=" & FilesToIgnore_TextBox.Text)
            writer.WriteLine("SourceFolder=" & sourcefolder)
            writer.WriteLine("BackupFolder=" & backupfolder)
            writer.Flush()
            writer.Close()
            writer = Nothing
            StatusLabel.Text = "Application Settings Saved"
        Catch ex As Exception
            Error_Handler(ex, "Save Settings")
        End Try
    End Sub

    Private Sub Main_Screen_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        Try
            SaveSettings()
            If AutoUpdate = True Then
                If My.Computer.FileSystem.FileExists((Application.StartupPath & "\AutoUpdate.exe").Replace("\\", "\")) = True Then
                    Dim startinfo As ProcessStartInfo = New ProcessStartInfo
                    startinfo.FileName = (Application.StartupPath & "\AutoUpdate.exe").Replace("\\", "\")
                    startinfo.Arguments = "force"
                    startinfo.CreateNoWindow = False
                    Process.Start(startinfo)
                End If
            End If
            StatusLabel.Text = "Application Shutting Down"
        Catch ex As Exception
            Error_Handler(ex, "Closing Application")
        End Try
    End Sub
    Private Sub NotifyIcon1_BalloonTipClicked(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NotifyIcon1.BalloonTipClicked
        Try
            Me.WindowState = FormWindowState.Normal
            Me.ShowInTaskbar = True
            NotifyIcon1.Visible = False
            Me.Refresh()
        Catch ex As Exception
            Error_Handler(ex, "Click on NotifyIcon")
        End Try
    End Sub


    Private Sub NotifyIcon1_MouseClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles NotifyIcon1.MouseClick
        Try
            Me.WindowState = FormWindowState.Normal
            Me.ShowInTaskbar = True
            NotifyIcon1.Visible = False
            Me.Refresh()
        Catch ex As Exception
            Error_Handler(ex, "Click on NotifyIcon")
        End Try
    End Sub


    Private Sub NotifyIcon1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NotifyIcon1.Click
        Try
            Me.WindowState = FormWindowState.Normal
            Me.ShowInTaskbar = True
            NotifyIcon1.Visible = False
            Me.Refresh()
        Catch ex As Exception
            Error_Handler(ex, "Click on NotifyIcon")
        End Try
    End Sub

    Private Sub Main_Screen_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        Try
            If Me.WindowState = FormWindowState.Minimized Then
                Me.ShowInTaskbar = False
                NotifyIcon1.Visible = True
                If shownminimizetip = False Then
                    NotifyIcon1.ShowBalloonTip(1)
                    shownminimizetip = True
                End If
            End If
        Catch ex As Exception
            Error_Handler(ex, "Change Window State")
        End Try
    End Sub

    Private Sub HelpToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HelpToolStripMenuItem1.Click
        Try
            HelpBox1.ShowDialog()
            StatusLabel.Text = "Help Dialog Viewed"
        Catch ex As Exception
            Error_Handler(ex, "Display Help Screen")
        End Try
    End Sub

    Private Sub AutoUpdateToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AutoUpdateToolStripMenuItem.Click
        Try
            StatusLabel.Text = "AutoUpdate Requested"
            AutoUpdate = True
            Me.Close()
        Catch ex As Exception
            Error_Handler(ex, "AutoUpdate")
        End Try
    End Sub

    Private Sub AboutToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem1.Click
        Try
            AboutBox1.ShowDialog()
            StatusLabel.Text = "About Dialog Viewed"
        Catch ex As Exception
            Error_Handler(ex, "Display About Screen")
        End Try
    End Sub

    Private Sub Control_Enabler(ByVal IsEnabled As Boolean)
        Try
            Select Case IsEnabled
                Case True
                    Button1.Enabled = True
                    Button2.Enabled = False
                    MenuStrip1.Enabled = True
                    FilesToIgnore_TextBox.Enabled = True
                    Me.ControlBox = True
                    ProgressBar1.Enabled = False
                Case False
                    Button1.Enabled = False
                    Button2.Enabled = True
                    MenuStrip1.Enabled = False
                    FilesToIgnore_TextBox.Enabled = False
                    Me.ControlBox = False
                    ProgressBar1.Enabled = True
            End Select
            StatusLabel.Text = "Control Enabler Run"
        Catch ex As Exception
            Error_Handler(ex, "Control Enabler")
        End Try
    End Sub





    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Try
            BackgroundWorker1.ReportProgress(0)
            If cancelled = False Then
                StatusLabel.Text = "Running Precount Function"
                run_precount(sourcefolder)
                Label1.Text = "0 Files"
                Label2.Text = Math.Round(precountfiles / 2, 0) & " Files"
                Label3.Text = precountfiles & " Files"
            End If
            If cancelled = False Then
                StatusLabel.Text = "Copying Files"
                FilesToIgnore_TextBox.Text = FilesToIgnore_TextBox.Text.ToLower
                While FilesToIgnore_TextBox.Text.EndsWith(";")
                    FilesToIgnore_TextBox.Text = FilesToIgnore_TextBox.Text.Remove(FilesToIgnore_TextBox.Text.Length - 1, 1)
                End While
                FilesToIgnore_TextBox.Text = FilesToIgnore_TextBox.Text.Replace(";", ";.").Replace(";..", ";.")
                Dim filestoignore As Array = FilesToIgnore_TextBox.Text.Split(";")
                run_copyfiles(sourcefolder, filestoignore)
                filestoignore = Nothing
            End If
            BackgroundWorker1.ReportProgress(100)
        Catch ex As Exception
            Error_Handler(ex, "Copy Operation")
        End Try
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        Try
            Control_Enabler(True)
            If cancelled = False And e.Cancelled = False And e.Error Is Nothing Then
                StatusLabel.Text = "Operation Complete"
            Else
                If Not e.Error Is Nothing Then
                    StatusLabel.Text = "Operation Failed"
                Else
                    StatusLabel.Text = "Operation Cancelled"
                End If
            End If
            MsgBox(StatusLabel.Text & vbCrLf & vbCrLf & "Folders Processed: " & foldersprocessed & " (of " & precountfolders & ")" & vbCrLf & "Files Processed: " & (filesprocessed + filesignored) & " (of " & precountfiles & ")" & vbCrLf & "Files Copied: " & filesprocessed & vbCrLf & "Files Ignored: " & filesignored & vbCrLf & "Data Copied: " & datacopied & " bytes", MsgBoxStyle.Information, "Results Summary")
            busyworking = False
        Catch ex As Exception
            Error_Handler(ex, "Operation Complete")
        End Try
    End Sub


    Private Sub BackgroundWorker1_ProgressChanged(ByVal sender As System.Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        Try
            If e.ProgressPercentage < 0 Then
                ProgressBar1.Value = 0
            Else
                ProgressBar1.Value = e.ProgressPercentage
            End If
            StatusLabel.Text = "Copying Files (" & filesprocessed + filesignored & ")"
        Catch ex As Exception
            Error_Handler(ex, "Progress Changed (" & e.ProgressPercentage & ")")
        End Try

    End Sub

    Private Sub runworker()
        Try
            If busyworking = False Then
                FolderBrowserDialog1.ShowNewFolderButton = False
                FolderBrowserDialog1.Description = "Please select the folder which you want to copy from."
                If My.Computer.FileSystem.DirectoryExists(sourcefolder) Then
                    FolderBrowserDialog1.SelectedPath = sourcefolder
                End If
                If FolderBrowserDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                    sourcefolder = FolderBrowserDialog1.SelectedPath
                    FolderBrowserDialog1.ShowNewFolderButton = True
                    FolderBrowserDialog1.Description = "Please select the folder which you want to copy to."
                    If My.Computer.FileSystem.DirectoryExists(backupfolder) Then
                        FolderBrowserDialog1.SelectedPath = backupfolder
                    End If
                    If FolderBrowserDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                        backupfolder = FolderBrowserDialog1.SelectedPath
                        Dim dinfo As DirectoryInfo = New DirectoryInfo(backupfolder)
                        Dim proceed As Boolean = False
                        If dinfo.GetFiles.Length > 0 Or dinfo.GetDirectories.Length > 0 Then
                            If MsgBox("It has been detected that the folder that you wish to copy to is currently not empty. Any existing files WILL be overwritten by this process. Are you sure you want to continue? (Note that there might be some hidden operating system files, like 'desktop.ini', in the target folder, hence the triggering of this message)", MsgBoxStyle.YesNo, "Directory not empty") = MsgBoxResult.Yes Then
                                proceed = True
                            Else
                                proceed = False
                            End If
                        Else
                            proceed = True
                        End If
                        If proceed = True Then
                            busyworking = True
                            cancelled = False
                            Control_Enabler(False)
                            Label1.Text = "0 Files"
                            Label2.Text = "0 Files"
                            Label3.Text = "0 Files"
                            ProgressBar1.Value = 0
                            foldersprocessed = 0
                            filesprocessed = 0
                            filesignored = 0
                            precountfolders = 0
                            precountfiles = 0
                            StatusLabel.Text = "Initializing Operation"
                            BackgroundWorker1.RunWorkerAsync()
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            Error_Handler(ex, "Run Worker")
        End Try
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Try
            runworker()
        Catch ex As Exception
            Error_Handler(ex, "Start Button Click")
        End Try
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Try
            BackgroundWorker1.CancelAsync()
            cancelled = True
        Catch ex As Exception
            Error_Handler(ex, "Cancel Button Click")
        End Try
    End Sub

    Private Sub run_precount(ByVal path As String)
        Try
            StatusLabel.Text = "Running Precount Function"
            If cancelled = False Then
                Dim dinfo As DirectoryInfo = New DirectoryInfo(path)
                precountfolders = precountfolders + 1
                For Each finfo As FileInfo In dinfo.GetFiles
                    If cancelled = True Then
                        Exit For
                    End If
                    precountfiles = precountfiles + 1
                    finfo = Nothing
                Next
                For Each sdinfo As DirectoryInfo In dinfo.GetDirectories
                    If cancelled = True Then
                        Exit For
                    End If
                    run_precount(sdinfo.FullName)
                    sdinfo = Nothing
                Next
                dinfo = Nothing
            End If
        Catch ex As Exception
            Error_Handler(ex, "Precount Function (" & path & ")")
        End Try
    End Sub

    Private Sub run_copyfiles(ByVal path As String, ByVal filestoignore As Array)
        Try
            If cancelled = False Then
                Dim dinfo As DirectoryInfo = New DirectoryInfo(path)
                Dim bdinfo As DirectoryInfo = New DirectoryInfo(path.Replace(sourcefolder, backupfolder))
                If bdinfo.Exists = False Then
                    bdinfo.Create()
                End If

                For Each finfo As FileInfo In dinfo.GetFiles
                    If cancelled = True Then
                        Exit For
                    End If
                    Dim processfile As Boolean = True
                    For Each strval In filestoignore
                        If finfo.Extension.ToLower = strval Then
                            processfile = False
                        End If
                    Next
                    If processfile = True Then
                        finfo.CopyTo((bdinfo.FullName & "\" & finfo.Name).Replace("\\", "\"), True)
                        datacopied = datacopied + finfo.Length
                        filesprocessed = filesprocessed + 1
                    Else
                        filesignored = filesignored + 1
                    End If
                    BackgroundWorker1.ReportProgress((Math.Round(((filesprocessed + filesignored) / precountfiles) * 100, 0)) - 1)

                    finfo = Nothing
                Next
                For Each sdinfo As DirectoryInfo In dinfo.GetDirectories
                    If cancelled = True Then
                        Exit For
                    End If
                    run_copyfiles(sdinfo.FullName, filestoignore)
                    sdinfo = Nothing
                Next
                foldersprocessed = foldersprocessed + 1
                dinfo = Nothing
                bdinfo = Nothing
            End If
        Catch ex As Exception
            Error_Handler(ex, "Copy Files Function (" & path & ")")
        End Try
    End Sub

End Class
