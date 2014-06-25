'Verweis Microsoft.VisualBasic entfernt
'Helpers Projekt bereitgestellt von ErfinderDesRades
'Projekt zeigt wie die BindingSurce des DataSet auf verschiede Forms umgehängt werden kann
Option Strict On
Option Explicit On

Imports System.IO
Imports TestHelpers.DataSet1

Public Class Form1

    Private _Kundendaten As New FileInfo("Kundendaten.xml")

    Sub New()
        InitializeComponent()
        DataSet1.DataFile(_Kundendaten.FullName).Register(Me, True).Fill()
    End Sub

    Private Sub Form2ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles Form2ToolStripMenuItem.Click
        Form2.Show(Me)
    End Sub

    Private Sub BeendenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BeendenToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub SpeichernToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SpeichernToolStripMenuItem.Click
        DataSet1.Save(Me)
        MessageBox.Show("Daten gespeichert")
    End Sub
End Class
