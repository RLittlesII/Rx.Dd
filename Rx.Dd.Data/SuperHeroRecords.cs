using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rx.Dd.Data
{
    public partial class SuperHeroRecords
    {
        [JsonPropertyName("response")]
        public string Response { get; set; }

        [JsonPropertyName("results-for")]
        public string ResultsFor { get; set; }

        [JsonPropertyName("results")]
        public List<SuperHeroRecord> Heroes { get; set; }
    }
}