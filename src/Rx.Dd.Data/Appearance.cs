using System.Text.Json.Serialization;

namespace Rx.Dd.Data
{
    public partial class Appearance
    {
        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("race")]
        public string Race { get; set; }

        [JsonPropertyName("height")]
        public string[] Height { get; set; }

        [JsonPropertyName("weight")]
        public string[] Weight { get; set; }

        [JsonPropertyName("eye-color")]
        public string EyeColor { get; set; }

        [JsonPropertyName("hair-color")]
        public string HairColor { get; set; }
    }
}