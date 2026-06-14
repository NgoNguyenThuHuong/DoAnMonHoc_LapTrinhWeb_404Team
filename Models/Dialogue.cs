using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LingoToneMVC.Models
{
    public class Dialogue
    {
        public int Id { get; set; }

        public int LessonId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public virtual List<DialogueLine> Lines { get; set; } = new List<DialogueLine>();

        [ForeignKey(nameof(LessonId))]
        public virtual Lesson? Lesson { get; set; }
    }
}
