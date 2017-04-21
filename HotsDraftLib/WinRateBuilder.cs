namespace HotsDraftLib
{
    public struct WinRateBuilder
    {
        private int _gamesPlayed;
        public int GamesPlayed => _gamesPlayed;
        private int _gamesWon;
        public int GamesWon => _gamesWon;

        public Statistic WinRate => new Statistic(GamesPlayed == 0 ? 0 : (double)GamesWon / GamesPlayed, GamesPlayed);

        private WinRateBuilder(int gamesPlayed, int gamesWon)
        {
            _gamesPlayed = gamesPlayed;
            _gamesWon = gamesWon;
        }
        
        public static void Increment(ref WinRateBuilder builder, bool won)
        {
            builder._gamesPlayed += 1;
            if (won)
                builder._gamesWon += 1;
        }
    }
}
