using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LingoToneMVC.Data;
using LingoToneMVC.Models;
using LingoToneMVC.ViewModels;

namespace LingoToneMVC.Controllers
{
    [Authorize]
    public class SrsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public SrsController(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Ensure user has SRS cards
            await SeedData.CreateDefaultSrsCardsAsync(_db, user.Id);

            SetUserViewBag(user);

            var dueCards = await _db.SrsCards
                .Where(s => s.UserId == user.Id && s.NextReviewAt <= DateTime.Now)
                .OrderBy(s => s.NextReviewAt)
                .ToListAsync();

            var vm = new SrsViewModel
            {
                DueCards = dueCards,
                TotalDue = dueCards.Count,
                CurrentCard = dueCards.FirstOrDefault()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(int cardId, int rating)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Json(new { success = false });

            var card = await _db.SrsCards.FindAsync(cardId);
            if (card == null || card.UserId != user.Id)
                return Json(new { success = false });

            int xpEarned;
            if (rating == 2) // Dễ
            {
                card.SrsLevel = Math.Min(card.SrsLevel + 2, 5);
                card.NextReviewAt = DateTime.Now.AddDays(Math.Max(Math.Pow(2, card.SrsLevel), 4));
                xpEarned = 15;
            }
            else if (rating == 1) // Khó
            {
                card.SrsLevel = Math.Min(card.SrsLevel + 1, 5);
                card.NextReviewAt = DateTime.Now.AddDays(Math.Max(Math.Pow(2, card.SrsLevel - 1), 1));
                xpEarned = 10;
            }
            else // Quên
            {
                card.SrsLevel = Math.Max(card.SrsLevel - 1, 0);
                card.NextReviewAt = DateTime.Now.AddDays(1);
                xpEarned = 5;
            }

            user.XP += xpEarned;
            user.Level = (user.XP / 500) + 1;

            await _userManager.UpdateAsync(user);
            await _db.SaveChangesAsync();

            // Get next due card
            var nextCard = await _db.SrsCards
                .Where(s => s.UserId == user.Id && s.NextReviewAt <= DateTime.Now && s.Id != cardId)
                .OrderBy(s => s.NextReviewAt)
                .FirstOrDefaultAsync();

            var remainingCount = await _db.SrsCards
                .CountAsync(s => s.UserId == user.Id && s.NextReviewAt <= DateTime.Now);

            return Json(new
            {
                success = true,
                xpEarned,
                xp = user.XP,
                level = user.Level,
                remaining = remainingCount,
                nextCardId = nextCard?.Id,
                nextWord = nextCard?.Word,
                nextPinyin = nextCard?.Pinyin,
                nextMeaning = nextCard?.Meaning,
                nextSrsLevel = nextCard?.SrsLevel
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
