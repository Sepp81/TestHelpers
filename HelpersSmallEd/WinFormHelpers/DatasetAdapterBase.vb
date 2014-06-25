Imports System.ComponentModel
Imports System.Windows.Forms

Namespace System.Data

   ''' <summary>some common basics of DatasetXmlAdapter and DatasetAdapter</summary>
   Public MustInherit Class DatasetAdapterBase

      Public Shared ReadOnly Attachs As New ComponentProperty(Of DataSet, DatasetAdapterBase)

      Protected _DataSet As DataSet
      Public HasChanges As Func(Of Boolean)
      Public ReadOnly HandleFormClosing As FormClosingEventHandler = AddressOf _HandleFormClosing


      Protected MustOverride Sub _Save(attaches As List(Of DataTableAttach))
      'Protected MustOverride Sub _Save( tbls As IEnumerable(Of DataTable))

      Protected MustOverride Sub _Fill(tbls As IEnumerable(Of DataTable))

      Public Sub Fill(tbls As Collections.Generic.IEnumerable(Of DataTable))
         '_DataSet.Clear()
         _Fill(tbls)
      End Sub
      Public Overridable Property DataSet() As DataSet
         Get
            Return _DataSet
         End Get
         Set(value As DataSet)
            If _DataSet Is value Then Return
            _DataSet = value
            HasChanges = AddressOf _HasChanges
         End Set
      End Property
      Public Sub Save(form As Form, Optional checkForChanges As Boolean = True)
         If form.NotNull AndAlso Not form.Validate() Then
            System.Media.SystemSounds.Hand.Play()
            Return
         End If
         Dim atts = _DataSet.GetRankedTables.ConvertAll(Function(tb) tb.Attach)
         For Each bs In TableAttachs(atts, Persistance.YesOrGuess).SelectMany(Function(att) att.BindingSources)
            bs.EndEdit()
         Next
         System.Media.SystemSounds.Asterisk.Play()
         If checkForChanges AndAlso Not HasChanges.Invoke() Then Return
         _Save(atts)
      End Sub
      Protected Function TableAttachs(persistance As Persistance) As IEnumerable(Of DataTableAttach)
         Return From tb In _DataSet.GetRankedTables Let att = tb.Attach Where 0 <> (att.Persistance And persistance) Select att
      End Function
      Protected Function TableAttachs(attachs As IEnumerable(Of DataTableAttach), persistance As Persistance) _
            As IEnumerable(Of DataTableAttach)
         Return From att In attachs Where 0 <> (att.Persistance And persistance)
      End Function
      Private Function _HasChanges() As Boolean
         Return TableAttachs(Persistance.YesOrGuess).SelectMany(Function(att) att.Table.ChangedRows).FirstOrDefault.NotNull
      End Function

      Private Sub _HandleFormClosing(sender As Object, e As CancelEventArgs)
         If e.Cancel Then Return
         Dim frm = DirectCast(sender, Form)
         If Not HasChanges.Invoke() Then Return
         Select Case MessageBox.Show(frm, "Änderungen speichern?", "Das Ende ist nahe", MessageBoxButtons.YesNoCancel)
            Case Windows.Forms.DialogResult.Yes
               Save(frm, False)
            Case Windows.Forms.DialogResult.Cancel
               e.Cancel = True
               Return
         End Select
         e.Cancel = False
      End Sub

   End Class

End Namespace
