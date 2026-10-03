# Hangfire Project

A clean ASP.NET Core foundation for running recurring background jobs with [Hangfire](https://www.hangfire.io/), SQL Server storage, and a protected Hangfire Dashboard.

> **Project status:** This repository is an extensible starter application. The job infrastructure is wired up, while the application database and business-specific jobs are intentionally ready for implementation.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-13-239120?logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![Hangfire](https://img.shields.io/badge/Hangfire-1.8.24-8A2BE2)](https://www.hangfire.io/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-supported-CC2927?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Overview

Hangfire Project provides the base infrastructure for scheduling and processing background work in an ASP.NET Core Web API. It separates the web host, reusable job infrastructure, and data-access layer into focused projects.

The application currently provides:

- Hangfire server configuration backed by SQL Server.
- Automatic discovery and dependency-injection registration of job services.
- A reusable `JobBaseService` for recurring jobs.
- Named queues for default, created, edited, and deleted work.
- A Hangfire Dashboard at `/hangfire` protected by HTTP Basic Authentication.
- Entity Framework Core SQL Server configuration through `HangfireDbContext`.
- Development-only Swagger/OpenAPI support.
- A sample `WeatherForecast` endpoint retained as the default API scaffold.

## Architecture

```text
HangfireProject.Web
├── ASP.NET Core host and middleware pipeline
├── Configuration and connection strings
├── Hangfire server and dashboard setup
└── Sample WeatherForecast API
        │
        ▼
HangfireProject.Core
├── Job contracts and base implementation
├── Queue definitions
├── Job discovery and registration extensions
├── Service locator used during application startup
└── Dashboard authorization filter
        │
        ▼
HangfireProject.Data
├── Entity Framework Core SQL Server dependency
└── HangfireDbContext ready for application entities and DbSets
```

### Projects

| Project | Responsibility |
| --- | --- |
| `HangfireProject.Web` | Executable ASP.NET Core Web API, dependency-injection composition root, Swagger, Hangfire server, dashboard, and sample controller. |
| `HangfireProject.Core` | Job abstractions, queue names, automatic job registration, dashboard authorization, and reusable Hangfire behavior. |
| `HangfireProject.Data` | Entity Framework Core context and the persistence layer foundation. |

## How job registration works

Jobs are discovered from the `HangfireProject.Core` assembly. Any non-abstract class that implements `IJobBaseService` is registered automatically as a transient service when `AddJob()` runs.

To add a recurring job:

1. Create a class in `HangfireProject.Core` (the `Jobs` folder is reserved for this purpose).
2. Inherit from `JobBaseService`.
3. Provide a cron expression.
4. Implement `Execute`.
5. Optionally select one of the configured queues.

Example:

```csharp
using HangfireProject.Core.Common;
using HangfireProject.Core.Hangfire;

namespace HangfireProject.Core.Jobs;

public sealed class ExampleJob : JobBaseService
{
    public override string CronExpression => "0 * * * *";

    public override string QueueName => nameof(EnHangfireQueue.defaults);

    public override Task Execute(CancellationToken cancellationToken = default)
    {
        // Add application-specific background work here.
        return Task.CompletedTask;
    }
}
```

At startup, `RegisterJob()` resolves all discovered jobs and calls `Register()` on each one. The base implementation creates or updates a Hangfire recurring job using the class name as its recurring-job ID and the `Iran Standard Time` time zone.

### Available queues

| Queue | Intended use |
| --- | --- |
| `defaults` | General background work. |
| `created` | Work triggered by creation events. |
| `edited` | Work triggered by update events. |
| `deleted` | Work triggered by deletion events. |

The Hangfire server listens to all four queues. Add new queues to `EnHangfireQueue` and to the `option.Queues` list in `HangfireProject.Web/Program.cs` when required.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server 2019 or later, or a compatible SQL Server instance
- A SQL Server login with permission to create and update the application and Hangfire databases
- Git (optional, for cloning the repository)

## Getting started

### 1. Clone the repository

```bash
git clone https://github.com/MohammadHasanp/Hangfire_Project.git
cd Hangfire_Project
```

### 2. Configure SQL Server securely

The application expects two connection strings:

- `DefaultConnectionString` for the application database.
- `HangfireContextString` for Hangfire's storage database.

Set them in user secrets, environment variables, or a deployment secret store. Do not commit passwords or production connection strings to source control. The checked-in configuration is only a local-development starting point and should be reviewed before use.

For local development with .NET user secrets:

```bash
dotnet user-secrets init --project HangfireProject.Web
dotnet user-secrets set "ConnectionStrings:DefaultConnectionString" "<application-sql-server-connection-string>" --project HangfireProject.Web
dotnet user-secrets set "ConnectionStrings:HangfireContextString" "<hangfire-sql-server-connection-string>" --project HangfireProject.Web
```

### 3. Restore, build, and run

```bash
dotnet restore HangfireProject.slnx
dotnet build HangfireProject.slnx
dotnet run --project HangfireProject.Web
```

The launch profiles use:

- HTTP: `http://localhost:5272`
- HTTPS: `https://localhost:7216`

### 4. Explore the application

- Swagger UI: `https://localhost:7216/swagger` in the Development environment.
- Sample endpoint: `GET /WeatherForecast`.
- Hangfire Dashboard: `https://localhost:7216/hangfire`.

The current dashboard authorization filter uses the development credentials `admin` / `admin`. Replace this implementation with a secure, configurable authentication and authorization mechanism before exposing the dashboard outside local development. Basic Authentication must only be used over HTTPS.

## Configuration reference

| Setting | Purpose |
| --- | --- |
| `ConnectionStrings:DefaultConnectionString` | SQL Server connection used by `HangfireDbContext`. |
| `ConnectionStrings:HangfireContextString` | SQL Server storage used by Hangfire. |
| `ASPNETCORE_ENVIRONMENT` | Enables Swagger when set to `Development`. |

## Data layer status

`HangfireDbContext` is intentionally a foundation at this stage. It currently has no application `DbSet` properties or model configuration. Add entities under `HangfireProject.Data/Entities`, expose them through the context, and add migrations when application persistence is introduced.

Example migration commands:

```bash
dotnet ef migrations add InitialCreate --project HangfireProject.Data --startup-project HangfireProject.Web
dotnet ef database update --project HangfireProject.Data --startup-project HangfireProject.Web
```

## Testing and quality

No test project is included yet. Before adding production jobs, add unit tests for job behavior, queue selection, authorization, and persistence; add integration tests against an isolated SQL Server instance for Hangfire registration and execution.

Build the complete solution with:

```bash
dotnet build HangfireProject.slnx
```

## Repository metadata

- **Owner and maintainer:** [MohammadHasanp](https://github.com/MohammadHasanp)
- **Repository:** [MohammadHasanp/Hangfire_Project](https://github.com/MohammadHasanp/Hangfire_Project)
- **Primary language:** C#
- **Supporting formats:** XML project/solution files, JSON configuration, and Markdown documentation
- **Framework:** ASP.NET Core on .NET 10
- **Persistence:** SQL Server through Entity Framework Core and Hangfire.SqlServer
- **Project type:** Multi-project ASP.NET Core Web API and background-job foundation
- **License:** [MIT License](LICENSE)

## Suggested GitHub About metadata

Use this description for the repository About field:

> ASP.NET Core .NET 10 foundation for SQL Server-backed Hangfire recurring jobs, automatic job discovery, named queues, and a protected dashboard.

Suggested repository topics:

```text
aspnet-core
csharp
dotnet
dotnet10
hangfire
background-jobs
background-services
job-scheduler
recurring-jobs
sql-server
entity-framework-core
web-api
swagger
```

To apply the About description and topics with GitHub CLI:

```bash
gh repo edit MohammadHasanp/Hangfire_Project \
  --description "ASP.NET Core .NET 10 foundation for SQL Server-backed Hangfire recurring jobs, automatic job discovery, named queues, and a protected dashboard." \
  --add-topic aspnet-core \
  --add-topic csharp \
  --add-topic dotnet \
  --add-topic dotnet10 \
  --add-topic hangfire \
  --add-topic background-jobs \
  --add-topic background-services \
  --add-topic job-scheduler \
  --add-topic recurring-jobs \
  --add-topic sql-server \
  --add-topic entity-framework-core \
  --add-topic web-api \
  --add-topic swagger
```

## Contributors

See the [repository contributors graph](https://github.com/MohammadHasanp/Hangfire_Project/graphs/contributors).

## Author

Created and maintained by [Mohammad Hasan Pirayandeh](https://github.com/MohammadHasanp).
