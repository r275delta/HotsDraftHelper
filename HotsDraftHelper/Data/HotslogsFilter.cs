namespace HotsDraftHelper.Data
{
    internal sealed class HotslogsFilter
    {
        public double? LookbackDays { get; set; }
        public GameMode Mode { get; set; } = GameMode.QuickMatch;
    }
}
