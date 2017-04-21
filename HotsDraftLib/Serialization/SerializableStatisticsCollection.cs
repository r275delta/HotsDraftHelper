using System.Collections.Generic;

namespace HotsDraftLib.Serialization
{
    internal sealed class SerializableStatisticsCollection
    {
        public List<SerializableHero> Heroes { get; } = new List<SerializableHero>();
        public List<SerializableMap> Maps { get; } = new List<SerializableMap>();
        public List<SerializableWinRate> WinRates { get; } = new List<SerializableWinRate>();
        public List<SerializableMapAdjustment> MapAdjustments { get; } = new List<SerializableMapAdjustment>();
        public List<SerializableHeroAdjustments> HeroAdjustments { get; } = new List<SerializableHeroAdjustments>();
    }
}
