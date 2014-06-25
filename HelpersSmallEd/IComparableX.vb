Imports System.Runtime.CompilerServices
Imports System.Globalization
Imports System.ComponentModel


Namespace System

   Public Module IComparableX

      <Extension()> _
      Public Sub Maximize(Of T As IComparable)(ByRef subj As T, other As T)
         If other.CompareTo(subj) > 0 Then subj = other
      End Sub

      <Extension()> _
      Public Sub Minimize(Of T As IComparable)(ByRef subj As T, other As T)
         If other.CompareTo(subj) < 0 Then subj = other
      End Sub

      <Extension()> _
      Public Function IsBetween(Of T As IComparable)( _
             ToTest As T, _
             Bord0 As T, _
             Bord1 As T) As Boolean
         Return Bord0.CompareTo(ToTest) <= 0 AndAlso ToTest.CompareTo(Bord1) <= 0
      End Function

      <Extension()> _
      Public Function IsBetween(Of T As IComparable)( _
             ToTest As T, _
             Bord0 As T, _
             Bord1 As T, _
             AutoSort As Boolean, _
             LBoundInclude As Boolean, _
             UBoundInclude As Boolean) As Boolean
         If AutoSort AndAlso Bord0.CompareTo(Bord1) > 0 Then
            Dim Temp = Bord0
            Bord0 = Bord1
            Bord1 = Temp
         End If
         Dim C0 As Integer = Bord0.CompareTo(ToTest)
         Dim C1 As Integer = ToTest.CompareTo(Bord1)
         If LBoundInclude AndAlso C0 = 0 Then Return True
         If UBoundInclude AndAlso C1 = 0 Then Return True
         Return C0 < 0 AndAlso C1 < 0
      End Function

      <Extension()> _
      Public Function Intersect(Of T As IComparable)(Range0 As IList(Of T), Range1 As IList(Of T)) As T()
         Return New T() {If(Range0(0).CompareTo(Range1(0)) > 0, Range0(0), Range1(0)), _
                               If(Range0(1).CompareTo(Range1(1)) < 0, Range0(1), Range1(1))}
      End Function
      Public Enum IntersectResult : No : Touch : Yes : YesOrTouch : End Enum
      <Extension()> _
      Public Function IsIntersectResult(Of T As IComparable)(Range() As T, query As IntersectResult) As Boolean
         Select Case query
            Case IntersectResult.No : Return Range(0).CompareTo(Range(1)) > 0
            Case IntersectResult.Touch : Return Range(0).CompareTo(Range(1)) = 0
            Case IntersectResult.Yes : Return Range(0).CompareTo(Range(1)) < 0
            Case IntersectResult.YesOrTouch : Return Range(0).CompareTo(Range(1)) <= 0
         End Select
      End Function

      <Extension()> _
      Public Function ClipIn(Of T As IComparable)( _
             Subj As T, LBound As T, UBound As T) As T
         If Subj.CompareTo(LBound) < 0 Then Return LBound
         If Subj.CompareTo(UBound) > 0 Then Return UBound
         Return Subj
      End Function

      <Extension()> _
      Public Function Min(Of T As IComparable)(subj As T, other As T) As T
         If subj.CompareTo(other) < 0 Then Return subj
         Return other
      End Function
      <Extension()> _
      Public Function Max(Of T As IComparable)(subj As T, other As T) As T
         If subj.CompareTo(other) > 0 Then Return subj
         Return other
      End Function

      ''' <summary>
      ''' konvertiert eine Comparison in ein IComparer-implementierendes Objekt
      ''' </summary>
      ''' <remarks>selbst einen Delegaten kann man also erweitern</remarks>
      <Extension()> _
      Public Function ToComparer(Of T)(Subj As Comparison(Of T)) As ComparisonComparer(Of T)
         Return New ComparisonComparer(Of T)(Subj)
      End Function

      ''' <summary>erstellt einen IComparer, dem eine der beiden Seiten als Key fest vorgegeben ist - für optimierte BinarySearches.</summary>
      Public Class BinarySearchComparer(Of T, TProp As IComparable(Of TProp)) : Inherits Comparer(Of T)
         Private _Selector As Func(Of T, TProp), _Key As TProp
         Public Sub New(selector As Func(Of T, TProp), key As TProp)
            _Selector = selector : _Key = key
         End Sub
         Public Overrides Function Compare(x As T, y As T) As Integer
            Return _Selector(x).CompareTo(_Key)
         End Function
      End Class

      Public Class StringKeyComparer(Of T) : Inherits ComparisonComparer(Of T)
         Public Sub New(selector As Func(Of T, String), Optional options As CompareOptions = CompareOptions.StringSort Or CompareOptions.IgnoreCase)
            MyBase.New(Function(x As T, y As T) String.Compare(selector(x), selector(y), CultureInfo.InvariantCulture, options))
         End Sub
      End Class

      ''' <summary>IComparer-implementierender Wrapper um eine Comparison </summary>
      Public Class ComparisonComparer(Of T) : Inherits Comparer(Of T)
         Private _Comparison As Comparison(Of T)
         Public Sub New(Comparison As Comparison(Of T))
            _Comparison = Comparison
         End Sub
         Public Overrides Function Compare(x As T, y As T) As Integer
            Return _Comparison(x, y)
         End Function
      End Class
   End Module
   Public Class Comparer
      Public Shared Function FromSelector(Of T, TProp As IComparable(Of TProp))(selector As Func(Of T, TProp), Optional sortDirection As ListSortDirection = ListSortDirection.Ascending) As ComparisonComparer(Of T)
         Return New ComparisonComparer(Of T)( _
               If(sortDirection = ListSortDirection.Ascending, _
               Function(x As T, y As T) selector(x).CompareTo(selector(y)), _
               Function(x As T, y As T) selector(y).CompareTo(selector(x))))
      End Function
   End Class
End Namespace
