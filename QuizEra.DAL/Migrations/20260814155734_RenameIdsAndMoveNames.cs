using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizEra.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RenameIdsAndMoveNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamQuestions_Exams_ExamID",
                table: "ExamQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamQuestions_Questions_QuestionID",
                table: "ExamQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentCourses_Courses_CourseID",
                table: "StudentCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentCourses_Students_StudentID",
                table: "StudentCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentExamAttempts_Exams_ExamID",
                table: "StudentExamAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentExamAttempts_Students_StudentID",
                table: "StudentExamAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentExamQuestionAnswers_ExamQuestions_ExamQID",
                table: "StudentExamQuestionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentExamQuestionAnswers_StudentExamAttempts_StudExamID",
                table: "StudentExamQuestionAnswers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Instructors");

            migrationBuilder.RenameColumn(
                name: "TopicID",
                table: "Topics",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "StudentID",
                table: "Students",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "StudExamID",
                table: "StudentExamQuestionAnswers",
                newName: "StudentExamAttemptId");

            migrationBuilder.RenameColumn(
                name: "ExamQID",
                table: "StudentExamQuestionAnswers",
                newName: "ExamQuestionsId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentExamQuestionAnswers_StudExamID",
                table: "StudentExamQuestionAnswers",
                newName: "IX_StudentExamQuestionAnswers_StudentExamAttemptId");

            migrationBuilder.RenameColumn(
                name: "StudentID",
                table: "StudentExamAttempts",
                newName: "StudentId");

            migrationBuilder.RenameColumn(
                name: "ExamID",
                table: "StudentExamAttempts",
                newName: "ExamId");

            migrationBuilder.RenameColumn(
                name: "StudExamID",
                table: "StudentExamAttempts",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_StudentExamAttempts_StudentID",
                table: "StudentExamAttempts",
                newName: "IX_StudentExamAttempts_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentExamAttempts_ExamID",
                table: "StudentExamAttempts",
                newName: "IX_StudentExamAttempts_ExamId");

            migrationBuilder.RenameColumn(
                name: "CourseID",
                table: "StudentCourses",
                newName: "CourseId");

            migrationBuilder.RenameColumn(
                name: "StudentID",
                table: "StudentCourses",
                newName: "StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentCourses_CourseID",
                table: "StudentCourses",
                newName: "IX_StudentCourses_CourseId");

            migrationBuilder.RenameColumn(
                name: "QuestionID",
                table: "Questions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "InstructorID",
                table: "Instructors",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "FeedbackID",
                table: "Feedbacks",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ExamID",
                table: "Exams",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "QuestionID",
                table: "ExamQuestions",
                newName: "QuestionId");

            migrationBuilder.RenameColumn(
                name: "ExamID",
                table: "ExamQuestions",
                newName: "ExamId");

            migrationBuilder.RenameColumn(
                name: "ExamQID",
                table: "ExamQuestions",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_ExamQuestions_QuestionID",
                table: "ExamQuestions",
                newName: "IX_ExamQuestions_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_ExamQuestions_ExamID",
                table: "ExamQuestions",
                newName: "IX_ExamQuestions_ExamId");

            migrationBuilder.RenameColumn(
                name: "CourseID",
                table: "Courses",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamQuestions_Exams_ExamId",
                table: "ExamQuestions",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamQuestions_Questions_QuestionId",
                table: "ExamQuestions",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCourses_Courses_CourseId",
                table: "StudentCourses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCourses_Students_StudentId",
                table: "StudentCourses",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentExamAttempts_Exams_ExamId",
                table: "StudentExamAttempts",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentExamAttempts_Students_StudentId",
                table: "StudentExamAttempts",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentExamQuestionAnswers_ExamQuestions_ExamQuestionsId",
                table: "StudentExamQuestionAnswers",
                column: "ExamQuestionsId",
                principalTable: "ExamQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentExamQuestionAnswers_StudentExamAttempts_StudentExamAttemptId",
                table: "StudentExamQuestionAnswers",
                column: "StudentExamAttemptId",
                principalTable: "StudentExamAttempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamQuestions_Exams_ExamId",
                table: "ExamQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamQuestions_Questions_QuestionId",
                table: "ExamQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentCourses_Courses_CourseId",
                table: "StudentCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentCourses_Students_StudentId",
                table: "StudentCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentExamAttempts_Exams_ExamId",
                table: "StudentExamAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentExamAttempts_Students_StudentId",
                table: "StudentExamAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentExamQuestionAnswers_ExamQuestions_ExamQuestionsId",
                table: "StudentExamQuestionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentExamQuestionAnswers_StudentExamAttempts_StudentExamAttemptId",
                table: "StudentExamQuestionAnswers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Topics",
                newName: "TopicID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Students",
                newName: "StudentID");

            migrationBuilder.RenameColumn(
                name: "StudentExamAttemptId",
                table: "StudentExamQuestionAnswers",
                newName: "StudExamID");

            migrationBuilder.RenameColumn(
                name: "ExamQuestionsId",
                table: "StudentExamQuestionAnswers",
                newName: "ExamQID");

            migrationBuilder.RenameIndex(
                name: "IX_StudentExamQuestionAnswers_StudentExamAttemptId",
                table: "StudentExamQuestionAnswers",
                newName: "IX_StudentExamQuestionAnswers_StudExamID");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "StudentExamAttempts",
                newName: "StudentID");

            migrationBuilder.RenameColumn(
                name: "ExamId",
                table: "StudentExamAttempts",
                newName: "ExamID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "StudentExamAttempts",
                newName: "StudExamID");

            migrationBuilder.RenameIndex(
                name: "IX_StudentExamAttempts_StudentId",
                table: "StudentExamAttempts",
                newName: "IX_StudentExamAttempts_StudentID");

            migrationBuilder.RenameIndex(
                name: "IX_StudentExamAttempts_ExamId",
                table: "StudentExamAttempts",
                newName: "IX_StudentExamAttempts_ExamID");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "StudentCourses",
                newName: "CourseID");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "StudentCourses",
                newName: "StudentID");

            migrationBuilder.RenameIndex(
                name: "IX_StudentCourses_CourseId",
                table: "StudentCourses",
                newName: "IX_StudentCourses_CourseID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Questions",
                newName: "QuestionID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Instructors",
                newName: "InstructorID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Feedbacks",
                newName: "FeedbackID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Exams",
                newName: "ExamID");

            migrationBuilder.RenameColumn(
                name: "QuestionId",
                table: "ExamQuestions",
                newName: "QuestionID");

            migrationBuilder.RenameColumn(
                name: "ExamId",
                table: "ExamQuestions",
                newName: "ExamID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ExamQuestions",
                newName: "ExamQID");

            migrationBuilder.RenameIndex(
                name: "IX_ExamQuestions_QuestionId",
                table: "ExamQuestions",
                newName: "IX_ExamQuestions_QuestionID");

            migrationBuilder.RenameIndex(
                name: "IX_ExamQuestions_ExamId",
                table: "ExamQuestions",
                newName: "IX_ExamQuestions_ExamID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Courses",
                newName: "CourseID");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Instructors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Instructors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamQuestions_Exams_ExamID",
                table: "ExamQuestions",
                column: "ExamID",
                principalTable: "Exams",
                principalColumn: "ExamID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamQuestions_Questions_QuestionID",
                table: "ExamQuestions",
                column: "QuestionID",
                principalTable: "Questions",
                principalColumn: "QuestionID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCourses_Courses_CourseID",
                table: "StudentCourses",
                column: "CourseID",
                principalTable: "Courses",
                principalColumn: "CourseID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCourses_Students_StudentID",
                table: "StudentCourses",
                column: "StudentID",
                principalTable: "Students",
                principalColumn: "StudentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentExamAttempts_Exams_ExamID",
                table: "StudentExamAttempts",
                column: "ExamID",
                principalTable: "Exams",
                principalColumn: "ExamID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentExamAttempts_Students_StudentID",
                table: "StudentExamAttempts",
                column: "StudentID",
                principalTable: "Students",
                principalColumn: "StudentID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentExamQuestionAnswers_ExamQuestions_ExamQID",
                table: "StudentExamQuestionAnswers",
                column: "ExamQID",
                principalTable: "ExamQuestions",
                principalColumn: "ExamQID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentExamQuestionAnswers_StudentExamAttempts_StudExamID",
                table: "StudentExamQuestionAnswers",
                column: "StudExamID",
                principalTable: "StudentExamAttempts",
                principalColumn: "StudExamID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
