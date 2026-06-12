using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LingoToneMVC.Models
{
    public class GrammarRule
    {
        [Key]
        public int GrammarId { get; set; }
        
        public int LessonId { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Pattern { get; set; } = string.Empty;
        
        public string Explanation { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? ExampleChinese { get; set; }
        
        [MaxLength(500)]
        public string? ExamplePinyin { get; set; }
        
        [MaxLength(500)]
        public string? ExampleVietnamese { get; set; }
        
        [ForeignKey("LessonId")]
        public virtual Lesson? Lesson { get; set; }
    }
}
