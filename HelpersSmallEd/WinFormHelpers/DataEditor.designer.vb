Namespace System.Data

   <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
   Partial Class DataEditor
      Inherits System.Windows.Forms.Form

      'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
      <System.Diagnostics.DebuggerNonUserCode()> _
      Protected Overrides Sub Dispose(disposing As Boolean)
         Try
            If disposing AndAlso components IsNot Nothing Then
               components.Dispose()
            End If
         Finally
            MyBase.Dispose(disposing)
         End Try
      End Sub

      'Wird vom Windows Form-Designer benötigt.
      Private components As System.ComponentModel.IContainer

      'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
      'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
      'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
      <System.Diagnostics.DebuggerStepThrough()> _
      Private Sub InitializeComponent()
         Me.Grid = New System.Windows.Forms.DataGridView()
         Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
         Me.lstTable = New System.Windows.Forms.ListBox()
         CType(Me.Grid, System.ComponentModel.ISupportInitialize).BeginInit()
         CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
         Me.SplitContainer1.Panel1.SuspendLayout()
         Me.SplitContainer1.Panel2.SuspendLayout()
         Me.SplitContainer1.SuspendLayout()
         Me.SuspendLayout()
         '
         'Grid
         '
         Me.Grid.AllowUserToAddRows = False
         Me.Grid.AllowUserToDeleteRows = False
         Me.Grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader
         Me.Grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
         Me.Grid.Dock = System.Windows.Forms.DockStyle.Fill
         Me.Grid.Location = New System.Drawing.Point(0, 0)
         Me.Grid.Name = "Grid"
         Me.Grid.ReadOnly = True
         Me.Grid.Size = New System.Drawing.Size(471, 363)
         Me.Grid.TabIndex = 0
         '
         'SplitContainer1
         '
         Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
         Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
         Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
         Me.SplitContainer1.Name = "SplitContainer1"
         '
         'SplitContainer1.Panel1
         '
         Me.SplitContainer1.Panel1.Controls.Add(Me.lstTable)
         '
         'SplitContainer1.Panel2
         '
         Me.SplitContainer1.Panel2.Controls.Add(Me.Grid)
         Me.SplitContainer1.Size = New System.Drawing.Size(591, 363)
         Me.SplitContainer1.SplitterDistance = 112
         Me.SplitContainer1.SplitterWidth = 8
         Me.SplitContainer1.TabIndex = 1
         '
         'lstTable
         '
         Me.lstTable.Dock = System.Windows.Forms.DockStyle.Fill
         Me.lstTable.FormattingEnabled = True
         Me.lstTable.Location = New System.Drawing.Point(0, 0)
         Me.lstTable.Name = "lstTable"
         Me.lstTable.Size = New System.Drawing.Size(112, 363)
         Me.lstTable.TabIndex = 0
         '
         'DataEditor
         '
         Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
         Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
         Me.ClientSize = New System.Drawing.Size(591, 363)
         Me.Controls.Add(Me.SplitContainer1)
         Me.Name = "DataEditor"
         Me.Text = "DataEditor"
         CType(Me.Grid, System.ComponentModel.ISupportInitialize).EndInit()
         Me.SplitContainer1.Panel1.ResumeLayout(False)
         Me.SplitContainer1.Panel2.ResumeLayout(False)
         CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
         Me.SplitContainer1.ResumeLayout(False)
         Me.ResumeLayout(False)

      End Sub
      Friend WithEvents Grid As System.Windows.Forms.DataGridView
      Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
      Friend WithEvents lstTable As System.Windows.Forms.ListBox
   End Class

End Namespace