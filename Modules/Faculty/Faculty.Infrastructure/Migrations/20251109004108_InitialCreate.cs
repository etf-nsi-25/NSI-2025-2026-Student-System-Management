using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Faculty.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Faculties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Abbreviation = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faculties", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Faculties",
                columns: new[] { "Id", "Abbreviation", "CreatedAt", "Description", "Name", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "FCS", new DateTime(2025, 11, 9, 0, 41, 8, 437, DateTimeKind.Utc).AddTicks(3057), "Leading technology and innovation education", "Faculty of Computer Science", 1, new DateTime(2025, 11, 9, 0, 41, 8, 437, DateTimeKind.Utc).AddTicks(3057) },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "FENG", new DateTime(2025, 11, 9, 0, 41, 8, 437, DateTimeKind.Utc).AddTicks(3057), "Comprehensive engineering programs", "Faculty of Engineering", 1, new DateTime(2025, 11, 9, 0, 41, 8, 437, DateTimeKind.Utc).AddTicks(3057) },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "FBUS", new DateTime(2025, 11, 9, 0, 41, 8, 437, DateTimeKind.Utc).AddTicks(3057), "Business administration and management", "Faculty of Business", 0, new DateTime(2025, 11, 9, 0, 41, 8, 437, DateTimeKind.Utc).AddTicks(3057) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Faculties_Abbreviation",
                table: "Faculties",
                column: "Abbreviation",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Faculties_Name",
                table: "Faculties",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Faculties");
        }
    }
}
