<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.SerialPort1 = New System.IO.Ports.SerialPort(Me.components)
        Me.PumpMenu = New System.Windows.Forms.MenuStrip()
        Me.StartTrainingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TrainingStartToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StartToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StartNewSubjectToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutThisProgramToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitProgramToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.RichTextBox1 = New System.Windows.Forms.RichTextBox()
        Me.StopDataCollectionBtn = New System.Windows.Forms.Button()
        Me.HeartRateLbl = New System.Windows.Forms.Label()
        Me.DelayLbl = New System.Windows.Forms.Label()
        Me.HRD_Lbl = New System.Windows.Forms.Label()
        Me.HR_Lbl = New System.Windows.Forms.Label()
        Me.HB_Indicator = New System.Windows.Forms.Button()
        Me.UserInfoTmr = New System.Windows.Forms.Timer(Me.components)
        Me.CurrentTrialLbl = New System.Windows.Forms.Label()
        Me.ComTimer = New System.Windows.Forms.Timer(Me.components)
        Me.Label3 = New System.Windows.Forms.Label()
        Me.PulseLbl = New System.Windows.Forms.Label()
        Me.TrialNumberLbl = New System.Windows.Forms.Label()
        Me.UserInfoLbl = New System.Windows.Forms.Label()
        Me.exitTimer = New System.Windows.Forms.Timer(Me.components)
        Me.PumpMenu.SuspendLayout()
        Me.SuspendLayout()
        '
        'SerialPort1
        '
        Me.SerialPort1.BaudRate = 2000000
        Me.SerialPort1.ReadBufferSize = 100
        Me.SerialPort1.ReadTimeout = 2000
        Me.SerialPort1.WriteBufferSize = 64
        '
        'PumpMenu
        '
        Me.PumpMenu.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PumpMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StartTrainingToolStripMenuItem, Me.StartToolStripMenuItem, Me.AboutToolStripMenuItem, Me.ExitToolStripMenuItem})
        Me.PumpMenu.Location = New System.Drawing.Point(0, 0)
        Me.PumpMenu.Name = "PumpMenu"
        Me.PumpMenu.Size = New System.Drawing.Size(622, 30)
        Me.PumpMenu.TabIndex = 18
        Me.PumpMenu.Text = "MenuStrip1"
        '
        'StartTrainingToolStripMenuItem
        '
        Me.StartTrainingToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TrainingStartToolStripMenuItem})
        Me.StartTrainingToolStripMenuItem.Font = New System.Drawing.Font("Arial", 14.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.StartTrainingToolStripMenuItem.Name = "StartTrainingToolStripMenuItem"
        Me.StartTrainingToolStripMenuItem.Size = New System.Drawing.Size(132, 26)
        Me.StartTrainingToolStripMenuItem.Text = "Start Training"
        '
        'TrainingStartToolStripMenuItem
        '
        Me.TrainingStartToolStripMenuItem.Name = "TrainingStartToolStripMenuItem"
        Me.TrainingStartToolStripMenuItem.Size = New System.Drawing.Size(230, 26)
        Me.TrainingStartToolStripMenuItem.Text = "Start New Subject"
        '
        'StartToolStripMenuItem
        '
        Me.StartToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StartNewSubjectToolStripMenuItem})
        Me.StartToolStripMenuItem.Font = New System.Drawing.Font("Arial", 14.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.StartToolStripMenuItem.Name = "StartToolStripMenuItem"
        Me.StartToolStripMenuItem.Size = New System.Drawing.Size(111, 26)
        Me.StartToolStripMenuItem.Text = "Start Trials"
        '
        'StartNewSubjectToolStripMenuItem
        '
        Me.StartNewSubjectToolStripMenuItem.Name = "StartNewSubjectToolStripMenuItem"
        Me.StartNewSubjectToolStripMenuItem.Size = New System.Drawing.Size(230, 26)
        Me.StartNewSubjectToolStripMenuItem.Text = "Start New Subject"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AboutThisProgramToolStripMenuItem})
        Me.AboutToolStripMenuItem.Font = New System.Drawing.Font("Arial", 14.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(72, 26)
        Me.AboutToolStripMenuItem.Text = "About"
        '
        'AboutThisProgramToolStripMenuItem
        '
        Me.AboutThisProgramToolStripMenuItem.Name = "AboutThisProgramToolStripMenuItem"
        Me.AboutThisProgramToolStripMenuItem.Size = New System.Drawing.Size(249, 26)
        Me.AboutThisProgramToolStripMenuItem.Text = "About This Program"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ExitToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExitProgramToolStripMenuItem})
        Me.ExitToolStripMenuItem.Font = New System.Drawing.Font("Arial", 14.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(53, 26)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'ExitProgramToolStripMenuItem
        '
        Me.ExitProgramToolStripMenuItem.Name = "ExitProgramToolStripMenuItem"
        Me.ExitProgramToolStripMenuItem.Size = New System.Drawing.Size(189, 26)
        Me.ExitProgramToolStripMenuItem.Text = "Exit Program"
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 200
        '
        'RichTextBox1
        '
        Me.RichTextBox1.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.RichTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.RichTextBox1.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RichTextBox1.Location = New System.Drawing.Point(400, 70)
        Me.RichTextBox1.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.RichTextBox1.Name = "RichTextBox1"
        Me.RichTextBox1.Size = New System.Drawing.Size(211, 334)
        Me.RichTextBox1.TabIndex = 19
        Me.RichTextBox1.Text = ""
        '
        'StopDataCollectionBtn
        '
        Me.StopDataCollectionBtn.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.StopDataCollectionBtn.Enabled = False
        Me.StopDataCollectionBtn.FlatAppearance.BorderColor = System.Drawing.Color.Black
        Me.StopDataCollectionBtn.FlatAppearance.BorderSize = 3
        Me.StopDataCollectionBtn.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.StopDataCollectionBtn.Location = New System.Drawing.Point(14, 336)
        Me.StopDataCollectionBtn.Margin = New System.Windows.Forms.Padding(2, 3, 2, 3)
        Me.StopDataCollectionBtn.Name = "StopDataCollectionBtn"
        Me.StopDataCollectionBtn.Size = New System.Drawing.Size(373, 67)
        Me.StopDataCollectionBtn.TabIndex = 39
        Me.StopDataCollectionBtn.Text = "Stop Data Collection"
        Me.StopDataCollectionBtn.UseVisualStyleBackColor = False
        '
        'HeartRateLbl
        '
        Me.HeartRateLbl.AutoSize = True
        Me.HeartRateLbl.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HeartRateLbl.Location = New System.Drawing.Point(14, 171)
        Me.HeartRateLbl.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.HeartRateLbl.Name = "HeartRateLbl"
        Me.HeartRateLbl.Size = New System.Drawing.Size(134, 27)
        Me.HeartRateLbl.TabIndex = 40
        Me.HeartRateLbl.Text = "Heart Rate:"
        '
        'DelayLbl
        '
        Me.DelayLbl.AutoSize = True
        Me.DelayLbl.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DelayLbl.Location = New System.Drawing.Point(14, 228)
        Me.DelayLbl.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.DelayLbl.Name = "DelayLbl"
        Me.DelayLbl.Size = New System.Drawing.Size(80, 27)
        Me.DelayLbl.TabIndex = 41
        Me.DelayLbl.Text = "Delay:"
        '
        'HRD_Lbl
        '
        Me.HRD_Lbl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.HRD_Lbl.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HRD_Lbl.Location = New System.Drawing.Point(197, 230)
        Me.HRD_Lbl.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.HRD_Lbl.Name = "HRD_Lbl"
        Me.HRD_Lbl.Size = New System.Drawing.Size(188, 27)
        Me.HRD_Lbl.TabIndex = 43
        Me.HRD_Lbl.Text = "N/A"
        '
        'HR_Lbl
        '
        Me.HR_Lbl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.HR_Lbl.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HR_Lbl.Location = New System.Drawing.Point(197, 173)
        Me.HR_Lbl.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.HR_Lbl.Name = "HR_Lbl"
        Me.HR_Lbl.Size = New System.Drawing.Size(188, 27)
        Me.HR_Lbl.TabIndex = 42
        Me.HR_Lbl.Text = "N/A"
        '
        'HB_Indicator
        '
        Me.HB_Indicator.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.HB_Indicator.Enabled = False
        Me.HB_Indicator.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.HB_Indicator.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HB_Indicator.Location = New System.Drawing.Point(197, 287)
        Me.HB_Indicator.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.HB_Indicator.Name = "HB_Indicator"
        Me.HB_Indicator.Size = New System.Drawing.Size(188, 24)
        Me.HB_Indicator.TabIndex = 44
        Me.HB_Indicator.UseVisualStyleBackColor = False
        '
        'UserInfoTmr
        '
        Me.UserInfoTmr.Interval = 1000
        '
        'CurrentTrialLbl
        '
        Me.CurrentTrialLbl.AutoSize = True
        Me.CurrentTrialLbl.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CurrentTrialLbl.Location = New System.Drawing.Point(14, 115)
        Me.CurrentTrialLbl.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.CurrentTrialLbl.Name = "CurrentTrialLbl"
        Me.CurrentTrialLbl.Size = New System.Drawing.Size(159, 27)
        Me.CurrentTrialLbl.TabIndex = 46
        Me.CurrentTrialLbl.Text = "Current Trial: "
        '
        'ComTimer
        '
        '
        'Label3
        '
        Me.Label3.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.Label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label3.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.Label3.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(400, 44)
        Me.Label3.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(210, 24)
        Me.Label3.TabIndex = 47
        Me.Label3.Text = "Data received from hardware"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PulseLbl
        '
        Me.PulseLbl.AutoSize = True
        Me.PulseLbl.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PulseLbl.Location = New System.Drawing.Point(14, 285)
        Me.PulseLbl.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.PulseLbl.Name = "PulseLbl"
        Me.PulseLbl.Size = New System.Drawing.Size(176, 27)
        Me.PulseLbl.TabIndex = 48
        Me.PulseLbl.Text = "Pulse Detected"
        '
        'TrialNumberLbl
        '
        Me.TrialNumberLbl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TrialNumberLbl.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TrialNumberLbl.Location = New System.Drawing.Point(198, 116)
        Me.TrialNumberLbl.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.TrialNumberLbl.Name = "TrialNumberLbl"
        Me.TrialNumberLbl.Size = New System.Drawing.Size(188, 27)
        Me.TrialNumberLbl.TabIndex = 49
        Me.TrialNumberLbl.Text = "N/A"
        '
        'UserInfoLbl
        '
        Me.UserInfoLbl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.UserInfoLbl.Font = New System.Drawing.Font("Arial", 20.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UserInfoLbl.ForeColor = System.Drawing.Color.Red
        Me.UserInfoLbl.Location = New System.Drawing.Point(14, 44)
        Me.UserInfoLbl.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.UserInfoLbl.Name = "UserInfoLbl"
        Me.UserInfoLbl.Size = New System.Drawing.Size(373, 42)
        Me.UserInfoLbl.TabIndex = 50
        Me.UserInfoLbl.Text = "Press Space Bar to log value"
        Me.UserInfoLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'exitTimer
        '
        Me.exitTimer.Interval = 1000
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(622, 420)
        Me.Controls.Add(Me.UserInfoLbl)
        Me.Controls.Add(Me.TrialNumberLbl)
        Me.Controls.Add(Me.PulseLbl)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.CurrentTrialLbl)
        Me.Controls.Add(Me.HB_Indicator)
        Me.Controls.Add(Me.HRD_Lbl)
        Me.Controls.Add(Me.HR_Lbl)
        Me.Controls.Add(Me.DelayLbl)
        Me.Controls.Add(Me.HeartRateLbl)
        Me.Controls.Add(Me.StopDataCollectionBtn)
        Me.Controls.Add(Me.RichTextBox1)
        Me.Controls.Add(Me.PumpMenu)
        Me.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Heart Beat Perception Study 2023"
        Me.PumpMenu.ResumeLayout(False)
        Me.PumpMenu.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents SerialPort1 As System.IO.Ports.SerialPort
    Friend WithEvents PumpMenu As System.Windows.Forms.MenuStrip
    Friend WithEvents AboutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AboutThisProgramToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitProgramToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents RichTextBox1 As System.Windows.Forms.RichTextBox
    Friend WithEvents StopDataCollectionBtn As System.Windows.Forms.Button
    Friend WithEvents HeartRateLbl As System.Windows.Forms.Label
    Friend WithEvents DelayLbl As System.Windows.Forms.Label
    Friend WithEvents HRD_Lbl As System.Windows.Forms.Label
    Friend WithEvents HR_Lbl As System.Windows.Forms.Label
    Friend WithEvents HB_Indicator As System.Windows.Forms.Button
    Friend WithEvents UserInfoTmr As System.Windows.Forms.Timer
    Friend WithEvents CurrentTrialLbl As System.Windows.Forms.Label
    Friend WithEvents ComTimer As System.Windows.Forms.Timer
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents PulseLbl As System.Windows.Forms.Label
    Friend WithEvents TrialNumberLbl As System.Windows.Forms.Label
    Friend WithEvents StartToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StartNewSubjectToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents UserInfoLbl As System.Windows.Forms.Label
    Friend WithEvents StartTrainingToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TrainingStartToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitTimer As System.Windows.Forms.Timer

End Class
