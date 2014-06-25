'Verweis Microsoft.VisualBasic entfernt
Option Strict On
Option Explicit On

Imports TestHelpers.DataSet1
Imports System.IO

Public Class Form2

    Sub New()
        InitializeComponent()
        DataSet1.Register(Me, False)
    End Sub

    Private Sub SchliessenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SchliessenToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class