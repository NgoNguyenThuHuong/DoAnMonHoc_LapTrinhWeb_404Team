using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LingoToneMVC.Models
{
    public class Vocabulary
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Chinese { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Pinyin { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Vietnamese { get; set; } = string.Empty;

        public int LessonId { get; set; }

        [ForeignKey(nameof(LessonId))]
        public virtual Lesson? Lesson { get; set; }
    }
}
