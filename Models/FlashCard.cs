using System.ComponentModel.DataAnnotations;

namespace LingoToneMVC.Models
{
    public class FlashCard
    {
        [Key]
        public int CardId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Hanzi { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Pinyin { get; set; } = string.Empty;

        [MaxLength(500)]
        public string MeaningVietnamese { get; set; } = string.Empty;

        [MaxLength(500)]
        public string MeaningEnglish { get; set; } = string.Empty;
    }
}
