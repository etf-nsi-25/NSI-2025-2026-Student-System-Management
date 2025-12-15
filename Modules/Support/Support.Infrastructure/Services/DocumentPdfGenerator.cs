using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Support.Infrastructure.Services
{
    public interface IDocumentPdfGenerator
    {
        byte[] Generate(string title, string message);
    }

    public class DocumentPdfGenerator : IDocumentPdfGenerator
    {
        public byte[] Generate(string title, string message)
        {
            string pdf = $@"%PDF-1.4
1 0 obj
<< /Type /Catalog /Pages 2 0 R >>
endobj
2 0 obj
<< /Type /Pages /Kids [3 0 R] /Count 1 >>
endobj
3 0 obj
<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R >>
endobj
4 0 obj
<< /Length 300 >>
stream
BT
/F1 26 Tf
70 740 Td
({title}) Tj
ET

BT
/F1 14 Tf
70 710 Td
(Date: {DateTime.Now:dd.MM.yyyy}) Tj
ET

0.5 w
70 700 m
545 700 l
S

BT
/F1 12 Tf
70 660 Td
({message}) Tj
ET
endstream
endobj

xref
0 5
0000000000 65535 f
0000000010 00000 n
0000000061 00000 n
0000000114 00000 n
0000000200 00000 n

trailer
<< /Size 5 /Root 1 0 R >>
startxref
330
%%EOF";

            return Encoding.ASCII.GetBytes(pdf);
        }
    }
}