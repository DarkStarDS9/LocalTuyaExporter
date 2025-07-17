using System.Collections.Generic;
using System.Text.Json.Serialization;
using TuyaNet;

namespace TuyaExporter
{
    public class AppSettings
    {
        public List<TuyaDeviceSetting> TuyaDevices { get; set; }
    }

    public class TuyaDeviceSetting
    {
        public string Ip { get; set; }
        public string Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public TuyaDevice Device { get; set; }
    }
}
