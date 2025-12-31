using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Support.API.PdfPoc;

public class StudentConfirmationPdf : IDocument
{
    private readonly StudentConfirmationData _data;

    public StudentConfirmationPdf(StudentConfirmationData data)
    {
        _data = data;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(40);
            page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Times New Roman"));

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

    void ComposeHeader(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                
                row.RelativeItem().Column(left =>
                {
                    left.Item().Text(_data.University)
                        .Bold()
                        .FontSize(10);

                    left.Item().Text(_data.Faculty)
                        .FontSize(9);

                    left.Item().Text(_data.City)
                        .FontSize(9);
                });

                
                row.ConstantItem(120)
                    .AlignCenter()
                    .Image("PdfPoc/Assets/unsa-logo.png", ImageScaling.FitArea);

               
                row.RelativeItem().AlignRight().Column(right =>
                {
                    right.Item().Text(_data.University)
                        .Bold()
                        .FontSize(10)
                        .AlignRight();

                    right.Item().Text(_data.Faculty)
                        .FontSize(9)
                        .AlignRight();

                    right.Item().Text("Contact information")
                        .FontSize(9)
                        .AlignRight();
                });
            });

            col.Item().PaddingTop(12).LineHorizontal(1);
        });
    }

void ComposeContent(IContainer container)
{
    container.PaddingTop(18).ShowEntire().Column(col =>
    {
        col.Spacing(12);

        col.Item().AlignCenter()
            .Text("C O N F I R M A T I O N")
            .Bold()
            .FontSize(18);

        col.Item().AlignCenter()
            .Text("certifying that")
            .FontSize(10)
            .FontColor(Colors.Grey.Darken1);

        col.Item().PaddingTop(18);

        col.Item().Row(row =>
        {
            row.RelativeItem(6)
                .Text(_data.StudentName)
                .Bold()
                .FontSize(12);

            row.RelativeItem(3)
                .AlignRight()
                .Text("File number")
                .FontSize(9)
                .FontColor(Colors.Grey.Darken1);

            row.RelativeItem(3)
                .AlignRight()
                .Text(_data.IndexNumber)
                .Bold();
        });

        col.Item().LineHorizontal(1);

        FormRow(col, "Born on", $"{_data.Date}, {_data.City}");
        FormRow(col, "Enrolled", $"First time in the academic year {_data.AcademicYear}, {_data.StudyCycle}");
        FormRow(col, "Status", "Regular student");
        FormRow(col, "Institution", $"{_data.University} – {_data.Faculty}");
        FormRow(col, "Department", _data.StudyProgram);

        col.Item().PaddingTop(18)
            .Border(1)
            .BorderColor(Colors.Grey.Lighten1)
            .Padding(8)
            .Text(
                "This confirmation is issued for the purpose of regulating transportation rights " +
                "and may not be used for other purposes."
            )
            .FontSize(9)
            .FontColor(Colors.Grey.Darken2);

        col.Item().PaddingTop(18)
            .Text($"Issued in {_data.City}, {_data.Date}");
    });
}
void FormRow(ColumnDescriptor col, string label, string value)
{
    col.Item().Row(row =>
    {
        row.ConstantItem(110)
            .Text(label)
            .FontSize(9)
            .FontColor(Colors.Grey.Darken1);

        row.RelativeItem()
            .Text(string.IsNullOrWhiteSpace(value) ? " " : value)
            .FontSize(10);
    });

    col.Item().LineHorizontal(1);
}



void Field(ColumnDescriptor col, string label, string value)
{
    col.Item().Text(label)
        .FontSize(9)
        .FontColor(Colors.Grey.Darken1);

    col.Item().PaddingBottom(3)
        .Text(value)
        .Bold()
        .FontSize(11);

    col.Item().LineHorizontal(1);
}



    void ComposeFooter(IContainer container)
    {
        container.PaddingTop(45).Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text("______________________________");
                col.Item().Text("Authorized person");
            });

            row.RelativeItem().AlignRight().Column(col =>
            {
                col.Item().Text("______________________________");
                col.Item().Text("Official stamp");
            });
        });
    }
}
