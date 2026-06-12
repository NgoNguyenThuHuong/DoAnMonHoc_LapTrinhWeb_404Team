using System;
using System.ComponentModel.DataAnnotations;

namespace LingoToneMVC.Models
{
    public class QuizResult
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int LessonId { get; set; }
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
        public int MaxCombo { get; set; }
        public int XPGained { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ApplicationUser? User { get; set; }
    }
}
