using System.ComponentModel.DataAnnotations;

namespace LingoToneMVC.Models
{
    public class HskWord
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Hanzi { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string Pinyin { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string MeaningEnglish { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string MeaningVietnamese { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string HskLevel { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? Radical { get; set; }
        
        public int? StrokeCount { get; set; }
        
        [MaxLength(500)]
        public string? ExampleChinese { get; set; }
        
        [MaxLength(500)]
        public string? ExamplePinyin { get; set; }
        
        [MaxLength(500)]
        public string? ExampleVietnamese { get; set; }
    }
}
