using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LingoToneMVC.Models;

namespace LingoToneMVC.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Vocabulary> Vocabularies { get; set; }
        public DbSet<ChineseCharacter> ChineseCharacters { get; set; }
        public DbSet<GrammarPoint> GrammarPoints { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<QuizResult> QuizResults { get; set; }
        public DbSet<UserProgress> UserProgresses { get; set; }
        public DbSet<UserLearnedWord> UserLearnedWords { get; set; }
        public DbSet<SrsCard> SrsCards { get; set; }

        // New Models for EF Core Migration
        public DbSet<HskWord> HskWords { get; set; }
        public DbSet<GrammarRule> GrammarRules { get; set; }
        public DbSet<FlashCard> FlashCards { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<RoleplayScenario> RoleplayScenarios { get; set; }
        public DbSet<Dialogue> Dialogues { get; set; }
        public DbSet<DialogueLine> DialogueLines { get; set; }

        public DbSet<UserQuizAttempt> UserQuizAttempts { get; set; }
        public DbSet<UserSrsReview> UserSrsReviews { get; set; }
        public DbSet<UserDailyLogin> UserDailyLogins { get; set; }
        public DbSet<DailyMissionClaim> DailyMissionClaims { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed Lessons
            builder.Entity<Lesson>().HasData(
                new Lesson { Id = 1, Title = "Bài 1: Chào hỏi", Description = "Giới thiệu bản thân và chào hỏi cơ bản", OrderIndex = 1, ImageUrl = "/images/lesson1.jpg" },
                new Lesson { Id = 2, Title = "Bài 2: Giới thiệu", Description = "Giới thiệu nghề nghiệp và bản thân", OrderIndex = 2, ImageUrl = "/images/lesson2.jpg" },
                new Lesson { Id = 3, Title = "Bài 3: Số đếm", Description = "Học các số đếm cơ bản trong tiếng Trung", OrderIndex = 3, ImageUrl = "/images/lesson3.jpg" },
                new Lesson { Id = 4, Title = "Bài 4: Gia đình", Description = "Giới thiệu và nói về các thành viên trong gia đình", OrderIndex = 4, ImageUrl = "/images/lesson4.jpg" },
                new Lesson { Id = 5, Title = "Bài 5: Sở thích", Description = "Diễn đạt sở thích và hoạt động yêu thích", OrderIndex = 5, ImageUrl = "/images/lesson5.jpg" },
                new Lesson { Id = 6, Title = "Bài 6: Mua sắm", Description = "Học cách hỏi giá và mua đồ", OrderIndex = 6, ImageUrl = "/images/lesson6.jpg" },
                new Lesson { Id = 7, Title = "Bài 7: Thời gian", Description = "Học cách hỏi và nói về thời gian", OrderIndex = 7, ImageUrl = "/images/lesson7.jpg" },
                new Lesson { Id = 8, Title = "Bài 8: Đi lại", Description = "Học cách hỏi đường và các phương tiện đi lại", OrderIndex = 8, ImageUrl = "/images/lesson8.jpg" }
            );

            // Seed Vocabularies
            builder.Entity<Vocabulary>().HasData(
                // Bài 1
                new Vocabulary { Id = 1, Chinese = "你", Pinyin = "nǐ", Vietnamese = "bạn, anh, chị", LessonId = 1 },
                new Vocabulary { Id = 2, Chinese = "好", Pinyin = "hǎo", Vietnamese = "tốt, được", LessonId = 1 },
                new Vocabulary { Id = 3, Chinese = "你好", Pinyin = "nǐ hǎo", Vietnamese = "xin chào", LessonId = 1 },
                new Vocabulary { Id = 4, Chinese = "我", Pinyin = "wǒ", Vietnamese = "tôi, mình", LessonId = 1 },
                new Vocabulary { Id = 5, Chinese = "叫", Pinyin = "jiào", Vietnamese = "tên là, gọi là", LessonId = 1 },
                // Bài 2
                new Vocabulary { Id = 6, Chinese = "是", Pinyin = "shì", Vietnamese = "là", LessonId = 2 },
                new Vocabulary { Id = 7, Chinese = "老师", Pinyin = "lǎoshī", Vietnamese = "giáo viên, thầy giáo", LessonId = 2 },
                new Vocabulary { Id = 8, Chinese = "学生", Pinyin = "xuéshēng", Vietnamese = "học sinh, sinh viên", LessonId = 2 },
                new Vocabulary { Id = 9, Chinese = "吗", Pinyin = "ma", Vietnamese = "trợ từ câu hỏi", LessonId = 2 },
                new Vocabulary { Id = 10, Chinese = "呢", Pinyin = "ne", Vietnamese = "còn...thì sao?", LessonId = 2 },
                // Bài 3
                new Vocabulary { Id = 11, Chinese = "一", Pinyin = "yī", Vietnamese = "một", LessonId = 3 },
                new Vocabulary { Id = 12, Chinese = "二", Pinyin = "èr", Vietnamese = "hai", LessonId = 3 },
                new Vocabulary { Id = 13, Chinese = "三", Pinyin = "sān", Vietnamese = "ba", LessonId = 3 },
                new Vocabulary { Id = 14, Chinese = "四", Pinyin = "sì", Vietnamese = "bốn", LessonId = 3 },
                new Vocabulary { Id = 15, Chinese = "五", Pinyin = "wǔ", Vietnamese = "năm", LessonId = 3 },
                // Bài 4
                new Vocabulary { Id = 16, Chinese = "爸爸", Pinyin = "bàba", Vietnamese = "bố, ba", LessonId = 4 },
                new Vocabulary { Id = 17, Chinese = "妈妈", Pinyin = "māma", Vietnamese = "mẹ, má", LessonId = 4 },
                new Vocabulary { Id = 18, Chinese = "哥哥", Pinyin = "gēge", Vietnamese = "anh trai", LessonId = 4 },
                new Vocabulary { Id = 19, Chinese = "姐姐", Pinyin = "jiějie", Vietnamese = "chị gái", LessonId = 4 },
                new Vocabulary { Id = 20, Chinese = "弟弟", Pinyin = "dìdi", Vietnamese = "em trai", LessonId = 4 },
                // Bài 5
                new Vocabulary { Id = 21, Chinese = "喜欢", Pinyin = "xǐhuān", Vietnamese = "thích, yêu thích", LessonId = 5 },
                new Vocabulary { Id = 22, Chinese = "看书", Pinyin = "kàn shū", Vietnamese = "đọc sách", LessonId = 5 },
                new Vocabulary { Id = 23, Chinese = "听音乐", Pinyin = "tīng yīnyuè", Vietnamese = "nghe nhạc", LessonId = 5 },
                new Vocabulary { Id = 24, Chinese = "运动", Pinyin = "yùndòng", Vietnamese = "thể dục, vận động", LessonId = 5 },
                new Vocabulary { Id = 25, Chinese = "电影", Pinyin = "diànyǐng", Vietnamese = "phim, điện ảnh", LessonId = 5 },
                // Bài 6
                new Vocabulary { Id = 26, Chinese = "多少钱", Pinyin = "duōshao qián", Vietnamese = "bao nhiêu tiền?", LessonId = 6 },
                new Vocabulary { Id = 27, Chinese = "贵", Pinyin = "guì", Vietnamese = "đắt", LessonId = 6 },
                new Vocabulary { Id = 28, Chinese = "便宜", Pinyin = "piányí", Vietnamese = "rẻ", LessonId = 6 },
                new Vocabulary { Id = 29, Chinese = "买", Pinyin = "mǎi", Vietnamese = "mua", LessonId = 6 },
                new Vocabulary { Id = 30, Chinese = "卖", Pinyin = "mài", Vietnamese = "bán", LessonId = 6 },
                // Bài 7
                new Vocabulary { Id = 31, Chinese = "几点", Pinyin = "jǐ diǎn", Vietnamese = "mấy giờ?", LessonId = 7 },
                new Vocabulary { Id = 32, Chinese = "现在", Pinyin = "xiànzài", Vietnamese = "bây giờ, hiện tại", LessonId = 7 },
                new Vocabulary { Id = 33, Chinese = "上午", Pinyin = "shàngwǔ", Vietnamese = "buổi sáng", LessonId = 7 },
                new Vocabulary { Id = 34, Chinese = "下午", Pinyin = "xiàwǔ", Vietnamese = "buổi chiều", LessonId = 7 },
                new Vocabulary { Id = 35, Chinese = "晚上", Pinyin = "wǎnshang", Vietnamese = "buổi tối", LessonId = 7 },
                // Bài 8
                new Vocabulary { Id = 36, Chinese = "去", Pinyin = "qù", Vietnamese = "đi (tới)", LessonId = 8 },
                new Vocabulary { Id = 37, Chinese = "哪里", Pinyin = "nǎlǐ", Vietnamese = "ở đâu, đâu", LessonId = 8 },
                new Vocabulary { Id = 38, Chinese = "怎么", Pinyin = "zěnme", Vietnamese = "như thế nào, bằng cách nào", LessonId = 8 },
                new Vocabulary { Id = 39, Chinese = "公交", Pinyin = "gōngjiāo", Vietnamese = "xe buýt công cộng", LessonId = 8 },
                new Vocabulary { Id = 40, Chinese = "地铁", Pinyin = "dìtiě", Vietnamese = "tàu điện ngầm (metro)", LessonId = 8 }
            );

            // Seed GrammarPoints
            builder.Entity<GrammarPoint>().HasData(
                new GrammarPoint { Id = 1, Title = "是 (shì) - Là", Pattern = "S + 是 + N", Example = "我是学生。", ExamplePinyin = "Wǒ shì xuéshēng.", ExampleMeaning = "Tôi là học sinh.", LessonId = 1 },
                new GrammarPoint { Id = 2, Title = "不 (bù) - Phủ định", Pattern = "S + 不 + V", Example = "我不是老师。", ExamplePinyin = "Wǒ bù shì lǎoshī.", ExampleMeaning = "Tôi không phải giáo viên.", LessonId = 2 },
                new GrammarPoint { Id = 3, Title = "吗 (ma) - Câu hỏi Yes/No", Pattern = "S + V + O + 吗？", Example = "你是学生吗？", ExamplePinyin = "Nǐ shì xuéshēng ma?", ExampleMeaning = "Bạn là học sinh phải không?", LessonId = 2 },
                new GrammarPoint { Id = 4, Title = "呢 (ne) - Còn...thì sao?", Pattern = "N + 呢？", Example = "我呢？", ExamplePinyin = "Wǒ ne?", ExampleMeaning = "Còn tôi thì sao?", LessonId = 2 },
                new GrammarPoint { Id = 5, Title = "有 (yǒu) - Có", Pattern = "S + 有 + N", Example = "我有一个苹果。", ExamplePinyin = "Wǒ yǒu yī ge píngguǒ.", ExampleMeaning = "Tôi có một quả táo.", LessonId = 4 },
                new GrammarPoint { Id = 6, Title = "喜欢 (xǐhuān) - Thích", Pattern = "S + 喜欢 + V/N", Example = "我喜欢看书。", ExamplePinyin = "Wǒ xǐhuān kàn shū.", ExampleMeaning = "Tôi thích đọc sách.", LessonId = 5 },
                new GrammarPoint { Id = 7, Title = "数词 + 量词 - Số đếm", Pattern = "Số + Lượng từ + N", Example = "三个学生", ExamplePinyin = "sān ge xuéshēng", ExampleMeaning = "Ba học sinh", LessonId = 3 },
                new GrammarPoint { Id = 8, Title = "去 (qù) - Đi đến", Pattern = "S + 去 + Địa điểm", Example = "你去哪里？", ExamplePinyin = "Nǐ qù nǎlǐ?", ExampleMeaning = "Bạn đi đâu?", LessonId = 8 }
            );

            // Seed ChineseCharacters (15 characters)
            builder.Entity<ChineseCharacter>().HasData(
                new ChineseCharacter { Id = 1, Character = "你", Pinyin = "nǐ", HanViet = "nhĩ", Meaning = "bạn", Radical = "亻", StrokeCount = 7, HskLevel = "HSK1" },
                new ChineseCharacter { Id = 2, Character = "好", Pinyin = "hǎo", HanViet = "hảo", Meaning = "tốt", Radical = "女", StrokeCount = 6, HskLevel = "HSK1" },
                new ChineseCharacter { Id = 3, Character = "我", Pinyin = "wǒ", HanViet = "ngã", Meaning = "tôi", Radical = "戈", StrokeCount = 7, HskLevel = "HSK1" },
                new ChineseCharacter { Id = 4, Character = "叫", Pinyin = "jiào", HanViet = "khiếu", Meaning = "tên là", Radical = "口", StrokeCount = 5, HskLevel = "HSK1" },
                new ChineseCharacter { Id = 5, Character = "是", Pinyin = "shì", HanViet = "thị", Meaning = "là", Radical = "日", StrokeCount = 9, HskLevel = "HSK1" },
                new ChineseCharacter { Id = 6, Character = "学", Pinyin = "xué", HanViet = "học", Meaning = "học", Radical = "子", StrokeCount = 8, HskLevel = "HSK1" },
                new ChineseCharacter { Id = 7, Character = "生", Pinyin = "shēng", HanViet = "sinh", Meaning = "sinh viên", Radical = "生", StrokeCount = 5, HskLevel = "HSK1" },
                new ChineseCharacter { Id = 8, Character = "老", Pinyin = "lǎo", HanViet = "lão", Meaning = "già", Radical = "老", StrokeCount = 6, HskLevel = "HSK1" },
                new ChineseCharacter { Id = 9, Character = "师", Pinyin = "shī", HanViet = "sư", Meaning = "thầy", Radical = "巾", StrokeCount = 6, HskLevel = "HSK1" },
                new ChineseCharacter { Id = 10, Character = "喜", Pinyin = "xǐ", HanViet = "hỷ", Meaning = "thích", Radical = "口", StrokeCount = 12, HskLevel = "HSK2" },
                new ChineseCharacter { Id = 11, Character = "欢", Pinyin = "huān", HanViet = "hoan", Meaning = "vui", Radical = "欠", StrokeCount = 6, HskLevel = "HSK2" },
                new ChineseCharacter { Id = 12, Character = "吃", Pinyin = "chī", HanViet = "ngật", Meaning = "ăn", Radical = "口", StrokeCount = 6, HskLevel = "HSK1" },
                new ChineseCharacter { Id = 13, Character = "喝", Pinyin = "hē", HanViet = "hát", Meaning = "uống", Radical = "口", StrokeCount = 12, HskLevel = "HSK2" },
                new ChineseCharacter { Id = 14, Character = "去", Pinyin = "qù", HanViet = "khứ", Meaning = "đi", Radical = "厶", StrokeCount = 5, HskLevel = "HSK1" },
                new ChineseCharacter { Id = 15, Character = "来", Pinyin = "lái", HanViet = "lai", Meaning = "đến", Radical = "人", StrokeCount = 7, HskLevel = "HSK1" }
            );

            // Seed QuizQuestions
            builder.Entity<QuizQuestion>().HasData(
                new QuizQuestion { Id = 1, LessonId = 1, Question = "\"你好\" có nghĩa là gì?", OptionA = "Xin chào", OptionB = "Tạm biệt", OptionC = "Cảm ơn", OptionD = "Xin lỗi", CorrectAnswer = "A", Explanation = "你好 / nǐ hǎo / nghĩa là xin chào.", QuestionType = "Vocabulary", CreatedAt = new DateTime(2023, 1, 1) },
                new QuizQuestion { Id = 2, LessonId = 1, Question = "Pinyin của từ \"谢谢\" là gì?", OptionA = "nǐ hǎo", OptionB = "zài jiàn", OptionC = "xiè xie", OptionD = "duì bu qǐ", CorrectAnswer = "C", Explanation = "谢谢 đọc là xiè xie, nghĩa là cảm ơn.", QuestionType = "Vocabulary", CreatedAt = new DateTime(2023, 1, 1) },
                new QuizQuestion { Id = 3, LessonId = 1, Question = "\"再见\" nghĩa là gì?", OptionA = "Xin lỗi", OptionB = "Tạm biệt", OptionC = "Không sao", OptionD = "Rất tốt", CorrectAnswer = "B", Explanation = "再见 / zài jiàn / nghĩa là tạm biệt.", QuestionType = "Vocabulary", CreatedAt = new DateTime(2023, 1, 1) },
                new QuizQuestion { Id = 4, LessonId = 1, Question = "Số \"一\" đọc là gì?", OptionA = "èr", OptionB = "sān", OptionC = "yī", OptionD = "wǔ", CorrectAnswer = "C", Explanation = "一 đọc là yī, nghĩa là số một.", QuestionType = "Vocabulary", CreatedAt = new DateTime(2023, 1, 1) },
                new QuizQuestion { Id = 5, LessonId = 1, Question = "\"我是学生\" có nghĩa là gì?", OptionA = "Tôi là giáo viên", OptionB = "Tôi là học sinh", OptionC = "Bạn là học sinh", OptionD = "Bạn là giáo viên", CorrectAnswer = "B", Explanation = "我是学生 / wǒ shì xué shēng / nghĩa là Tôi là học sinh.", QuestionType = "Grammar", CreatedAt = new DateTime(2023, 1, 1) }
            );
        }
    }
}
