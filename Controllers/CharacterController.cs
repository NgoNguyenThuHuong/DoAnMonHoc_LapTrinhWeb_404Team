using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LingoToneMVC.Data;
using LingoToneMVC.Models;
using System.Text.Json;
using System.IO;

namespace LingoToneMVC.Controllers
{
    public class HanziDictDto
    {
        public int id { get; set; }
        public int level { get; set; }
        public string hanzi { get; set; }
        public string pinyin { get; set; }
        public string radicals { get; set; }
        public string strokes { get; set; }
        public HskTranslationDto translations { get; set; }
    }
    public class CharacterController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public CharacterController(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? search = null, string? hsk = null, int page = 1)
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

            int pageSize = 24;
            var query = _db.HskWords.Where(w => w.Hanzi.Length == 1).AsQueryable();

            if (!string.IsNullOrEmpty(hsk) && hsk != "all")
            {
                query = query.Where(w => w.HskLevel == hsk);
            }

            if (!string.IsNullOrEmpty(search))
            {
                string searchLower = search.ToLower();
                query = query.Where(w =>
                    w.Hanzi.Contains(searchLower) ||
                    w.Pinyin.ToLower().Contains(searchLower) ||
                    w.MeaningVietnamese.ToLower().Contains(searchLower) ||
                    w.MeaningEnglish.ToLower().Contains(searchLower)
                );
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var characters = await query
                .OrderBy(w => w.HskLevel)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.SelectedHsk = hsk ?? "all";
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(characters);
        }

        public async Task<IActionResult> Detail(string hanzi)
        {
            var charInfo = await _db.HskWords.FirstOrDefaultAsync(c => c.Hanzi == hanzi);
            if (charInfo == null) return NotFound();

            // Find compound words containing this character
            var compounds = await _db.HskWords
                                   .Where(c => c.Hanzi.Length > 1 && c.Hanzi.Contains(hanzi))
                                   .Take(5)
                                   .Select(c => new
                                   {
                                       word = c.Hanzi,
                                       pinyin = c.Pinyin,
                                       meaning = !string.IsNullOrEmpty(c.MeaningVietnamese) ? c.MeaningVietnamese : c.MeaningEnglish
                                   })
                                   .ToListAsync();

            string primaryMeaning = !string.IsNullOrEmpty(charInfo.MeaningVietnamese) ? charInfo.MeaningVietnamese : charInfo.MeaningEnglish;

            return Json(new
            {
                hanzi = charInfo.Hanzi,
                pinyin = charInfo.Pinyin,
                meaning = primaryMeaning,
                radical = string.IsNullOrEmpty(charInfo.Radical) ? "Đang cập nhật" : charInfo.Radical,
                strokes = charInfo.StrokeCount == null ? "Đang cập nhật" : charInfo.StrokeCount.ToString(),
                compounds = compounds
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
