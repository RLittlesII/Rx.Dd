using System;
using System.Text.Json.Serialization;

namespace Rx.Dd.Data
{
    public partial class Image
    {
        [JsonPropertyName("url")]
        public Uri Url { get; set; }
    }
}