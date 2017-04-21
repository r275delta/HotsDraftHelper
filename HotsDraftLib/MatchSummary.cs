using System;

namespace HotsDraftLib
{
    internal sealed class MatchSummary
    {
        public long MatchId { get; }
        public GameMode GameMode { get; }
        public int MapId { get; }
        public DateTime Timestamp { get; }

        public MatchSummary(long matchId, GameMode gameMode, int mapId, DateTime timestamp)
        {
            MatchId = matchId;
            GameMode = gameMode;
            MapId = mapId;
            Timestamp = timestamp;
        }
    }
}
