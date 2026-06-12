using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LingoToneMVC.Models
{
    public class GrammarPoint
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Pattern { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Explanation { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Example { get; set; } = string.Empty;

        [MaxLength(500)]
        public string ExamplePinyin { get; set; } = string.Empty;

        [MaxLength(500)]
        public string ExampleMeaning { get; set; } = string.Empty;

        public int LessonId { get; set; }

        [ForeignKey(nameof(LessonId))]
        public virtual Lesson? Lesson { get; set; }
    }
}
