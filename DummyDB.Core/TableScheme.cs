using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DummyDB.Core
{
    public class TableScheme
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("columns")]
        public List<Column> Columns { get; set; }

        public static TableScheme ReadFile(string path)
        {
            return JsonSerializer.Deserialize<TableScheme>(File.ReadAllText(path));
        }
    }
}
