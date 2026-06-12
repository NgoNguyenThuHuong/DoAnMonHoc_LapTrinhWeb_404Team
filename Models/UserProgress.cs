using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LingoToneMVC.Models
{
    public class UserProgress
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public int LessonId { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedAt { get; set; }

        public int XpEarned { get; set; } = 0;
        
        public int CompletionPercent { get; set; } = 0;
        
        public int XP { get; set; } = 0;
        
        public int Streak { get; set; } = 0;
        
        public DateTime? LastStudyDate { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey(nameof(LessonId))]
        public virtual Lesson? Lesson { get; set; }
    }
}
