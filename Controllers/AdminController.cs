using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LingoToneMVC.Models;
using LingoToneMVC.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LingoToneMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public AdminController(AppDbContext db, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _db = db;
            _userManager = userManager;
            _env = env;
        }

        private async Task<List<Lesson>> GetHskLessonsAsync()
        {
            var jsonPath = Path.Combine(_env.WebRootPath, "data", "hsk.json");
            var json = await System.IO.File.ReadAllTextAsync(jsonPath);
            var hskData = JsonSerializer.Deserialize<List<HskWordDto>>(json) ?? new List<HskWordDto>();

            var lessons = new List<Lesson>();
            var lessonNames = new[] { "Từ vựng nền tảng", "Giao tiếp cơ bản", "Gia đình & con người", "Thời gian & thời tiết", "Ăn uống & sở thích", "Mua sắm & du lịch", "Công việc & học tập", "Sở thích 2", "Mở rộng 1", "Mở rộng 2", "Mở rộng 3" };

            for (int i = 1; i <= 6; i++)
            {
                var levelWords = hskData.Where(w => w.level == i).ToList();
                if (!levelWords.Any()) continue;

                int chunks = (int)Math.Ceiling(levelWords.Count / 15.0);

                for (int j = 0; j < chunks; j++)
                {
                    var chunkWords = levelWords.Skip(j * 15).Take(15).ToList();
                    var name = j < lessonNames.Length ? lessonNames[j] : $"Chủ đề {j + 1}";
                    
                    var lesson = new Lesson
                    {
                        Id = i * 1000 + (j + 1), // Format: 1001, 2005, etc.
                        Title = $"Bài {j + 1}: {name}",
                        Description = $"Chinh phục {chunkWords.Count} từ vựng",
                        OrderIndex = j + 1,
                        HskLevel = i.ToString(),
                        Vocabularies = chunkWords.Select(w => new Vocabulary
                        {
                            Chinese = w.hanzi,
                            Pinyin = w.pinyin,
                            Vietnamese = GetVietnameseMeaning(w.hanzi) ?? "Đang dịch...",
                            LessonId = i * 1000 + (j + 1)
                        }).ToList()
                    };
                    
                    foreach (var v in lesson.Vocabularies)
                    {
                        v.Lesson = lesson;
                    }
                    
                    lessons.Add(lesson);
                }
            }
            return lessons;
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

        public async Task<IActionResult> Index()
        {
            var hskLessons = await GetHskLessonsAsync();
            ViewBag.TotalUsers = await _db.Users.CountAsync();
            ViewBag.TotalLessons = hskLessons.Count;
            ViewBag.TotalVocabs = hskLessons.Sum(l => l.Vocabularies.Count);
            ViewBag.TotalQuizQuestions = await _db.QuizQuestions.CountAsync();
            return View();
        }

        public async Task<IActionResult> Users()
        {
            var users = await _db.Users.ToListAsync();
            var userRoles = new Dictionary<string, IList<string>>();
            foreach (var u in users)
                userRoles[u.Id] = await _userManager.GetRolesAsync(u);

            ViewBag.UserRoles = userRoles;
            return View(users);
        }

        public async Task<IActionResult> Lessons()
        {
            var lessons = await GetHskLessonsAsync();
            return View(lessons);
        }

        public async Task<IActionResult> Vocabularies()
        {
            var lessons = await GetHskLessonsAsync();
            var vocabs = lessons.SelectMany(l => l.Vocabularies)
                .OrderBy(v => v.LessonId)
                .Take(200)
                .ToList();
            return View(vocabs);
        }

        public async Task<IActionResult> Quizzes()
        {
            var quizzes = await _db.QuizQuestions
                .Include(q => q.Lesson)
                .OrderBy(q => q.LessonId)
                .ToListAsync();
            return View(quizzes);
        }
    }
}
