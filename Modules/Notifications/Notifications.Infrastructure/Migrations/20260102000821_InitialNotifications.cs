using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notifications.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ChannelType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationLog", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLog_SentAt",
                table: "NotificationLog",
                column: "SentAt");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLog_Status",
                table: "NotificationLog",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLog_UserId",
                table: "NotificationLog",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationLog");
        }
    }
}
