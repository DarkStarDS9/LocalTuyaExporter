using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Prometheus;

namespace TuyaExporter
{
    class Program
    {
        private static readonly Gauge DpsValueGauge = Metrics.CreateGauge("tuya_dps_value", "Value of a Tuya device's data point (DP).", "device_name", "dp_name", "dp_id");

        static async Task Main(string[] args)
        {
            var devicesSettings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText("devices.json"));

            var server = new KestrelMetricServer(port: 8080);
            server.Start();

            Console.WriteLine("Exporter started on port 8080");

            foreach (var deviceSetting in devicesSettings.TuyaDevices)
            {
                deviceSetting.Device = new TuyaNet.TuyaDevice(ip: deviceSetting.Ip, localKey: deviceSetting.Key, deviceId: deviceSetting.Id);
                Console.WriteLine($"Added device: {deviceSetting.Name}");
            }

            while (true)
            {
                foreach (var deviceSetting in devicesSettings.TuyaDevices)
                {
                    try
                    {
                        var dps = await deviceSetting.Device.GetDps();
                        foreach (var dp in dps)
                        {
                            if (double.TryParse(dp.Value.ToString(), out var dpValue))
                            {
                                DpsValueGauge.WithLabels(deviceSetting.Name, dp.Key, dp.Key).Set(dpValue);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error getting DPS for device {deviceSetting.Id}: {ex.Message}");
                    }
                }
                Thread.Sleep(5000);
            }
        }
    }
}
