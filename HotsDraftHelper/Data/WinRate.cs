namespace HotsDraftHelper.Data
{
    public struct WinRate
    {
        public int GamesPlayed { get; }
        public int GamesWon { get; }

        private WinRate(int gamesPlayed, int gamesWon)
        {
            GamesPlayed = gamesPlayed;
            GamesWon = gamesWon;
        }

        public WinRate Increment(bool won)
        {
            return new WinRate(GamesPlayed + 1, GamesWon + (won ? 1 : 0));
        }

        public double Percentage
        {
            get
            {
                if (GamesPlayed < Settings.Default.MinSampleSize)
                    return 0.0;
                return (double)GamesWon / GamesPlayed;
            }
        }
    }
}
