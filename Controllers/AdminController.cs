using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LingoToneMVC.Models;
using LingoToneMVC.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

using LingoToneMVC.Services;

namespace LingoToneMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HskLessonService _hskLessonService;
        private readonly IAiService _aiService;

        public AdminController(AppDbContext db, UserManager<ApplicationUser> userManager, HskLessonService hskLessonService, IAiService aiService)
        {
            _db = db;
            _userManager = userManager;
            _hskLessonService = hskLessonService;
            _aiService = aiService;
        }

        public async Task<IActionResult> Index()
        {
            var hskLessons = await _hskLessonService.GetAllHskLessonsAsync();
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
            var lessons = await _hskLessonService.GetAllHskLessonsAsync();
            return View(lessons);
        }

        public async Task<IActionResult> Vocabularies()
        {
            var lessons = await _hskLessonService.GetAllHskLessonsAsync();
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

        [HttpPost]
        public async Task<IActionResult> TranslateVocabulary(int id)
        {
            var vocab = await _db.Vocabularies.FindAsync(id);
            if (vocab == null) return NotFound(new { success = false, message = "Không tìm thấy từ vựng." });

            var result = await _aiService.TranslateVocabularyAsync(vocab.Pinyin + " - " + vocab.Chinese);
            if (result.Success && !result.IsFallback)
            {
                try
                {
                    var doc = JsonDocument.Parse(result.Content);
                    var vi = doc.RootElement.GetProperty("vietnamese").GetString();
                    vocab.Vietnamese = vi;
                    await _db.SaveChangesAsync();
                    return Json(new { success = true, vietnamese = vi });
                }
                catch
                {
                    return Json(new { success = false, message = "Lỗi khi xử lý phản hồi từ AI." });
                }
            }

            return Json(new { success = false, message = result.ErrorMessage ?? "AI đang quá tải, không thể dịch lúc này." });
        }
    }
}
