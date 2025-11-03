using ManagementDashboard.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ManagementDashboard.Services
{
    public class PdfService : IPdfService
    {
        public byte[] GenerateUserPdfReport(List<User> users)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontColor(Colors.Black));

                    page.Header()
                        .PaddingBottom(10)  // Move padding to container level
                        .Text("User Report")
                        .FontSize(20)
                        .Bold()
                        .AlignCenter();

                    page.Content()
                        .Table(table =>
                        {
                            // Define columns
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(50); // ID
                                columns.RelativeColumn();    // Name
                                columns.RelativeColumn();    // Email
                                columns.RelativeColumn();    // Role
                                columns.RelativeColumn();    // CreatedAt
                            });

                            // Table Header
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("ID").Bold();
                                header.Cell().Element(CellStyle).Text("Name").Bold();
                                header.Cell().Element(CellStyle).Text("Email").Bold();
                                header.Cell().Element(CellStyle).Text("Role").Bold();
                                header.Cell().Element(CellStyle).Text("Created At").Bold();
                            });

                            // Table Rows
                            foreach (var user in users)
                            {
                                table.Cell().Element(CellStyle).Text(user.Id.ToString());
                                table.Cell().Element(CellStyle).Text(user.Name);
                                table.Cell().Element(CellStyle).Text(user.Email);
                                table.Cell().Element(CellStyle).Text(user.Role.ToString());
                                table.Cell().Element(CellStyle).Text(user.CreatedAt.ToString("yyyy-MM-dd"));
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .PaddingTop(10)  // Move padding to container level
                        .Text(x =>
                        {
                            x.Span("Generated on ");
                            x.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                        });
                        
                });
            });

            return document.GeneratePdf();
        }

        // Helper method for cell styling
        private static IContainer CellStyle(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(5);
        }
    }
}