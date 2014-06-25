Option Compare Text

Imports Microsoft.VisualBasic.ControlChars
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Collections
Imports System.Text.RegularExpressions
Imports System.Globalization
Imports System.Diagnostics

Namespace System

   Public Module StringX

      <Extension()> _
      Public Function FirstDifference(texts As IList(Of String)) As Integer
         Dim commonLength = texts.Min(Function(s) s.Length)
         Dim ubound = texts.Count - 1
         For retVal = 0 To commonLength - 1
            Dim cmp = texts(0)(retVal)
            For ii = 1 To ubound
               If Not texts(ii)(retVal).Equals(cmp) Then Return retVal
            Next
         Next
         Return commonLength
      End Function


      <Extension()> _
      Public Function SplitAt(Subj As String, positions As IList(Of Integer)) As String()
         If positions.Count = 0 Then Return {Subj}
         Dim res$(positions.Count)
         Dim start, i As Integer
         For i = 0 To positions.Count - 1
            Dim pos = positions(i)
            If pos < start Then Throw New ArgumentException("must be ordered ascending", "positions")
            res(i) = Subj.Substring(start, pos - start)
            start = pos
         Next
         res(i) = Subj.Substring(start, Subj.Length - start)
         Return res
      End Function

      <Extension()> _
      Public Function SplitAt(Subj As String, ParamArray positions As Integer()) As String()
         Return Subj.SplitAt(DirectCast(positions, IList(Of Integer)))
      End Function

      ''' <summary> positive value cuts from left, negative cuts from right </summary>
      <Extension()> _
      Public Function SubstringX(Subj As String, cut As Integer) As String
         If cut < 0 Then Return Subj.Substring(0, Subj.Length + cut)
         Return Subj.Substring(cut)
      End Function
      ''' <summary> start and end instead of start and length. Adds negative numbers to Subj.Length </summary>
      <Extension()> _
      Public Function SubstringX(Subj As String, start As Integer, [end] As Integer) As String
         If start < 0 Then start += Subj.Length
         If [end] <= 0 Then [end] += Subj.Length
         Return Subj.Substring(start, [end] - start)
      End Function
      ''' <summary> more comfortable than String.Split(separator as String(),count,mode) </summary>
      <Extension()> _
      Public Function Split(Subj As String, separator As String, count As Integer, Optional mode As StringSplitOptions = StringSplitOptions.None) As String()
         Return Subj.Split(New String() {separator}, count, mode)
      End Function

      ''' <summary> more comfortable to use than String.Split(separator as String(),mode) </summary>
      <Extension()> _
      Public Function Split(Subj As String, separator As String, Optional mode As StringSplitOptions = StringSplitOptions.None) As String()
         Return Subj.Split(New String() {separator}, mode)
      End Function

      '''<summary> verkettet alles</summary>
      <Extension()> _
      Public Sub [Set](ByRef Subj As String, ParamArray others() As Object)
         Subj = Subj.And(others)
      End Sub

      '''<summary> interpretiert alle \n am Anfang als newline</summary>
      Private Function ReplaceBackslashN(s As String) As StringBuilder
         With New StringBuilder
            For i = 0 To s.Length - 2 Step 2
               If s.IndexOf("\n", i, 2) < 0 Then Exit For
               .Append(CrLf)
            Next
            Return .Append(s.Substring(.Length))
         End With
      End Function

      '''<summary> verkettet alles, interpretiert alle \n am Anfang als newline</summary>
      <Extension()> _
      Public Function And2(Subj As String, ParamArray others() As Object) As String
         Dim sb = ReplaceBackslashN(Subj)
         For Each itm In others
            sb.Append(ReplaceBackslashN(itm.ToString).ToString)
         Next
         Return sb.ToString
      End Function

      '''<summary> verkettet alles</summary>
      <Extension()> _
      Public Function [And](Subj As String, ParamArray others() As Object) As String
         Return String.Concat(Subj, String.Concat(others))
      End Function

      ''' <summary> verkettet alles Strings, fügt Separator dazwischen ein </summary>
      <Extension()> _
      Public Function Join( _
    Subj As IEnumerable(Of String), _
   Optional Separator As String = Microsoft.VisualBasic.ControlChars.NewLine) As String
         With New System.Text.StringBuilder
            For Each S In Subj
               .Append(S).Append(Separator)
            Next
            If .Length > 0 Then
               .Remove(.Length - Separator.Length, Separator.Length)
            End If
            Return .ToString
         End With
      End Function


      ''' <summary>gibt den String-Abschnitt links des letzten gefundenen Matches zurück - sonst Nothing</summary>
      <Extension(), DebuggerStepThrough()> _
      Public Function LastLeftCut(Subj As String, Pattern As String) As String
         Dim i = Subj.LastIndexOf(Pattern)
         Return If(i >= 0, Subj.Remove(i), Nothing)
      End Function

      ''' <summary> gibt den String-Abschnitt links des ersten gefundenen Matches zurück - sonst den String selbst</summary>
      <Extension(), DebuggerStepThrough()> _
      Public Function FirstSegment(Subj As String, Pattern As String) As String
         Return Subj.Split({Pattern}, 2, StringSplitOptions.None)(0)
         Dim i = Subj.IndexOf(Pattern)
         Return If(i >= 0, Subj.Remove(i), Subj)
      End Function

      ''' <summary> gibt den String-Abschnitt links des ersten gefundenen Matches zurück - sonst Nothing</summary>
      <Extension(), DebuggerStepThrough()> _
      Public Function LeftCut(Subj As String, Pattern As String) As String
         Dim i = Subj.IndexOf(Pattern)
         Return If(i >= 0, Subj.Remove(i), Nothing)
      End Function

      ''' <summary>Stringabschnitt ab dem letzten Auftreten von sMatch (ohne sMatch) - sonst den String selbst</summary>
      <Extension(), DebuggerStepThrough()> _
      Public Function LastRightCutOrDefault(S$, sMatch$) As String
         Dim I% = S.LastIndexOf(sMatch)
         If I < 0 Then Return S
         Return S.Substring(I + sMatch.Length)
      End Function
      ''' <summary>Stringabschnitt ab dem letzten Auftreten von sMatch (ohne sMatch) - sonst Nothing</summary>
      <Extension(), DebuggerStepThrough()> _
      Public Function LastRightCut(S$, sMatch$) As String
         Dim I% = S.LastIndexOf(sMatch)
         If I < 0 Then Return Nothing
         Return S.Substring(I + sMatch.Length)
      End Function

      ''' <summary>Stringabschnitt ab dem ersten Auftreten von sMatch (ohne sMatch)</summary>
      <Extension(), DebuggerStepThrough()> _
      Public Function RightCut(S$, sMatch$) As String
         Dim I% = S.IndexOf(sMatch)
         If I < 0 Then Return Nothing
         Return S.Substring(I + sMatch.Length)
      End Function

      Public Function CreateUnescapeRegex(Pattern As String) As Regex
         Return New Regex("(?<=(^|[^\\](\\\\)*))" & Pattern) 'der vorangestellte Pattern schließt escaped Matches ('\X' etc.) aus
      End Function

      <Extension(), DebuggerStepThrough()> _
      Public Function HasValue(Subj As System.String) As Boolean
         Return Not String.IsNullOrEmpty(Subj)
      End Function

      <Extension(), DebuggerStepThrough()> _
      Public Function HasNoValue(Subj As String) As Boolean
         Return String.IsNullOrEmpty(Subj)
      End Function

      <Extension(), DebuggerStepThrough()> _
      Public Function Between(Delimiter As Char, ParamArray Args() As Object) As String
         Return Args.Join(Delimiter)
      End Function

      <Extension(), DebuggerStepThrough()> _
      Public Function Between(Delimiter As Char, Args As IEnumerable) As String
         Return Args.Join(Delimiter)
      End Function

      <Extension(), DebuggerStepThrough()> _
      Public Function Between(Delimiter As String, ParamArray Args() As Object) As String
         Return Args.Join(Delimiter)
      End Function

      <Extension(), DebuggerStepThrough()> _
      Public Function Between(Delimiter As String, Args As IEnumerable) As String
         Return Args.Join(Delimiter)
      End Function

      <Extension(), DebuggerStepThrough()> _
      Private Function Join(Subj As IEnumerable, Delimiter As String) As String
         Dim _SB As New System.Text.StringBuilder
         With Subj.GetEnumerator
            If .MoveNext Then
               _SB.Append(If(.Current, "").ToString)
               While .MoveNext
                  _SB.Append(Delimiter).Append(If(.Current, "").ToString)
               End While
            End If
         End With
         Return _SB.ToString
      End Function

      <Extension()> _
      Public Function IntParsed(ByRef Subj As String) As Integer
         Return Integer.Parse(Subj)
      End Function

      <Extension()> _
      Public Function BoolParsed(ByRef Subj As String) As Boolean
         Return Boolean.Parse(Subj)
      End Function

      <Extension()> _
      Public Function CountChars(S As String, Search As Char) As Integer
         'ca 18 mal schneller als:     Return s.Count(Function(c) c = Search)
         CountChars = 0
         Dim I As Integer = S.IndexOf(Search)
         While I >= 0
            CountChars += 1
            I = S.IndexOf(Search, I + 1)
         End While
      End Function

      <Extension()> _
      Public Function CountSubStrings( _
    S As String, _
    Search As String, _
   Optional Comparison As StringComparison = StringComparison.CurrentCulture) As Integer
         CountSubStrings = 0
         Dim I As Integer = S.IndexOf(Search, 0, Comparison)
         While I >= 0
            CountSubStrings += 1
            I = S.IndexOf(Search, I + 1, Comparison)
         End While
      End Function

   End Module


End Namespace