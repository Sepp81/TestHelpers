Namespace System.ComponentModel

   Public Class ComponentProperty(Of Tcomp As IComponent, T)
      Inherits Dictionary(Of Tcomp, T)
      Private ReadOnly _Init As Func(Of Tcomp, T)

      Private Sub cmp_Disposed(s As Object, e As EventArgs)
         Dim cmp = DirectCast(s, Tcomp)
         RemoveHandler cmp.Disposed, AddressOf cmp_Disposed
         MyBase.Remove(cmp)
      End Sub

      Public Sub New(Optional init As Func(Of Tcomp, T) = Nothing)
         _Init = init
      End Sub

      Default Public Shadows Property Item(cmp As Tcomp) As T
         Get
            Dim ret As T = Nothing
            If Not MyBase.TryGetValue(cmp, ret) Then
               If _Init.NotNull Then ret = _Init(cmp)
               MyBase.Add(cmp, ret)
               AddHandler cmp.Disposed, AddressOf cmp_Disposed
            End If
            Return ret
         End Get
         Set(value As T)
            If Not MyBase.ContainsKey(cmp) Then _
               AddHandler cmp.Disposed, AddressOf cmp_Disposed
            MyBase.Item(cmp) = value
         End Set
      End Property

   End Class

End Namespace
