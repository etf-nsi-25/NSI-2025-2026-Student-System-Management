using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeanName",
                schema: "university",
                table: "Faculties");

            migrationBuilder.DropColumn(
                name: "HeadOfDepartment",
                schema: "university",
                table: "Departments");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "university",
                table: "Programs",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "university",
                table: "Faculties",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<Guid>(
                name: "DeanId",
                schema: "university",
                table: "Faculties",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "university",
                table: "Departments",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<Guid>(
                name: "HeadOfDepartmentId",
                schema: "university",
                table: "Departments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "university",
                table: "AcademicYears",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    FacultyId = table.Column<Guid>(type: "uuid", nullable: false),
                    IndexNumber = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Faculties_DeanId",
                schema: "university",
                table: "Faculties",
                column: "DeanId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_HeadOfDepartmentId",
                schema: "university",
                table: "Departments",
                column: "HeadOfDepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_User_HeadOfDepartmentId",
                schema: "university",
                table: "Departments",
                column: "HeadOfDepartmentId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Faculties_User_DeanId",
                schema: "university",
                table: "Faculties",
                column: "DeanId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_User_HeadOfDepartmentId",
                schema: "university",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Faculties_User_DeanId",
                schema: "university",
                table: "Faculties");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropIndex(
                name: "IX_Faculties_DeanId",
                schema: "university",
                table: "Faculties");

            migrationBuilder.DropIndex(
                name: "IX_Departments_HeadOfDepartmentId",
                schema: "university",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "DeanId",
                schema: "university",
                table: "Faculties");

            migrationBuilder.DropColumn(
                name: "HeadOfDepartmentId",
                schema: "university",
                table: "Departments");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "university",
                table: "Programs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "university",
                table: "Faculties",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeanName",
                schema: "university",
                table: "Faculties",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "university",
                table: "Departments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeadOfDepartment",
                schema: "university",
                table: "Departments",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "university",
                table: "AcademicYears",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
