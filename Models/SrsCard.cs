using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LingoToneMVC.Models
{
    public class SrsCard
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Word { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Pinyin { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Meaning { get; set; } = string.Empty;

        public int SrsLevel { get; set; } = 0;

        public DateTime NextReviewAt { get; set; } = DateTime.Now;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; }
    }
}
