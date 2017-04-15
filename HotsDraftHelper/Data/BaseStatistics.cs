using System;
using System.Collections.Generic;

namespace HotsDraftHelper.Data
{
    internal sealed class BaseStatistics
    {
        public sealed class Builder
        {
            private readonly IReadOnlyDictionary<int, Hero> _heroes;
            private readonly MatchSummaryCollection _matches;
            private readonly WinRate[] _totals;
            private readonly int _minHeroId;

            public Builder(IReadOnlyDictionary<int, Hero> heroes, MatchSummaryCollection matches)
            {
                _heroes = heroes;
                _matches = matches;
                if (heroes.Count == 0)
                {
                    _totals = Array.Empty<WinRate>();
                    return;
                }
                int? minHeroId = null;
                int? maxHeroId = null;
                foreach (var hero in heroes.Values)
                {
                    if (minHeroId == null || hero.Id < minHeroId)
                        minHeroId = hero.Id;
                    if (maxHeroId == null || hero.Id > maxHeroId)
                        maxHeroId = hero.Id;
                }
                _minHeroId = minHeroId ?? 0;
                _totals = new WinRate[maxHeroId - minHeroId + 1 ?? 0];
            }

            public void ProcessMatchDetail(MatchDetail detail)
            {
                if (!_matches.Summaries.ContainsKey(detail.MatchId))
                    return;
                int heroIdx = detail.HeroId - _minHeroId;
                if (heroIdx < 0 || heroIdx >= _totals.Length)
                    return;
                _totals[heroIdx] = _totals[heroIdx].Increment(detail.Winner);
            }

            public BaseStatistics Build()
            {
                var winRates = new Dictionary<Hero, WinRate>(_heroes.Count);
                foreach (var hero in _heroes.Values)
                    winRates[hero] = _totals[hero.Id - _minHeroId];
                return new BaseStatistics(winRates);
            }
        }

        private readonly Dictionary<Hero, WinRate> _winRates;
        public IReadOnlyDictionary<Hero, WinRate> WinRates => _winRates;

        private BaseStatistics(Dictionary<Hero, WinRate> winRates)
        {
            _winRates = winRates;
        }

        //public static BaseStatistics Build()
        //{
        //    return null;
        //}
    }
}
