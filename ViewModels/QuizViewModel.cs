using LingoToneMVC.Models;

namespace LingoToneMVC.ViewModels
{
    public class QuizViewModel
    {
        public int LessonId { get; set; }
        public string LessonTitle { get; set; } = string.Empty;
        public string HskLevel { get; set; } = string.Empty;
        public List<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
        public List<LeaderboardItemViewModel> WeeklyLeaderboard { get; set; } = new();
    }

    public class LeaderboardItemViewModel
    {
        public int Rank { get; set; }
        public string UserName { get; set; } = "";
        public int XP { get; set; }
        public bool IsCurrentUser { get; set; }
    }
}
