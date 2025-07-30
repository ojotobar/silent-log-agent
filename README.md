![Docker Image Version](https://img.shields.io/docker/v/blueclikk/silent-log-agent?sort=semver&label=version)
![Docker Pulls](https://img.shields.io/docker/pulls/blueclikk/silent-log-agent)

# 🕵️ SilentLogAgent

**SilentLogAgent** is a lightweight .NET-based monitoring tool designed to detect unexpected silence in log files — because sometimes *no logs* means something's gone wrong.

It’s built for containerized environments and integrates seamlessly with Prometheus and Slack for alerts and observability.

---

## 🚀 Features

- ⏱ Detects gaps in expected log activity  
- 🔔 Slack alerts for silence anomalies  
- 📊 Prometheus metrics endpoint for monitoring  
- 🔧 Flexible configuration via JSON file or environment variables  
- 🐳 Docker-ready with GitHub Actions CI/CD  

---

## 🗂 Project Structure

SilentLogAgent/
├── .github/
│ └── workflows/
│ └── docker-publish.yml # GitHub Actions for Docker Hub deployment
│
├── specs/
│ └── spec.sample.json # Example config file
│
├── Alerts/ # Alerting integrations (Slack, Prometheus)
├── Extensions/ # DI extension helpers
├── Models/ # Spec and LogEntry models
├── Services/ # Core services: log tailing, analysis, monitoring
│
├── appsettings.json # Runtime config (can be overridden via env vars)
├── Dockerfile # For building Docker image
├── .dockerignore
├── Program.cs
└── SilentLogAgent.csproj


---

## ⚙️ Configuration

SilentLogAgent supports configuration via:

- A mounted `spec.json` file for log monitoring rules  
- `appsettings.json` for alert sink settings  
- Environment variables for overrides (ideal for Docker/Kubernetes)  

---

### 🔍 `spec.json`

This file defines the expected log behavior. Mount it at `/app/specs/spec.json` inside the container.

```json
[
  {
    "id": "sample-log",
    "name": "Sample Log Monitor",
    "logFilePath": "/var/log/app/sample.log",
    "pattern": "(?<timestamp>\\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}Z)\\s+Sample Log",
    "silenceThresholdMinutes": 10,
    "timestampGroupName": "timestamp",
    "isCritical": true,
    "labels": {
      "service": "SampleService",
      "environment": "Production"
    }
  }
]
```

## 🚨 Alert Sink Configuration
SilentLogAgent currently supports the following alerting backends:

### 🔔 Slack Alerts
Enable Slack alerts and expose Prometheus metrics by configuring the following settings in appsettings.json or as environment variables:

<details> 
  <summary>
    <code>
      appsettings.json
    </code>
  </summary>
  {
    "Slack": {
      "WebhookUrl": "https://hooks.slack.com/services/XXX/YYY/ZZZ"
    },
    "Prometheus": {
      "Enabled": true,
      "Port": 5000
    }
  }
</details>

<details> 
  <summary>
    Environment Variables
  </summary>
  Slack__WebhookUrl=https://hooks.slack.com/services/XXX/YYY/ZZZ
  Prometheus__Enabled=true
  Prometheus__Port=5000
</details>

## 🐳 Docker Instructions
SilentLogAgent is designed to run seamlessly in containerized environments.

### 🛠️ Build the Docker Image
```bash
docker build -t silentlog-agent .
```

Or pull from Docker Hub:
```bash
docker pull blueclikk/silent-log-agent:latest
```

## 🚀 Run the Container
Mount the log file and spec.json:
```bash
docker run -d \
  --name silentlog-agent \
  -v /path/to/your/logfile.log:/logs/app.log \
  -v $(pwd)/specs/spec.json:/app/specs/spec.json \
  -e Slack__WebhookUrl="https://hooks.slack.com/services/XXX/YYY/ZZZ" \
  -e Prometheus__Enabled=true \
  -e Prometheus__Port=5000 \
  blueclikk/silent-log-agent:latest
```
