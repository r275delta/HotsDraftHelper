using System;

namespace HeroesDrafter
{
    public sealed class BaseStatistics
    {
        public Hero Hero { get; }
        public double WinPercent { get; }
        public double PickPercent { get; }
        public double BanPercent { get; }

        public BaseStatistics(Hero hero, double winPercent, double pickPercent, double banPercent)
        {
            Hero = hero;
            WinPercent = winPercent;
            PickPercent = pickPercent;
            BanPercent = banPercent;
        }

        public static BaseStatistics ForHero(Hero hero, int gamesPlayed, int gamesBanned, double popularity, double winPercent)
        {
            return new BaseStatistics(
                hero,
                winPercent,
                popularity * gamesPlayed / (gamesPlayed + gamesBanned),
                popularity * gamesBanned / (gamesPlayed + gamesBanned));
        }

        public static BaseStatistics ForChoGall(
            int choGamesPlayed, int choGamesBanned, double choPopulariy, double choWinPercent,
            int gallGamesPlayed, int gallGamesBanned, double gallPopularity, double gallWinPercent)
        {
            return ForHero(
                Hero.ChoGall,
                Math.Max(choGamesPlayed, gallGamesPlayed),
                choGamesBanned + gallGamesBanned,
                (choPopulariy + gallPopularity) / 2,
                (choWinPercent + gallWinPercent) / 2);
        }
    }
}
