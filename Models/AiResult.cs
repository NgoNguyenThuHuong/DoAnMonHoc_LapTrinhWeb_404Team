namespace LingoToneMVC.Models
{
    public class AiResult
    {
        public bool Success { get; set; }
        public string Content { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public bool IsFallback { get; set; }
    }
}
