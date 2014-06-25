Imports System.Runtime.CompilerServices
Imports System.ComponentModel
Imports System.Data.Common
Imports System.IO
Imports System.Text

Namespace System.Data

   Public Module DatasetX

      <Extension()> _
      Public Function GetRankedTables(dts As DataSet) As List(Of DataTable)
         Return dts.Tables.Cast(Of DataTable).GetRankedTables
      End Function
      <Extension()> _
      Public Function GetRankedTables(dts As IEnumerable(Of DataTable)) As List(Of DataTable)
         'Bevor ein Node aus dem Hashset in die Ergebnisliste verschoben wird,
         ' werden rekursiv all seine Vorgänger verschoben
         Dim tableSrc = New HashSet(Of DataTable)(dts)
         GetRankedTables = New List(Of DataTable)(tableSrc.Count)
         Dim recurse As Action(Of DataTable) = Nothing
         recurse = Sub(tb)
                      If Not tableSrc.Remove(tb) Then Return 'gegen Kreisbezüge sichern
                      tb.ParentTables.ForEach(recurse)
                      GetRankedTables.Add(tb)
                   End Sub
         While tableSrc.Count > 0
            recurse(tableSrc.First)
         End While
      End Function

#Region "ReadWriteCompact"

#End Region 'ReadWriteCompact
   End Module

End Namespace
