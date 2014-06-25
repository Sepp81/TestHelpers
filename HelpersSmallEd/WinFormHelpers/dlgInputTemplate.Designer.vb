<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class dlgInputTemplate
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
      Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
      Me.btOk = New System.Windows.Forms.Button()
      Me.btCancel = New System.Windows.Forms.Button()
      Me.Panel1 = New System.Windows.Forms.Panel()
      Me.TableLayoutPanel1.SuspendLayout()
      Me.SuspendLayout()
      '
      'TableLayoutPanel1
      '
      Me.TableLayoutPanel1.ColumnCount = 2
      Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
      Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
      Me.TableLayoutPanel1.Controls.Add(Me.btOk, 0, 0)
      Me.TableLayoutPanel1.Controls.Add(Me.btCancel, 1, 0)
      Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom
      Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 232)
      Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
      Me.TableLayoutPanel1.RowCount = 1
      Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
      Me.TableLayoutPanel1.Size = New System.Drawing.Size(292, 41)
      Me.TableLayoutPanel1.TabIndex = 0
      '
      'btOk
      '
      Me.btOk.Anchor = System.Windows.Forms.AnchorStyles.None
      Me.btOk.DialogResult = System.Windows.Forms.DialogResult.OK
      Me.btOk.Location = New System.Drawing.Point(35, 9)
      Me.btOk.Name = "btOk"
      Me.btOk.Size = New System.Drawing.Size(75, 23)
      Me.btOk.TabIndex = 0
      Me.btOk.Text = "Ok"
      Me.btOk.UseVisualStyleBackColor = True
      '
      'btCancel
      '
      Me.btCancel.Anchor = System.Windows.Forms.AnchorStyles.None
      Me.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
      Me.btCancel.Location = New System.Drawing.Point(181, 9)
      Me.btCancel.Name = "btCancel"
      Me.btCancel.Size = New System.Drawing.Size(75, 23)
      Me.btCancel.TabIndex = 1
      Me.btCancel.Text = "Cancel"
      Me.btCancel.UseVisualStyleBackColor = True
      '
      'Panel1
      '
      Me.Panel1.AutoScroll = True
      Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
      Me.Panel1.Location = New System.Drawing.Point(0, 0)
      Me.Panel1.Name = "Panel1"
      Me.Panel1.Size = New System.Drawing.Size(292, 232)
      Me.Panel1.TabIndex = 1
      '
      'dlgInputTemplate
      '
      Me.AcceptButton = Me.btOk
      Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.CancelButton = Me.btCancel
      Me.ClientSize = New System.Drawing.Size(292, 273)
      Me.Controls.Add(Me.Panel1)
      Me.Controls.Add(Me.TableLayoutPanel1)
      Me.Name = "dlgInputTemplate"
      Me.Text = "dlgInputTemplate"
      Me.TableLayoutPanel1.ResumeLayout(False)
      Me.ResumeLayout(False)

   End Sub
   Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
   Friend WithEvents btOk As System.Windows.Forms.Button
   Friend WithEvents btCancel As System.Windows.Forms.Button
   Friend WithEvents Panel1 As System.Windows.Forms.Panel
End Class
