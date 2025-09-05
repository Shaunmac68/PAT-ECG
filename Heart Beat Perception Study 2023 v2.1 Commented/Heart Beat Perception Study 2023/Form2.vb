Public Class ConfidenceForm

    Dim showNow As Boolean

    Private Sub ConfidenceForm_KeyDown(sender As System.Object, e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown, TrackBar1.KeyDown
        If (e.KeyCode = Keys.Space) And (showNow = True) Then
            Form1.ConfidenceValue = TrackBar1.Value
            Form1.ConfidenceValueSet = True
            Me.Enabled = False
            Me.Hide()
            showNow = False
            Form1.Show()
            Me.Close()
        End If
    End Sub

    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        Me.Enabled = True
        Timer1.Enabled = False
        showNow = True
    End Sub

    Private Sub ConfidenceForm_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Timer1.Enabled = True
        showNow = False
    End Sub

End Class