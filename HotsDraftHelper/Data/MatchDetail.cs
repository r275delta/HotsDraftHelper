namespace HotsDraftHelper.Data
{
    internal sealed class MatchDetail
    {
        public long MatchId { get; }
        public int HeroId { get; }
        public int HeroLevel { get; }
        public bool Winner { get; }
        public int MMR { get; }
        public bool MirrorMatch { get; set; }

        public MatchDetail(long matchId, int heroId, int heroLevel, bool winner, int mmr)
        {
            MatchId = matchId;
            HeroId = heroId;
            HeroLevel = heroLevel;
            Winner = winner;
            MMR = mmr;
        }
    }
}
