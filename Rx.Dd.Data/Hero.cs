using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rx.Dd.Data
{
    public class Hero
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("leaderId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string LeaderId { get; set; }

        [JsonPropertyName("superHeroApiId")]
        public long? SuperHeroApiId { get; set; }

        [JsonPropertyName("publisher")]
        public string Publisher { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("realName")]
        public string RealName { get; set; }

        [JsonPropertyName("alignment")]
        public string Alignment { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("race")]
        public string Race { get; set; }

        [JsonPropertyName("powerstats")]
        public Powerstats Powerstats { get; set; }

        [JsonPropertyName("teams")]
        public List<string> Teams { get; set; }

        [JsonPropertyName("avatarUrl")]
        public string AvatarUrl { get; set; }

        [JsonPropertyName("created")]
        public string Created { get; set; }

        [JsonPropertyName("updated")]
        public string Updated { get; set; }
    }
}