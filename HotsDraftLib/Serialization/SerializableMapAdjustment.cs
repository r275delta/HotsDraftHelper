using Newtonsoft.Json;

namespace HotsDraftLib.Serialization
{
    internal sealed class SerializableMapAdjustment
    {
        public int HeroId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string HeroName { get; set; }
        public int MapId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MapName { get; set; }
        public double AdjFactor { get; set; }
        public int SampleSize { get; set; }
    }
}
