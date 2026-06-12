using Microsoft.AspNetCore.Identity;

namespace LingoToneMVC.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; } = "Học Viên";
        public int XP { get; set; } = 0;
        public int Level { get; set; } = 1;
        public int Streak { get; set; } = 0;
        public DateTime? LastLoginDate { get; set; }

        // Navigation properties
        public virtual ICollection<UserProgress> UserProgresses { get; set; } = new List<UserProgress>();
        public virtual ICollection<UserLearnedWord> LearnedWords { get; set; } = new List<UserLearnedWord>();
        public virtual ICollection<SrsCard> SrsCards { get; set; } = new List<SrsCard>();
    }
}
