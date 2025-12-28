using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Support.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnrollmentTypeMigration : Migration
    {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			// 1) Drop old int column
			migrationBuilder.DropColumn(
				name: "FacultyId",
				table: "EnrollmentRequests");

			// 2) Add new uuid column
			migrationBuilder.AddColumn<Guid>(
				name: "FacultyId",
				table: "EnrollmentRequests",
				type: "uuid",
				nullable: false,
				defaultValue: Guid.Empty);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			// Revert back: drop uuid column
			migrationBuilder.DropColumn(
				name: "FacultyId",
				table: "EnrollmentRequests");

			// Add int column back
			migrationBuilder.AddColumn<int>(
				name: "FacultyId",
				table: "EnrollmentRequests",
				type: "integer",
				nullable: false,
				defaultValue: 0);
		}
	}
}
