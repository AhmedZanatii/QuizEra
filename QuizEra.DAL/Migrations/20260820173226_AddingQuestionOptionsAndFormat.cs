using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizEra.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddingQuestionOptionsAndFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "StudentExamQuestionAnswers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatorUser",
                table: "StudentExamQuestionAnswers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "StudentExamQuestionAnswers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeleterUser",
                table: "StudentExamQuestionAnswers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StudentExamQuestionAnswers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "StudentExamQuestionAnswers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifierUser",
                table: "StudentExamQuestionAnswers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuestionFormat",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "QuestionOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    OptionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionOptions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_QuestionId",
                table: "QuestionOptions",
                column: "QuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionOptions");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "StudentExamQuestionAnswers");

            migrationBuilder.DropColumn(
                name: "CreatorUser",
                table: "StudentExamQuestionAnswers");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "StudentExamQuestionAnswers");

            migrationBuilder.DropColumn(
                name: "DeleterUser",
                table: "StudentExamQuestionAnswers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StudentExamQuestionAnswers");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "StudentExamQuestionAnswers");

            migrationBuilder.DropColumn(
                name: "ModifierUser",
                table: "StudentExamQuestionAnswers");

            migrationBuilder.DropColumn(
                name: "QuestionFormat",
                table: "Questions");
        }
    }
}
