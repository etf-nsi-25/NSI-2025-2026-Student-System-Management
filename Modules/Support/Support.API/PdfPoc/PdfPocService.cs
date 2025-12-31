using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Support.API.PdfPoc;

public class PdfPocService
{
    public byte[] GenerateStudentConfirmationPdf(StudentConfirmationData data)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = new StudentConfirmationPdf(data); 
        return document.GeneratePdf();
    }
}
