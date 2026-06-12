using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LingoToneMVC.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGrammarAndDialogues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Explanation",
                table: "GrammarPoints",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Dialogues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dialogues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dialogues_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DialogueLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DialogueId = table.Column<int>(type: "int", nullable: false),
                    Speaker = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Hanzi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Pinyin = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MeaningVi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DialogueLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DialogueLines_Dialogues_DialogueId",
                        column: x => x.DialogueId,
                        principalTable: "Dialogues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "GrammarPoints",
                keyColumn: "Id",
                keyValue: 1,
                column: "Explanation",
                value: "");

            migrationBuilder.UpdateData(
                table: "GrammarPoints",
                keyColumn: "Id",
                keyValue: 2,
                column: "Explanation",
                value: "");

            migrationBuilder.UpdateData(
                table: "GrammarPoints",
                keyColumn: "Id",
                keyValue: 3,
                column: "Explanation",
                value: "");

            migrationBuilder.UpdateData(
                table: "GrammarPoints",
                keyColumn: "Id",
                keyValue: 4,
                column: "Explanation",
                value: "");

            migrationBuilder.UpdateData(
                table: "GrammarPoints",
                keyColumn: "Id",
                keyValue: 5,
                column: "Explanation",
                value: "");

            migrationBuilder.UpdateData(
                table: "GrammarPoints",
                keyColumn: "Id",
                keyValue: 6,
                column: "Explanation",
                value: "");

            migrationBuilder.UpdateData(
                table: "GrammarPoints",
                keyColumn: "Id",
                keyValue: 7,
                column: "Explanation",
                value: "");

            migrationBuilder.UpdateData(
                table: "GrammarPoints",
                keyColumn: "Id",
                keyValue: 8,
                column: "Explanation",
                value: "");

            migrationBuilder.CreateIndex(
                name: "IX_DialogueLines_DialogueId",
                table: "DialogueLines",
                column: "DialogueId");

            migrationBuilder.CreateIndex(
                name: "IX_Dialogues_LessonId",
                table: "Dialogues",
                column: "LessonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DialogueLines");

            migrationBuilder.DropTable(
                name: "Dialogues");

            migrationBuilder.DropColumn(
                name: "Explanation",
                table: "GrammarPoints");
        }
    }
}
