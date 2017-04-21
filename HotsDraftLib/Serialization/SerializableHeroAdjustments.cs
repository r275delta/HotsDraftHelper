using Newtonsoft.Json;

namespace HotsDraftLib.Serialization
{
    internal sealed class SerializableHeroAdjustments
    {
        public int HeroId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string HeroName { get; set; }
        public int OtherHeroId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OtherHeroName { get; set; }
        public double SynergyAdjFactor { get; set; }
        public int SynergySampleSize { get; set; }
        public double CounterAdjFactor { get; set; }
        public int CounterSampleSize { get; set; }
    }
}
