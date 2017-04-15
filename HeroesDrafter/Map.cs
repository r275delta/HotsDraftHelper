using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HeroesDrafter
{
    public sealed class Map
    {
        public static readonly Map BattlefieldOfEternity = new Map("Battlefield of Eternity");
        public static readonly Map BlackheartsBay = new Map("Blackheart's Bay");
        public static readonly Map BraxisHoldout = new Map("Braxis Holdout");
        public static readonly Map CursedHollow = new Map("Cursed Hollow");
        public static readonly Map GardenOfTerror = new Map("Garden of Terror");
        public static readonly Map InfernalShrines = new Map("Infernal Shrines");
        public static readonly Map HauntedMines = new Map("Haunted Mines");
        public static readonly Map SkyTemple = new Map("Sky Temple");
        public static readonly Map TombOfTheSpiderQueen = new Map("Tomb of the Spider Queen");
        public static readonly Map TowersOfDoom = new Map("Towers of Doom");
        public static readonly Map WarheadJunction = new Map("Warhead Junction");

        public static IReadOnlyList<Map> All { get; }
        static Map()
        {
            All = typeof(Map)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(Map))
                .Select(f => f.GetValue(null))
                .Cast<Map>()
                .ToList();
        }

        public string Name { get; }

        private Map(string name)
        {
            Name = name;
        }
    }
}
