using HotsDraftLib.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HotsDraftLib
{
    public sealed class StatisticsCollection
    {
        public IReadOnlyDictionary<int, Hero> Heroes { get; }
        public IReadOnlyDictionary<int, Map> Maps { get; }
        public IReadOnlyDictionary<Hero, Statistic> BaseWinRates { get; }
        public IReadOnlyDictionary<(Hero hero, Map map), Statistic> MapAdjustments { get; }
        public IReadOnlyDictionary<(Hero hero, Hero ally), Statistic> Synergies { get; }
        public IReadOnlyDictionary<(Hero hero, Hero enemy), Statistic> Counters { get; }

        private StatisticsCollection(
            IReadOnlyDictionary<int, Hero> heroes, 
            IReadOnlyDictionary<int, Map> maps,
            IReadOnlyDictionary<Hero, Statistic> baseWinRates,
            IReadOnlyDictionary<(Hero hero, Map map), Statistic> mapAdjustments,
            IReadOnlyDictionary<(Hero hero, Hero ally), Statistic> synergies,
            IReadOnlyDictionary<(Hero hero, Hero enemy), Statistic> counters)
        {
            Heroes = heroes;
            Maps = maps;
            BaseWinRates = baseWinRates;
            MapAdjustments = mapAdjustments;
            Synergies = synergies;
            Counters = counters;
        }

        public Statistic GetHeroAdjustment(Hero hero)
        {
            var winRate = BaseWinRates[hero];
            return new Statistic(Utils.CalcAdjustment(0.5, winRate.Value), winRate.SampleSize);
        }

        public Statistic GetSynergyAdjustment(Hero hero, Hero ally)
        {
            return Synergies[hero.Id < ally.Id ? (hero, ally) : (ally, hero)];
        }

        public Statistic GetCounterAdjustment(Hero hero, Hero enemy)
        {
            if (hero.Id < enemy.Id)
                return Counters[(hero, enemy)];
            else
            {
                var inverse = Counters[(enemy, hero)];
                return new Statistic(-inverse.Value, inverse.SampleSize);
            }
        }

        public void SerializeToFile(string filePath)
        {
            filePath = Environment.ExpandEnvironmentVariables(filePath);

            var serialized = new SerializableStatisticsCollection();
            serialized.Heroes.AddRange(
                Heroes.Values.Select(
                    h => new SerializableHero
                    {
                        Id = h.Id, Name = h.Name, Group = h.Group, SubGroup = h.SubGroup
                    }));
            serialized.Maps.AddRange(
                Maps.Values.Select(
                    m => new SerializableMap
                    {
                        Id = m.Id,
                        Name = m.Name
                    }));
            serialized.WinRates.AddRange(
                BaseWinRates.Select(
                    kv => new SerializableWinRate
                    {
                        HeroId = kv.Key.Id,
                        HeroName = kv.Key.Name,
                        WinRate = kv.Value.Value,
                        SampleSize = kv.Value.SampleSize
                    }));
            serialized.MapAdjustments.AddRange(
                MapAdjustments.Select(
                    kv => new SerializableMapAdjustment
                    {
                        HeroId = kv.Key.hero.Id,
                        HeroName = kv.Key.hero.Name,
                        MapId = kv.Key.map.Id,
                        MapName = kv.Key.map.Name,
                        AdjFactor = kv.Value.Value,
                        SampleSize = kv.Value.SampleSize
                    }));
            var heroAdjustments = new Dictionary<(Hero, Hero), SerializableHeroAdjustments>();
            foreach (var synergyKv in Synergies)
            {
                SerializableHeroAdjustments adj = null;
                if (!heroAdjustments.TryGetValue(synergyKv.Key, out adj))
                {
                    heroAdjustments[synergyKv.Key] = adj = new SerializableHeroAdjustments
                    {
                        HeroId = synergyKv.Key.hero.Id,
                        HeroName = synergyKv.Key.hero.Name,
                        OtherHeroId = synergyKv.Key.ally.Id,
                        OtherHeroName = synergyKv.Key.ally.Name
                    };
                    serialized.HeroAdjustments.Add(adj);
                }
                adj.SynergyAdjFactor = synergyKv.Value.Value;
                adj.SynergySampleSize = synergyKv.Value.SampleSize;
            }
            foreach (var counterKv in Counters)
            {
                SerializableHeroAdjustments adj = null;
                if (!heroAdjustments.TryGetValue(counterKv.Key, out adj))
                {
                    heroAdjustments[counterKv.Key] = adj = new SerializableHeroAdjustments
                    {
                        HeroId = counterKv.Key.hero.Id,
                        HeroName = counterKv.Key.hero.Name,
                        OtherHeroId = counterKv.Key.enemy.Id,
                        OtherHeroName = counterKv.Key.enemy.Name
                    };
                    serialized.HeroAdjustments.Add(adj);
                }
                adj.CounterAdjFactor = counterKv.Value.Value;
                adj.CounterSampleSize = counterKv.Value.SampleSize;
            }

            using (var fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fileStream))
                {
                    JsonSerializer serializer = new JsonSerializer { Formatting = Formatting.Indented };
                    serializer.Serialize(sw, serialized);
                }
            }
        }

        public static StatisticsCollection DeserializeFromFile(string filePath)
        {
            SerializableStatisticsCollection serialized;
            filePath = Environment.ExpandEnvironmentVariables(filePath);
            using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fileStream))
                {
                    using (var jr = new JsonTextReader(sr))
                    {
                        serialized = new JsonSerializer().Deserialize<SerializableStatisticsCollection>(jr);
                    }
                }
            }

            var heroes = serialized.Heroes.ToDictionary(h => h.Id, h => new Hero(h.Id, h.Name, h.Group, h.SubGroup));
            var maps = serialized.Maps.ToDictionary(m => m.Id, m => new Map(m.Id, m.Name));
            var baseWinRates = serialized.WinRates.ToDictionary(wr => heroes[wr.HeroId], wr => new Statistic(wr.WinRate, wr.SampleSize));
            var mapAdjustments = serialized.MapAdjustments.ToDictionary(wr => (heroes[wr.HeroId], maps[wr.MapId]), wr => new Statistic(wr.AdjFactor, wr.SampleSize));
            var synergies = serialized.HeroAdjustments.ToDictionary(ha => (heroes[ha.HeroId], heroes[ha.OtherHeroId]), ha => new Statistic(ha.SynergyAdjFactor, ha.SynergySampleSize));
            var counters = serialized.HeroAdjustments.ToDictionary(ha => (heroes[ha.HeroId], heroes[ha.OtherHeroId]), ha => new Statistic(ha.CounterAdjFactor, ha.CounterSampleSize));

            return new StatisticsCollection(
                heroes,
                maps,
                baseWinRates,
                mapAdjustments,
                synergies,
                counters);
        }

        public static StatisticsCollection FromHotslogsExport(
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

            var baseStatisticsBuilder = new BaseWinRatesBuilder(heroes, baseMatches);
            var mapStatisticsBuilder = new MapAdjustmentsBuilder(adjMatches, heroes.Values, maps.Values);
            var heroAdjustmentsBuilder = new HeroAdjustmentsBuilder(adjMatches, heroes.Values);
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

            var baseStatistics = baseStatisticsBuilder.GetWinRates();
            var mapAdjustments = mapStatisticsBuilder.Build();
            var (synergies, counters) = heroAdjustmentsBuilder.Build();

            return new StatisticsCollection(
                heroes,
                maps,
                baseStatistics,
                mapAdjustments,
                synergies,
                counters);
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
