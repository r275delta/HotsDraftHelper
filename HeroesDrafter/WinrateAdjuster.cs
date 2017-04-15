namespace HeroesDrafter
{
    public interface IWinrateAdjuster
    {
        double GetAdjustment(double winPercentBefore, double winPercentAfter);
        double ApplyAdjustment(double baseWinPercent, double adjustment);
    }

    public sealed class AdditiveAdjuster : IWinrateAdjuster
    {
        public double ApplyAdjustment(double baseWinPercent, double adjustment)
        {
            return baseWinPercent + adjustment;
        }

        public double GetAdjustment(double winPercentBefore, double winPercentAfter)
        {
            return winPercentAfter - winPercentBefore;
        }
    }

    public abstract class Context
    {
        public sealed class Impl : Context
        {
            public override IWinrateAdjuster WinrateAdjuster { get; } = new AdditiveAdjuster();
        }

        public static Context Instance { get; } = new Impl();

        public abstract IWinrateAdjuster WinrateAdjuster { get; }

        private Context()
        {
        }
    }
}
