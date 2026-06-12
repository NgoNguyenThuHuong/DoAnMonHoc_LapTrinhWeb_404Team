using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LingoToneMVC.Models
{
    public class QuizQuestion
    {
        public int Id { get; set; }

        public int LessonId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Question { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string OptionA { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string OptionB { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string OptionC { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string OptionD { get; set; } = string.Empty;

        [Required]
        public string CorrectAnswer { get; set; } = string.Empty;

        public int XpReward { get; set; } = 20;

        public string? Explanation { get; set; }

        public string QuestionType { get; set; } = "Vocabulary";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("LessonId")]
        public Lesson? Lesson { get; set; }
    }
}
