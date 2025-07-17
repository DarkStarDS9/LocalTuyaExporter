using System.Text.Json.Serialization;
using com.clusterrr.TuyaNet;

namespace LocalTuyaExporter
{
    public class TuyaDeviceSetting
    {
        public required string Ip { get; set; }
        public required string Id { get; set; }
        public required string Key { get; set; }
        public required string Name { get; set; }

        [JsonIgnore]
        public TuyaDevice? TuyaDevice { get; set; }

        [JsonIgnore]
        public HashSet<string> DpsBlacklist { get; } = [];

        [JsonIgnore]
        public HashSet<string> PublishedDpsKeys { get; } = [];
    }
}
