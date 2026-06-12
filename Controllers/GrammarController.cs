using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LingoToneMVC.Data;
using LingoToneMVC.Models;

namespace LingoToneMVC.Controllers
{
    public class GrammarController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public GrammarController(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
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

            var grammarPoints = await _db.GrammarPoints
                .Include(g => g.Lesson)
                .OrderBy(g => g.Id)
                .ToListAsync();

            return View(grammarPoints);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckAnswer(int questionIndex, int selectedAnswer)
        {
            var user = await _userManager.GetUserAsync(User);

            // Simple quiz: check answer against grammar examples
            var grammarPoints = await _db.GrammarPoints.OrderBy(g => g.Id).ToListAsync();
            if (questionIndex < 0 || questionIndex >= grammarPoints.Count)
                return Json(new { correct = false, message = "Câu hỏi không hợp lệ" });

            // For demo: answer 0 is always correct for grammar mini-quiz
            bool isCorrect = selectedAnswer == 0;

            if (isCorrect && user != null)
            {
                user.XP += 10;
                user.Level = (user.XP / 500) + 1;
                await _userManager.UpdateAsync(user);
            }

            return Json(new
            {
                correct = isCorrect,
                message = isCorrect ? "Chính xác! +10 XP 🎉" : "Sai rồi! Hãy thử lại.",
                xp = user?.XP ?? 0
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
