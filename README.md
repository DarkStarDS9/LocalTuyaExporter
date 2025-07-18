# LocalTuya Exporter
This will connect to your Tuya devices using your local network and export all numeric data-points for prometheus (/metrics)

# Configuration
See the provided example config.json:
```
[
  {
    "Ip": "192.168.1.2",
    "Id": "deviceid1234567890",
    "Key": "secretkey123",
    "Name": "Friendly Device Name"
  }
]
```
You will have to look for current information how to get your local key, a good place to start would be to look at tinytuya.

# Grafana
<img width="781" height="188" alt="image" src="https://github.com/user-attachments/assets/a967a1c5-1f94-4db3-9ca0-dc0792e301d6" />

# Docker
Yes, there is a Dockerfile :smirk:
