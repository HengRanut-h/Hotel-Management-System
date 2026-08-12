using System.Text;
namespace HotelManagement.Infrastructure.Pdf;
public static class SimplePdfWriter
{
    public static byte[] Create(params string[] lines)
    {
        static string Esc(string s)=>s.Replace("\\","\\\\").Replace("(","\\(").Replace(")","\\)");
        var text=new StringBuilder("BT /F1 12 Tf 50 780 Td ");
        foreach(var line in lines){text.Append('(').Append(Esc(line)).Append(") Tj 0 -18 Td ");} text.Append("ET");
        var stream=text.ToString(); var objects=new List<string>{"1 0 obj << /Type /Catalog /Pages 2 0 R >> endobj","2 0 obj << /Type /Pages /Kids [3 0 R] /Count 1 >> endobj","3 0 obj << /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 5 0 R >> >> /Contents 4 0 R >> endobj",$"4 0 obj << /Length {Encoding.ASCII.GetByteCount(stream)} >> stream\n{stream}\nendstream endobj","5 0 obj << /Type /Font /Subtype /Type1 /BaseFont /Helvetica >> endobj"};
        using var ms=new MemoryStream(); using var sw=new StreamWriter(ms,Encoding.ASCII,1024,true); sw.NewLine="\n"; sw.Write("%PDF-1.4\n"); sw.Flush(); var offsets=new List<long>{0}; foreach(var o in objects){offsets.Add(ms.Position);sw.Write(o+"\n");sw.Flush();}var xref=ms.Position;sw.Write($"xref\n0 {objects.Count+1}\n0000000000 65535 f \n");for(var i=1;i<offsets.Count;i++)sw.Write($"{offsets[i]:D10} 00000 n \n");sw.Write($"trailer << /Size {objects.Count+1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF");sw.Flush();return ms.ToArray();
    }
}
