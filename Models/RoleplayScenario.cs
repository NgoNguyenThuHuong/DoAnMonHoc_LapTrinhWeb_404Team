using System.ComponentModel.DataAnnotations;

namespace LingoToneMVC.Models
{
    public class RoleplayScenario
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string HskLevel { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Context { get; set; }
        
        public string? ExpectedDialogue { get; set; }
    }
}
