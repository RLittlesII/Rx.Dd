using System.Text.Json.Serialization;

namespace Rx.Dd.Data
{
    public partial class Connections
    {
        [JsonPropertyName("group-affiliation")]
        public string GroupAffiliation { get; set; }

        [JsonPropertyName("relatives")]
        public string Relatives { get; set; }
    }
}