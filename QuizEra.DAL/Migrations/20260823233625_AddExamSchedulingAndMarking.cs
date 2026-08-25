using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizEra.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddExamSchedulingAndMarking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Topics_TopicID",
                table: "Exams");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TimeSpent",
                table: "StudentExamQuestionAnswers",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "StudentExamAttempts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "StudentExamAttempts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<double>(
                name: "TotalMarks",
                table: "Exams",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Exams",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Exams",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<double>(
                name: "ActualMark",
                table: "ExamQuestions",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<double>(
                name: "BonusMark",
                table: "ExamQuestions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "NegativeMark",
                table: "ExamQuestions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Topics_TopicID",
                table: "Exams",
                column: "TopicID",
                principalTable: "Topics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Topics_TopicID",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "TimeSpent",
                table: "StudentExamQuestionAnswers");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "StudentExamAttempts");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "StudentExamAttempts");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "BonusMark",
                table: "ExamQuestions");

            migrationBuilder.DropColumn(
                name: "NegativeMark",
                table: "ExamQuestions");

            migrationBuilder.AlterColumn<int>(
                name: "TotalMarks",
                table: "Exams",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "ActualMark",
                table: "ExamQuestions",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Topics_TopicID",
                table: "Exams",
                column: "TopicID",
                principalTable: "Topics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
