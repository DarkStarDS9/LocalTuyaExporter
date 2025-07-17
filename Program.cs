using System.Text.Json;
using com.clusterrr.TuyaNet;
using Prometheus;

namespace LocalTuyaExporter
{
    class Program
    {
        private static readonly Gauge DpsValueGauge = Metrics.CreateGauge("tuya_dps_value", "Value of a Tuya device's data point (DP).", "device_name", "dp_id");

        static async Task Main(string[] _)
        {
            var devices = JsonSerializer.Deserialize<List<TuyaDeviceSetting>>(File.ReadAllText("devices.json"))
                ?? throw new InvalidOperationException("Failed to deserialize devices.json");

            var server = new KestrelMetricServer(port: 8080);
            server.Start();

            Console.WriteLine("Exporter started on port 8080");

            foreach (var deviceSetting in devices)
            {
                deviceSetting.TuyaDevice = new TuyaDevice(ip: deviceSetting.Ip, localKey: deviceSetting.Key, deviceId: deviceSetting.Id);
                Console.WriteLine($"Added device: {deviceSetting.Name}");
            }

            while (true)
            {
                foreach (var device in devices)
                {
                    try
                    {
                        var dps = await (device.TuyaDevice ?? throw new InvalidOperationException("TuyaDevice is null?!")).GetDpsAsync();
                       
                        foreach (var dp in dps.Where(dp => !device.DpsBlacklist.Contains(dp.Key.ToString())))
                        {
                            var dpKey = dp.Key.ToString();
                            double? dpValue = dp.Value switch
                            {
                                bool b => b ? 1.0 : 0.0,
                                byte by => (double)by,
                                int i => (double)i,
                                long l => (double)l,
                                float f => (double)f,
                                double d => d,
                                decimal dec => (double)dec,
                                _ => null
                            };

                            if (dpValue.HasValue)
                            {
                                DpsValueGauge.WithLabels(device.Name, dpKey).Set(dpValue.Value);
                                
                                // Track published DPS keys
                                device.PublishedDpsKeys.Add(dpKey);
                            }
                            else
                            {
                                device.DpsBlacklist.Add(dpKey);
                                Console.WriteLine($"Blacklisted DPS key '{dpKey}' for device '{device.Name}' - unsupported type: {dp.Value?.GetType().Name ?? "null"} with value '{dp.Value}'");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error getting DPS for device {device.Id}: {ex.Message}");
                        
                        // Unpublish all metrics for this device
                        foreach (var dpKey in device.PublishedDpsKeys)
                        {
                            DpsValueGauge.WithLabels(device.Name, dpKey).Unpublish();
                        }
                        device.PublishedDpsKeys.Clear();
                    }
                }
                Thread.Sleep(5000);
            }
        }
    }
}