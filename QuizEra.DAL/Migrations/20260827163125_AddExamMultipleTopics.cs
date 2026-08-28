using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizEra.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddExamMultipleTopics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Topics_TopicID",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Exams_TopicID",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "TopicID",
                table: "Exams");

            migrationBuilder.CreateTable(
                name: "ExamTopics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    TopicId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamTopics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamTopics_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamTopics_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamTopics_ExamId_TopicId",
                table: "ExamTopics",
                columns: new[] { "ExamId", "TopicId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamTopics_TopicId",
                table: "ExamTopics",
                column: "TopicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamTopics");

            migrationBuilder.AddColumn<int>(
                name: "TopicID",
                table: "Exams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Exams_TopicID",
                table: "Exams",
                column: "TopicID");

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
