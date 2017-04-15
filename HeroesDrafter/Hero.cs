using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HeroesDrafter
{
    public sealed class Hero
    {
        public static readonly Hero Abathur = new Hero("Abathur");
        public static readonly Hero Alarak = new Hero("Alarak");
        public static readonly Hero Anubarak = new Hero("Anub'arak");
        public static readonly Hero Artanis = new Hero("Artanis");
        public static readonly Hero Arthas = new Hero("Arthas");
        public static readonly Hero Auriel = new Hero("Auriel");
        public static readonly Hero Azmodan = new Hero("Azmodan");
        public static readonly Hero Brightwing = new Hero("Brightwing");
        public static readonly Hero Cassia = new Hero("Cassia");
        public static readonly Hero Chen = new Hero("Chen");
        public static readonly Hero ChoGall = new Hero("Cho'Gall");
        public static readonly Hero Chromie = new Hero("Chromie");
        public static readonly Hero Dehaka = new Hero("Dehaka");
        public static readonly Hero Diablo = new Hero("Diablo");
        public static readonly Hero ETC = new Hero("E.T.C.");
        public static readonly Hero Falstad = new Hero("Falstad");
        public static readonly Hero Gazlowe = new Hero("Gazlowe");
        public static readonly Hero Greymane = new Hero("Greymane");
        public static readonly Hero Guldan = new Hero("Gul'dan");
        public static readonly Hero Illidan = new Hero("Illidan");
        public static readonly Hero Jaina = new Hero("Jaina");
        public static readonly Hero Johanna = new Hero("Johanna");
        public static readonly Hero Kaelthas = new Hero("Kael'thas");
        public static readonly Hero Kerrigan = new Hero("Kerrigan");
        public static readonly Hero Kharazim = new Hero("Kharazim");
        public static readonly Hero Leoric = new Hero("Leoric");
        public static readonly Hero LiLi = new Hero("Li Li");
        public static readonly Hero LiMing = new Hero("Li-Ming");
        public static readonly Hero LtMorales = new Hero("Lt. Morales");
        public static readonly Hero Lunara = new Hero("Lunara");
        public static readonly Hero Lucio = new Hero("Lúcio");
        public static readonly Hero Malfurion = new Hero("Malfurion");
        public static readonly Hero Medivh = new Hero("Medivh");
        public static readonly Hero Muradin = new Hero("Muradin");
        public static readonly Hero Murky = new Hero("Murky");
        public static readonly Hero Nazeebo = new Hero("Nazeebo");
        public static readonly Hero Nova = new Hero("Nova");
        public static readonly Hero Probius = new Hero("Probius");
        public static readonly Hero Ragnaros = new Hero("Ragnaros");
        public static readonly Hero Raynor = new Hero("Raynor");
        public static readonly Hero Rehgar = new Hero("Rehgar");
        public static readonly Hero Rexxar = new Hero("Rexxar");
        public static readonly Hero Samuro = new Hero("Samuro");
        public static readonly Hero SgtHammer = new Hero("Sgt. Hammer");
        public static readonly Hero Sonya = new Hero("Sonya");
        public static readonly Hero Stitches = new Hero("Stitches");
        public static readonly Hero Sylvanas = new Hero("Sylvanas");
        public static readonly Hero Tassadar = new Hero("Tassadar");
        public static readonly Hero TheButcher = new Hero("The Butcher");
        public static readonly Hero TheLostVikings = new Hero("The Lost Vikings");
        public static readonly Hero Thrall = new Hero("Thrall");
        public static readonly Hero Tracer = new Hero("Tracer");
        public static readonly Hero Tychus = new Hero("Tychus");
        public static readonly Hero Tyrael = new Hero("Tyrael");
        public static readonly Hero Tyrande = new Hero("Tyrande");
        public static readonly Hero Uther = new Hero("Uther");
        public static readonly Hero Valeera = new Hero("Valeera");
        public static readonly Hero Valla = new Hero("Valla");
        public static readonly Hero Varian = new Hero("Varian");
        public static readonly Hero Xul = new Hero("Xul");
        public static readonly Hero Zagara = new Hero("Zagara");
        public static readonly Hero Zarya = new Hero("Zarya");
        public static readonly Hero Zeratul = new Hero("Zeratul");
        public static readonly Hero Zuljin = new Hero("Zul'jin");

        public static IReadOnlyList<Hero> All { get; }
        static Hero()
        {
            All = typeof(Hero)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(Hero))
                .Select(f => f.GetValue(null))
                .Cast<Hero>()
                .ToList();
        }

        public string Name { get; }

        private Hero(string name)
        {
            Name = name;
        }
    }
}
