using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Faculty.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExamLocationAndType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamRegistration_Exam_ExamId",
                schema: "public",
                table: "ExamRegistration");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentExamGrade_Exam_ExamId",
                schema: "public",
                table: "StudentExamGrade");

            migrationBuilder.AddColumn<string>(
                name: "ExamType",
                schema: "public",
                table: "Exam",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Written");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                schema: "public",
                table: "Exam",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamRegistration_Exam_ExamId",
                schema: "public",
                table: "ExamRegistration",
                column: "ExamId",
                principalSchema: "public",
                principalTable: "Exam",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentExamGrade_Exam_ExamId",
                schema: "public",
                table: "StudentExamGrade",
                column: "ExamId",
                principalSchema: "public",
                principalTable: "Exam",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamRegistration_Exam_ExamId",
                schema: "public",
                table: "ExamRegistration");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentExamGrade_Exam_ExamId",
                schema: "public",
                table: "StudentExamGrade");

            migrationBuilder.DropColumn(
                name: "ExamType",
                schema: "public",
                table: "Exam");

            migrationBuilder.DropColumn(
                name: "Location",
                schema: "public",
                table: "Exam");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamRegistration_Exam_ExamId",
                schema: "public",
                table: "ExamRegistration",
                column: "ExamId",
                principalSchema: "public",
                principalTable: "Exam",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentExamGrade_Exam_ExamId",
                schema: "public",
                table: "StudentExamGrade",
                column: "ExamId",
                principalSchema: "public",
                principalTable: "Exam",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
