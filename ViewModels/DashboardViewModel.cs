using LingoToneMVC.Models;

namespace LingoToneMVC.ViewModels
{
    public class DashboardViewModel
    {
        public string DisplayName { get; set; } = "Học Viên";
        public int XP { get; set; }
        public int Level { get; set; }
        public int Streak { get; set; }
        public int WordsLearned { get; set; }
        public int LessonsCompleted { get; set; }
        public int TotalLessons { get; set; }
        public double CourseProgressPercent => TotalLessons > 0 ? (double)LessonsCompleted / TotalLessons * 100 : 0;
        public Lesson? CurrentLesson { get; set; }
        public List<UserProgress> RecentProgresses { get; set; } = new();
        public List<DailyMissionViewModel> DailyMissions { get; set; } = new();
    }
}
