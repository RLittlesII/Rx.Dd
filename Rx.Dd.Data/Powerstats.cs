using System.Text.Json.Serialization;

namespace Rx.Dd.Data
{
    public partial class Powerstats
    {
        [JsonPropertyName("intelligence")]
        public string Intelligence { get; set; }

        [JsonPropertyName("strength")]
        public string Strength { get; set; }

        [JsonPropertyName("speed")]
        public string Speed { get; set; }

        [JsonPropertyName("durability")]
        public string Durability { get; set; }

        [JsonPropertyName("power")]
        public string Power { get; set; }

        [JsonPropertyName("combat")]
        public string Combat { get; set; }
    }
}