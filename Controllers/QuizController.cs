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
    public class QuizController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HskLessonService _hskLessonService;

        public QuizController(AppDbContext db, UserManager<ApplicationUser> userManager, HskLessonService hskLessonService)
        {
            _db = db;
            _userManager = userManager;
            _hskLessonService = hskLessonService;
        }

        private async Task<Lesson?> GetLessonAsync(int id)
        {
            if (id < 1000)
            {
                return await _db.Lessons.Include(l => l.Vocabularies).FirstOrDefaultAsync(l => l.Id == id);
            }

            return await _hskLessonService.GetHskLessonByIdAsync(id);
        }

        private bool LooksLikeEnglishMeaning(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return true;

            var lower = text.ToLower();

            return lower.StartsWith("to ")
                || lower.Contains("variant of")
                || lower.Contains("classifier for")
                || lower.Contains("surname")
                || lower.Contains("old variant")
                || lower.Contains("you are")
                || lower.Contains("you're")
                || lower.Contains("you're welcome")
                || lower.Contains("highest")
                || lower.Contains("to go to bed")
                || lower.Contains("brother")
                || lower.Contains("sister")
                || lower == "big"
                || lower.Contains("english")
                || lower.Contains("definition");
        }

        private async Task<List<QuizQuestion>> GenerateQuizFromVocab(Lesson lesson)
        {
            var questions = new List<QuizQuestion>();
            var vocabs = lesson.Vocabularies?.ToList() ?? new List<Vocabulary>();
            if (!vocabs.Any()) return questions;

            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "hsk.json");
            var json = await System.IO.File.ReadAllTextAsync(jsonPath);
            var hskData = JsonSerializer.Deserialize<List<HskWordDto>>(json) ?? new List<HskWordDto>();

            // Filter to same level for better distractors
            int level = lesson.Id >= 1000 ? lesson.Id / 1000 : 1;
            var levelData = hskData.Where(w => w.level == level).ToList();
            if (!levelData.Any()) levelData = hskData; // fallback

            // Fetch all vocabularies from DB for Vietnamese distractors (exclude 'Đang dịch...' and empty)
            var allVocabs = await _db.Vocabularies
                .Where(v => !string.IsNullOrEmpty(v.Vietnamese) && v.Vietnamese != "Đang dịch...")
                .Select(v => v.Vietnamese)
                .Distinct()
                .ToListAsync();

            // 1 question per vocabulary max
            int targetQuestionCount = Math.Min(15, vocabs.Count);
            var random = new Random();
            int qId = 10000;

            // Generate exactly one unique question per word (randomly Pinyin or Meaning)
            var questionPool = new List<(Vocabulary v, bool isPinyin)>();
            foreach (var v in vocabs)
            {
                bool askPinyin = random.Next(2) == 0;

                // FORCE Pinyin question if the meaning in DB accidentally contains English, is empty, or is "Đang dịch..."
                if (!askPinyin && (string.IsNullOrWhiteSpace(v.Vietnamese) || v.Vietnamese == "Đang dịch..." || LooksLikeEnglishMeaning(v.Vietnamese)))
                {
                    askPinyin = true;
                }

                questionPool.Add((v, askPinyin));
            }

            // Shuffle the pool
            questionPool = questionPool.OrderBy(x => random.Next()).ToList();

            // Take the required amount
            var selectedQuestions = questionPool.Take(targetQuestionCount).ToList();

            foreach (var item in selectedQuestions)
            {
                var v = item.v;
                bool isPinyin = item.isPinyin;

                if (isPinyin)
                {
                    var options = new HashSet<string> { v.Pinyin };
                    while (options.Count < 4)
                    {
                        var randWord = levelData[random.Next(levelData.Count)];
                        if (!string.IsNullOrEmpty(randWord.pinyin))
                            options.Add(randWord.pinyin);
                    }
                    var optsList = options.OrderBy(x => random.Next()).ToList();
                    var correctIdx = optsList.IndexOf(v.Pinyin);
                    var letters = new[] { "A", "B", "C", "D" };

                    questions.Add(new QuizQuestion
                    {
                        Id = qId++,
                        LessonId = lesson.Id,
                        Question = $"Pinyin của từ \"{v.Chinese}\" là gì?",
                        OptionA = optsList[0],
                        OptionB = optsList[1],
                        OptionC = optsList[2],
                        OptionD = optsList[3],
                        CorrectAnswer = letters[correctIdx],
                        Explanation = $"{v.Chinese} / {v.Pinyin} / nghĩa là {v.Vietnamese}.",
                        QuestionType = "Vocabulary",
                        XpReward = 20
                    });
                }
                else
                {
                    var options = new HashSet<string> { v.Vietnamese };

                    // Distractors from Vietnamese DB (exclude English looking strings)
                    var wrongOptions = allVocabs
                        .Where(x => x != v.Vietnamese && !LooksLikeEnglishMeaning(x))
                        .OrderBy(x => random.Next())
                        .Take(3)
                        .ToList();

                    foreach (var w in wrongOptions) options.Add(w);

                    // Fallback to generic Vietnamese if DB has less than 3 other vocabs
                    string[] genericDistractors = { "xin chào", "cảm ơn", "tạm biệt", "không có gì", "rất tốt", "ngày mai", "hôm nay", "điện thoại" };
                    while (options.Count < 4)
                    {
                        options.Add(genericDistractors[random.Next(genericDistractors.Length)]);
                    }

                    var optsList = options.OrderBy(x => random.Next()).ToList();
                    var correctIdx = optsList.IndexOf(v.Vietnamese);
                    var letters = new[] { "A", "B", "C", "D" };

                    questions.Add(new QuizQuestion
                    {
                        Id = qId++,
                        LessonId = lesson.Id,
                        Question = $"Từ \"{v.Chinese}\" có nghĩa là gì?",
                        OptionA = optsList[0],
                        OptionB = optsList[1],
                        OptionC = optsList[2],
                        OptionD = optsList[3],
                        CorrectAnswer = letters[correctIdx],
                        Explanation = $"{v.Chinese} / {v.Pinyin} / nghĩa là {v.Vietnamese}.",
                        QuestionType = "Vocabulary",
                        XpReward = 20
                    });
                }
            }

            return questions;
        }

        public async Task<IActionResult> Index(int? lessonId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null) SetUserViewBag(user);
            else
            {
                ViewBag.UserDisplayName = "Khách";
                ViewBag.UserXP = 0;
                ViewBag.UserLevel = 1;
                ViewBag.UserStreak = 0;
            }

            if (lessonId == null)
            {
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

                // Get Weekly Leaderboard
                var arenaCurrentUserId = _userManager.GetUserId(User);
                var arenaStartOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);

                var arenaRawLeaderboard = await _db.QuizResults
                    .Include(x => x.User)
                    .Where(x => x.CreatedAt >= arenaStartOfWeek)
                    .GroupBy(x => new { x.UserId, x.User!.DisplayName })
                    .Select(g => new
                    {
                        UserId = g.Key.UserId,
                        UserName = g.Key.DisplayName ?? "Người dùng",
                        XP = g.Sum(x => x.XPGained)
                    })
                    .OrderByDescending(x => x.XP)
                    .Take(10)
                    .ToListAsync();

                var arenaLeaderboard = arenaRawLeaderboard.Select((x, idx) => new LeaderboardItemViewModel
                {
                    Rank = idx + 1,
                    UserName = x.UserName,
                    XP = x.XP,
                    IsCurrentUser = x.UserId == arenaCurrentUserId
                }).ToList();

                if (arenaCurrentUserId != null && !arenaLeaderboard.Any(x => x.IsCurrentUser))
                {
                    var myXp = await _db.QuizResults
                        .Where(x => x.UserId == arenaCurrentUserId && x.CreatedAt >= arenaStartOfWeek)
                        .SumAsync(x => x.XPGained);

                    arenaLeaderboard.Add(new LeaderboardItemViewModel
                    {
                        Rank = arenaLeaderboard.Count > 0 ? arenaLeaderboard.Max(x => x.Rank) + 1 : 1,
                        UserName = "Bạn",
                        XP = myXp,
                        IsCurrentUser = true
                    });
                }

                // Get User statistics
                var completedQuizIds = new List<int>();
                int totalQuizzes = 0;
                int totalXpEarned = 0;

                if (arenaCurrentUserId != null)
                {
                    completedQuizIds = await _db.QuizResults
                        .Where(x => x.UserId == arenaCurrentUserId)
                        .Select(x => x.LessonId)
                        .Distinct()
                        .ToListAsync();

                    totalQuizzes = await _db.QuizResults
                        .Where(x => x.UserId == arenaCurrentUserId)
                        .CountAsync();

                    totalXpEarned = await _db.QuizResults
                        .Where(x => x.UserId == arenaCurrentUserId)
                        .SumAsync(x => x.XPGained);
                }

                var arenaVm = new QuizArenaViewModel
                {
                    LessonGroups = groups,
                    WeeklyLeaderboard = arenaLeaderboard,
                    CompletedQuizLessonIds = completedQuizIds,
                    TotalQuizzesTaken = totalQuizzes,
                    TotalXPEarned = totalXpEarned
                };

                return View("Arena", arenaVm);
            }

            var lesson = await GetLessonAsync(lessonId.Value);

            if (lesson == null) return NotFound("Không tìm thấy bài học.");

            var questions = await _db.QuizQuestions
                .Where(x => x.LessonId == lessonId.Value)
                .OrderBy(x => x.Id)
                .ToListAsync();

            if (!questions.Any())
            {
                questions = await GenerateQuizFromVocab(lesson);
                if (questions.Any())
                {
                    HttpContext.Session.SetString($"quiz_lesson_{lessonId.Value}", JsonSerializer.Serialize(questions, new JsonSerializerOptions { ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles }));
                }
            }

            var currentUserId = _userManager.GetUserId(User);
            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);

            var rawLeaderboard = await _db.QuizResults
                .Include(x => x.User)
                .Where(x => x.CreatedAt >= startOfWeek)
                .GroupBy(x => new { x.UserId, x.User.DisplayName })
                .Select(g => new
                {
                    UserId = g.Key.UserId,
                    UserName = g.Key.DisplayName ?? "Người dùng",
                    XP = g.Sum(x => x.XPGained)
                })
                .OrderByDescending(x => x.XP)
                .Take(10)
                .ToListAsync();

            var leaderboard = rawLeaderboard.Select((x, i) => new LeaderboardItemViewModel
            {
                Rank = i + 1,
                UserName = x.UserName,
                XP = x.XP,
                IsCurrentUser = x.UserId == currentUserId
            }).ToList();

            if (currentUserId != null && !leaderboard.Any(x => x.IsCurrentUser))
            {
                // Add the current user at the bottom if they have XP but not in top 10
                var myXp = await _db.QuizResults
                    .Where(x => x.UserId == currentUserId && x.CreatedAt >= startOfWeek)
                    .SumAsync(x => x.XPGained);

                leaderboard.Add(new LeaderboardItemViewModel
                {
                    Rank = leaderboard.Count > 0 ? leaderboard.Max(x => x.Rank) + 1 : 1,
                    UserName = "Bạn",
                    XP = myXp,
                    IsCurrentUser = true
                });
            }

            var vm = new QuizViewModel
            {
                LessonId = lesson.Id,
                LessonTitle = lesson.Title,
                HskLevel = lesson.HskLevel ?? "HSK1",
                Questions = questions,
                WeeklyLeaderboard = leaderboard
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateQuiz(int lessonId)
        {
            var lesson = await GetLessonAsync(lessonId);
            if (lesson == null) return NotFound();

            var existed = await _db.QuizQuestions.AnyAsync(x => x.LessonId == lessonId);
            if (existed)
            {
                TempData["Message"] = "Bài này đã có câu hỏi Quiz trong hệ thống.";
                return RedirectToAction("Index", new { lessonId });
            }

            // You can use AI here, or just save the generated vocab quiz to DB permanently
            var generated = await GenerateQuizFromVocab(lesson);
            if (generated.Any())
            {
                if (lessonId >= 1000)
                {
                    var lessonExists = await _db.Lessons.AnyAsync(l => l.Id == lessonId);
                    if (!lessonExists)
                    {
                        await _db.Database.ExecuteSqlRawAsync(
                            "SET IDENTITY_INSERT Lessons ON; " +
                            "INSERT INTO Lessons (Id, Title, Description, OrderIndex, HskLevel, ImageUrl) " +
                            "VALUES ({0}, {1}, {2}, {3}, {4}, {5}); " +
                            "SET IDENTITY_INSERT Lessons OFF;",
                            lesson.Id,
                            lesson.Title ?? "",
                            lesson.Description ?? "",
                            lesson.OrderIndex,
                            lesson.HskLevel ?? "",
                            lesson.ImageUrl
                        );
                    }
                }

                // To avoid tracking issue, set Id to 0 for auto-increment if we insert to DB
                foreach (var q in generated) { q.Id = 0; }
                _db.QuizQuestions.AddRange(generated);
                await _db.SaveChangesAsync();
                TempData["Message"] = "Đã lưu Quiz cố định vào Database.";
            }

            return RedirectToAction("Index", new { lessonId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(List<string> answers, int lessonId, int? comboXp = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(new { success = false, message = "Bạn cần đăng nhập để lưu XP." });

            var questions = await _db.QuizQuestions.Where(q => q.LessonId == lessonId).OrderBy(q => q.Id).ToListAsync();

            if (!questions.Any())
            {
                var sessionJson = HttpContext.Session.GetString($"quiz_lesson_{lessonId}");
                if (!string.IsNullOrEmpty(sessionJson))
                {
                    questions = JsonSerializer.Deserialize<List<QuizQuestion>>(sessionJson) ?? new List<QuizQuestion>();
                }
            }

            int score = 0;
            int totalXp = 0;
            int currentCombo = 0;
            int maxCombo = 0;
            var results = new List<object>();

            for (int i = 0; i < Math.Min(answers.Count, questions.Count); i++)
            {
                bool isCorrect = answers[i] == questions[i].CorrectAnswer;
                if (isCorrect)
                {
                    score++;
                    currentCombo++;
                    maxCombo = Math.Max(maxCombo, currentCombo);
                    int xpGained = questions[i].XpReward + (currentCombo > 2 ? 5 : 0);
                    totalXp += xpGained;
                }
                else
                {
                    currentCombo = 0;
                }

                results.Add(new
                {
                    questionId = questions[i].Id,
                    correct = isCorrect,
                    correctAnswer = questions[i].CorrectAnswer,
                    yourAnswer = answers[i]
                });
            }

            var quizResult = new QuizResult
            {
                UserId = user.Id,
                LessonId = lessonId,
                CorrectAnswers = score,
                TotalQuestions = questions.Count,
                MaxCombo = maxCombo,
                XPGained = totalXp,
                CreatedAt = DateTime.Now
            };
            _db.QuizResults.Add(quizResult);

            if (totalXp > 0)
            {
                user.XP += totalXp;
                user.Level = (user.XP / 500) + 1;
                await _userManager.UpdateAsync(user);
            }

            await _db.SaveChangesAsync();

            return Json(new
            {
                success = true,
                score,
                total = questions.Count,
                xpEarned = totalXp,
                xp = user.XP,
                level = user.Level,
                results
            });
        }

        private void SetUserViewBag(ApplicationUser user)
        {
            ViewBag.UserDisplayName = user.DisplayName;
            ViewBag.UserXP = user.XP;
            ViewBag.UserLevel = user.Level;
            ViewBag.UserStreak = user.Streak;
        }
    }
}
