using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LingoToneMVC.Data;
using LingoToneMVC.Models;
using LingoToneMVC.ViewModels;
using System.Text.Json;
using System.IO;

using LingoToneMVC.Services;

namespace LingoToneMVC.Controllers
{
    public class HskWordDto
    {
        public int id { get; set; }
        public int level { get; set; }
        public string hanzi { get; set; }
        public string pinyin { get; set; }
        public HskTranslationDto translations { get; set; }
    }

    public class HskTranslationDto
    {
        public List<string> eng { get; set; }
    }

    public class LessonController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HskLessonService _hskLessonService;

        public LessonController(AppDbContext db, UserManager<ApplicationUser> userManager, HskLessonService hskLessonService)
        {
            _db = db;
            _userManager = userManager;
            _hskLessonService = hskLessonService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var lessons = await _hskLessonService.GetAllHskLessonsAsync();
            var titles = new[] { "Cơ bản", "Sơ cấp", "Tiền trung cấp", "Trung cấp", "Cao cấp", "Thành thạo" };
            var groups = new List<LessonGroupViewModel>();

            for (int i = 1; i <= 6; i++)
            {
                var levelLessons = lessons.Where(l => l.HskLevel == i.ToString()).ToList();
                if (!levelLessons.Any()) continue;

                var group = new LessonGroupViewModel 
                { 
                    Level = i, 
                    Title = $"HSK {i}: {titles[i - 1]}" 
                };
                group.Lessons.AddRange(levelLessons);
                groups.Add(group);
            }

            if (user == null)
            {
                ViewBag.CompletedLessonIds = new List<int>();
                ViewBag.UserDisplayName = "Khách";
                ViewBag.UserXP = 0;
                ViewBag.UserLevel = 1;
                ViewBag.UserStreak = 0;
                return View(groups);
            }

            var completedIds = await _db.UserProgresses
                .Where(p => p.UserId == user.Id && p.IsCompleted)
                .Select(p => p.LessonId)
                .ToListAsync();

            ViewBag.CompletedLessonIds = completedIds;
            SetUserViewBag(user);

            return View(groups);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (id < 1000)
            {
                var dbLesson = await _db.Lessons
                    .Include(l => l.Vocabularies)
                    .Include(l => l.GrammarPoints)
                    .Include(l => l.Dialogues)
                        .ThenInclude(d => d.Lines)
                    .FirstOrDefaultAsync(l => l.Id == id);
                if (dbLesson == null) return NotFound();

                int levelFromDb = 1;
                if (!string.IsNullOrEmpty(dbLesson.HskLevel) && dbLesson.HskLevel.StartsWith("HSK"))
                {
                    int.TryParse(dbLesson.HskLevel.Replace("HSK", ""), out levelFromDb);
                }

                EnsureLessonContent(dbLesson, levelFromDb);

                var isCompletedDb = false;
                if (user != null)
                {
                    isCompletedDb = await _db.UserProgresses
                        .AnyAsync(p => p.UserId == user.Id && p.LessonId == id && p.IsCompleted);
                    SetUserViewBag(user);
                }
                else
                {
                    ViewBag.UserDisplayName = "Khách";
                    ViewBag.UserXP = 0;
                    ViewBag.UserLevel = 1;
                    ViewBag.UserStreak = 0;
                }

                return View(new LessonDetailViewModel
                {
                    Lesson = dbLesson,
                    IsCompleted = isCompletedDb,
                    Vocabularies = dbLesson.Vocabularies.ToList(),
                    GrammarPoints = dbLesson.GrammarPoints.ToList(),
                    Dialogues = dbLesson.Dialogues.ToList()
                });
            }

            int level = id / 1000;
            var lesson = await _hskLessonService.GetHskLessonByIdAsync(id);

            if (lesson == null) return NotFound();

            EnsureLessonContent(lesson, level);

            var isCompleted = false;
            if (user != null)
            {
                isCompleted = await _db.UserProgresses
                    .AnyAsync(p => p.UserId == user.Id && p.LessonId == id && p.IsCompleted);
                SetUserViewBag(user);
            }
            else
            {
                ViewBag.UserDisplayName = "Khách";
                ViewBag.UserXP = 0;
                ViewBag.UserLevel = 1;
                ViewBag.UserStreak = 0;
            }

            var vm = new LessonDetailViewModel
            {
                Lesson = lesson,
                IsCompleted = isCompleted,
                Vocabularies = lesson.Vocabularies.ToList(),
                GrammarPoints = lesson.GrammarPoints.ToList(),
                Dialogues = lesson.Dialogues.ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Complete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Json(new { success = false });

            var alreadyCompleted = await _db.UserProgresses
                .AnyAsync(p => p.UserId == user.Id && p.LessonId == id && p.IsCompleted);

            if (!alreadyCompleted)
            {
                if (id >= 1000)
                {
                    var lessonExists = await _db.Lessons.AnyAsync(l => l.Id == id);
                    if (!lessonExists)
                    {
                        var dynamicLesson = await _hskLessonService.GetHskLessonByIdAsync(id);
                        if (dynamicLesson != null)
                        {
                            await _db.Database.ExecuteSqlRawAsync(
                                "SET IDENTITY_INSERT Lessons ON; " +
                                "INSERT INTO Lessons (Id, Title, Description, OrderIndex, HskLevel, ImageUrl) " +
                                "VALUES ({0}, {1}, {2}, {3}, {4}, {5}); " +
                                "SET IDENTITY_INSERT Lessons OFF;",
                                dynamicLesson.Id,
                                dynamicLesson.Title ?? "",
                                dynamicLesson.Description ?? "",
                                dynamicLesson.OrderIndex,
                                dynamicLesson.HskLevel ?? "",
                                dynamicLesson.ImageUrl
                            );
                        }
                    }
                }

                // Note: We skip fetching from DB for HSK levels to avoid mismatch. 
                // We just mark it as complete in UserProgress.
                var progress = new UserProgress
                {
                    UserId = user.Id,
                    LessonId = id,
                    IsCompleted = true,
                    CompletedAt = DateTime.Now,
                    XpEarned = 100
                };
                _db.UserProgresses.Add(progress);

                user.XP += 100;
                user.Level = (user.XP / 500) + 1;
                await _userManager.UpdateAsync(user);
                await _db.SaveChangesAsync();

                return Json(new { success = true, xp = user.XP, level = user.Level, xpEarned = 100 });
            }

            return Json(new { success = false, message = "Bài học này đã hoàn thành rồi!" });
        }

        private void SetUserViewBag(ApplicationUser user)
        {
            ViewBag.UserDisplayName = user.DisplayName;
            ViewBag.UserXP = user.XP;
            ViewBag.UserLevel = user.Level;
            ViewBag.UserStreak = user.Streak;
        }

        private void EnsureLessonContent(Lesson lesson, int level)
        {
            if (lesson.GrammarPoints == null || !lesson.GrammarPoints.Any())
            {
                lesson.GrammarPoints = GenerateDefaultGrammars(lesson, level);
            }

            if (lesson.Dialogues == null || !lesson.Dialogues.Any())
            {
                lesson.Dialogues = GenerateDefaultDialogues(lesson, level);
            }
        }

        private List<GrammarPoint> GenerateDefaultGrammars(Lesson lesson, int level)
        {
            var grammars = new List<GrammarPoint>();
            if (level == 1)
            {
                grammars.Add(new GrammarPoint
                {
                    Title = "A 是 B",
                    Pattern = "A + 是 + B",
                    Explanation = "Dùng để giới thiệu hoặc xác nhận ai/cái gì là gì.",
                    Example = "我是学生。",
                    ExamplePinyin = "Wǒ shì xuéshēng.",
                    ExampleMeaning = "Tôi là học sinh."
                });
                grammars.Add(new GrammarPoint
                {
                    Title = "不 + Động từ/Tính từ",
                    Pattern = "S + 不 + V/Adj",
                    Explanation = "Dùng để phủ định hành động hoặc tính chất.",
                    Example = "我不忙。",
                    ExamplePinyin = "Wǒ bù máng.",
                    ExampleMeaning = "Tôi không bận."
                });
                grammars.Add(new GrammarPoint
                {
                    Title = "吗 dùng để hỏi",
                    Pattern = "Câu trần thuật + 吗？",
                    Explanation = "Biến câu trần thuật thành câu hỏi Yes/No.",
                    Example = "你是学生吗？",
                    ExamplePinyin = "Nǐ shì xuéshēng ma?",
                    ExampleMeaning = "Bạn là học sinh phải không?"
                });
            }
            else if (level == 2)
            {
                grammars.Add(new GrammarPoint
                {
                    Title = "Đã xảy ra với 了",
                    Pattern = "V + 了",
                    Explanation = "Diễn tả hành động đã hoàn thành.",
                    Example = "我吃了。",
                    ExamplePinyin = "Wǒ chī le.",
                    ExampleMeaning = "Tôi ăn rồi."
                });
                grammars.Add(new GrammarPoint
                {
                    Title = "Đã từng với 过",
                    Pattern = "V + 过",
                    Explanation = "Diễn tả trải nghiệm đã từng xảy ra trong quá khứ.",
                    Example = "我去过中国。",
                    ExamplePinyin = "Wǒ qù guò Zhōngguó.",
                    ExampleMeaning = "Tôi đã từng đi Trung Quốc."
                });
                grammars.Add(new GrammarPoint
                {
                    Title = "Muốn/Sắp với 要",
                    Pattern = "S + 要 + V",
                    Explanation = "Diễn tả ý định hoặc sự việc sắp xảy ra.",
                    Example = "我要去北京。",
                    ExamplePinyin = "Wǒ yào qù Běijīng.",
                    ExampleMeaning = "Tôi muốn đi Bắc Kinh."
                });
            }
            else
            {
                grammars.Add(new GrammarPoint
                {
                    Title = "Mặc dù... Nhưng... (虽然...但是...)",
                    Pattern = "虽然 + Mệnh đề 1, 但是 + Mệnh đề 2",
                    Explanation = "Biểu thị sự nhượng bộ, trái ngược.",
                    Example = "虽然下雨，但是我还是去。",
                    ExamplePinyin = "Suīrán xiàyǔ, dànshì wǒ háishì qù.",
                    ExampleMeaning = "Mặc dù trời mưa, nhưng tôi vẫn đi."
                });
                grammars.Add(new GrammarPoint
                {
                    Title = "Nếu... thì... (如果...就...)",
                    Pattern = "如果 + Điều kiện, (S) + 就 + Kết quả",
                    Explanation = "Biểu thị giả thiết và kết quả.",
                    Example = "如果有时间，我就去。",
                    ExamplePinyin = "Rúguǒ yǒu shíjiān, wǒ jiù qù.",
                    ExampleMeaning = "Nếu có thời gian, tôi sẽ đi."
                });
                grammars.Add(new GrammarPoint
                {
                    Title = "Câu chữ 把",
                    Pattern = "S + 把 + O + V + Thành phần khác",
                    Explanation = "Nhấn mạnh sự tác động của chủ ngữ lên tân ngữ.",
                    Example = "我把书看完了。",
                    ExamplePinyin = "Wǒ bǎ shū kàn wán le.",
                    ExampleMeaning = "Tôi đã xem xong sách rồi."
                });
            }
            return grammars;
        }

        private List<Dialogue> GenerateDefaultDialogues(Lesson lesson, int level)
        {
            var vocabList = lesson.Vocabularies?.ToList() ?? new List<Vocabulary>();
            string targetHanzi1 = vocabList.Count > 0 ? vocabList[0].Chinese : "你好";
            string targetPinyin1 = vocabList.Count > 0 ? vocabList[0].Pinyin : "Nǐ hǎo";
            string targetVi1 = vocabList.Count > 0 ? vocabList[0].Vietnamese : "Xin chào";

            string targetHanzi2 = vocabList.Count > 1 ? vocabList[1].Chinese : "谢谢";
            string targetPinyin2 = vocabList.Count > 1 ? vocabList[1].Pinyin : "Xièxie";
            string targetVi2 = vocabList.Count > 1 ? vocabList[1].Vietnamese : "Cảm ơn";

            return new List<Dialogue>
            {
                new Dialogue
                {
                    Title = $"Hội thoại thực hành bài {lesson.Id}",
                    Lines = new List<DialogueLine>
                    {
                        new DialogueLine { Speaker = "A", Hanzi = $"你学习 '{targetHanzi1}' 吗？", Pinyin = $"Nǐ xuéxí '{targetPinyin1}' ma?", MeaningVi = $"Bạn có học từ '{targetVi1}' không?" },
                        new DialogueLine { Speaker = "B", Hanzi = $"对，我学习 '{targetHanzi1}'。", Pinyin = $"Duì, wǒ xuéxí '{targetPinyin1}'.", MeaningVi = $"Đúng vậy, tôi có học từ '{targetVi1}'." },
                        new DialogueLine { Speaker = "A", Hanzi = $"'{targetHanzi2}' 呢？", Pinyin = $"'{targetPinyin2}' ne?", MeaningVi = $"Còn từ '{targetVi2}' thì sao?" },
                        new DialogueLine { Speaker = "B", Hanzi = $"我也学习 '{targetHanzi2}'。", Pinyin = $"Wǒ yě xuéxí '{targetPinyin2}'.", MeaningVi = $"Tôi cũng học từ '{targetVi2}'." }
                    }
                }
            };
        }
    }
}
