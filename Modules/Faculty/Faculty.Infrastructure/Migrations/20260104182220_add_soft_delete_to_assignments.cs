using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Faculty.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_soft_delete_to_assignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "public",
                table: "Assignment",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "public",
                table: "Assignment");
        }
    }
}
