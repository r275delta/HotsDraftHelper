using System;
using System.Collections.Generic;

namespace HotsDraftLib
{
    internal sealed class MapAdjustmentsBuilder
    {
        private readonly MatchSummaryCollection _matches;
        private readonly IReadOnlyCollection<Hero> _heroes;
        private readonly IReadOnlyCollection<Map> _maps;
        private readonly int _minHeroId;
        private readonly int _minMapId;
        private readonly WinRateBuilder[] _totalsByHero;
        private readonly WinRateBuilder[,] _totalsByHeroAndMap;

        public MapAdjustmentsBuilder(MatchSummaryCollection matches, IReadOnlyCollection<Hero> heroes, IReadOnlyCollection<Map> maps)
        {
            _matches = matches;
            _heroes = heroes;
            _maps = maps;

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
            _totalsByHero = new WinRateBuilder[maxHeroId - minHeroId + 1 ?? 0];

            int? minMapId = null;
            int? maxMapId = null;
            foreach (var map in maps)
            {
                if (minMapId == null || map.Id < minMapId)
                    minMapId = map.Id;
                if (maxMapId == null || map.Id > maxMapId)
                    maxMapId = map.Id;
            }
            _minMapId = minMapId ?? 0;
            _totalsByHeroAndMap = new WinRateBuilder[_totalsByHero.Length, maxMapId - minMapId + 1 ?? 0];
        }

        public void ProcessMatchDetail(MatchDetail detail)
        {
            if (!_matches.Summaries.TryGetValue(detail.MatchId, out MatchSummary summary))
                return;
            int heroIdx = detail.HeroId - _minHeroId;
            if (heroIdx < 0 || heroIdx >= _totalsByHero.Length)
                return;
            WinRateBuilder.Increment(ref _totalsByHero[heroIdx], detail.Winner);
            int mapIdx = summary.MapId - _minMapId;
            if (mapIdx < 0 || mapIdx >= _totalsByHeroAndMap.GetLength(1))
                return;
            WinRateBuilder.Increment(ref _totalsByHeroAndMap[heroIdx, mapIdx], detail.Winner);
        }

        public Dictionary<(Hero hero, Map map), Statistic> Build()
        {
            var adjustments = new Dictionary<ValueTuple<Hero, Map>, Statistic>();
            foreach (var hero in _heroes)
            {
                double baseWinRate = _totalsByHero[hero.Id - _minHeroId].WinRate.Value;
                foreach (var map in _maps)
                {
                    Statistic mapWinRate = _totalsByHeroAndMap[hero.Id - _minHeroId, map.Id - _minMapId].WinRate;
                    adjustments[(hero, map)] = new Statistic(Utils.CalcAdjustment(baseWinRate, mapWinRate.Value), mapWinRate.SampleSize);
                }
            }
            return adjustments;
        }
    }
}
