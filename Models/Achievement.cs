using System.ComponentModel.DataAnnotations;

namespace LingoToneMVC.Models
{
    public class Achievement
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        public int RequiredXp { get; set; }
        
        [MaxLength(500)]
        public string? IconUrl { get; set; }
    }
}
