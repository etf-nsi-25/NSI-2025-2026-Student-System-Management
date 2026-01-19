using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteFacultyIntegerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Faculties_FacultyGuid",
                schema: "public",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Programs_Departments_DepartmentId",
                schema: "public",
                table: "Programs");

            migrationBuilder.DropIndex(
                name: "IX_Programs_DepartmentId",
                schema: "public",
                table: "Programs");

            migrationBuilder.DropIndex(
                name: "IX_Departments_FacultyGuid",
                schema: "public",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "Faculties");

            migrationBuilder.RenameColumn(
                name: "FacultyGuid",
                schema: "public",
                table: "Departments",
                newName: "FacultyId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "Programs",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "public",
                table: "Programs",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "Faculties",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "public",
                table: "Faculties",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "public",
                table: "Faculties",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "Departments",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "public",
                table: "Departments",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Year",
                schema: "public",
                table: "AcademicYears",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_DepartmentId_Code",
                schema: "public",
                table: "Programs",
                columns: new[] { "DepartmentId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Faculties_Code",
                schema: "public",
                table: "Faculties",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_FacultyId_Code",
                schema: "public",
                table: "Departments",
                columns: new[] { "FacultyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AcademicYears_Year",
                schema: "public",
                table: "AcademicYears",
                column: "Year",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Faculties_FacultyId",
                schema: "public",
                table: "Departments",
                column: "FacultyId",
                principalSchema: "public",
                principalTable: "Faculties",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Programs_Departments_DepartmentId",
                schema: "public",
                table: "Programs",
                column: "DepartmentId",
                principalSchema: "public",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Faculties_FacultyId",
                schema: "public",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Programs_Departments_DepartmentId",
                schema: "public",
                table: "Programs");

            migrationBuilder.DropIndex(
                name: "IX_Programs_DepartmentId_Code",
                schema: "public",
                table: "Programs");

            migrationBuilder.DropIndex(
                name: "IX_Faculties_Code",
                schema: "public",
                table: "Faculties");

            migrationBuilder.DropIndex(
                name: "IX_Departments_FacultyId_Code",
                schema: "public",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_AcademicYears_Year",
                schema: "public",
                table: "AcademicYears");

            migrationBuilder.RenameColumn(
                name: "FacultyId",
                schema: "public",
                table: "Departments",
                newName: "FacultyGuid");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "Programs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "public",
                table: "Programs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "Faculties",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "public",
                table: "Faculties",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "public",
                table: "Faculties",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                schema: "public",
                table: "Faculties",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "Departments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "public",
                table: "Departments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Year",
                schema: "public",
                table: "AcademicYears",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_Programs_DepartmentId",
                schema: "public",
                table: "Programs",
                column: "DepartmentId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Programs_Departments_DepartmentId",
                schema: "public",
                table: "Programs",
                column: "DepartmentId",
                principalSchema: "public",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
