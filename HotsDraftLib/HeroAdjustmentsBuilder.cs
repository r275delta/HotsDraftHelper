using System.Collections.Generic;

namespace HotsDraftLib
{
    internal sealed class HeroAdjustmentsBuilder
    {
        private readonly MatchSummaryCollection _matches;
        private readonly IReadOnlyCollection<Hero> _heroes;
        private readonly int _minHeroId;
        private readonly WinRateBuilder[] _baseWinRates;
        private readonly WinRateBuilder[,] _allyWinRates;
        private readonly WinRateBuilder[,] _enemyWinRates;

        public HeroAdjustmentsBuilder(MatchSummaryCollection matches, IReadOnlyCollection<Hero> heroes)
        {
            _matches = matches;
            _heroes = heroes;

            int? minHeroId = null;
            int? maxHeroId = null;
            foreach (var hero in heroes)
            {
                if (minHeroId == null || hero.Id < minHeroId)
                    minHeroId = hero.Id;
                if (maxHeroId == null || hero.Id > maxHeroId)
                    maxHeroId = hero.Id;
            }
            _minHeroId = minHeroId ?? 0;
            _baseWinRates = new WinRateBuilder[maxHeroId - minHeroId + 1 ?? 0];
            _allyWinRates = new WinRateBuilder[_baseWinRates.Length, _baseWinRates.Length];
            _enemyWinRates = new WinRateBuilder[_baseWinRates.Length, _baseWinRates.Length];
        }

        public void ProcessMatchDetails(MatchDetail[] detail)
        {
            if (detail.Length != 10 || !_matches.Summaries.ContainsKey(detail[0].MatchId))
                return;

            for (int i = 0; i < 10; i++)
            {
                var detail1 = detail[i];
                int hero1Idx = detail1.HeroId - _minHeroId;
                if (hero1Idx < 0 || hero1Idx >= _baseWinRates.Length || detail1.MirrorMatch)
                    continue;
                WinRateBuilder.Increment(ref _baseWinRates[hero1Idx], detail[i].Winner);
                for (int j = 0; j < i; j++)
                {
                    var detail2 = detail[j];
                    int hero2Idx = detail2.HeroId - _minHeroId;
                    if (hero2Idx < 0 || hero2Idx >= _baseWinRates.Length || detail2.MirrorMatch)
                        continue;
                    bool ordered = hero1Idx < hero2Idx;
                    (int heroA, int heroB) key = ordered ? (hero1Idx, hero2Idx) : (hero2Idx, hero1Idx);
                    if (detail1.Winner == detail2.Winner)
                        WinRateBuilder.Increment(ref _allyWinRates[key.heroA, key.heroB], detail1.Winner);
                    else
                        WinRateBuilder.Increment(ref _enemyWinRates[key.heroA, key.heroB], ordered ? detail1.Winner : detail2.Winner);
                }
            }
        }

        public (Dictionary<(Hero hero1, Hero hero2), Statistic> synergies, Dictionary<(Hero hero1, Hero hero2), Statistic> counters) Build()
        {
            var synergies = new Dictionary<(Hero hero1, Hero hero2), Statistic>();
            var counters = new Dictionary<(Hero hero1, Hero hero2), Statistic>();
            foreach (var hero1 in _heroes)
            {
                int hero1Idx = hero1.Id - _minHeroId;
                Statistic hero1BaseWinRate = _baseWinRates[hero1Idx].WinRate;
                double hero1BaseAdj = Utils.CalcAdjustment(0.5, hero1BaseWinRate.Value);

                foreach (var hero2 in _heroes)
                {
                    if (hero2 == hero1)
                        break;

                    int hero2Idx = hero2.Id - _minHeroId;
                    Statistic hero2BaseWinRate = _baseWinRates[hero2Idx].WinRate;
                    double expectedAllyWinRate = Utils.ApplyAdjustment(hero2BaseWinRate.Value, hero1BaseAdj);
                    double expectedEnemyWinRate = Utils.ApplyAdjustment(hero2BaseWinRate.Value, -hero1BaseAdj);
                    bool ordered = hero1.Id < hero2.Id;
                    if (ordered)
                        expectedEnemyWinRate = 1 - expectedEnemyWinRate;
                    (Hero heroA, Hero heroB) heroKey = ordered ? (hero1, hero2) : (hero2, hero1);
                    Statistic actualAllyWinRate = _allyWinRates[heroKey.heroA.Id - _minHeroId, heroKey.heroB.Id - _minHeroId].WinRate;
                    synergies[heroKey] = new Statistic(Utils.CalcAdjustment(expectedAllyWinRate, actualAllyWinRate.Value), actualAllyWinRate.SampleSize);
                    var actualEnemyWinRate = _enemyWinRates[heroKey.heroA.Id - _minHeroId, heroKey.heroB.Id - _minHeroId].WinRate;
                    double actualEnemyWinrateValue = actualEnemyWinRate.Value;
                    if (ordered && actualEnemyWinRate.SampleSize != 0)
                        actualEnemyWinrateValue = 1 - actualEnemyWinrateValue;
                    counters[heroKey] = new Statistic(Utils.CalcAdjustment(expectedEnemyWinRate, actualEnemyWinrateValue), actualEnemyWinRate.SampleSize);
                }
            }
            return (synergies, counters);
        }
    }
}
