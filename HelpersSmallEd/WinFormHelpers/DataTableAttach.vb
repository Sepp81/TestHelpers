Imports System.Runtime.CompilerServices
Imports System.Windows.Forms
Imports System.Collections.Generic
Imports System.Diagnostics

Namespace System.Data
   <Flags()> _
   Public Enum Persistance As Integer : No = 1 : Yes = 2 : Guess = 4 : YesOrGuess = 6 : End Enum

   ''' <summary>stores Stuff to suspend databinding-events</summary>
   <DebuggerDisplay("{Table.TableName}")> _
   Public Class DataTableAttach : Implements IDisposable

      Public WithEvents Table As DataTable
      Public BindingSources As New List(Of BindingSource)
      Private _Level As Integer
      Private _DisabledChilds As New List(Of DataTableAttach)
      Public Persistance As Persistance = Data.Persistance.Guess

      Public Sub New(tb As DataTable)
         Table = tb
      End Sub

      Public Sub AddSource(bs As BindingSource)
         If BindingSources.Contains(bs) Then Throw Me.Exception( _
   "Die BindingSource mit DataMember '", bs.DataMember, _
   "' ist bereits fürs EventDisabling registriert.", _
   "Wird versucht, dasselbe Form mehrmals zu registrieren?")
         BindingSources.Add(bs)
         If _Level > 0 Then bs.RaiseListChangedEvents = False
         AddHandler bs.Disposed, AddressOf BindingSource_Disposed
      End Sub

      Private Sub BindingSource_Disposed(sender As Object, e As EventArgs)
         BindingSources.Remove(DirectCast(sender, BindingSource))
      End Sub

      Public Function Enable() As Boolean
         Return _Level = 0
      End Function
      Private Function Enable(value As Boolean, tables As Boolean, bindingSources As Boolean, resetBindings As Boolean) As DataTableAttach
         If value Then
            _Level -= 1
            If _Level = 0 Then
               If tables Then Table.EndLoadData()
               If bindingSources Then
                  For Each bs In Me.BindingSources
                     bs.RaiseListChangedEvents = True
                     If resetBindings Then bs.ResetBindings(False)
                  Next
               End If
            End If
         Else
            _Level += 1
            If _Level = 1 Then
               If bindingSources Then
                  For Each bs In Me.BindingSources
                     bs.RaiseListChangedEvents = False
                  Next
               End If
               If tables Then Table.BeginLoadData()
            End If
         End If
         Return Me
      End Function

      Public Function EnableComplete(value As Boolean, Optional resetBindings As Boolean = True) As DataTableAttach
         Return Enable(value, True, True, resetBindings)
      End Function
      Public Function EnableBindingSources(value As Boolean, Optional resetBindings As Boolean = True) As DataTableAttach
         Return Enable(value, False, True, resetBindings)
      End Function

      Protected Overridable Sub Dispose(disposing As Boolean)
         If Not disposing Then Return
         If BindingSources Is Nothing Then Return
         For Each bs In BindingSources
            RemoveHandler bs.Disposed, AddressOf BindingSource_Disposed
         Next
         BindingSources = Nothing
         Table = Nothing
      End Sub

      Public Sub Dispose() Implements IDisposable.Dispose
         Dispose(True)
         GC.SuppressFinalize(Me)
      End Sub

      Private Sub _Table_Disposed(sender As Object, e As EventArgs) Handles Table.Disposed
         Me.Dispose()
      End Sub


      Private Sub Table_RowDeleting(sender As Object, e As DataRowChangeEventArgs) Handles Table.RowDeleting
         Dim getChilds As GetChilds(Of DataTable) = Function(tb) tb.ChildTables
         Dim all = getChilds.AllAsList(Table.ChildTables)
         _DisabledChilds.AddRange(From tb In all Select tb.Attach.EnableBindingSources(False))
      End Sub

      Private Sub Table_RowDeleted(sender As Object, e As DataRowChangeEventArgs) Handles Table.RowDeleted
         'While _DisabledChilds.Count > 0 : _DisabledChilds.Pop.EnableBindingSources(True) : End While
         For Each ev In _DisabledChilds : ev.EnableBindingSources(True) : Next
         _DisabledChilds.Clear()
      End Sub

   End Class

End Namespace
