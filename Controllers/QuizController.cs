using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LingoToneMVC.Data;
using LingoToneMVC.Models;
using LingoToneMVC.ViewModels;
using System.Text.Json;
using System.IO;

namespace LingoToneMVC.Controllers
{
    public class QuizController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuizController(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        private string? GetVietnameseMeaning(string hanzi)
        {
            var dictionary = new Dictionary<string, string>
            {
                { "大", "lớn, to" },
                { "爱", "yêu, thích" },
                { "的", "của (trợ từ sở hữu)" },
                { "我", "tôi, mình" },
                { "你", "bạn, cậu" },
                { "好", "tốt, khỏe" },
                { "是", "là" },
                { "不", "không" },
                { "人", "người" },
                { "很", "rất" }
            };
            if (dictionary.TryGetValue(hanzi, out var meaning)) return meaning;
            return null;
        }

        private async Task<Lesson?> GetLessonAsync(int id)
        {
            if (id < 1000)
            {
                return await _db.Lessons.Include(l => l.Vocabularies).FirstOrDefaultAsync(l => l.Id == id);
            }

            int level = id / 1000;
            int lessonIndex = (id % 1000) - 1;

            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "hsk.json");
            var json = await System.IO.File.ReadAllTextAsync(jsonPath);
            var hskData = JsonSerializer.Deserialize<List<HskWordDto>>(json) ?? new List<HskWordDto>();

            var levelWords = hskData.Where(w => w.level == level).ToList();
            var words = levelWords.Skip(lessonIndex * 15).Take(15).ToList();

            if (!words.Any()) return null;

            var lessonNames = new[] { "Từ vựng nền tảng", "Giao tiếp cơ bản", "Gia đình & con người", "Thời gian & thời tiết", "Ăn uống & sở thích", "Mua sắm & du lịch", "Công việc & học tập", "Sở thích 2", "Mở rộng 1", "Mở rộng 2", "Mở rộng 3" };
            var name = lessonIndex < lessonNames.Length ? lessonNames[lessonIndex] : $"Chủ đề {lessonIndex + 1}";

            return new Lesson
            {
                Id = id,
                Title = $"Bài {lessonIndex + 1}: {name}",
                HskLevel = $"HSK {level}",
                Vocabularies = words.Select(w => new Vocabulary
                {
                    Chinese = w.hanzi,
                    Pinyin = w.pinyin,
                    Vietnamese = GetVietnameseMeaning(w.hanzi) ?? ""
                }).ToList()
            };
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

            if (lessonId == null) return RedirectToAction("Index", "Lesson");

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
