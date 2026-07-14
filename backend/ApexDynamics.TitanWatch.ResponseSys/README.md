# ApexDynamics.TitanWatch.ResponseSys

A CoreEx microservice for the `ResponseSys` domain.

## This solution

- **Data provider:** PostgreSQL (`CoreEx.Database.Postgres`, `CoreEx.EntityFrameworkCore`)
- **Reference data:** Enabled — `tools/ApexDynamics.TitanWatch.ResponseSys.CodeGen/` + `src/ApexDynamics.TitanWatch.ResponseSys.Application/ReferenceDataService.cs`
- **Transactional outbox:** Enabled — events committed atomically with data
- **Messaging:** Azure Service Bus (`CoreEx.Azure.Messaging.ServiceBus`)

---

## Prerequisites

| Requirement | Notes |
|---|---|
| **.NET SDK** | `net10.0` — see `Directory.Build.props` |
| **Podman** (preferred) or **Docker** | Required to run the infrastructure containers |

### Git configuration (Windows)

```bash
git config --global core.autocrlf input
```

Run this once. Git for Windows defaults to `core.autocrlf=true`, which overrides the `.gitattributes` LF enforcement and causes noisy diffs. `input` mode stores LF on commit without expanding to CRLF on checkout.

---

## Solution structure

```
ApexDynamics.TitanWatch.ResponseSys/
├── src/
│   ├── ApexDynamics.TitanWatch.ResponseSys.Contracts/        # Public contracts, DTOs, event schemas
│   ├── ApexDynamics.TitanWatch.ResponseSys.Application/      # Services, validators, repository interfaces
│   └── ApexDynamics.TitanWatch.ResponseSys.Infrastructure/   # EF Core repositories, outbox, external adapters
├── tools/
│   ├── ApexDynamics.TitanWatch.ResponseSys.CodeGen/          # Reference data C# generation (reads ref-data.yaml)
│   └── ApexDynamics.TitanWatch.ResponseSys.Database/         # Database migrations and seeding (DbEx)
└── tests/
    ├── ApexDynamics.TitanWatch.ResponseSys.Test.Common/      # Shared test infrastructure and seed data
    └── ApexDynamics.TitanWatch.ResponseSys.Test.Unit/        # Fast isolated unit tests (no I/O)
```

Host projects (`ApexDynamics.TitanWatch.ResponseSys.Api`, `ApexDynamics.TitanWatch.ResponseSys.Relay`, `ApexDynamics.TitanWatch.ResponseSys.Subscribe`) are added separately with `dotnet new coreex-api`, `coreex-relay`, and `coreex-subscribe`. See [Adding hosts](#adding-hosts) below.

---

## Infrastructure

Start the containers before running the solution or any integration tests:

```bash
podman compose up -d   # Podman (preferred)
docker compose up -d   # Docker
```

| Container | Port(s) | Purpose |
|---|---|---|
| `db-postgres` | 5432 | PostgreSQL — domain schema and data |
| `redis-cache` | 6379 | Redis — FusionCache distributed backplane |
| `servicebus-emulator` | 5672 (AMQP), 5300 (mgmt) | Azure Service Bus emulator |
| `aspire-dashboard` | 18888 (UI), 4317 (OTLP) | OpenTelemetry traces and logs |

Stop and remove containers:

```bash
podman compose down
```

### Connection strings

Connection strings are in each host's `appsettings.Development.json` under the `Aspire:` key hierarchy. The `Database` default:

```json
"Aspire": {
  "Npgsql": {
    "ConnectionString": "Server=127.0.0.1;Database=responsesys;Username=postgres;Password=yourStrong#!Password"
  }
}
```

---

## Database

Migrate and seed — required once on first run and after any schema change:

```bash
dotnet run --project tools/ApexDynamics.TitanWatch.ResponseSys.Database -- all
```

| Command | Effect |
|---|---|
| `-- all` | Create schema, run migrations, seed reference data |
| `-- drop` | Drop and recreate the database |
| `-- reset` | Reset seed data only (schema stays) |
| `-- migrate` | Apply migrations without seeding |

See `tools/ApexDynamics.TitanWatch.ResponseSys.Database/Migrations/` for migration scripts and `tools/ApexDynamics.TitanWatch.ResponseSys.Database/Data/` for seed data.

---

## Reference data code generation

After editing `tools/ApexDynamics.TitanWatch.ResponseSys.CodeGen/ref-data.yaml`, regenerate the C# reference data layer:

```bash
dotnet run --project tools/ApexDynamics.TitanWatch.ResponseSys.CodeGen
```

Commit the generated `*.g.cs` files alongside the `ref-data.yaml` changes. **Never edit generated files by hand** — they are overwritten on the next run.

---

## Build and test

```bash
dotnet build
dotnet test
```

Unit tests in `tests/ApexDynamics.TitanWatch.ResponseSys.Test.Unit` are fast and isolated — no infrastructure required. Integration tests (in host test projects) require the containers to be running and the database migrated.

---

## Adding hosts

Use the CoreEx templates to add host projects into this solution:

```bash
dotnet new coreex-api       -n ApexDynamics.TitanWatch.ResponseSys.Api              -o .   # HTTP API + test project
dotnet new coreex-relay     -n ApexDynamics.TitanWatch.ResponseSys.Relay     -o .   # Outbox relay + test project
dotnet new coreex-subscribe -n ApexDynamics.TitanWatch.ResponseSys.Subscribe        -o .   # Event subscriber + test project
```

Each template adds the host project, its test project, and wires both into the solution file.

---

## Coding conventions

| Rule | Detail |
|---|---|
| **Line endings** | LF everywhere (enforced by `.gitattributes` + `.editorconfig`) |
| **Indentation** | 4 spaces (C#) · 2 spaces (JSON, YAML, XML, `*.csproj`) |
| **Nullable** | Enabled — nullable warnings are treated as errors; never suppress with `!` without justification |
| **`using` statements** | One `GlobalUsing.cs` per project — never in individual source files |
| **Namespaces** | File-scoped only: `namespace Foo.Bar;` |
| **Private fields** | Prefixed `_camelCase` |
| **Generated files** | Never edit `*.g.cs`, `*.g.sql`, `*.g.pgsql` — re-run the owning generator |
| **ConfigureAwait** | Always `.ConfigureAwait(false)` in service and repository code |

Full rules are in `.editorconfig`. AI-oriented guidance is in `AGENTS.md`.
