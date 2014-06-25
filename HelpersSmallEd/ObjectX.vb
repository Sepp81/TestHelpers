Imports System.Runtime.CompilerServices
Imports System.Diagnostics

Namespace System

   Public Module ObjectX

      ''' <summary>trycast value to desired Type and assigns it.</summary>
      <DebuggerStepThrough()> _
      <Extension()> _
      Public Function TryBe(Of T As Class)(ByRef target As T, value As Object) As Boolean
         Dim x = TryCast(value, T)
         If x Is Nothing Then Return False
         target = x
         Return True
      End Function
      ''' <summary>cast value to desired Type and assigns it. Causes InvalidCastException on failure</summary>
      <DebuggerStepThrough(), Extension()> _
      Public Sub Be(Of T)(ByRef target As T, value As Object)
         target = DirectCast(value, T)
      End Sub
      ''' <summary>create target by creater if null. then return it</summary>
      <Extension()> _
      Public Function CreateLazy(Of T)(ByRef target As T, creater As Func(Of T)) As T
         If target Is Nothing Then target = creater()
         Return target
      End Function
      <DebuggerStepThrough()> _
      <Extension()> _
      Public Function CastX(Of T)(Obj As Object) As T
         Return DirectCast(Obj, T)
      End Function
      <DebuggerStepThrough()> _
      <Extension()> _
      Public Function TryCastX(Of T As Class)(Obj As Object) As T
         Return TryCast(Obj, T)
      End Function

      <DebuggerStepThrough()>
      <Extension()> _
      Public Function CloneX(Of T As ICloneable)(Obj As T) As T
         Return DirectCast(Obj.Clone(), T)
      End Function

      ''' <summary>returns the subject itself. Useful in With-Blocks</summary>
      <DebuggerStepThrough()>
      <Extension()> _
      Public Function Self(Of T)(subj As T) As T
         Return subj
      End Function

      <Extension()> _
      Public Function NullSafe(Of T As Class)(subj As T, prop As String) As String
         If subj Is Nothing Then Return "--"
         Dim dsc = GetType(T).GetProperty(prop)
         Return dsc.GetValue(subj, Nothing).ToString
      End Function

      ''' <summary> returns, wether src is not null. If so, assign src to dst </summary>
      <Extension()> _
      Public Function AssignNotNull(Of T As Class)(ByRef Dest As T, src As T) As Boolean
         If Src Is Nothing Then Return False
         Dest = Src
         Return True
      End Function

      ''' <summary> testet vor einer Zuweisung, ob der neue Wert überhaupt eine Änderung bringt </summary>
      ''' <remarks>
      ''' nützlich bei Zuweisungen an performance-intensive Properties, 
      ''' oder wenn auf Änderungen reagiert werden muß
      ''' </remarks>
      <Extension()> _
      Public Function Assign(Of T)(ByRef Dest As T, src As T) As Boolean
         If Object.Equals(Dest, Src) Then Return False
         Dest = Src
         Return True
      End Function
      '''' <summary> testet vor einer Zuweisung, ob der neue Wert überhaupt eine Änderung bringt </summary>
      '''' <remarks>
      '''' nützlich bei Zuweisungen an performance-intensive Properties, 
      '''' oder wenn auf Änderungen reagiert werden muß
      '''' </remarks>
      '<Extension()> _
      'Public Function Assign(Of T, T2 As T)(ByRef Dest As T,  src As T2) As Boolean
      '   If Object.Equals(Dest, src) Then Return False
      '   Dest = src
      '   Return True
      'End Function

      <DebuggerStepThrough()> _
      <Extension()> _
      Public Function Null(Of T As Class)(Subj As T) As Boolean
         Return Subj Is Nothing
      End Function

      <DebuggerStepThrough()> _
      <Extension()> _
      Public Function NotNull(Of T As Class)(Subj As T) As Boolean
         Return Subj IsNot Nothing
      End Function

   End Module
End Namespace
