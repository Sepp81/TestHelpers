Imports System.Linq
Imports System.Collections
Imports System.Collections.Generic
Imports System.Data
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.ComponentModel
Imports System.Reflection

Namespace System.Windows.Forms

   Public Module BindingSourceX

      ''' <summary> editiert bs.Current im angegebenen Dialog-Form. </summary>
      <Extension()> _
      Public Function EditNew(Of TEditForm As {Form, New})(bs As BindingSource) As DialogResult
         Return EditItem(Of TEditForm)(bs, bs.AddNew)
      End Function
      ''' <summary> editiert bs.AddNew im angegebenen Dialog-Form. </summary>
      <Extension()> _
      Public Function EditCurrent(Of TEditForm As {Form, New})(bs As BindingSource) As DialogResult
         Return EditItem(Of TEditForm)(bs, bs.Current)
      End Function
      Private Function EditItem(Of T As {Form, New})(bs As BindingSource, item As Object) As DialogResult
         Dim tb = DirectCast(item, DataRowView).Row.Table
         Using frm = New T
            tb.DataSet.Register(frm, False)
            Dim allCtls = New GetChilds(Of Control)(Function(ctl) ctl.Controls).AllAsList(frm)
            Dim bindFlags As BindingFlags = BindingFlags.Instance Or BindingFlags.Public _
               Or BindingFlags.NonPublic Or BindingFlags.GetField
            For Each ctl In allCtls.Where(Function(c) TypeOf c Is ContainerControl AndAlso TypeOf c Is Form OrElse TypeOf c Is UserControl)
               For Each fld In ctl.GetType.GetFields(bindFlags).Where(Function(f) f.FieldType = GetType(BindingSource))
                  Dim bs2 = DirectCast(fld.GetValue(ctl), BindingSource)
                  If bs2 Is Nothing Then Continue For
                  If tb Is bs2.DataTable Then
                     bs2.DataSource = item
                     EditItem = frm.ShowDialog()
                     If EditItem = Windows.Forms.DialogResult.OK Then
                        bs2.EndEdit()
                        bs.ResetCurrentItem() 'den von der anneren BS geänderten Datensatz neu einlesen.
                     Else
                        bs.CancelEdit()
                     End If
                     Exit Function
                  End If
               Next
            Next
            Throw New Exception("es konnte keine geeignete BindingSource gefunden werden.".And2( _
               "\nHinweis:", _
               "\nDie Extension-Method '<BindingSource>.", (New StackTrace).GetFrame(1).GetMethod.Name, "()' funktioniert nur, wenn zuvor", _
               "\n<", tb.DataSet.GetType.Name, ">.Register(<", GetType(T).Name, ">)", _
               "\naufgerufen wurde."))
         End Using
      End Function

      ''' <summary>
      ''' provides a placeholder-syntax for filters. Sample: dv.FilterX("Datum >= ? And ? >= Datum", dtpVon.Value, dtpBis.Value)
      ''' </summary>
      ''' <exception cref="ArgumentException">expression contains more placeholders than values are passed</exception>
      <Extension()> _
      Public Sub FilterX(bs As BindingSource, expression As String, ParamArray values() As Object)
         bs.Filter = GetFilterString(expression, values)
      End Sub

      ''' <summary>
      ''' provides a placeholder-syntax for filters. Sample: dv.FilterX("Datum >= ? And ? >= Datum", dtpVon.Value, dtpBis.Value)
      ''' </summary>
      ''' <exception cref="ArgumentException">expression contains more placeholders than values are passed</exception>
      <Extension()> _
      Public Sub FilterX(dv As DataView, expression As String, ParamArray values() As Object)
         dv.RowFilter = GetFilterString(expression, values)
      End Sub

      Private Function GetFilterString(expression As String, values() As Object) As String
         Dim splits = expression.Split("?"c)      ' identify placeholder '?'
         If splits.Count > values.Length + 1 Then Throw New ArgumentException( _
            "expression contains more placeholders than values are passed")
         For i = 0 To splits.Length - 2
            Dim val = values(i)
            Dim sb = New Text.StringBuilder("{0}")
            If TypeOf (val) Is Date Then
               sb.Insert(0, "#"c).Append("#"c)
            ElseIf TypeOf (val) Is String Then
               With splits(i)
                  If .EndsWith("*") Then sb.Insert(0, "*"c) : splits(i) = .Remove(.Length - 1, 1)
               End With
               With splits(i + 1)
                  If .StartsWith("*") Then sb.Append("*"c) : splits(i + 1) = .Remove(0, 1)
               End With
               sb.Insert(0, "'"c).Append("'"c)
            End If
            splits(i) &= String.Format(Globalization.CultureInfo.InvariantCulture, sb.ToString, val)
         Next
         Return String.Concat(splits)
      End Function

      <Extension()> _
      Public Sub ListChangedEnabled(bs As BindingSource, value As Boolean)
         bs.RaiseListChangedEvents = value
         If value Then bs.ResetBindings(False)
      End Sub

      <Extension()> _
      Public Sub ChangeBinding(bs As BindingSource, dataSource As Object, datamember As String)
         With bs
            .RaiseListChangedEvents = False
            .DataSource = dataSource
            .DataMember = datamember
            .RaiseListChangedEvents = True
            .ResetBindings(False)
         End With
      End Sub

      <Extension()> _
      Public Function Dataset(bs As BindingSource) As DataSet
         Dim BindSource As BindingSource = Nothing
         Do Until bs Is Nothing
            BindSource = bs
            bs = TryCast(bs.DataSource, BindingSource)
         Loop
         Dim src = BindSource.DataSource
         Dim ds = TryCast(src, DataSet)
         If ds.NotNull Then Return ds
         Dim tb = TryCast(src, DataTable)
         If tb.NotNull Then Return tb.DataSet
         Dim rw = TryCast(src, DataRow)
         If rw.NotNull Then Return rw.Table.DataSet
         Dim drv = TryCast(src, DataRowView)
         If drv.NotNull Then Return drv.Row.Table.DataSet
         If TypeOf src Is DataView Then Throw New Exception( _
            "DatasetFromBindingSource: Dont set BindingSources DataSource on DataView. It's buggy!")
         Return Nothing
      End Function

      <Extension()> _
      Public Function DataTable(Source As BindingSource) As DataTable
         While Source.DataMember = ""
            Dim drv = TryCast(Source.DataSource, DataRowView)
            If drv.NotNull Then Return drv.Row.Table
            Source = TryCast(Source.DataSource, BindingSource)
            If Source.Null Then Return Nothing
         End While
         Dim DTS As DataSet = Dataset(Source)
         If DTS Is Nothing Then Return Nothing
         Dim SourceDataMember As String = Source.DataMember
         If DTS.Relations.Contains(SourceDataMember) Then
            Return DTS.Relations(SourceDataMember).ChildTable
         ElseIf DTS.Tables.Contains(SourceDataMember) Then
            Return DTS.Tables(SourceDataMember)
         Else
            Throw New Exception(String.Concat("Im Dataset anhand des BindingSource-Datamembers '", _
             SourceDataMember, "' keine Tabelle gefunden"))
         End If
      End Function

      <Extension()> _
      Public Function All(Of T As DataRow)(bs As BindingSource) As IEnumerable(Of T)
         Return From drv In bs.Cast(Of DataRowView)() Select DirectCast(drv.Row, T)
      End Function

      <Extension()> _
      Public Function All(Of T As DataRow)(subj As DataView) As IEnumerable(Of T)
         Return From drv In subj.Cast(Of DataRowView)() Select DirectCast(drv.Row, T)
      End Function

      ''' <summary>
      ''' returnt die Datarow am index. Bei ungültigem index Nothing (keine OutOfRange-Exception!)
      ''' </summary>
      <Extension()> _
      Public Function At(Of T As DataRow)(subj As DataView, index As Integer) As T
         If index < 0 OrElse index >= subj.Count Then Return Nothing
         Return DirectCast(DirectCast(subj(index), DataRowView).Row, T)
      End Function

      ''' <summary> returnt die typisierte Datarow an aktueller Position - oder Nothing. </summary>
      <Extension()> _
      Public Function At(Of T As DataRow)(bs As BindingSource) As T
         Return DirectCast(bs.At(bs.Position), T)
      End Function

      ''' <summary>
      ''' returnt die typisierte Datarow am index. Bei ungültigem index Nothing (keine OutOfRange-Exception!)
      ''' </summary>
      <Extension()> _
      Public Function At(Of T As DataRow)(bs As BindingSource, index As Integer) As T
         Return DirectCast(bs.At(index), T)
      End Function

      ''' <summary> returnt die untypisierte Datarow an aktueller Position. </summary>
      <Extension()> _
      Public Function At(bs As BindingSource) As DataRow
         Return bs.At(bs.Position)
      End Function

      ''' <summary>
      ''' returnt die Datarow am index. Bei ungültigem index Nothing (keine OutOfRange-Exception!)
      ''' </summary>
      <Extension()> _
      Public Function At(bs As BindingSource, index As Integer) As DataRow
         If index < 0 OrElse index >= bs.Count Then Return Nothing
         Return DirectCast(bs(index), DataRowView).Row
      End Function

      <Extension()> _
      Public Function FindX(Of T As DataRow)(bs As BindingSource, predicate As Func(Of T, Boolean)) As Integer
         For i As Integer = 0 To bs.Count - 1
            If predicate(DirectCast(DirectCast(bs(i), DataRowView).Row, T)) Then Return i
         Next
         Return -1
      End Function

      <Extension()> _
      Public Function FindX(bs As BindingSource, columnName As String, Key As Object) As Integer
         'BindingSource.Find funzt nicht bei relateten BindingSourcses
         'FindX = bs.Find(PropertyName, Key)
         For i As Integer = 0 To bs.Count - 1
            If DirectCast(bs(i), DataRowView)(columnName).Equals(Key) Then Return i
         Next
         Return -1
      End Function

      Private Function KeysMatch(row As DataRow, cols As DataColumn(), keys As Object()) As Boolean
         For ii = 0 To cols.Length - 1
            If Not Object.Equals(keys(ii), row(cols(ii))) Then Return False
         Next
         Return True
      End Function

      ''' <summary> stellt den Datensatz mit den angegebenen Primärschlüssel-Werten ein </summary>
      <Extension()> _
      Public Function MoveTo(bs As BindingSource, primKeys() As Object) As Boolean
         If bs.Count = 0 Then Return False
         Dim rw = DirectCast(bs(0), DataRowView).Row
         Dim cols = rw.Table.PrimaryKey
         Dim i = 0
         Try
            If KeysMatch(rw, cols, primKeys) Then Return True
            For i = 1 To bs.Count - 1
               rw = DirectCast(bs(i), DataRowView).Row
               If KeysMatch(rw, cols, primKeys) Then Return True
            Next
         Finally
            bs.Position = i
         End Try
         Return False
      End Function

      ''' <summary> stellt den Datensatz mit dem angegebenen Primärschlüssel-Wert ein </summary>
      <Extension()> _
      Public Function MoveTo(bs As BindingSource, primKey As Object) As Boolean
         If bs.Count = 0 Then Return False
         Return bs.MoveTo(DirectCast(bs(0), DataRowView).Row.Table.PrimaryKey(0).ColumnName, primKey)
      End Function

      ''' <summary> 
      ''' stellt den ersten Datensatz ein, dessen Wert in der angegebenen spalte mit Key übereinstimmt
      ''' </summary>
      <Extension()> _
      Public Function MoveTo(bs As BindingSource, columnName As String, Key As Object) As Boolean
         Dim i = bs.FindX(columnName, Key)
         MoveTo = i >= 0
         bs.Position = i
      End Function

      ''' <summary> stellt die angegebene DataRow ein </summary>
      <Extension()> _
      Public Function MoveTo(bs As BindingSource, row As DataRow) As Boolean
         Dim dv = DirectCast(bs.List, DataView)
         For i As Integer = 0 To bs.Count - 1
            If dv(i).Row Is row Then
               bs.CancelEdit() 'ansonsten setzter u.U. beim Moven die vorherige Row auf Rowstate.Changed
               bs.Position = i
               Return True
            End If
         Next
         Return False
      End Function

      ''' <summary> stellt die erste matchende DataRow ein </summary>
      <Extension()> _
      Public Function MoveTo(Of T As DataRow)(bs As BindingSource, predicate As Func(Of T, Boolean)) As Boolean
         Dim dv = DirectCast(bs.List, DataView)
         For i As Integer = 0 To bs.Count - 1
            If predicate(DirectCast(dv(i).Row, T)) Then
               bs.Position = i
               Return True
            End If
         Next
         Return False
      End Function

      <System.Diagnostics.DebuggerStepThrough()> _
      <Extension()> _
      Public Function AddNewX(Of T As DataRow)(bs As BindingSource) As T
         Return DirectCast(DirectCast(bs.AddNew(), DataRowView).Row, T)
      End Function

      <System.Diagnostics.DebuggerStepThrough()> _
      <Extension()> _
      Public Function RowX(Of T As DataRow)(subj As DataRowView) As T
         Return DirectCast(subj.Row, T)
      End Function

      <System.Diagnostics.DebuggerStepThrough()> _
      <Extension()> _
      Public Function AddNewX(Of T)(subj As IBindingList) As T
         Return DirectCast(subj.AddNew(), T)
      End Function

      ''' <summary> for debugging-purposes </summary>
      Public Function CheckChanges(dts As DataSet) As Boolean
         dts = dts.GetChanges
         If dts.Null Then messagebox.show("no Changes") : Return False
         Dim tbls = dts.Tables.Cast(Of DataTable)().ToArray
         Dim rws = tbls.SelectMany(Function(tb) tb.Rows.Cast(Of DataRow)())
         Dim infos = From tbl In tbls From rw In tbl.Rows.Cast(Of DataRow)() _
   Select String.Concat(rw.GetType.Name, _
   String.Concat(rw.ItemArray.Select(Function(itm) " " & itm.ToString).ToArray))
         MessageBox.Show(String.Join(Lf, infos.ToArray))
         Return True
      End Function

   End Module

End Namespace
