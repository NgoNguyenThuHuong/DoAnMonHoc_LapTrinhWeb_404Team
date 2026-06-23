using LingoToneMVC.Models;
using System.Collections.Generic;

namespace LingoToneMVC.ViewModels
{
    public class QuizArenaViewModel
    {
        public List<LessonGroupViewModel> LessonGroups { get; set; } = new();
        public List<LeaderboardItemViewModel> WeeklyLeaderboard { get; set; } = new();
        public List<int> CompletedQuizLessonIds { get; set; } = new();
        public int TotalQuizzesTaken { get; set; }
        public int TotalXPEarned { get; set; }
    }
}
