Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports System.IO

Namespace System.Data

   Public Module DataTableX

      <Extension()> _
      Public Function ChangedRows(table As DataTable) As IEnumerable(Of DataRow)
         'DataRowState.Unchanged und DataRowState.Detached ausschließen
         Return From rw In table.Rows.Cast(Of DataRow)() Where rw.RowState >= DataRowState.Added
      End Function

      <Extension()> _
      Public Sub CopyTo(Of T As DataRow)(subj As T, other As T)
         Dim itms = subj.ItemArray
         If other.RowState <> DataRowState.Detached Then
            Dim tb = other.Table
            For Each indx In tb.PrimaryKey.Select(Function(cl) cl.Ordinal)
               itms(indx) = other(indx)
            Next
         End If
         other.ItemArray = itms
      End Sub

      <Extension()> _
      Public Function All(Of T As DataRow)(subj As TypedTableBase(Of T)) As IEnumerable(Of T)
         Return From rw In subj Where 0 = (rw.RowState And (DataRowState.Deleted Or DataRowState.Detached))
      End Function
      <Extension()> _
      Public Function All(subj As DataTable) As IEnumerable(Of DataRow)
         Return From rw In subj Where 0 = (rw.RowState And (DataRowState.Deleted Or DataRowState.Detached))
      End Function

      <Extension()> _
      Public Function ChildRels(subj As DataTable) As IEnumerable(Of DataRelation)
         Return subj.ChildRelations.Cast(Of DataRelation)()
      End Function

      <Extension()> _
      Public Function ParentRels(subj As DataTable) As IEnumerable(Of DataRelation)
         Return subj.ParentRelations.Cast(Of DataRelation)()
      End Function

      <Extension()> _
      Public Function ChildTables(subj As DataTable) As IEnumerable(Of DataTable)
         Return From rl In subj.ChildRelations.Cast(Of DataRelation)() _
            Where rl.ChildKeyConstraint.NotNull Select rl.ChildTable
         Return subj.ChildRelations.Cast(Of DataRelation).Select(Function(rl) rl.ChildTable)
      End Function

      <Extension()> _
      Public Function ParentTables(subj As DataTable) As IEnumerable(Of DataTable)
         Return subj.ParentRelations.Cast(Of DataRelation).Select(Function(rl) rl.ParentTable)
      End Function

      Private _ClearingTables As New HashSet(Of DataTable)
      <Extension()> _
      Public Sub ClearRecursive(subj As DataTable)
         If subj.Rows.Count = 0 OrElse Not _ClearingTables.Add(subj) Then Return
         subj.ChildTables.ForEach(AddressOf ClearRecursive)
         _ClearingTables.Remove(subj)
         subj.Clear()
      End Sub

      ''' <summary>eases to implement standard-reactions on Row -init, -change, -delete</summary>
      <Extension()> _
      Public Sub WireUp(Of T As DataRow)(table As TypedTableBase(Of T), Optional onNewRow As Action(Of T) = Nothing, Optional onChange As Action(Of T) = Nothing, Optional onDelete As Action(Of T) = Nothing)
         If onNewRow.NotNull() Then
            AddHandler table.TableNewRow, Sub(s, e) onNewRow(DirectCast(e.Row, T))
         End If
         If onChange.NotNull() Then
            AddHandler table.RowChanged, _
               Sub(s, e)
                  If 0 < (e.Action And (DataRowAction.Add Or DataRowAction.Change)) Then
                     onChange(DirectCast(e.Row, T))
                  End If
               End Sub
         End If
         If onDelete.NotNull() Then
            AddHandler table.RowDeleting, Sub(s, e) onDelete(DirectCast(e.Row, T))
         End If
      End Sub

   End Module

End Namespace