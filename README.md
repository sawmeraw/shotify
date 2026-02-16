# shotify

[![Production Build & Deploy](https://github.com/sawmeraw/shotify/actions/workflows/deploy.yml/badge.svg)](https://github.com/sawmeraw/shotify/actions/workflows/deploy.yml)

Product image management and B2B Excel parsing tool. Automates downloading product images from brand CDNs and parsing wholesale price lists into a standardized format.

## Architecture

```
shotify/
├── shotify-dotnet/    # ASP.NET Core 10 MVC web app
├── parser-go/         # Go microservice for Excel parsing
├── docker-compose.yml # PostgreSQL 17
└── Makefile           # Orchestration
```

**Web App** — Manages brand configurations, generates image URLs from CDN patterns, fetches and edits product images, and proxies Excel files to the parser service.

**Parser** — Accepts B2B Excel price lists (Adidas, Nike, Asics) via HTTP or CLI, normalizes them into a standardized output format with product codes, sizes, SKUs, and pricing.

## Tech Stack

| Layer          | Technology                              |
|----------------|-----------------------------------------|
| Web App        | .NET 10, Razor, EF Core, jQuery         |
| Parser         | Go 1.24, Excelize                       |
| Database       | PostgreSQL 17, Npgsql                   |
| Auth           | JWT (HMAC-SHA256), HttpOnly cookies     |
| Infrastructure | Linode VPS, Nginx, systemd              |
| CI/CD          | GitHub Actions, SCP, self-contained bin |

## Local Development

Prerequisites: .NET 10 SDK, Go 1.24+, Docker

```bash
# Start PostgreSQL
make up

# Run EF Core migration
make migrate

# Run both apps
make all
```

Or run them individually:

```bash
make dotnet   # web app on :5012
make go       # parser on :8080
```

## Deployment

Push to `main` triggers the GitHub Actions pipeline which:

1. Builds a self-contained .NET binary (`linux-x64`)
2. Cross-compiles the Go binary (`GOOS=linux GOARCH=amd64`)
3. Transfers both to the VPS via SCP
4. Restarts the systemd services

## Environment Variables

The .NET app reads these from the systemd service environment:

| Variable | Description |
|----------|-------------|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string |
| `JWT_SECRET_KEY` | HMAC-SHA256 signing key |
| `LOGIN` | Application password |
| `GO_API_URL` | Parser service URL (default: `http://localhost:8080`) |
| `ASPNETCORE_ENVIRONMENT` | `Production` |
