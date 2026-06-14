using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LingoToneMVC.Models;
using LingoToneMVC.Data;
using Microsoft.EntityFrameworkCore;

namespace LingoToneMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalUsers = await _db.Users.CountAsync();
            ViewBag.TotalLessons = await _db.Lessons.CountAsync();
            ViewBag.TotalVocabs = await _db.Vocabularies.CountAsync();
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
            var lessons = await _db.Lessons
                .Include(l => l.Vocabularies)
                .OrderBy(l => l.Id)
                .ToListAsync();
            return View(lessons);
        }

        public async Task<IActionResult> Vocabularies()
        {
            var vocabs = await _db.Vocabularies
                .Include(v => v.Lesson)
                .OrderBy(v => v.LessonId)
                .Take(200)
                .ToListAsync();
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
