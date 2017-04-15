using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace HotsDraftHelper.Data
{
    internal sealed class HeroesStatistics
    {
        public IReadOnlyDictionary<int, Hero> Heroes { get; }
        public IReadOnlyDictionary<int, Map> Maps { get; }
        public BaseStatistics BaseStatistics { get; }
        public MapAdjustments MapAdjustments { get; }
        public HeroAdjustments HeroAdjustments { get; }

        private HeroesStatistics(
            IReadOnlyDictionary<int, Hero> heroes,
            IReadOnlyDictionary<int, Map> maps,
            BaseStatistics baseStatistics,
            MapAdjustments mapAdjustments,
            HeroAdjustments heroAdjustments)
        {
            Heroes = heroes;
            Maps = maps;
            BaseStatistics = baseStatistics;
            MapAdjustments = mapAdjustments;
            HeroAdjustments = heroAdjustments;
        }

        public static HeroesStatistics FromHotslogsExport(
            string exportDataPath,
            HotslogsFilter baseFilter,
            HotslogsFilter adjFilter)
        {
            exportDataPath = Environment.ExpandEnvironmentVariables(exportDataPath);
            var (heroes, maps) = LoadHeroesAndMaps(Path.Combine(exportDataPath, "HeroIDAndMapID.csv"));
            var (baseMatches, adjMatches) = LoadMatchSummaries(
                Path.Combine(exportDataPath, "Replays.csv"),
                baseFilter,
                adjFilter);

            var baseStatisticsBuilder = new BaseStatistics.Builder(heroes, baseMatches);
            var mapStatisticsBuilder = new MapAdjustments.Builder(adjMatches, heroes.Values, maps.Values);
            var heroAdjustmentsBuilder = new HeroAdjustments.Builder(adjMatches, heroes.Values);
            var matchDetails = new MatchDetail[10];
            long currentMatchId = -1;
            int matchDetailIdx = 0;
            ReadCsv(
                Path.Combine(exportDataPath, "ReplayCharacters.csv"),
                new(string, Regex)[]
                {
                    ("ReplayID", null),
                    ("HeroID", null),
                    ("Hero Level", null),
                    ("Is Winner", null),
                    ("MMR Before", null),
                },
                vals =>
                {
                    if (!Utils.FastParse(vals[0], out long matchId) ||
                        !Utils.FastParse(vals[1], out int heroId) ||
                        !Utils.FastParse(vals[2], out int heroLevel) ||
                        !Utils.FastParse(vals[3], out int winner) ||
                        !Utils.FastParse(vals[4], out int mmr))
                        return;
                    var detail = new MatchDetail(matchId, heroId, heroLevel, winner != 0, mmr);
                    if (matchId != currentMatchId)
                    {
                        currentMatchId = matchId;
                        matchDetailIdx = 0;
                    }
                    if (matchDetailIdx >= matchDetails.Length)
                        return;
                    matchDetails[matchDetailIdx++] = detail;

                    if (matchDetailIdx == 10)
                    {
                        for (int i = 1; i < 10; i++)
                        {
                            for (int j = 0; j < i; j++)
                            {
                                if (matchDetails[i].HeroId == matchDetails[j].HeroId)
                                {
                                    matchDetails[i].MirrorMatch = true;
                                    matchDetails[j].MirrorMatch = true;
                                }
                            }
                        }
                        for (int i = 0; i < 10; i++)
                        {
                            if (!matchDetails[i].MirrorMatch)
                            {
                                baseStatisticsBuilder.ProcessMatchDetail(matchDetails[i]);
                                mapStatisticsBuilder.ProcessMatchDetail(matchDetails[i]);
                            }
                            heroAdjustmentsBuilder.ProcessMatchDetails(matchDetails);
                        }
                    }
                });

            var baseStatistics = baseStatisticsBuilder.Build();
            var mapAdjustments = mapStatisticsBuilder.Build();
            var heroAdjustments = heroAdjustmentsBuilder.Build();

            return new HeroesStatistics(
                heroes,
                maps,
                baseStatistics,
                mapAdjustments,
                heroAdjustments);
        }

        private static (MatchSummaryCollection baseMatches, MatchSummaryCollection adjMatches) LoadMatchSummaries(
            string path, 
            HotslogsFilter baseFilter, 
            HotslogsFilter adjFilter)
        {
            var baseMatchBuilder = new MatchSummaryCollection.Builder(baseFilter);
            var adjMatchBuilder = new MatchSummaryCollection.Builder(adjFilter);
            ReadCsv(
                path,
                new[]
                {
                    ("ReplayID", null),
                    ("GameMode", new Regex("GameMode.*")),
                    ("MapID", null),
                    ("Timestamp", new Regex("Timestamp.*"))
                },
                cellValues =>
                {
                    if (!Utils.FastParse(cellValues[0], out long id) ||
                        !Utils.FastParse(cellValues[1], out int gameModeInt) ||
                        !Utils.FastParse(cellValues[2], out int mapId) ||
                        !DateTime.TryParse(cellValues[3], out DateTime timestamp))
                        return;
                    timestamp = DateTime.SpecifyKind(timestamp, DateTimeKind.Utc);
                    var gameMode = (GameMode)gameModeInt;
                    var summary = new MatchSummary(id, gameMode, mapId, timestamp);
                    baseMatchBuilder.TryAddSummary(summary);
                    adjMatchBuilder.TryAddSummary(summary);
                });
            return (baseMatchBuilder.Build(), adjMatchBuilder.Build());
        }

        private static (Dictionary<int, Hero> heroes, Dictionary<int, Map> maps) LoadHeroesAndMaps(string path)
        {
            var heroes = new Dictionary<int, Hero>();
            var maps = new Dictionary<int, Map>();
            ReadCsv(
                path,
                new(string, Regex)[] { ("ID", null), ("Name", null), ("Group", null), ("SubGroup", null) },
                cellValues =>
                {
                    if (!Int32.TryParse(cellValues[0], out int id))
                        return;
                    if (id >= 1 && id < 1000)
                        heroes.Add(id, new Hero(id, cellValues[1], cellValues[2], cellValues[3]));
                    else if (id >= 1000 && id < 2000)
                        maps.Add(id, new Map(id, cellValues[1]));
                });
            return (heroes, maps);
        }

        private static void ReadCsv(string path, IReadOnlyList<(string name, Regex matcher)> cols, Action<IReadOnlyList<string>> readFunction)
        {
            if (!File.Exists(path))
                throw new Exception($"File not found at '{path}'");
            using (var sr = new StreamReader(path))
            {
                string line = sr.ReadLine();
                if (line == null)
                    throw new Exception("Header row missing");
                var colIndexes = new int[cols.Count];
                var headers = line.Split(',');
                for (int i = 0; i < cols.Count; i++)
                {
                    var col = cols[i];
                    int idx;
                    if (col.matcher != null)
                        idx = headers.IndexOf(x => col.matcher.IsMatch(x));
                    else
                        idx = headers.IndexOf(col.name, StringComparer.InvariantCultureIgnoreCase);
                    if (idx < 0)
                        throw new Exception($"{col} column missing");
                    colIndexes[i] = idx;
                }

                var cellValues = new string[cols.Count];
                while ((line = sr.ReadLine()) != null)
                {
                    var cells = line.Split(',');
                    for (int i = 0; i < colIndexes.Length; i++)
                    {
                        int colIdx = colIndexes[i];
                        if (colIdx < cells.Length)
                            cellValues[i] = cells[colIdx];
                        else
                            cellValues[i] = "";
                    }
                    readFunction(cellValues);
                }
            }
        }
    }
}
