using System.Text.Json.Serialization;

namespace Rx.Dd.Data
{
    public partial class Work
    {
        [JsonPropertyName("occupation")]
        public string Occupation { get; set; }

        [JsonPropertyName("base")]
        public string Base { get; set; }
    }
}