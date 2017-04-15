using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HeroesDrafter
{
    public sealed class Data
    {
        public static readonly Data CurrentData = new Data().AddBaseStats(BaseStatisticsTeamLeague20170407);

        //private static readonly HeroAdjustments ZaryaQuickMatch20170410 = new HeroAdjustments(Context.Instance)
        //    .Add(48.5,
        //         new []
        //         {
        //            Tuple.Create
        //         })

        private static readonly BaseStatistics[] BaseStatisticsTeamLeague20170407 =
        {
            BaseStatistics.ForHero(Hero.Abathur, 1239, 118, 4.8, 48.2),
            BaseStatistics.ForHero(Hero.Alarak, 2044, 76, 7.5, 47.8),
            BaseStatistics.ForHero(Hero.Anubarak, 6697, 1822, 30.1, 56.6),
            BaseStatistics.ForHero(Hero.Artanis, 8876, 2665, 40.8, 49.6),
            BaseStatistics.ForHero(Hero.Arthas, 9757, 5204, 52.9, 54.3),
            BaseStatistics.ForHero(Hero.Auriel, 4834, 963, 20.5, 50.5),
            BaseStatistics.ForHero(Hero.Azmodan, 1737, 142, 6.6, 49.1),
            BaseStatistics.ForHero(Hero.Brightwing, 5112, 1227, 22.4, 48.2),
            BaseStatistics.ForHero(Hero.Cassia, 1022, 436, 5.2, 49.1),
            BaseStatistics.ForHero(Hero.Chen, 746, 19, 2.7, 50.3),
            BaseStatistics.ForChoGall(1680, 347, 7.2, 56, 1635, 237, 6.6, 56.2),
            BaseStatistics.ForHero(Hero.Chromie, 3095, 939, 14.3, 45.8),
            BaseStatistics.ForHero(Hero.Dehaka, 5207, 7256, 44.1, 50.8),
            BaseStatistics.ForHero(Hero.Diablo, 6561, 1275, 27.7, 46.5),
            BaseStatistics.ForHero(Hero.ETC, 10001, 8414, 65.2, 51.1),
            BaseStatistics.ForHero(Hero.Falstad, 6628, 3685, 36.5, 50.8),
            BaseStatistics.ForHero(Hero.Gazlowe, 1170, 155, 4.7, 44.4),
            BaseStatistics.ForHero(Hero.Greymane, 5663, 1220, 24.4, 54.4),
            BaseStatistics.ForHero(Hero.Guldan, 7770, 2850, 37.6, 52.4),
            BaseStatistics.ForHero(Hero.Illidan, 1791, 258, 7.3, 47.5),
            BaseStatistics.ForHero(Hero.Jaina, 6740, 1088, 27.7, 50.3),
            BaseStatistics.ForHero(Hero.Johanna, 6518, 1071, 26.9, 49.4),
            BaseStatistics.ForHero(Hero.Kaelthas, 8574, 3131, 41.4, 50.2),
            BaseStatistics.ForHero(Hero.Kerrigan, 2166, 258, 8.6, 50.6),
            BaseStatistics.ForHero(Hero.Kharazim, 3279, 164, 12.2, 51.1),
            BaseStatistics.ForHero(Hero.Leoric, 3143, 515, 12.9, 51.1),
            BaseStatistics.ForHero(Hero.LiLi, 2529, 114, 9.4, 43.9),
            BaseStatistics.ForHero(Hero.LiMing, 9410, 1487, 38.6, 46.8),
            BaseStatistics.ForHero(Hero.LtMorales, 2554, 725, 11.6, 47.3),
            BaseStatistics.ForHero(Hero.Lunara, 3264, 261, 12.5, 48.9),
            BaseStatistics.ForHero(Hero.Lucio, 11477, 9581, 74.5, 53.4),
            BaseStatistics.ForHero(Hero.Malfurion, 16687, 7293, 84.8, 50.6),
            BaseStatistics.ForHero(Hero.Medivh, 857, 90, 3.4, 44.7),
            BaseStatistics.ForHero(Hero.Muradin, 6294, 531, 24.1, 45.1),
            BaseStatistics.ForHero(Hero.Murky, 803, 208, 3.6, 49.7),
            BaseStatistics.ForHero(Hero.Nazeebo, 6749, 578, 25.9, 53.0),
            BaseStatistics.ForHero(Hero.Nova, 1026, 97, 4.0, 44.9),
            BaseStatistics.ForHero(Hero.Probius, 1168, 180, 4.8, 53.1),
            BaseStatistics.ForHero(Hero.Ragnaros, 5516, 14312, 70.2, 48.2),
            BaseStatistics.ForHero(Hero.Raynor, 3384, 81, 12.3, 47.5),
            BaseStatistics.ForHero(Hero.Rehgar, 6547, 1481, 28.4, 50.8),
            BaseStatistics.ForHero(Hero.Rexxar, 841, 251, 3.9, 52.8),
            BaseStatistics.ForHero(Hero.Samuro, 1223, 385, 5.7, 47.3),
            BaseStatistics.ForHero(Hero.SgtHammer, 1182, 213, 4.9, 47.5),
            BaseStatistics.ForHero(Hero.Sonya, 5832, 1063, 24.4, 53.6),
            BaseStatistics.ForHero(Hero.Stitches, 2013, 71, 7.4, 44.6),
            BaseStatistics.ForHero(Hero.Sylvanas, 6242, 8375, 51.7, 50.6),
            BaseStatistics.ForHero(Hero.Tassadar, 4601, 5537, 35.9, 53.5),
            BaseStatistics.ForHero(Hero.TheButcher, 2508, 366, 10.2, 47.2),
            BaseStatistics.ForHero(Hero.TheLostVikings, 691, 977, 5.9, 61.1),
            BaseStatistics.ForHero(Hero.Thrall, 3108, 201, 11.7, 47.1),
            BaseStatistics.ForHero(Hero.Tracer, 2568, 523, 10.9, 49.9),
            BaseStatistics.ForHero(Hero.Tychus, 3589, 1266, 17.2, 47.0),
            BaseStatistics.ForHero(Hero.Tyrael, 1872, 69, 6.9, 49.8),
            BaseStatistics.ForHero(Hero.Tyrande, 1831, 77, 6.8, 49.8),
            BaseStatistics.ForHero(Hero.Uther, 1085, 27, 3.9, 45.1),
            BaseStatistics.ForHero(Hero.Valeera, 3173, 3256, 22.7, 46.7),
            BaseStatistics.ForHero(Hero.Valla, 12924, 2929, 56.1, 51.4),
            BaseStatistics.ForHero(Hero.Varian, 8546, 2163, 37.9, 46.8),
            BaseStatistics.ForHero(Hero.Xul, 1628, 1266, 10.2, 52.0),
            BaseStatistics.ForHero(Hero.Zagara, 1192, 85, 4.5, 44.4),
            BaseStatistics.ForHero(Hero.Zarya, 4983, 2028, 24.8, 56.2),
            BaseStatistics.ForHero(Hero.Zeratul, 1862, 459, 8.2, 51.0),
            BaseStatistics.ForHero(Hero.Zuljin, 2384, 162, 9.0, 49.5),
        };

        private readonly Dictionary<Hero, BaseStatistics> _baseStats = new Dictionary<Hero, BaseStatistics>();
        public IReadOnlyDictionary<Hero, BaseStatistics> BaseStats => _baseStats;

        private Data()
        {
        }

        public Data AddBaseStats(IEnumerable<BaseStatistics> stats)
        {
            foreach (var stat in stats)
            {
                if (!_baseStats.ContainsKey(stat.Hero))
                    _baseStats[stat.Hero] = stat;
            }
            return this;
        }
    }
}
