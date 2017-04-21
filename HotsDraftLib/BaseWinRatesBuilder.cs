using System;
using System.Collections.Generic;

namespace HotsDraftLib
{
    internal sealed class BaseWinRatesBuilder
    {
        private readonly IReadOnlyDictionary<int, Hero> _heroes;
        private readonly MatchSummaryCollection _matches;
        private readonly WinRateBuilder[] _totals;
        private readonly int _minHeroId;

        public BaseWinRatesBuilder(IReadOnlyDictionary<int, Hero> heroes, MatchSummaryCollection matches)
        {
            _heroes = heroes;
            _matches = matches;
            if (heroes.Count == 0)
            {
                _totals = Array.Empty<WinRateBuilder>();
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
            _totals = new WinRateBuilder[maxHeroId - minHeroId + 1 ?? 0];
        }

        public void ProcessMatchDetail(MatchDetail detail)
        {
            if (!_matches.Summaries.ContainsKey(detail.MatchId))
                return;
            int heroIdx = detail.HeroId - _minHeroId;
            if (heroIdx < 0 || heroIdx >= _totals.Length)
                return;
            WinRateBuilder.Increment(ref _totals[heroIdx], detail.Winner);
        }

        public IReadOnlyDictionary<Hero, Statistic> GetWinRates()
        {
            var winRates = new Dictionary<Hero, Statistic>(_heroes.Count);
            foreach (var hero in _heroes.Values)
                winRates[hero] = _totals[hero.Id - _minHeroId].WinRate;
            return winRates;
        }
    }
}
