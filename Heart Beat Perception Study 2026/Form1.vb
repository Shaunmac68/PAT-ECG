Imports VB = Microsoft.VisualBasic

Public Class Form1
    Inherits System.Windows.Forms.Form

    Public ConfidenceValueSet As Boolean    'Whether or not the confidence value has been set by the subject (true or false)
    Public ConfidenceValue As Integer       'The value of confidence that the subject selected from 1 to 10

    Dim comName As String   'Stores comport name, e.g. Com3, Com5, etc
    Dim comNum As Integer   'Com port number, e.g. 3, 5, ....
    Dim comPortFound As Boolean 'Gives the status of whether a port has been found, true / false
    Dim receivedData As String = "" 'Stores data received from the black box

    Dim Foldername As String    'Folder name for Excel data files storage
    Dim Subject As String       'Subject number
    Dim subjectName As String   'String variable that is either "trainingSubject" or "Subject"

    Dim Trial As Integer        'Trial number
    Dim LastTrial As Integer    'Number of trials to be collected

    Dim CollectingData As Boolean   'Start collecting data, true / false

    Public ReadPort As Boolean  'Whether or not to read new data from port, true / false

    Dim newLineReceived As Boolean  'Whether or not a new line of data has been received from black box, true / false

    Dim newChars As String      'New data received from black box

    Dim LatestChars As String   'Latest data read from serial port

    Dim HR_Period As String 'Period of time between consecutive 'R' wave pulses
    Dim HR_Delay As String  'Delay produced by altering the encoder / dial
    Dim AVE_HRP As String   'Average heart rate period over 'HRP_DataPoints' (see Arduino IDE code)

    Dim Training As Boolean 'Whether the program is running in training mode or not, true / false

    Dim HR_Val(100) As Integer          'Array store for heart rate
    Dim HR_Period_Val(100) As Integer   'Array store for heart rate period between 'R' wave pulses
    Dim HR_Delay_Val(100) As Integer    'Array store for tone delay period
    Dim AVE_HRPeriod(100) As Integer    'Array store for average heart rate period
    Dim Confidence_Val(100)             'Array store for data entered for confidence feedback

    Dim menuItem As String

    Private Sub AboutThisProgramToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutThisProgramToolStripMenuItem.Click
        MsgBox("This program was written by S.R.Mckiernan for Bangor University (2026)", vbInformation, "About This Program")
    End Sub


    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.KeyPreview = True
        UserInfoLbl.Text = ""
        Foldername = "C:\Heart Beat Perception Data"

        'If foldername doesn't exist, make new folder called "C:\Heart Beat Perception Data"
        If System.IO.Directory.Exists(Foldername) = False Then
            MkDir(Foldername)
        End If

        comPortFound = False
        comNum = 0
        LastTrial = 44
        ConfidenceValueSet = False
    End Sub


    Private Sub SerialPort1_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived

        ' If serial data received then ...

        If ReadPort = True Then ' If port open, then read new serial data into newChars

            Try
                newChars = SerialPort1.ReadLine
                'If chrNum <> 13 Then newChars &= Chr(chrNum)
            Catch ex As Exception

            End Try

            LatestChars = newChars
            newChars = ""
            newLineReceived = True

        Else    'If serial port is still closed, check to see if serial data received = "OK". If it does, this must have been sent from the black box, so correct port has been found!
            Try
                newChars = SerialPort1.ReadLine
                SerialPort1.DiscardInBuffer()
                If newChars.Contains("OK") Then
                    ComTimer.Enabled = False
                    comName = "COM" & comNum    'Set current serial port to COM 'comNum'
                    MsgBox("Found device on port " & comName, vbOKOnly, "Hardware Found")
                    comPortFound = True
                End If
            Catch ex As Exception
                MsgBox("Error reading port!")   'Display error message 
            End Try

        End If

    End Sub

    ' Run this every 200mS to see if any new serial data has arrived
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Try
            Dim Arr() As String
            If (Len(LatestChars) > 0) And (ReadPort = True) And (newLineReceived = True) Then
                newLineReceived = False

                'If Data has arrived, split it into the three data values expected, then store in 'HR_Delay', 'HR_Period' and 'AVE_HRP'
                Arr = LatestChars.Split(",")
                HR_Delay = Arr(0)
                HR_Period = Arr(1)
                AVE_HRP = Arr(2)

                If HR_Period <> 0 Then  'Display current heart rate in HR_Lbl text box
                    PulseLbl.Text = "Pulse Detected"
                    HR_Lbl.Text = Int(60000 / HR_Period)
                Else
                    PulseLbl.Text = "Pulse"
                    HR_Lbl.Text = "N/A"
                End If

                HRD_Lbl.Text = HR_Delay 'Display HR_Delay in HRD_Lbl text box
                RichTextBox1.AppendText(LatestChars) ' Add received serial data to the text window RichTextBox1
                RichTextBox1.ScrollToCaret()
                HB_Indicator.BackColor = Color.Red  '  Set HB_Indicator button colour red, to indicate that a pulse signal was received
                Application.DoEvents()
                Threading.Thread.Sleep(100)
                HB_Indicator.BackColor = Color.FromArgb(224, 224, 224)  '  Set HB_Indicator button colour back to grey after 100mS

                LatestChars = ""
                newLineReceived = False
            End If
        Catch ex As Exception

        End Try
    End Sub

    '   If SPACEBAR pressed and CollectingData = True then handle the data received from the black box
    Private Sub Form1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown, StopDataCollectionBtn.KeyDown
        If (e.KeyCode = Keys.Space) And (CollectingData = True) Then
            CollectingData = False

            If StopDataCollectionBtn.Focused Then e.Handled = True

            Dim tempHR_Delay As String = HR_Delay
            Dim tempAVE_HRPeriod As String = AVE_HRP

            If Training = False Then    'Wait until data received from black box
                Do
                    Application.DoEvents()
                Loop Until newLineReceived = True
                Do
                    Application.DoEvents()
                Loop Until newLineReceived = False  '*** Wait for last R-R time period data to be measured (HR period in which the spacebar was pressed! ***

                'Store received trial data into arrays
                HR_Period_Val(Trial) = HR_Period
                HR_Val(Trial) = HR_Lbl.Text
            End If

            SerialPort1.Write("AYOF" & Chr(13)) '   Tell the program within black box to pause for now
            ConfidenceForm.Show()
            Me.Hide()
            Do  'Wait until confidence value comes back from Form2
                Application.DoEvents()
            Loop Until ConfidenceValueSet = True
            ConfidenceValueSet = False
            Me.Show()

            SerialPort1.DiscardOutBuffer()  'Clear serial output buffer

            Static start As Single
            start = VB.Timer()  '   Wait 1 second, for data to be send and received by black box
            Do While VB.Timer() < start + 1
                System.Windows.Forms.Application.DoEvents()
            Loop

            SerialPort1.Write("AYON AYON AYON" & Chr(13)) '   Start program within black box

            start = VB.Timer()  '   Wait 1 second, for data to be sent from PC and received by black box
            Do While VB.Timer() < start + 1
                System.Windows.Forms.Application.DoEvents()
            Loop

            SerialPort1.DiscardInBuffer()

            If Trial < LastTrial Then UserInfoLbl.Text = "Data Logged!"

            AVE_HRPeriod(Trial) = tempAVE_HRPeriod
            HR_Delay_Val(Trial) = tempHR_Delay
            Confidence_Val(Trial) = ConfidenceValue

            ' Store received data into Excel data file that was created earlier

            Dim FSi1 As New System.IO.StreamWriter(Foldername & "\" & subjectName & Subject & ".csv", True)
            If Training = True Then
                FSi1.WriteLine(Trial & "," & HR_Delay_Val(Trial) & "," & Confidence_Val(Trial))
            Else
                FSi1.WriteLine(Trial & "," & HR_Val(Trial) & ", " & HR_Period_Val(Trial) & "," & AVE_HRPeriod(Trial) & "," & HR_Delay_Val(Trial) & "," & Confidence_Val(Trial))
            End If
            FSi1.Close()
            FSi1.Dispose()

            If Trial >= LastTrial Then  'If all data is collected, display messages to that effect

                UserInfoLbl.Text = "All Trials Collected!"
                If Training = True Then
                    MsgBox("All training trials have now been collected for this subject" & vbCrLf & "Please click 'StartTrials' to test this subject", vbOKOnly, "Training Completed")
                Else
                    MsgBox("All trials have now been collected for this subject" & vbCrLf & "Please click 'StartTraining' to test a new subject", vbOKOnly, "Trial Data Collection Completed")
                End If
                SerialPort1.Write("AYOF" & Chr(13))
                CollectingData = False
                UserInfoLbl.Text = ""
                StartTrainingToolStripMenuItem.Enabled = True
                StartToolStripMenuItem.Enabled = True
                StopDataCollectionBtn.Enabled = False
                Exit Sub
            End If

            Trial += 1
            TrialNumberLbl.Text = Trial
            UserInfoTmr.Enabled = True

            CollectingData = True
        End If
    End Sub


    Private Sub StartNewSubjectToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NS.Click, NAS.Click, TR.Click, ATR.Click, ASTR.Click
        menuItem = sender.ToString
        Select Case (menuItem)
            Case Is = "Start New Subject Training"              'Training Subject
                Training = True
                subjectName = "TR_Subject"
            Case Is = "Start New Subject (Absolute) Training"             '(Absolute) Training Subject
                Training = True
                subjectName = "ATR_Subject"
            Case Is = "Start New Subject (Absolute Signed) Training"            '(Absolute Signed) Training Subject
                Training = True
                subjectName = "ASTR_Subject"
            Case Is = "Start New Subject"              'New Subject
                Training = False
                subjectName = "NS_Subject"
            Case Is = "Start New Subject (Absolute)"             'New Absolute Subject
                Training = False
                subjectName = "NAS_Subject"
        End Select

        If comPortFound = False Then    'Find available com ports
            ComTimer.Enabled = True
            Do
                Application.DoEvents()
            Loop Until comPortFound = True
            'Else
            '    Select Case (sender.ToString)
            '        Case Is = "Start New Subject (Training)"              'Training Subject
            '    SerialPort1.Write("AYTR1" & Chr(13))
            '         Case Is = "Start New Subject (ABS Training)"             '(Absolute) Training Subject
            '     SerialPort1.Write("AYTR2" & Chr(13))
            '         Case Is = "Start New Subject (ABSS Training)"            '(Absolute Signed) Training Subject
            '    SerialPort1.Write("AYTR3" & Chr(13))
            '        Case Is = "Start New Subject"              'New Subject
            '    SerialPort1.Write("AYNT1" & Chr(13))
            '        Case Is = "Start New Subject (ABS)"             'New Absolute Subject
            '    SerialPort1.Write("AYNT2" & Chr(13))
            '    End Select
        End If

        Subject = InputBox("Subject Number", "Please enter a participant number")

        'Check to see if file exists. If it doesn't, create a blank subject with title row showing data columns. If it does, delete the data file & create a new one with no data, just the title row

        If System.IO.File.Exists(Foldername & "\" & subjectName & Subject & ".csv") = True Then
            Select Case MsgBox("A file for that subject already exists! Do you wish to overwrite this data?", MsgBoxStyle.Exclamation + vbYesNoCancel, "Subject Exists Already!")
                Case vbYes
                    System.IO.File.Delete(Foldername & "\" & subjectName & Subject & ".csv")
                Case vbNo
                    Exit Sub
                Case vbCancel
                    If SerialPort1.IsOpen = True Then SerialPort1.Close()
                    Exit Sub
            End Select
        End If

        SerialPort1.Write("AYT0" & Chr(13)) 'Reset trial number to 1 (in the black box)

        '  Create new Excel data file for data, for training/real trials

        Dim FSi1 As New System.IO.StreamWriter(Foldername & "\" & subjectName & Subject & ".csv", True)
        If Training = True Then
            FSi1.WriteLine("Trial,Delay/mS,ConfidenceValue")
        Else
            FSi1.WriteLine("Trial,HRate,HR_Period/mS,AVE_HR_Period/mS,Delay/mS,ConfidenceValue")
        End If
        FSi1.Close()
        FSi1.Dispose()
        Trial = 1

        ReadPort = True
        TrialNumberLbl.Text = 1

        SerialPort1.DiscardInBuffer()

        ReadPort = True
        StopDataCollectionBtn.Enabled = True
        UserInfoLbl.Text = "Press Space Bar to log value"
        CollectingData = True
        StartTrainingToolStripMenuItem.Enabled = False
        StartToolStripMenuItem.Enabled = False
    End Sub


    'Stop collecting data if stop button presses
    Private Sub StopTrialBtn_Click(sender As System.Object, e As System.EventArgs) Handles StopDataCollectionBtn.Click
        SerialPort1.DiscardOutBuffer()
        CollectingData = False
        SerialPort1.Write("AYOF" & Chr(13)) 'Stop tone from playing (from the black box!)
        UserInfoLbl.Text = ""
        StartTrainingToolStripMenuItem.Enabled = True
        StartToolStripMenuItem.Enabled = True
        StopDataCollectionBtn.Enabled = False
    End Sub

    ' Display the message "Press Space Bar to log value" in UserInfoTmr textbox
    Private Sub UserInfoTmr_Tick(sender As System.Object, e As System.EventArgs) Handles UserInfoTmr.Tick
        UserInfoTmr.Enabled = False
        UserInfoLbl.Text = "Press Space Bar to log value"
    End Sub


    'Use ComTimer timer tick procedure to check to see whether com ports exist
    Private Sub ComTimer_Tick(sender As System.Object, e As System.EventArgs) Handles ComTimer.Tick
        If (comNum > 0) And (SerialPort1.IsOpen = True) Then SerialPort1.Close()

        comNum += 1
        If comNum > 50 Then '   Quit procedure and feedback message to say black box is not connected or detected by the PC!
            ComTimer.Enabled = False
            MsgBox("Hardware not connected or not reset since last use!" & vbCrLf & "Please unplug and reconnect the hardware to the computer, then re-run this program.", vbOKOnly, "Hardware Not Detected!")
            SerialPort1.Close()
            SerialPort1.Dispose()
            End
            Exit Sub
        End If

        Try '   If com port exists, try sending message to black box to begin training / real trials
            SerialPort1.PortName = "COM" & comNum
            SerialPort1.Open()
            ComTimer.Interval = 2000

            Select Case (menuItem)
                Case Is = "Start New Subject (Training)"              'Training Subject
                    SerialPort1.Write("AYTR1" & Chr(13))
                Case Is = "Start New Subject (ABS Training)"             '(Absolute) Training Subject
                    SerialPort1.Write("AYTR2" & Chr(13))
                Case Is = "Start New Subject (ABSS Training)"            '(Absolute Signed) Training Subject
                    SerialPort1.Write("AYTR3" & Chr(13))
                Case Is = "Start New Subject"              'New Subject
                    SerialPort1.Write("AYNT1" & Chr(13))
                Case Is = "Start New Subject (ABS)"             'New Absolute Subject
                    SerialPort1.Write("AYNT2" & Chr(13))
            End Select

        Catch
            ComTimer.Interval = 100 '50
        End Try
    End Sub


    '   Exit program by closing form
    Private Sub Form1_FormClosing(sender As System.Object, e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        Try
            SerialPort1.Write("AYEX" & Chr(13)) 'Re-boot black box!
            exitTimer.Enabled = True
        Catch ex As Exception
            End
        End Try
    End Sub


    '   Exit program by clicking 'Exit'
    Private Sub ExitProgramToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitProgramToolStripMenuItem.Click
        Try
            SerialPort1.Write("AYEX" & Chr(13)) 'Re-boot black box!
            exitTimer.Enabled = True
        Catch ex As Exception
            End
        End Try
    End Sub


    '   Exit program
    Private Sub exitTimer_Tick(sender As System.Object, e As System.EventArgs) Handles exitTimer.Tick
        Try
            SerialPort1.Close()
            SerialPort1.Dispose()
        Catch ex As Exception

        End Try
        End
    End Sub

End Class
