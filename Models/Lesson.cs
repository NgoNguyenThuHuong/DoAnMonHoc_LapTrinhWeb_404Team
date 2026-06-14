using System.ComponentModel.DataAnnotations;

namespace LingoToneMVC.Models
{
    public class Lesson
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public int OrderIndex { get; set; }

        [MaxLength(20)]
        public string HskLevel { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        // Navigation properties
        public virtual ICollection<Vocabulary> Vocabularies { get; set; } = new List<Vocabulary>();
        public virtual ICollection<GrammarPoint> GrammarPoints { get; set; } = new List<GrammarPoint>();
        public virtual ICollection<Dialogue> Dialogues { get; set; } = new List<Dialogue>();
        public virtual ICollection<UserProgress> UserProgresses { get; set; } = new List<UserProgress>();
    }
}
