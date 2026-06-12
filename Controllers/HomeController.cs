using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LingoToneMVC.Data;
using LingoToneMVC.Models;
using LingoToneMVC.ViewModels;

namespace LingoToneMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return View("LandingPage");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return View("LandingPage");
            }

            // Update streak
            await UpdateStreakAsync(user);

            var totalLessons = await _db.Lessons.CountAsync();
            var completedLessons = await _db.UserProgresses
                .CountAsync(p => p.UserId == user.Id && p.IsCompleted);
            var wordsLearned = await _db.UserLearnedWords
                .CountAsync(w => w.UserId == user.Id);

            var currentLesson = await _db.Lessons
                .Include(l => l.Vocabularies)
                .Include(l => l.GrammarPoints)
                .Where(l => !_db.UserProgresses.Any(p => p.UserId == user.Id && p.LessonId == l.Id && p.IsCompleted))
                .OrderBy(l => l.OrderIndex)
                .FirstOrDefaultAsync() ?? await _db.Lessons
                .Include(l => l.Vocabularies)
                .Include(l => l.GrammarPoints)
                .OrderBy(l => l.OrderIndex)
                .FirstOrDefaultAsync();

            var recentProgresses = await _db.UserProgresses
                .Include(p => p.Lesson)
                .Where(p => p.UserId == user.Id && p.IsCompleted)
                .OrderByDescending(p => p.CompletedAt)
                .Take(5)
                .ToListAsync();

            // Daily Missions Logic
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var learnedWordsToday = await _db.UserLearnedWords
                .CountAsync(x => x.UserId == user.Id && x.LearnedAt >= today && x.LearnedAt < tomorrow);

            var quizCountToday = await _db.UserQuizAttempts
                .CountAsync(x => x.UserId == user.Id && x.CreatedAt >= today && x.CreatedAt < tomorrow);

            var srsCountToday = await _db.UserSrsReviews
                .CountAsync(x => x.UserId == user.Id && x.ReviewedAt >= today && x.ReviewedAt < tomorrow);

            var completedLessonsToday = await _db.UserProgresses
                .CountAsync(x => x.UserId == user.Id && x.IsCompleted && x.CompletedAt >= today && x.CompletedAt < tomorrow);

            var loggedInToday = await _db.UserDailyLogins
                .AnyAsync(x => x.UserId == user.Id && x.LoginDate == today);

            if (!loggedInToday && user.LastLoginDate?.Date == today)
            {
                // Ensure daily login is tracked
                _db.UserDailyLogins.Add(new UserDailyLogin { UserId = user.Id, LoginDate = today });
                await _db.SaveChangesAsync();
                loggedInToday = true;
            }

            var missions = new List<DailyMissionViewModel>
            {
                new() { Key = "learn_10_words", Title = "Học 10 từ mới", CurrentValue = learnedWordsToday, TargetValue = 10, XpReward = 20 },
                new() { Key = "quiz_5_questions", Title = "Làm 5 câu quiz", CurrentValue = quizCountToday, TargetValue = 5, XpReward = 20 },
                new() { Key = "review_srs", Title = "Ôn SRS cards", CurrentValue = srsCountToday, TargetValue = 1, XpReward = 20 },
                new() { Key = "complete_1_lesson", Title = "Hoàn thành 1 bài học", CurrentValue = completedLessonsToday, TargetValue = 1, XpReward = 20 },
                new() { Key = "login_today", Title = "Đăng nhập hôm nay", CurrentValue = loggedInToday ? 1 : 0, TargetValue = 1, XpReward = 10 }
            };

            // Award XP for newly completed missions
            var claimedMissionsToday = await _db.DailyMissionClaims
                .Where(x => x.UserId == user.Id && x.ClaimDate == today)
                .Select(x => x.MissionKey)
                .ToListAsync();

            bool needSave = false;
            foreach (var mission in missions)
            {
                if (mission.IsCompleted && !claimedMissionsToday.Contains(mission.Key))
                {
                    user.XP += mission.XpReward;
                    user.Level = (user.XP / 500) + 1;
                    _db.DailyMissionClaims.Add(new DailyMissionClaim
                    {
                        UserId = user.Id,
                        MissionKey = mission.Key,
                        ClaimDate = today,
                        XpAwarded = mission.XpReward
                    });
                    needSave = true;
                }
            }

            if (needSave)
            {
                await _userManager.UpdateAsync(user);
                await _db.SaveChangesAsync();
            }

            var vm = new DashboardViewModel
            {
                DisplayName = user.DisplayName,
                XP = user.XP,
                Level = user.Level,
                Streak = user.Streak,
                WordsLearned = wordsLearned,
                LessonsCompleted = completedLessons,
                TotalLessons = totalLessons,
                CurrentLesson = currentLesson,
                RecentProgresses = recentProgresses,
                DailyMissions = missions
            };

            return View("Dashboard", vm);
        }

        public IActionResult Dashboard()
        {
            return RedirectToAction("Index");
        }

        private async Task UpdateStreakAsync(ApplicationUser user)
        {
            var today = DateTime.Today;
            if (user.LastLoginDate?.Date == today) return;

            var yesterday = today.AddDays(-1);
            if (user.LastLoginDate?.Date == yesterday)
                user.Streak++;
            else if (user.LastLoginDate?.Date != today)
                user.Streak = 1;

            user.LastLoginDate = today;
            await _userManager.UpdateAsync(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddXP(int amount)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Json(new { success = false });

            user.XP += amount;
            user.Level = (user.XP / 500) + 1;
            await _userManager.UpdateAsync(user);

            return Json(new { success = true, xp = user.XP, level = user.Level });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
