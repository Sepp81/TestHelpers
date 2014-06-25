Imports System.IO
Imports System.ComponentModel

Namespace System.Data

   Public Class DatasetXmlAdapter : Inherits DatasetAdapterBase

      Public DataFile As FileInfo

      Public Sub New(_Dts As DataSet)
         DataSet = _Dts
#If DEBUG Then
         DataFile = New FileInfo("..\..\").Combine(_Dts.GetType.Name & ".xml")
#Else
         DataFile = New FileInfo(Application.LocalUserAppDataPath).Combine(_Dts.GetType.Name & ".xml")
#End If
      End Sub

      Protected Overrides Sub _Save(attaches As List(Of DataTableAttach))
         TableAttachs(attaches, Persistance.No).ForEach(Sub(attch) attch.Table.ClearRecursive())
         Synchronisize(FileAccess.Write)
         attaches.ForEach(Sub(att) AcceptChangesRowwise(att.Table.Rows))
      End Sub

      Protected Overrides Sub _Fill(tbls As Collections.Generic.IEnumerable(Of DataTable))
         If tbls.FirstOrDefault.NotNull Then Throw New NotSupportedException( _
               "Bei DatasetOnly werden immer alle Tabellen befüllt - bestimmte Tabellen anzugeben ist sinnlos")
         DataFile.Refresh()
         If DataFile.Exists() Then
            Dim tblls = _DataSet.Tables
            For Each tb As DataTable In DataSet.Tables
               tb.Attach.EnableComplete(False, True)
            Next
            _DataSet.Clear()
            Synchronisize(FileAccess.Read)
            For Each tb As DataTable In DataSet.Tables
               tb.Attach.EnableComplete(True, True)
               AcceptChangesRowwise(tb.Rows)
            Next
            DataSet.EnforceConstraints = True 'tb.BeginLoadData setzt  EnforceConstraints.False 
         Else
            MessageBox.Show(String.Concat( _
               "Leider (noch) kein DatenFile vorhanden", Lf, _
               "Sie können aber trotzdem fortfahren, und eines anlegen."))
         End If
      End Sub

      Private Sub AcceptChangesRowwise(rows As DataRowCollection)
         For i = rows.Count - 1 To 0 Step -1
            With rows(i)
               'nicht auf DataRowState.Unchanged oder DataRowState.Detached anwenden
               If .RowState >= DataRowState.Added Then .AcceptChanges()
            End With
         Next
      End Sub

      Private Sub Synchronisize(access As FileAccess)
         If DataFile.Extension.ToLower = ".txt" Then
            'If access = FileAccess.Read Then
            '   DataSet.LoadCompact(DataFile.FullName)
            'Else
            '   DataSet.SaveCompact(DataFile.FullName)
            'End If
         Else
            If access = FileAccess.Read Then
               DataSet.ReadXml(DataFile.FullName)
            Else
               DataSet.WriteXml(DataFile.FullName)
            End If
         End If
      End Sub

   End Class

End Namespace