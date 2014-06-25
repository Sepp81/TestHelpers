Imports System.Runtime.CompilerServices
Namespace System

   ''' <summary> generische Exception mit genauen Daten über das fehler-werfende Objekt </summary>
   Public Class Exception(Of T) : Inherits System.Exception

      Private Shared Function TypeInfo() As String
         Dim tp = GetType(T)
         With tp.Name
            Dim i = .IndexOf("`"c)
            If i < 0 Then Return tp.Name
            Return String.Concat(.Substring(0, i), "<", String.Join(",", Array.ConvertAll(tp.GetGenericArguments, Function(TParam) TParam.Name)), ">")
         End With
      End Function

      Public Sub New(sender As T, innerException As Exception, message As String)
         MyBase.New(String.Concat( _
            TypeInfo, "-Exception", _
            If(String.IsNullOrEmpty(message), "", ": " & Environment.NewLine & message)), innerException)
#If DEBUG Then
         'If System.Windows.Forms.Application.OpenForms.Count = 0 Then Stop
#End If
      End Sub

   End Class
   Public Module ExceptionX
      <Extension()> _
      Public Function Exception(Of T)(sender As T, firstMsgSegment As Object, ParamArray msgSegments As Object()) As Exception(Of T)
         Return Exception(Of T)(sender, Nothing, firstMsgSegment, String.Concat(msgSegments))
      End Function
      <Extension()> _
      Public Function Exception(Of T)(sender As T, innerException As Exception, ParamArray msgSegments As Object()) As Exception(Of T)
         Return New Exception(Of T)(sender, innerException, String.Concat(msgSegments))
      End Function
      <Extension()> _
      Public Function Exception(Of T)(sender As T) As Exception(Of T)
         Return Exception(Of T)(sender, Nothing)
      End Function
   End Module
End Namespace
