using Newtonsoft.Json;

namespace HotsDraftLib.Serialization
{
    internal sealed class SerializableWinRate
    {
        public int HeroId { get; set; }
        /// <remarks>Not actually needed for serialization purposes; just makes the serialized data human-readable.</remarks>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string HeroName { get; set; }
        public double WinRate { get; set; }
        public int SampleSize { get; set; }
    }
}
