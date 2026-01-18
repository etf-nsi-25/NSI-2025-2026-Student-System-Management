using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFacultyGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "public",
                table: "Programs",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "Programs",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "public",
                table: "Faculties",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EstablishedDate",
                schema: "public",
                table: "Faculties",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "Faculties",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                schema: "public",
                table: "Faculties",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "public",
                table: "Departments",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "Departments",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<Guid>(
               name: "FacultyGuid",
               table: "DocumentRequests",
               type: "uuid",
               nullable: false,
               defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "FacultyGuid",
                schema: "public",
                table: "Departments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "public",
                table: "AcademicYears",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                schema: "public",
                table: "AcademicYears",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                schema: "public",
                table: "AcademicYears",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "AcademicYears",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.Sql(@"
                UPDATE Public.""Departments"" d
                SET ""FacultyGuid"" = f.""Guid""
                FROM ""Faculties"" f
                WHERE f.""Id"" = d.""FacultyId""
                AND (
                    d.""FacultyGuid"" IS NULL
                    OR d.""FacultyGuid"" = '00000000-0000-0000-0000-000000000000'
                    );
                ");

            migrationBuilder.Sql(@"
                UPDATE Public.""DocumentRequests"" dr
                SET ""FacultyGuid"" = f.""Guid""
                FROM ""Faculties"" f
                WHERE f.""Id"" = dr.""FacultyId""
                AND (
                    dr.""FacultyGuid"" IS NULL
                    OR dr.""FacultyGuid"" = '00000000-0000-0000-0000-000000000000'
                    );
                ");

            migrationBuilder.DropColumn(
                name: "FacultyId",
                table: "DocumentRequests");

            migrationBuilder.RenameColumn(
                name: "FacultyGuid",
                table: "DocumentRequests",
                newName: "FacultyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Guid",
                schema: "public",
                table: "Faculties");

            migrationBuilder.DropColumn(
                name: "FacultyGuid",
                schema: "public",
                table: "Departments");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "public",
                table: "Programs",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "Programs",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "public",
                table: "Faculties",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EstablishedDate",
                schema: "public",
                table: "Faculties",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "Faculties",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "public",
                table: "Departments",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "Departments",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "public",
                table: "AcademicYears",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                schema: "public",
                table: "AcademicYears",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                schema: "public",
                table: "AcademicYears",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "AcademicYears",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<int>(
                name: "FacultyId",
                table: "DocumentRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.RenameColumn(
                name: "FacultyId",
                table: "DocumentRequests",
                newName: "FacultyGuid");
        }
    }
}
