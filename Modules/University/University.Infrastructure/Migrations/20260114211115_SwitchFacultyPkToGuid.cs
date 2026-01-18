using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace University.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SwitchFacultyPkToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Faculties_FacultyId",
                schema: "public",
                table: "Departments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Faculties",
                schema: "public",
                table: "Faculties");

            migrationBuilder.DropIndex(
                name: "IX_Departments_FacultyId",
                schema: "public",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "FacultyId",
                schema: "public",
                table: "Departments");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "public",
                table: "Faculties",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Faculties",
                schema: "public",
                table: "Faculties",
                column: "Guid");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_FacultyGuid",
                schema: "public",
                table: "Departments",
                column: "FacultyGuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Faculties_FacultyGuid",
                schema: "public",
                table: "Departments",
                column: "FacultyGuid",
                principalSchema: "public",
                principalTable: "Faculties",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Faculties_FacultyGuid",
                schema: "public",
                table: "Departments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Faculties",
                schema: "public",
                table: "Faculties");

            migrationBuilder.DropIndex(
                name: "IX_Departments_FacultyGuid",
                schema: "public",
                table: "Departments");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "public",
                table: "Faculties",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "FacultyId",
                schema: "public",
                table: "Departments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Faculties",
                schema: "public",
                table: "Faculties",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_FacultyId",
                schema: "public",
                table: "Departments",
                column: "FacultyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Faculties_FacultyId",
                schema: "public",
                table: "Departments",
                column: "FacultyId",
                principalSchema: "public",
                principalTable: "Faculties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
