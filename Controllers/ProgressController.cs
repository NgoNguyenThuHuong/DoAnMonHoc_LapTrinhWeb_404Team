using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LingoToneMVC.Data;
using LingoToneMVC.Models;

namespace LingoToneMVC.Controllers
{
    [Authorize]
    public class ProgressController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProgressController(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            SetUserViewBag(user);

            var completedLessons = await _db.UserProgresses
                .Include(p => p.Lesson)
                .Where(p => p.UserId == user.Id && p.IsCompleted)
                .OrderByDescending(p => p.CompletedAt)
                .ToListAsync();

            var wordsLearned = await _db.UserLearnedWords
                .Where(w => w.UserId == user.Id)
                .CountAsync();

            var totalLessons = await _db.Lessons.CountAsync();

            ViewBag.TotalLessons = totalLessons;
            ViewBag.CompletedLessons = completedLessons.Count;
            ViewBag.WordsLearned = wordsLearned;

            // Weekly XP chart data (mock - show last 7 days)
            var weeklyData = new List<object>();
            for (int i = 6; i >= 0; i--)
            {
                var day = DateTime.Today.AddDays(-i);
                var xpOnDay = completedLessons
                    .Where(p => p.CompletedAt?.Date == day)
                    .Sum(p => p.XpEarned);
                weeklyData.Add(new { day = day.ToString("ddd"), xp = xpOnDay });
            }
            ViewBag.WeeklyData = weeklyData;

            return View(completedLessons);
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
