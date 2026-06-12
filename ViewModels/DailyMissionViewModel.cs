namespace LingoToneMVC.ViewModels
{
    public class DailyMissionViewModel
    {
        public string Key { get; set; } = "";
        public string Title { get; set; } = "";
        public int CurrentValue { get; set; }
        public int TargetValue { get; set; }
        public int XpReward { get; set; }
        public bool IsCompleted => CurrentValue >= TargetValue;
    }
}
