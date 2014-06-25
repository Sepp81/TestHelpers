Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Xml.Serialization
Imports System.Diagnostics

Namespace System.IO

   Public Module SystemIOX

      <Extension(), DebuggerStepThrough()> _
      Public Function Combine(fi As FileInfo, ParamArray segments As String()) As FileInfo
         Return New FileInfo(String.Join(Path.DirectorySeparatorChar, {fi.FullName.TrimEnd(Path.DirectorySeparatorChar)}.Concat(segments)))
      End Function

      Public Function GetFullnameWithoutExtension(s As String) As String
         Dim l = Path.GetExtension(s).Length
         If Not s.Contains(":"c) Then s = Path.GetFullPath(s)
         Return s.Substring(0, s.Length - l)
      End Function


      ''' <summary> kopiert von einem Stream in einen anderen </summary>
      ''' <remarks> 
      ''' Es gibt Streams ohne festgelegtes Ende (zB. NetworkStream).
      ''' In solchem Fall **muß** 'count' angegeben werden.
      ''' </remarks>
      <Extension()> _
      Public Sub WriteTo( _
        readStream As Stream, _
        writeStream As Stream, _
       Optional count As Long = -1, _
       Optional bufSize As Integer = 1024)
         Dim buf(bufSize - 1) As Byte
         If count < 0 AndAlso readStream.CanSeek Then
            count = readStream.Length - readStream.Position
         End If
         If count < 0 Then
            ' Durch 0-Byte-Lesevorgang terminierte Kopier-Schleife
            ' Ein NetworkStream würde ein Timeout-Problem verursachen
            Do
               Dim portion = readStream.Read(buf, 0, bufSize)
               If portion = 0 Then Return
               writeStream.Write(buf, 0, portion)
            Loop
         Else
            ' zähler-gesteuerte Kopier-Schleife
            Do
               If count < bufSize Then bufSize = CInt(count)
               Dim portion = readStream.Read(buf, 0, bufSize)
               If portion = 0 Then Throw New ArgumentException( _
                  "Die angegebene Anzahl Bytes konnte nicht aus dem Lese-Stream gelesen werden", _
                  "readStream + count")
               count -= portion
               writeStream.Write(buf, 0, portion)
            Loop Until count = 0
         End If
      End Sub

      ''' <summary>Achtung! FileInfo.OpenWrite ist ungeeignet, denn es erneuert nicht unbedingt die ganze Datei</summary>
      <Extension()> _
      Public Sub SerializeXml(fi As FileInfo, obj As Object)
         Using strm = fi.Open(FileMode.Create)
            strm.SerializeXml(obj)
         End Using
      End Sub
      <Extension()> _
      Public Sub SerializeXml(strm As Stream, obj As Object)
         Dim seri = New XmlSerializer(obj.GetType)
         seri.Serialize(strm, obj)
      End Sub
      <Extension()> _
      Public Function DeserializeXml(Of T)(fi As FileInfo) As T
         Using strm = fi.OpenRead
            Return strm.DeserializeXml(Of T)()
         End Using
      End Function
      <Extension()> _
      Public Function DeserializeXml(Of T)(strm As Stream) As T
         Dim seri = New XmlSerializer(GetType(T))
         Return DirectCast(seri.Deserialize(strm), T)
      End Function

   End Module

End Namespace