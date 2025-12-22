using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Support.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedDocumentRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "DocumentRequests",
                columns: new[] { "Id", "CompletedAt", "CreatedAt", "DocumentType", "FacultyId", "Status", "UserId" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2025, 12, 21, 21, 20, 38, 189, DateTimeKind.Utc).AddTicks(2400), "Transcript of Records", 1, "Pending", "18001/2024" },
                    { 2, null, new DateTime(2025, 12, 21, 19, 20, 38, 189, DateTimeKind.Utc).AddTicks(2408), "Certificate of Enrollment", 1, "Pending", "19005/2023" },
                    { 3, null, new DateTime(2025, 12, 21, 17, 20, 38, 189, DateTimeKind.Utc).AddTicks(2410), "Diploma Supplement", 2, "Pending", "21045/2024" },
                    { 4, null, new DateTime(2025, 12, 21, 10, 20, 38, 189, DateTimeKind.Utc).AddTicks(2411), "Health Insurance Form", 1, "Pending", "18099/2022" },
                    { 5, null, new DateTime(2025, 12, 20, 22, 20, 38, 189, DateTimeKind.Utc).AddTicks(2413), "Tax Relief Certificate", 1, "Pending", "22011/2024" },
                    { 6, null, new DateTime(2025, 12, 19, 22, 20, 38, 189, DateTimeKind.Utc).AddTicks(2416), "Military Service Proof", 2, "Pending", "19033/2023" },
                    { 7, null, new DateTime(2025, 12, 21, 22, 5, 38, 189, DateTimeKind.Utc).AddTicks(2417), "Public Transport Discount", 1, "Pending", "20088/2024" },
                    { 8, null, new DateTime(2025, 12, 21, 21, 50, 38, 189, DateTimeKind.Utc).AddTicks(2419), "Student Visa Support", 2, "Pending", "25001/2024" },
                    { 9, null, new DateTime(2025, 12, 21, 18, 20, 38, 189, DateTimeKind.Utc).AddTicks(2420), "Erasmus Application Form", 1, "Pending", "24012/2024" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DocumentRequests",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "DocumentRequests",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "DocumentRequests",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "DocumentRequests",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "DocumentRequests",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "DocumentRequests",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "DocumentRequests",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "DocumentRequests",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "DocumentRequests",
                keyColumn: "Id",
                keyValue: 9);
        }
    }
}
