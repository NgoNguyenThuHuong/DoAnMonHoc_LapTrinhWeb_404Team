using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LingoToneMVC.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    XP = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Streak = table.Column<int>(type: "int", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChineseCharacters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Character = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Pinyin = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HanViet = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Meaning = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Radical = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StrokeCount = table.Column<int>(type: "int", nullable: false),
                    HskLevel = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChineseCharacters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuizQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OptionA = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OptionB = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OptionC = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OptionD = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CorrectAnswer = table.Column<int>(type: "int", nullable: false),
                    XpReward = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SrsCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Word = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Pinyin = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Meaning = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SrsLevel = table.Column<int>(type: "int", nullable: false),
                    NextReviewAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SrsCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SrsCards_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLearnedWords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Word = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LearnedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLearnedWords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLearnedWords_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrammarPoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Pattern = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Example = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExamplePinyin = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExampleMeaning = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LessonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrammarPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrammarPoints_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    XpEarned = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProgresses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProgresses_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vocabularies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Chinese = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Pinyin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Vietnamese = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LessonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vocabularies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vocabularies_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ChineseCharacters",
                columns: new[] { "Id", "Character", "HanViet", "HskLevel", "Meaning", "Pinyin", "Radical", "StrokeCount" },
                values: new object[,]
                {
                    { 1, "你", "nhĩ", "HSK1", "bạn", "nǐ", "亻", 7 },
                    { 2, "好", "hảo", "HSK1", "tốt", "hǎo", "女", 6 },
                    { 3, "我", "ngã", "HSK1", "tôi", "wǒ", "戈", 7 },
                    { 4, "叫", "khiếu", "HSK1", "tên là", "jiào", "口", 5 },
                    { 5, "是", "thị", "HSK1", "là", "shì", "日", 9 },
                    { 6, "学", "học", "HSK1", "học", "xué", "子", 8 },
                    { 7, "生", "sinh", "HSK1", "sinh viên", "shēng", "生", 5 },
                    { 8, "老", "lão", "HSK1", "già", "lǎo", "老", 6 },
                    { 9, "师", "sư", "HSK1", "thầy", "shī", "巾", 6 },
                    { 10, "喜", "hỷ", "HSK2", "thích", "xǐ", "口", 12 },
                    { 11, "欢", "hoan", "HSK2", "vui", "huān", "欠", 6 },
                    { 12, "吃", "ngật", "HSK1", "ăn", "chī", "口", 6 },
                    { 13, "喝", "hát", "HSK2", "uống", "hē", "口", 12 },
                    { 14, "去", "khứ", "HSK1", "đi", "qù", "厶", 5 },
                    { 15, "来", "lai", "HSK1", "đến", "lái", "人", 7 }
                });

            migrationBuilder.InsertData(
                table: "Lessons",
                columns: new[] { "Id", "Description", "ImageUrl", "OrderIndex", "Title" },
                values: new object[,]
                {
                    { 1, "Giới thiệu bản thân và chào hỏi cơ bản", "/images/lesson1.jpg", 1, "Bài 1: Chào hỏi" },
                    { 2, "Giới thiệu nghề nghiệp và bản thân", "/images/lesson2.jpg", 2, "Bài 2: Giới thiệu" },
                    { 3, "Học các số đếm cơ bản trong tiếng Trung", "/images/lesson3.jpg", 3, "Bài 3: Số đếm" },
                    { 4, "Giới thiệu và nói về các thành viên trong gia đình", "/images/lesson4.jpg", 4, "Bài 4: Gia đình" },
                    { 5, "Diễn đạt sở thích và hoạt động yêu thích", "/images/lesson5.jpg", 5, "Bài 5: Sở thích" },
                    { 6, "Học cách hỏi giá và mua đồ", "/images/lesson6.jpg", 6, "Bài 6: Mua sắm" },
                    { 7, "Học cách hỏi và nói về thời gian", "/images/lesson7.jpg", 7, "Bài 7: Thời gian" },
                    { 8, "Học cách hỏi đường và các phương tiện đi lại", "/images/lesson8.jpg", 8, "Bài 8: Đi lại" }
                });

            migrationBuilder.InsertData(
                table: "QuizQuestions",
                columns: new[] { "Id", "CorrectAnswer", "OptionA", "OptionB", "OptionC", "OptionD", "Question", "XpReward" },
                values: new object[,]
                {
                    { 1, 0, "Xin chào", "Tạm biệt", "Cảm ơn", "Xin lỗi", "\"你好\" có nghĩa là gì?", 20 },
                    { 2, 2, "Xin chào", "Tạm biệt", "Cảm ơn", "Xin lỗi", "\"谢谢\" có nghĩa là gì?", 20 },
                    { 3, 2, "èr", "sān", "yī", "wǔ", "Số \"一\" đọc là gì?", 20 },
                    { 4, 1, "Tôi là giáo viên", "Tôi là học sinh", "Bạn là học sinh", "Bạn là giáo viên", "\"我是学生\" có nghĩa là gì?", 20 }
                });

            migrationBuilder.InsertData(
                table: "GrammarPoints",
                columns: new[] { "Id", "Example", "ExampleMeaning", "ExamplePinyin", "LessonId", "Pattern", "Title" },
                values: new object[,]
                {
                    { 1, "我是学生。", "Tôi là học sinh.", "Wǒ shì xuéshēng.", 1, "S + 是 + N", "是 (shì) - Là" },
                    { 2, "我不是老师。", "Tôi không phải giáo viên.", "Wǒ bù shì lǎoshī.", 2, "S + 不 + V", "不 (bù) - Phủ định" },
                    { 3, "你是学生吗？", "Bạn là học sinh phải không?", "Nǐ shì xuéshēng ma?", 2, "S + V + O + 吗？", "吗 (ma) - Câu hỏi Yes/No" },
                    { 4, "我呢？", "Còn tôi thì sao?", "Wǒ ne?", 2, "N + 呢？", "呢 (ne) - Còn...thì sao?" },
                    { 5, "我有一个苹果。", "Tôi có một quả táo.", "Wǒ yǒu yī ge píngguǒ.", 4, "S + 有 + N", "有 (yǒu) - Có" },
                    { 6, "我喜欢看书。", "Tôi thích đọc sách.", "Wǒ xǐhuān kàn shū.", 5, "S + 喜欢 + V/N", "喜欢 (xǐhuān) - Thích" },
                    { 7, "三个学生", "Ba học sinh", "sān ge xuéshēng", 3, "Số + Lượng từ + N", "数词 + 量词 - Số đếm" },
                    { 8, "你去哪里？", "Bạn đi đâu?", "Nǐ qù nǎlǐ?", 8, "S + 去 + Địa điểm", "去 (qù) - Đi đến" }
                });

            migrationBuilder.InsertData(
                table: "Vocabularies",
                columns: new[] { "Id", "Chinese", "LessonId", "Pinyin", "Vietnamese" },
                values: new object[,]
                {
                    { 1, "你", 1, "nǐ", "bạn, anh, chị" },
                    { 2, "好", 1, "hǎo", "tốt, được" },
                    { 3, "你好", 1, "nǐ hǎo", "xin chào" },
                    { 4, "我", 1, "wǒ", "tôi, mình" },
                    { 5, "叫", 1, "jiào", "tên là, gọi là" },
                    { 6, "是", 2, "shì", "là" },
                    { 7, "老师", 2, "lǎoshī", "giáo viên, thầy giáo" },
                    { 8, "学生", 2, "xuéshēng", "học sinh, sinh viên" },
                    { 9, "吗", 2, "ma", "trợ từ câu hỏi" },
                    { 10, "呢", 2, "ne", "còn...thì sao?" },
                    { 11, "一", 3, "yī", "một" },
                    { 12, "二", 3, "èr", "hai" },
                    { 13, "三", 3, "sān", "ba" },
                    { 14, "四", 3, "sì", "bốn" },
                    { 15, "五", 3, "wǔ", "năm" },
                    { 16, "爸爸", 4, "bàba", "bố, ba" },
                    { 17, "妈妈", 4, "māma", "mẹ, má" },
                    { 18, "哥哥", 4, "gēge", "anh trai" },
                    { 19, "姐姐", 4, "jiějie", "chị gái" },
                    { 20, "弟弟", 4, "dìdi", "em trai" },
                    { 21, "喜欢", 5, "xǐhuān", "thích, yêu thích" },
                    { 22, "看书", 5, "kàn shū", "đọc sách" },
                    { 23, "听音乐", 5, "tīng yīnyuè", "nghe nhạc" },
                    { 24, "运动", 5, "yùndòng", "thể dục, vận động" },
                    { 25, "电影", 5, "diànyǐng", "phim, điện ảnh" },
                    { 26, "多少钱", 6, "duōshao qián", "bao nhiêu tiền?" },
                    { 27, "贵", 6, "guì", "đắt" },
                    { 28, "便宜", 6, "piányí", "rẻ" },
                    { 29, "买", 6, "mǎi", "mua" },
                    { 30, "卖", 6, "mài", "bán" },
                    { 31, "几点", 7, "jǐ diǎn", "mấy giờ?" },
                    { 32, "现在", 7, "xiànzài", "bây giờ, hiện tại" },
                    { 33, "上午", 7, "shàngwǔ", "buổi sáng" },
                    { 34, "下午", 7, "xiàwǔ", "buổi chiều" },
                    { 35, "晚上", 7, "wǎnshang", "buổi tối" },
                    { 36, "去", 8, "qù", "đi (tới)" },
                    { 37, "哪里", 8, "nǎlǐ", "ở đâu, đâu" },
                    { 38, "怎么", 8, "zěnme", "như thế nào, bằng cách nào" },
                    { 39, "公交", 8, "gōngjiāo", "xe buýt công cộng" },
                    { 40, "地铁", 8, "dìtiě", "tàu điện ngầm (metro)" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GrammarPoints_LessonId",
                table: "GrammarPoints",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_SrsCards_UserId",
                table: "SrsCards",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLearnedWords_UserId",
                table: "UserLearnedWords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProgresses_LessonId",
                table: "UserProgresses",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProgresses_UserId",
                table: "UserProgresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Vocabularies_LessonId",
                table: "Vocabularies",
                column: "LessonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ChineseCharacters");

            migrationBuilder.DropTable(
                name: "GrammarPoints");

            migrationBuilder.DropTable(
                name: "QuizQuestions");

            migrationBuilder.DropTable(
                name: "SrsCards");

            migrationBuilder.DropTable(
                name: "UserLearnedWords");

            migrationBuilder.DropTable(
                name: "UserProgresses");

            migrationBuilder.DropTable(
                name: "Vocabularies");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Lessons");
        }
    }
}
