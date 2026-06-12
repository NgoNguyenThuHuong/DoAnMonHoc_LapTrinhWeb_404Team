using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LingoToneMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddHskLevelToLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompletionPercent",
                table: "UserProgresses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastStudyDate",
                table: "UserProgresses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Streak",
                table: "UserProgresses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "XP",
                table: "UserProgresses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Explanation",
                table: "QuizQuestions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LessonId",
                table: "QuizQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "HskLevel",
                table: "Lessons",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RequiredXp = table.Column<int>(type: "int", nullable: false),
                    IconUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlashCards",
                columns: table => new
                {
                    CardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Hanzi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Pinyin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MeaningVietnamese = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MeaningEnglish = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlashCards", x => x.CardId);
                });

            migrationBuilder.CreateTable(
                name: "GrammarRules",
                columns: table => new
                {
                    GrammarId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    Pattern = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Explanation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExampleChinese = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExamplePinyin = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExampleVietnamese = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrammarRules", x => x.GrammarId);
                    table.ForeignKey(
                        name: "FK_GrammarRules_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HskWords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Hanzi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Pinyin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MeaningEnglish = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MeaningVietnamese = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    HskLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Radical = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StrokeCount = table.Column<int>(type: "int", nullable: true),
                    ExampleChinese = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExamplePinyin = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExampleVietnamese = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HskWords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleplayScenarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    HskLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Context = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExpectedDialogue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleplayScenarios", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 1,
                column: "HskLevel",
                value: "");

            migrationBuilder.UpdateData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 2,
                column: "HskLevel",
                value: "");

            migrationBuilder.UpdateData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 3,
                column: "HskLevel",
                value: "");

            migrationBuilder.UpdateData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 4,
                column: "HskLevel",
                value: "");

            migrationBuilder.UpdateData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 5,
                column: "HskLevel",
                value: "");

            migrationBuilder.UpdateData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 6,
                column: "HskLevel",
                value: "");

            migrationBuilder.UpdateData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 7,
                column: "HskLevel",
                value: "");

            migrationBuilder.UpdateData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 8,
                column: "HskLevel",
                value: "");

            migrationBuilder.UpdateData(
                table: "QuizQuestions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Explanation", "LessonId" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "QuizQuestions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Explanation", "LessonId" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "QuizQuestions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Explanation", "LessonId" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "QuizQuestions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Explanation", "LessonId" },
                values: new object[] { null, 0 });

            migrationBuilder.CreateIndex(
                name: "IX_GrammarRules_LessonId",
                table: "GrammarRules",
                column: "LessonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "FlashCards");

            migrationBuilder.DropTable(
                name: "GrammarRules");

            migrationBuilder.DropTable(
                name: "HskWords");

            migrationBuilder.DropTable(
                name: "RoleplayScenarios");

            migrationBuilder.DropColumn(
                name: "CompletionPercent",
                table: "UserProgresses");

            migrationBuilder.DropColumn(
                name: "LastStudyDate",
                table: "UserProgresses");

            migrationBuilder.DropColumn(
                name: "Streak",
                table: "UserProgresses");

            migrationBuilder.DropColumn(
                name: "XP",
                table: "UserProgresses");

            migrationBuilder.DropColumn(
                name: "Explanation",
                table: "QuizQuestions");

            migrationBuilder.DropColumn(
                name: "LessonId",
                table: "QuizQuestions");

            migrationBuilder.DropColumn(
                name: "HskLevel",
                table: "Lessons");
        }
    }
}
