using LingoToneMVC.Models;

namespace LingoToneMVC.ViewModels
{
    public class LessonDetailViewModel
    {
        public Lesson Lesson { get; set; } = null!;
        public bool IsCompleted { get; set; }
        public List<Vocabulary> Vocabularies { get; set; } = new();
        public List<GrammarPoint> GrammarPoints { get; set; } = new();
        public List<Dialogue> Dialogues { get; set; } = new();
    }
}
