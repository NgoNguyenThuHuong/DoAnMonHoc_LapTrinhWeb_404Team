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
        private readonly LingoToneMVC.Services.IAiService _aiService;

        public GrammarController(AppDbContext db, UserManager<ApplicationUser> userManager, LingoToneMVC.Services.IAiService aiService)
        {
            _db = db;
            _userManager = userManager;
            _aiService = aiService;
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

        [HttpPost]
        public async Task<IActionResult> CheckGrammar([FromBody] GrammarCheckRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Sentence))
            {
                return Json(new
                {
                    success = false,
                    message = "Vui lòng nhập câu cần kiểm tra."
                });
            }

            try
            {
                var original = request.Sentence.Trim();
                
                var aiResult = await _aiService.CheckGrammarAsync(original);
                
                if (aiResult.Success && !aiResult.IsFallback)
                {
                    try
                    {
                        var doc = System.Text.Json.JsonDocument.Parse(aiResult.Content);
                        var root = doc.RootElement;
                        return Json(new
                        {
                            success = true,
                            isFallback = false,
                            original = original,
                            corrected = root.GetProperty("corrected").GetString(),
                            explanation = root.GetProperty("explanation").GetString()
                        });
                    }
                    catch
                    {
                        return Json(new
                        {
                            success = false,
                            isFallback = true,
                            message = "AI trả về dữ liệu không hợp lệ."
                        });
                    }
                }
                
                string correctedText = "Dữ liệu mô phỏng do AI đang bận.";
                string explanationText = aiResult.ErrorMessage ?? "Hệ thống AI đang quá tải. Vui lòng thử lại sau.";

                if (original.Replace(" ", "") == "我吃苹果在学校")
                {
                    correctedText = "我在学校吃苹果";
                    explanationText = "Trong tiếng Trung, trạng ngữ chỉ địa điểm (在 + nơi chốn) phải đứng TRƯỚC động từ. Do đó '在学校' phải đứng trước '吃苹果'. <br><em class='text-secondary' style='font-size: 12px;'>(Dữ liệu mô phỏng do API quá tải)</em>";
                }

                return Json(new
                {
                    success = true,
                    isFallback = true,
                    original = original,
                    corrected = correctedText,
                    explanation = explanationText
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Lỗi khi kiểm tra ngữ pháp: " + ex.Message
                });
            }
        }
    }

    public class GrammarCheckRequest
    {
        public string Sentence { get; set; } = string.Empty;
    }
}
