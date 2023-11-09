using System.Text.Json.Serialization;

namespace DummyDB.Core
{
    public class Column
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("isPrimary")]
        public bool IsPrimary { get; set; }
    }
}
