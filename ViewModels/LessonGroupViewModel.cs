namespace LingoToneMVC.ViewModels
{
    public class LessonGroupViewModel
    {
        public int Level { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<LingoToneMVC.Models.Lesson> Lessons { get; set; } = new List<LingoToneMVC.Models.Lesson>();
    }
}
