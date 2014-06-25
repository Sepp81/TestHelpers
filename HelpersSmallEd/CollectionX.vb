Imports System.Runtime.CompilerServices
Imports System.Diagnostics

Namespace System.Collections.Generic
   <Microsoft.VisualBasic.HideModuleName()> _
   Public Module CollectionX

      Public Delegate Function GetChilds(Of T)(item As T) As IEnumerable

#Region "ICollection(mit und ohne T)"

      <Extension()> _
      Public Function ToArray(Of T)(subj As System.Collections.ICollection) As T()
         ReDim ToArray(subj.Count - 1)
         If subj.Count > 0 Then subj.CopyTo(ToArray, 0)
      End Function

#End Region 'ICollection(ohne T)

#Region "Array"
      ''' <summary>gibt den **typisierten** Enumerator des Arrays zurück</summary>
      <Extension(), DebuggerStepThrough()> _
      Public Function GetEnumeratorX(Of T)(subj As T()) As IEnumerator(Of T)
         Return DirectCast(subj, IList(Of T)).GetEnumerator
      End Function
      <Extension(), DebuggerStepThrough()> _
      Public Function Clone(Of T)(subj As T()) As T()
         ReDim Clone(subj.Length - 1)
         Array.Copy(subj, Clone, subj.Length)
      End Function
      <Extension(), DebuggerStepThrough()> _
      Public Sub CopyX(Arr1 As Array, Indx1 As Integer, Arr2 As Array, Indx2 As Integer, Length As Integer)
         Array.Copy(Arr1, Indx1, Arr2, Indx2, Length)
      End Sub
      <Extension(), DebuggerStepThrough()> _
      Public Sub CopyX(Arr1 As Array, Arr2 As Array, Indx2 As Integer, Length As Integer)
         Array.Copy(Arr1, 0, Arr2, Indx2, Length)
      End Sub
      <Extension(), DebuggerStepThrough()> _
      Public Sub CopyX(Arr1 As Array, Arr2 As Array, Indx2 As Integer)
         Array.Copy(Arr1, 0, Arr2, Indx2, Arr1.Length)
      End Sub
      <Extension(), DebuggerStepThrough()> _
      Public Function FindIndex(Of T)(Arr() As T, IsMatch As Predicate(Of T)) As Integer
         Return Array.FindIndex(Arr, IsMatch)
      End Function
      <Extension(), DebuggerStepThrough()> _
      Public Function Find(Of T)(Arr() As T, IsMatch As Predicate(Of T)) As T
         Return Array.Find(Arr, IsMatch)
      End Function
      <Extension(), DebuggerStepThrough()> _
      Public Function IndexOf(Of T)(Arr() As T, Element As T) As Integer
         Return Array.IndexOf(Arr, Element)
      End Function
      <DebuggerStepThrough(), Extension()> _
      Public Sub ForEach(Of T)(items As T(), Action As Action(Of T))
         Array.ForEach(items, Action)
      End Sub

      '<Extension(), DebuggerStepThrough()> _
      'Public Function Contains(Of T)( Arr() As T,  Element As T) As Boolean
      '   Return DirectCast(Arr, IList(Of T)).Contains(Element)
      'End Function

      <Extension(), DebuggerStepThrough()> _
      Public Function ConvertAll(Of TSrc, TDest)( _
      Arr() As TSrc, Converter As Converter(Of TSrc, TDest)) As TDest()
         Return Array.ConvertAll(Arr, Converter)
      End Function

      <Extension()> _
      Public Sub KeepOnly(Of T)(ByRef subj() As T, predicate As Predicate(Of T))
         Dim i = subj.CloseUp(predicate)
         ReDim Preserve subj(i - 1)
         'Dim newArr(i - 1) As T
         'Array.Copy(subj, newArr, i)
         'subj = newArr
      End Sub

      <Extension(), DebuggerStepThrough()> _
      Public Sub Sort(Of T)(subj() As T)
         Array.Sort(subj)
      End Sub

#End Region 'Array

#Region "List(Of T)"

      ''' <summary>returnt die erste Einfügeposition des items in die asc-sortierte Liste </summary>
      <Extension()> _
      Public Function InsertPosition(Of T As IComparable)( _
    subj As List(Of T), item As T) As Integer
         Dim i = subj.BinarySearch(item)
         If i < 0 Then Return Not i
         For i = i - 1 To 0 Step -1
            If subj(i).CompareTo(item) < 0 Then Exit For
         Next
         Return i + 1
      End Function

      ''' <summary>returnt die erste Einfügeposition des Keys in die asc-sortierte Liste </summary>
      <Extension()> _
      Public Function InsertPosition(Of T, TProp As IComparable(Of TProp))( _
      Subj As List(Of T), _
      Key As TProp, _
      selector As Func(Of T, TProp)) As Integer
         Dim i = Subj.BinarySearch(Nothing, New BinarySearchComparer(Of T, TProp)(selector, Key))
         If i < 0 Then Return Not i
         For i = i - 1 To 0 Step -1
            If selector(Subj(i)).CompareTo(Key) < 0 Then Exit For
         Next
         Return i + 1
      End Function

      <Extension()> _
      Public Function BinarySearch(Of T, TProp As IComparable(Of TProp))( _
      Subj As List(Of T), _
      Key As TProp, _
      selector As Func(Of T, TProp)) As Integer
         Return Subj.BinarySearch(Nothing, New BinarySearchComparer(Of T, TProp)(selector, Key))
      End Function

      ''' <summary> returns the matching Item or Nothing </summary> 
      <Extension()> _
      Public Function BinaryFind(Of T, TProp As IComparable(Of TProp))( _
    Subj As List(Of T), Key As TProp, selector As Func(Of T, TProp)) As T
         Dim i = Subj.BinarySearch(Nothing, New BinarySearchComparer(Of T, TProp)(selector, Key))
         Return If(i < 0, Nothing, Subj(i))
      End Function

      <Extension()> _
      Public Function BinarySearch(Of T)( _
      Subj As List(Of T), _
      Pattern As T, _
      Comparison As Comparison(Of T)) As Integer
         'ermöglicht, List(Of T).BinarySearch mit Comparisons aufzurufen, statt umständlich eine IComparer-Klasse implementieren zu müssen
         Return Subj.BinarySearch(Pattern, Comparison.ToComparer)
      End Function
      <Extension()> _
      Public Function BinarySearch(Of T)( _
      Subj As T(), _
      Pattern As T, _
      Comparison As Comparison(Of T)) As Integer
         'Umbau von Shared Array.BinarySearch in eine Objektfunktion, die außerdem Comparisons akzeptiert
         Return Array.BinarySearch(Subj, Pattern, Comparison.ToComparer)
      End Function
      <Extension()> _
      Public Function BinarySearch(Of T)( _
      Subj As T(), _
      Pattern As T) As Integer
         'Umbau von Shared Array.BinarySearch in eine Objektfunktion, die außerdem Comparisons akzeptiert
         Return Array.BinarySearch(Subj, Pattern)
      End Function

#End Region 'List(Of T)

#Region "Dictionary(Of T, T2)"

      <Extension()> _
      Public Function GetOrCreate(Of Tkey, Tvalue As New)( _
    subj As Dictionary(Of Tkey, Tvalue), key As Tkey) As Tvalue
         Dim ret As Tvalue
         If Not subj.TryGetValue(key, ret) Then
            ret = New Tvalue
            subj.Add(key, ret)
         End If
         Return ret
      End Function

      ''' <summary> returns the value or Nothing. Be careful with value-types! </summary> 
      <Extension()> _
      Public Function TryGetValue(Of Tkey, Tvalue)( _
    subj As Dictionary(Of Tkey, Tvalue), key As Tkey) As Tvalue
         Dim ret As Tvalue
         If subj.TryGetValue(key, ret) Then Return ret
         Return Nothing
      End Function

#End Region 'Dictionary(Of T, T2)

#Region "Enumerable"

      <Extension()> _
      Public Function ExceptAt(Of T)(subj As IEnumerable(Of T), index As Integer) As List(Of T)
         ExceptAt = New List(Of T)
         With subj.GetEnumerator
            If index >= 0 Then
               For i = 0 To index - 1
                  If Not .MoveNext Then Exit Function
                  ExceptAt.Add(.Current)
               Next
               If Not .MoveNext Then Exit Function
            End If
            While .MoveNext : ExceptAt.Add(.Current) : End While
         End With
      End Function

      Public Structure MinMax(Of T, T2 As IComparable)
         Public Min, Max As T
         Public MinVal, MaxVal As T2
         Private _Selector As Func(Of T, T2)
         Public ReadOnly HasValue As Boolean
         Public Sub New(first As T, selector As Func(Of T, T2))
            _Selector = selector
            Me.Min = first : Me.Max = first
            MinVal = _Selector(first) : MaxVal = MinVal
            HasValue = True
         End Sub
         Public Sub Check(itm As T)
            Dim val = _Selector(itm)
            If val.CompareTo(MinVal) < 0 Then
               Min = itm
               MinVal = val
            ElseIf val.CompareTo(MaxVal) > 0 Then
               Max = itm
               MaxVal = val
            End If
         End Sub
      End Structure
      <Extension()> _
      Public Function Extremum(Of T, T2 As IComparable)( _
    items As IEnumerable(Of T), selector As Func(Of T, T2)) As MinMax(Of T, T2)
         With items.GetEnumerator
            If Not .MoveNext Then .Dispose() : Return Nothing
            Extremum = New MinMax(Of T, T2)(.Current, selector)
            While .MoveNext : Extremum.Check(.Current) : End While
            .Dispose()
         End With
      End Function
      Private Function ExtremumStrict(Of T, T2 As IComparable)( _
    items As IEnumerable(Of T), selector As Func(Of T, T2)) As MinMax(Of T, T2)
         ExtremumStrict = items.Extremum(selector)
         If Not ExtremumStrict.HasValue Then Throw New InvalidOperationException("the collection does not contain elements")
      End Function
      <Extension()> _
      Public Function MaxBy(Of T, T2 As IComparable)( _
    items As IEnumerable(Of T), selector As Func(Of T, T2)) As T
         Return ExtremumStrict(items, selector).Max
      End Function
      <Extension()> _
      Public Function MinBy(Of T, T2 As IComparable)( _
    items As IEnumerable(Of T), selector As Func(Of T, T2)) As T
         Return ExtremumStrict(items, selector).Min
      End Function
      <Extension()> Public Function Sum(Of T)(times As IEnumerable(Of T), selector As Func(Of T, TimeSpan)) As TimeSpan
         Sum = Nothing
         For Each ti In times : Sum += selector(ti) : Next
      End Function
      <Extension()> Public Function Sum(times As IEnumerable(Of TimeSpan)) As TimeSpan
         Sum = Nothing
         For Each ti In times : Sum += ti : Next
      End Function

      <Extension()> _
      Public Function AllChilds(Of T)(GetChilds As Func(Of T, IEnumerable), Roots As IEnumerable) As IEnumerable(Of T)
         Dim Enumerate As Func(Of IEnumerable(Of T), IEnumerable(Of T)) = Nothing
         Enumerate = Function(enmbl) enmbl.Concat(enmbl.SelectMany(Function(nd) Enumerate(GetChilds(nd).Cast(Of T))))
         Return Enumerate(Roots.Cast(Of T))
      End Function

      <Extension()> _
      Public Function All(Of T)( _
    GetChilds As Func(Of T, IEnumerable), Root As T) As IEnumerable(Of T)
         'Root wird in die Enumeration hineingenommen, indem ein es enthaltendes IEnumerable(Of T) 
         'geschaffen wird, dessen Kinder ( = nur Root ) rekursiv enumeriert werden
         Return GetChilds.AllChilds(New T() {Root})
      End Function

      <Extension()> _
      Public Function AllAsList(Of T)(getChilds As GetChilds(Of T), Roots As IEnumerable) As List(Of T)
         Dim ret As New List(Of T)
         For Each child As T In Roots : ret.Add(child) : Next
         Dim i As Integer = 0
         While i < ret.Count
            For Each child As T In getChilds(ret(i)) : ret.Add(child) : Next
            i += 1
         End While
         Return ret
      End Function

      <Extension()> _
      Public Function AllAsList(Of T)(getChilds As GetChilds(Of T), Root As T) As List(Of T)
         Return getChilds.AllAsList(New T() {Root})
      End Function

      <Extension()> _
      Public Function DisposeAll(Of T As IDisposable)(Subj As IList(Of T)) As IList(Of T)
         For Each itm As T In Subj
            If itm IsNot Nothing Then itm.Dispose()
         Next
         If Not Subj.IsReadOnly Then Subj.Clear()
         Return Subj
      End Function

      <Extension()> _
      Public Sub DisposeAll(Of T As IDisposable)(Subj As IEnumerable(Of T))
         For Each itm As T In Subj
            If itm IsNot Nothing Then itm.Dispose()
         Next
      End Sub

      <Extension()> _
      Public Function FindIndex(Of T)(Subj As IList(Of T), predicate As Func(Of T, Boolean), Optional start As Integer = 0) As Integer
         For FindIndex = start To Subj.Count - 1
            If predicate(Subj(FindIndex)) Then Exit Function
         Next
         Return -1
      End Function

      <Extension()> _
      Public Function FindIndex(Of T)(Subj As IList, predicate As Func(Of T, Boolean), Optional start As Integer = 0) As Integer
         For FindIndex = start To Subj.Count - 1
            If predicate(DirectCast(Subj(FindIndex), T)) Then Exit Function
         Next
         Return -1
      End Function

      <DebuggerStepThrough(), Extension()> _
      Public Sub ForEach(Of T)(Subj As IEnumerable, Action As Action(Of T))
         For Each itm As T In Subj
            Action(itm)
         Next
      End Sub

      <DebuggerStepThrough(), Extension()> _
      Public Function ToDictionary(Of Tkey, TValue)(Subj As IEnumerable(Of Tuple(Of Tkey, TValue))) As Dictionary(Of Tkey, TValue)
         ToDictionary = New Dictionary(Of Tkey, TValue)
         For Each itm In Subj
            ToDictionary.Add(itm.Item1, itm.Item2)
         Next
      End Function

      <DebuggerStepThrough(), Extension()> _
      Public Sub ForEach(Of T)(items As IEnumerable(Of T), Action As Action(Of T))
         For Each itm In items
            Action(itm)
         Next
      End Sub

      <Extension()> _
      Public Function Except(Of T)(Subj As IEnumerable(Of T), item As T) As IEnumerable(Of T)
         Return Subj.Except(New T() {item})
      End Function

      <DebuggerStepThrough(), Extension()> _
      Public Function ToList(Of T, T2)(Subj As IEnumerable(Of T), converter As Func(Of T, T2)) As List(Of T2)
         Return (From itm In Subj Select converter(itm)).ToList
      End Function

      <Extension()> _
      Public Function ToList(Of T)(Subj As IEnumerable(Of T), other As IEnumerable(Of T), ParamArray more() As IEnumerable(Of T)) As List(Of T)
         Dim src(1 + more.Length) As IEnumerable(Of T)
         src(0) = Subj
         src(1) = other
         more.CopyX(0, src, 2, more.Length)
         Return src.SelectMany(Function(answers) answers).ToList
      End Function

#End Region 'Enumerable

#Region "Helpers"
      ''' <summary>überschreibt alle matchende items mit den folgenden. returnt den index, ab dem der rest gelöscht werden kann</summary>
      <Extension()> _
      Private Function CloseUp(Of T)(subj As IList(Of T), predicate As Predicate(Of T)) As Integer
         Dim i As Integer
         For i = 0 To subj.Count - 1
            If Not predicate(subj(i)) Then Exit For
         Next
         For ii As Integer = i + 1 To subj.Count - 1
            If predicate(subj(ii)) Then
               subj(i) = subj(ii)
               i += 1
            End If
         Next
         Return i
      End Function

#End Region 'Helpers

   End Module

End Namespace
