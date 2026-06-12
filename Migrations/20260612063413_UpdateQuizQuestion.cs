using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LingoToneMVC.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuizQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CorrectAnswer",
                table: "QuizQuestions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "QuizQuestions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "QuestionType",
                table: "QuizQuestions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "QuizQuestions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CorrectAnswer", "CreatedAt", "Explanation", "LessonId", "QuestionType" },
                values: new object[] { "A", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "你好 / nǐ hǎo / nghĩa là xin chào.", 1, "Vocabulary" });

            migrationBuilder.UpdateData(
                table: "QuizQuestions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CorrectAnswer", "CreatedAt", "Explanation", "LessonId", "OptionA", "OptionB", "OptionC", "OptionD", "Question", "QuestionType" },
                values: new object[] { "C", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "谢谢 đọc là xiè xie, nghĩa là cảm ơn.", 1, "nǐ hǎo", "zài jiàn", "xiè xie", "duì bu qǐ", "Pinyin của từ \"谢谢\" là gì?", "Vocabulary" });

            migrationBuilder.UpdateData(
                table: "QuizQuestions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CorrectAnswer", "CreatedAt", "Explanation", "LessonId", "OptionA", "OptionB", "OptionC", "OptionD", "Question", "QuestionType" },
                values: new object[] { "B", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "再见 / zài jiàn / nghĩa là tạm biệt.", 1, "Xin lỗi", "Tạm biệt", "Không sao", "Rất tốt", "\"再见\" nghĩa là gì?", "Vocabulary" });

            migrationBuilder.UpdateData(
                table: "QuizQuestions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CorrectAnswer", "CreatedAt", "Explanation", "LessonId", "OptionA", "OptionB", "OptionC", "OptionD", "Question", "QuestionType" },
                values: new object[] { "C", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "一 đọc là yī, nghĩa là số một.", 1, "èr", "sān", "yī", "wǔ", "Số \"一\" đọc là gì?", "Vocabulary" });

            migrationBuilder.InsertData(
                table: "QuizQuestions",
                columns: new[] { "Id", "CorrectAnswer", "CreatedAt", "Explanation", "LessonId", "OptionA", "OptionB", "OptionC", "OptionD", "Question", "QuestionType", "XpReward" },
                values: new object[] { 5, "B", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "我是学生 / wǒ shì xué shēng / nghĩa là Tôi là học sinh.", 1, "Tôi là giáo viên", "Tôi là học sinh", "Bạn là học sinh", "Bạn là giáo viên", "\"我是学生\" có nghĩa là gì?", "Grammar", 20 });

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestions_LessonId",
                table: "QuizQuestions",
                column: "LessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestions_Lessons_LessonId",
                table: "QuizQuestions",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestions_Lessons_LessonId",
                table: "QuizQuestions");

            migrationBuilder.DropIndex(
                name: "IX_QuizQuestions_LessonId",
                table: "QuizQuestions");

            migrationBuilder.DeleteData(
                table: "QuizQuestions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "QuizQuestions");

            migrationBuilder.DropColumn(
                name: "QuestionType",
                table: "QuizQuestions");

            migrationBuilder.AlterColumn<int>(
                name: "CorrectAnswer",
                table: "QuizQuestions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "QuizQuestions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CorrectAnswer", "Explanation", "LessonId" },
                values: new object[] { 0, null, 0 });

            migrationBuilder.UpdateData(
                table: "QuizQuestions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CorrectAnswer", "Explanation", "LessonId", "OptionA", "OptionB", "OptionC", "OptionD", "Question" },
                values: new object[] { 2, null, 0, "Xin chào", "Tạm biệt", "Cảm ơn", "Xin lỗi", "\"谢谢\" có nghĩa là gì?" });

            migrationBuilder.UpdateData(
                table: "QuizQuestions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CorrectAnswer", "Explanation", "LessonId", "OptionA", "OptionB", "OptionC", "OptionD", "Question" },
                values: new object[] { 2, null, 0, "èr", "sān", "yī", "wǔ", "Số \"一\" đọc là gì?" });

            migrationBuilder.UpdateData(
                table: "QuizQuestions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CorrectAnswer", "Explanation", "LessonId", "OptionA", "OptionB", "OptionC", "OptionD", "Question" },
                values: new object[] { 1, null, 0, "Tôi là giáo viên", "Tôi là học sinh", "Bạn là học sinh", "Bạn là giáo viên", "\"我是学生\" có nghĩa là gì?" });
        }
    }
}
