Imports System.Runtime.CompilerServices
Imports System.Reflection
Imports System.ComponentModel
Imports System.Data
Imports System.IO
Imports System.Diagnostics

Namespace System.Windows.Forms
   Public Module FormX

      <Extension()> _
      Public Function GetFullname(key As Environment.SpecialFolder, ParamArray moreSegments As String()) As String
         Return String.Join(Path.DirectorySeparatorChar, {Environment.GetFolderPath(key)}.Concat(moreSegments))
      End Function

      Private _TableAttachs As New ComponentProperty(Of DataTable, DataTableAttach)(Function(tb) New DataTableAttach(tb))
      <Extension(), DebuggerStepThrough()> _
      Public Function Attach(tb As DataTable) As DataTableAttach
         Return _TableAttachs(tb)
      End Function
      ''' <summary>mark this Table as to persist. All other tables will behave contrary. This call isn't necessary, but optimizes startup</summary>
      <Extension()> _
      Public Sub Persist(tb As DataTable, value As Boolean)
         tb.DataSet.Persist(value, tb)
      End Sub

      ''' <summary>mark these Tables as to persist. All other tables will behave contrary. This call isn't necessary, but optimizes startup</summary>
      <Extension()> _
      Public Function Persist(Of T As DataSet)(dts As T, value As Boolean, ParamArray tables As DataTable()) As T
         If DatasetAdapterBase.Attachs.ContainsKey(dts) Then Throw dts.Exception( _
      "only call DataSet.Persist() **before** its Attachement is set")
         For Each tb As DataTable In dts.Tables
            With tb.Attach
               If .Persistance <> Persistance.Guess Then Throw .Exception("Persistance already set")
               .Persistance = If(value = tables.Contains(tb), Persistance.Yes, Persistance.No)
            End With
         Next
         Return dts
      End Function

      ''' <summary>mark all Tables as to persist. This call isn't necessary, but optimizes startup</summary>
      <Extension()> _
      Public Function PersistAll(Of T As DataSet)(dts As T) As T
         Return dts.Persist(False)
      End Function

      <Extension()> _
      Private Function Attach(dts As DataSet) As DatasetAdapterBase
         Attach = Nothing
         If DatasetAdapterBase.Attachs.TryGetValue(dts, Attach) Then Exit Function
         Attach = New DatasetXmlAdapter(dts)
         DatasetAdapterBase.Attachs.Add(dts, Attach)
      End Function
      <Extension()> _
      Public Function DataFile(dts As DataSet, path As String) As DataSet
         DirectCast(dts.Attach, DatasetXmlAdapter).DataFile = New FileInfo(path)
         Return dts
      End Function
      <Extension()> _
      Public Function DataFile(dts As DataSet) As FileInfo
         Return DirectCast(dts.Attach, DatasetXmlAdapter).DataFile
      End Function

      ''' <summary>Clear dataset and refill. Optional only the given tables (not supported in dataset-only-scenarios)</summary>
      <Extension()> _
      Public Sub Fill(dts As DataSet, ParamArray tables() As DataTable)
         'Dim sw = Stopwatch.StartNew
         dts.Attach.Fill(tables)
         'Dbg(sw.ElapsedMilliseconds)
      End Sub
      <Extension()> _
      Public Sub Save(dts As DataSet, frm As Form, Optional checkForChanges As Boolean = True)
         dts.Attach.Save(frm, checkForChanges)
      End Sub

      ''' <summary>dockt das Form am oberern Bildschirmrand, in voller Breite</summary>
      <Extension()> _
      Public Function AlignOnTop(Of T As Form)(frm As T) As T
         AlignOnTop = frm
         If frm.WindowState = FormWindowState.Maximized Then Exit Function
         Dim rct = Screen.PrimaryScreen.WorkingArea
         rct.Height = frm.Height
         frm.StartPosition = FormStartPosition.Manual
         frm.Bounds = rct
      End Function
#Region "Winfom-Close-Bug"   ' Form hat den Bug, dasses die Komponenten vor den Controls disposet. Das ergibt Datagridview-Fehler, wenn im DGV ComboboxColumns verbaut sind, die an BindingSource-Komponenten gebunden sind. Hiermit wern also die Controls zuerst weggehauen - dass da nixme anbrennt

      Private Sub Form_Closed(s As Object, e As EventArgs)
         Dim o = My.Application
         With DirectCast(s, Form)
            .Validate() 'nochn Bug: DGV kriegt beim Disposen einen ZeilenIndex-Fehler, wenns sich die ZufügeZeile im Edit-Modus befindet
            .SuspendLayout()
            With .Controls
               'Controls könnten sich theor. auch gegenseitig disposen, über Events. dieses hier disposed ganz sicher immer das letzte Element was noch da ist
               While .Count > 0 : .Item(.Count - 1).Dispose() : End While
            End With
         End With
      End Sub

      'Beim Schließen des MainForms wird Form_Closed der anneren Forms garnet mehr aufgerufen. Daher hier alle alle Controls disposen
      Private Sub MainForm_Closed(s As Object, e As EventArgs)
         For Each frm As Form In Application.OpenForms
            Form_Closed(frm, e)
         Next
      End Sub

#End Region 'Winfom-Close-Bug

      Private _KnownDatasets As New Dictionary(Of Type, DataSet)
      Private _DgvErrorHandler As DataGridViewDataErrorEventHandler = _
         Sub()
            'beim Umstöpseln der DataSources können in DGVs DataErrors auftreten. Diese suspendieren durch temporäres Abonnieren des dgv.DataError-Events
         End Sub

      ''' <summary>
      ''' registers all datasets of this type and all bindingsources of the form, to improve performance and to avoid data-redundance
      ''' </summary>
      <Extension()> _
      Public Function Register(dts As DataSet, frm As Form, handleFormClosing As Boolean) As DataSet
         Dim tp = dts.GetType
         Register = Nothing
         If _KnownDatasets.TryGetValue(tp, Register) Then
            dts = Register
         Else
            Register = dts
            _KnownDatasets.Add(tp, dts)
         End If
         AddHandler frm.FormClosed, AddressOf Form_Closed
         If handleFormClosing Then AddHandler frm.FormClosing, dts.Attach.HandleFormClosing
         Dim allCtls = New GetChilds(Of Control)(Function(ctl) ctl.Controls).AllAsList(frm)
         Dim dgvs = allCtls.OfType(Of DataGridView).ToList
         dgvs.ForEach(Sub(dgv) AddHandler dgv.DataError, _DgvErrorHandler)
         Dim tpDts = dts.GetType
         Dim oldDatasets As New HashSet(Of DataSet)
         Dim shouldReplace As Func(Of Object, Boolean) = Function(src) _
            src IsNot Nothing AndAlso _
            (src Is tpDts OrElse src.GetType Is tpDts AndAlso src IsNot dts)
         Dim bindFlags As BindingFlags = BindingFlags.Instance Or BindingFlags.Public _
            Or BindingFlags.NonPublic Or BindingFlags.GetField
         For Each ctl In New Control() {frm}.Concat(allCtls.OfType(Of UserControl)())
            For Each fld In ctl.GetType.GetFields(bindFlags)
               If fld.FieldType Is tpDts Then
                  If oldDatasets.Add(DirectCast(fld.GetValue(ctl), DataSet)) Then fld.SetValue(ctl, dts)
                  Continue For
               End If
               If Not GetType(IComponent).IsAssignableFrom(fld.FieldType) Then Continue For
               Dim prp = fld.FieldType.GetProperty("DataSource")
               If prp.Null Then Continue For
               Dim itm = fld.GetValue(ctl)
               If itm Is Nothing Then Continue For
               Dim bs = TryCast(itm, BindingSource)
               If bs.NotNull Then
                  Dim tb = bs.DataTable
                  If tb.NotNull Then
                     tb = dts.Tables(tb.TableName)
                     If tb.NotNull Then tb.Attach.AddSource(bs)
                  End If
               End If
               If shouldReplace(prp.GetValue(itm, Nothing)) Then prp.SetValue(itm, dts, Nothing)
            Next
         Next
         dgvs.ForEach(Sub(dgv)
                         RemoveHandler dgv.DataError, _DgvErrorHandler
                         Dim bs = TryCast(dgv.DataSource, BindingSource)
                         If bs.NotNull Then bs.ResetBindings(False)
                      End Sub)
         oldDatasets.Remove(dts)
         For Each dts In oldDatasets : dts.Dispose() : Next
      End Function

   End Module
End Namespace

