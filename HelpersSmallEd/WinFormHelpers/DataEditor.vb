Imports System.Data.Common

Namespace System.Data

   Public Class DataEditor

      Private _QueryItems As New List(Of String)
      Private _Con As DbConnection
      Private _Factory As DbProviderFactory
      Private _Dts As DataSet
      Private _IMetaData As Integer

      Public Sub New()
         InitializeComponent()
      End Sub

      Public Sub New(Dts As DataSet)
         MyClass.New()
         _Dts = Dts
         _QueryItems.AddRange(From tb In Dts.Tables.Cast(Of DataTable)() Select tb.TableName)
         AddHandler lstTable.SelectedIndexChanged, AddressOf DtsTable_SelectedIndexChanged
         lstTable.DataSource = _QueryItems
      End Sub

      Private Sub DtsTable_SelectedIndexChanged(sender As Object, e As EventArgs)
         Select Case lstTable.SelectedIndex
            Case -1 : Grid.DataSource = Nothing
            Case Else : Grid.DataSource = _Dts.Tables(_QueryItems(lstTable.SelectedIndex))
         End Select
      End Sub

      Public Sub New(fct As DbProviderFactory, ParamArray sCon() As String)
         MyClass.New()
         Dim con = fct.CreateConnection
         con.ConnectionString = String.Concat(sCon)
         con.Open()
         Dim tb = con.GetSchema()
         Dim queryNames = From rw In tb Select Name = rw(0).ToString
         _QueryItems.AddRange(queryNames)
         _IMetaData = _QueryItems.Count
         tb = con.GetSchema("Tables")
         queryNames = _
            From rw In tb _
            Where rw("Table_Type").ToString = "TABLE" _
            Select Name = rw("TABLE_NAME").ToString
         _QueryItems.AddRange(queryNames)
         _Con = con
         _Factory = fct
         con.Close()
         AddHandler lstTable.SelectedIndexChanged, AddressOf DbTable_SelectedIndexChanged
         lstTable.DataSource = _QueryItems
         Show()
      End Sub

      Private Sub DbTable_SelectedIndexChanged(sender As Object, e As EventArgs)
         If lstTable.SelectedIndex < 0 Then Return
         Dim query = _QueryItems(lstTable.SelectedIndex)
         _Con.Open()
         Select Case lstTable.SelectedIndex
            Case Is < _IMetaData
               Try
                  Grid.DataSource = _Con.GetSchema(query)
               Catch ex As Exception
                  MessageBox.Show("die """.And(query, """-Abfrage konnte nicht ausgeführt wern"))
               End Try
            Case Else
               Dim tb = TryCast(Grid.DataSource, DataTable)
               If tb.NotNull Then tb.Dispose()
               Using adp = _Factory.CreateDataAdapter, cmd = _Factory.CreateCommand
                  cmd.Connection = _Con
                  cmd.CommandText = "Select * from [".And(query, "]")
                  adp.SelectCommand = cmd
                  tb = New DataTable
                  adp.Fill(tb)
                  Grid.DataSource = tb
               End Using
         End Select
         _Con.Close()
      End Sub
   End Class

End Namespace