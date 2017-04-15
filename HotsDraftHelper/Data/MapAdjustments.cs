using System;
using System.Collections.Generic;

namespace HotsDraftHelper.Data
{
    internal sealed class MapAdjustments
    {
        public sealed class Builder
        {
            private readonly MatchSummaryCollection _matches;
            private readonly IReadOnlyCollection<Hero> _heroes;
            private readonly IReadOnlyCollection<Map> _maps;
            private readonly int _minHeroId;
            private readonly int _minMapId;
            private readonly WinRate[] _totalsByHero;
            private readonly WinRate[,] _totalsByHeroAndMap;

            public Builder(MatchSummaryCollection matches, IReadOnlyCollection<Hero> heroes, IReadOnlyCollection<Map> maps)
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
                _totalsByHero = new WinRate[maxHeroId - minHeroId + 1 ?? 0];

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
                _totalsByHeroAndMap = new WinRate[_totalsByHero.Length, maxMapId - minMapId + 1 ?? 0];
            }

            public void ProcessMatchDetail(MatchDetail detail)
            {
                if (!_matches.Summaries.TryGetValue(detail.MatchId, out MatchSummary summary))
                    return;
                int heroIdx = detail.HeroId - _minHeroId;
                if (heroIdx < 0 || heroIdx >= _totalsByHero.Length)
                    return;
                _totalsByHero[heroIdx] = _totalsByHero[heroIdx].Increment(detail.Winner);
                int mapIdx = summary.MapId - _minMapId;
                if (mapIdx < 0 || mapIdx >= _totalsByHeroAndMap.GetLength(1))
                    return;
                _totalsByHeroAndMap[heroIdx, mapIdx] = _totalsByHeroAndMap[heroIdx, mapIdx].Increment(detail.Winner);
            }

            public MapAdjustments Build()
            {
                var adjustments = new Dictionary<ValueTuple<Hero, Map>, double>();
                foreach (var hero in _heroes)
                {
                    double baseWinRate = _totalsByHero[hero.Id - _minHeroId].Percentage;
                    foreach (var map in _maps)
                    {
                        double mapWinRate = _totalsByHeroAndMap[hero.Id - _minHeroId, map.Id - _minMapId].Percentage;
                        adjustments[(hero, map)] = Utils.CalcAdjustment(baseWinRate, mapWinRate);
                    }
                }
                return new MapAdjustments(adjustments);
            }
        }

        private readonly Dictionary<(Hero hero, Map map), double> _adjustments;
        public IReadOnlyDictionary<(Hero hero, Map map), double> Adjustments => _adjustments;

        private MapAdjustments(Dictionary<(Hero hero, Map map), double> adjustments)
        {
            _adjustments = adjustments;
        }
    }
}
