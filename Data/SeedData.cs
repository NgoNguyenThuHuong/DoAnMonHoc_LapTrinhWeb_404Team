using LingoToneMVC.Models;
using Microsoft.AspNetCore.Identity;

namespace LingoToneMVC.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var db = serviceProvider.GetRequiredService<AppDbContext>();

            // Seed default SRS cards for new users - handled per-user in SrsController
            await Task.CompletedTask;
        }

        /// <summary>Creates default SRS cards for a new user</summary>
        public static async Task CreateDefaultSrsCardsAsync(AppDbContext db, string userId)
        {
            var existing = db.SrsCards.Any(s => s.UserId == userId);
            if (existing) return;

            var defaultCards = new List<SrsCard>
            {
                new SrsCard { UserId = userId, Word = "你好", Pinyin = "nǐ hǎo", Meaning = "Xin chào", SrsLevel = 0, NextReviewAt = DateTime.Now },
                new SrsCard { UserId = userId, Word = "谢谢", Pinyin = "xiè xie", Meaning = "Cảm ơn", SrsLevel = 0, NextReviewAt = DateTime.Now },
                new SrsCard { UserId = userId, Word = "再见", Pinyin = "zài jiàn", Meaning = "Tạm biệt", SrsLevel = 0, NextReviewAt = DateTime.Now },
                new SrsCard { UserId = userId, Word = "对不起", Pinyin = "duì bu qǐ", Meaning = "Xin lỗi", SrsLevel = 0, NextReviewAt = DateTime.Now }
            };

            db.SrsCards.AddRange(defaultCards);
            await db.SaveChangesAsync();
        }
    }
}
