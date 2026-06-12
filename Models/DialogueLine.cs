using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LingoToneMVC.Models
{
    public class DialogueLine
    {
        public int Id { get; set; }

        public int DialogueId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Speaker { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Hanzi { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Pinyin { get; set; } = string.Empty;

        [MaxLength(500)]
        public string MeaningVi { get; set; } = string.Empty;

        [ForeignKey(nameof(DialogueId))]
        public virtual Dialogue? Dialogue { get; set; }
    }
}
