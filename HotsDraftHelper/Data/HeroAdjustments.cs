using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotsDraftHelper.Data
{
    internal sealed class HeroAdjustments
    {
        public sealed class Builder
        {
            private readonly MatchSummaryCollection _matches;
            private readonly IReadOnlyCollection<Hero> _heroes;
            private readonly int _minHeroId;
            private readonly WinRate[] _baseWinRates;
            private readonly WinRate[,] _allyWinRates;
            private readonly WinRate[,] _enemyWinRates;

            public Builder(MatchSummaryCollection matches, IReadOnlyCollection<Hero> heroes)
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
                _baseWinRates = new WinRate[maxHeroId - minHeroId + 1 ?? 0];
                _allyWinRates = new WinRate[_baseWinRates.Length, _baseWinRates.Length];
                _enemyWinRates = new WinRate[_baseWinRates.Length, _baseWinRates.Length];
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
                    _baseWinRates[hero1Idx] = _baseWinRates[hero1Idx].Increment(detail[i].Winner);
                    for (int j = 0; j < i; j++)
                    {
                        var detail2 = detail[j];
                        int hero2Idx = detail2.HeroId - _minHeroId;
                        if (hero2Idx < 0 || hero2Idx >= _baseWinRates.Length || detail2.MirrorMatch)
                            continue;
                        bool ordered = hero1Idx < hero2Idx;
                        (int heroA, int heroB) key = ordered ? (hero1Idx, hero2Idx) : (hero2Idx, hero1Idx);
                        if (detail1.Winner == detail2.Winner)
                            _allyWinRates[key.heroA, key.heroB] = _allyWinRates[key.heroA, key.heroB].Increment(detail1.Winner);
                        else
                            _enemyWinRates[key.heroA, key.heroB] = _enemyWinRates[key.heroA, key.heroB].Increment(ordered ? detail1.Winner : detail2.Winner);
                    }
                }
            }

            public HeroAdjustments Build()
            {
                var synergies = new Dictionary<(Hero hero1, Hero hero2), double>();
                var counters = new Dictionary<(Hero hero1, Hero hero2), double>();
                foreach (var hero1 in _heroes)
                {
                    int hero1Idx = hero1.Id - _minHeroId;
                    double hero1BaseWinRate = _baseWinRates[hero1Idx].Percentage;
                    double hero1BaseAdj = Utils.CalcAdjustment(0.5, hero1BaseWinRate);

                    foreach (var hero2 in _heroes)
                    {
                        if (hero2 == hero1)
                            break;

                        int hero2Idx = hero2.Id - _minHeroId;
                        double hero2BaseWinRate = _baseWinRates[hero2Idx].Percentage;
                        double expectedAllyWinRate = Utils.ApplyAdjustment(hero2BaseWinRate, hero1BaseAdj);
                        double expectedEnemyWinRate = Utils.ApplyAdjustment(hero2BaseWinRate, -hero1BaseAdj);
                        bool ordered = hero1.Id < hero2.Id;
                        if (ordered)
                            expectedEnemyWinRate = 1 - expectedEnemyWinRate;
                        (Hero heroA, Hero heroB) heroKey = ordered ? (hero1, hero2) : (hero2, hero1);
                        double actualAllyWinRate = _allyWinRates[heroKey.heroA.Id - _minHeroId, heroKey.heroB.Id - _minHeroId].Percentage;
                        synergies[heroKey] = Utils.CalcAdjustment(expectedAllyWinRate, actualAllyWinRate);
                        var actualEnemyWinRate = _enemyWinRates[heroKey.heroA.Id - _minHeroId, heroKey.heroB.Id - _minHeroId].Percentage;
                        if (ordered && actualEnemyWinRate != 0)
                            actualEnemyWinRate = 1 - actualEnemyWinRate;
                        counters[heroKey] = Utils.CalcAdjustment(expectedEnemyWinRate, actualEnemyWinRate);
                    }
                }
                return new HeroAdjustments(synergies, counters);
            }
        }

        private readonly Dictionary<(Hero hero, Hero ally), double> _synergyAdjustments;
        public IReadOnlyDictionary<(Hero hero, Hero ally), double> Synergies => _synergyAdjustments;
        private readonly Dictionary<(Hero hero, Hero enemy), double> _counterAdjustments;
        public IReadOnlyDictionary<(Hero hero, Hero enemy), double> Counters => _counterAdjustments;

        private HeroAdjustments(
            Dictionary<(Hero hero1, Hero hero2), double> synergyAdjustments,
            Dictionary<(Hero hero1, Hero hero2), double> counterAdjustments)
        {
            _synergyAdjustments = synergyAdjustments;
            _counterAdjustments = counterAdjustments;
        }

        public double GetSynergyAdjustment(Hero hero, Hero ally)
        {
            return _synergyAdjustments[hero.Id < ally.Id ? (hero, ally) : (ally, hero)];
        }

        public double GetCounterAdjustment(Hero hero, Hero enemy)
        {
            if (hero.Id < enemy.Id)
                return _counterAdjustments[(hero, enemy)];
            else
                return -_counterAdjustments[(enemy, hero)];
        }
    }
}
