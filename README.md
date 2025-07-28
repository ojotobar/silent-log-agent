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

```plaintext
SilentLogAgent/
├── .github/
│   └── workflows/
│       └── docker-publish.yml       # GitHub Actions for Docker Hub deployment
│
├── specs/
│   └── spec.sample.json             # Example config file
│
├── Alerts/                          # Alerting integrations (Slack, Prometheus)
├── Extensions/                      # DI extension helpers
├── Models/                          # Spec and LogEntry models
├── Services/                        # Core services: log tailing, analysis, monitoring
│
├── appsettings.json                 # Runtime config (can be overridden via env vars)
├── Dockerfile                       # For building Docker image
├── .dockerignore
├── Program.cs
└── SilentLogAgent.csproj
```

## ⚙️ Configuration

SilentLogAgent supports configuration via:

- A mounted `spec.json` file for log monitoring rules
- `appsettings.json` for alert sink settings
- Environment variables for overrides (ideal for Docker/Kubernetes)

---

### 🔍 spec.json

This file defines the expected log behavior. Mount it at `/app/specs/spec.json` inside the container.

```json
{
  "logFile": "/var/log/myapp/app.log",
  "expectedIntervalSeconds": 300,
  "alertThresholdSeconds": 600,
  "alerts": {
    "slack": true,
    "prometheus": true
  }
}
```

## 🚨 Alert Sink Configuration

SilentLogAgent currently supports the following alerting backends:

### 🔔 Slack Alerts

Enable Slack alerts and expose Prometheus metrics by configuring the following: `Slack.WebhookUrl` and `Prometheus` in `appsettings.json` or as environment variables:

```json
{
  "Slack": {
    "WebhookUrl": "https://hooks.slack.com/services/XXX/YYY/ZZZ"
  },
  "Prometheus": {
    "Enabled": true,
    "Port": 5000
  }
}
```

```
Slack__WebhookUrl=https://hooks.slack.com/services/XXX/YYY/ZZZ

Prometheus__Enabled=true
Prometheus__Port=5000
```

## 🐳 Docker Instructions

SilentLogAgent is designed to run seamlessly in containerized environments. Below are steps to build, run, and configure it using Docker.

### 🛠️ Build the Docker Image

```bash
docker build -t silentlog-agent .
```

Or pull from Docker Hub (if available):
```bash
docker pull your-dockerhub-username/silentlog-agent:latest
```

## 🚀 Run the Container
To run the agent, mount the log file and the spec.json configuration:

```bash
docker run -d \
  --name silentlog-agent \
  -v /path/to/your/logfile.log:/logs/app.log \
  -v $(pwd)/specs/spec.json:/app/specs/spec.json \
  -e Slack__WebhookUrl="https://hooks.slack.com/services/XXX/YYY/ZZZ" \
  -e Prometheus__Enabled=true \
  -e Prometheus__Port=5000 \
  your-dockerhub-username/silentlog-agent:latest
```