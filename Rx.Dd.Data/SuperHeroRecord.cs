using System.Text.Json.Serialization;

namespace Rx.Dd.Data
{
    public partial class SuperHeroRecord
    {
        [JsonPropertyName("response")]
        public string Response { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("powerstats")]
        public Powerstats Powerstats { get; set; }

        [JsonPropertyName("biography")]
        public Biography Biography { get; set; }

        [JsonPropertyName("appearance")]
        public Appearance Appearance { get; set; }

        [JsonPropertyName("work")]
        public Work Work { get; set; }

        [JsonPropertyName("connections")]
        public Connections Connections { get; set; }

        [JsonPropertyName("image")]
        public Image Image { get; set; }
    }
}