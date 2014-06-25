Imports System.Collections.Generic
Imports System.Text

Public Class StringList
   Inherits List(Of Object)
   'Vereinfacht das Zusammenstöpseln längerer Texte. Im Unterschied zum StringBuilder werden die Objekte nicht gleich nach String umgewandelt, sondern erst beim Abruf von ToString(), wobei dann noch einstellbar ist, was als "Wort"-Trenner, und was als Zeilentrenner verwendet werden soll.
   'Was passiert, wenn man ihn sich selbst addet? ;)

   Private Shared ReadOnly _NewLine As New Object()
   Private _sNewline As String = Environment.NewLine
   Private _Delimiter As String = " "

   Public Function ClearAddLine(ByVal ParamArray args As Object()) As StringList
      MyBase.Clear()
      Return AddLine(args)
   End Function
   Public Shadows Function Add(ByVal arg As Object) As StringList
      MyBase.Add(arg)
      Return Me
   End Function
   Public Shadows Function Add(ByVal ParamArray args As Object()) As StringList
      MyBase.AddRange(args)
      Return Me
   End Function
   Public Function AddLine(ByVal ParamArray args As Object()) As StringList
      MyBase.AddRange(args)
      MyBase.Add(_NewLine)
      Return Me
   End Function
   Public Function SetNewline(ByVal Value As String) As StringList
      _sNewline = Value
      Return Me
   End Function
   Public Function SetDelimiter(ByVal Value As String) As StringList
      _Delimiter = Value
      Return Me
   End Function

   Public Overrides Function ToString() As String
      Dim sb = New StringBuilder(MyBase.Count * 12)
      Dim lineStart = True
      For Each Item As Object In Me
         If Item Is _NewLine Then
            sb.Append(_sNewline)
            lineStart = True
         Else
            If lineStart Then
               lineStart = False
            Else
               sb.Append(_Delimiter)
            End If
            sb.Append(If(Item, "##Null##"))
         End If
      Next
      Return sb.ToString()
   End Function
End Class
