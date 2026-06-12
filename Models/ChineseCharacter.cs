using System.ComponentModel.DataAnnotations;

namespace LingoToneMVC.Models
{
    public class ChineseCharacter
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string Character { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Pinyin { get; set; } = string.Empty;

        [MaxLength(100)]
        public string HanViet { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Meaning { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Radical { get; set; } = string.Empty;

        public int StrokeCount { get; set; }

        [MaxLength(10)]
        public string HskLevel { get; set; } = "HSK1";
    }
}
