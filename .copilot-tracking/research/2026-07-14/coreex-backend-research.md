<!-- markdownlint-disable-file -->
# Research: CoreEx Backend for Titan Watch

## Scope

Stand up a REST-based backend for the Apex Dynamics Response Systems ("Project Titan Watch") command-center front-end using the Avanade CoreEx .NET framework. Replace the in-browser mocked simulation with a server-authoritative backend while keeping the existing React client largely intact.

## User-Selected Decisions (2026-07-14)

| Decision | Choice |
|----------|--------|
| Data provider | PostgreSQL |
| Live updates | REST polling (client polls `GET /roster` + `GET /feed`) |
| Messaging | Azure Service Bus + transactional outbox |
| ROP | Deferred (default off; revisit) |

Reference-data convention (user-directed): three-character uppercase `Code`, sentence-case `Text` for every reference-data type.

All choices match `dotnet new coreex` defaults (Postgres, ServiceBus, outbox-enabled, refdata-enabled).

## CoreEx Summary

- Modular .NET framework (opt-in NuGet packages) for enterprise back-end services. Not a runtime; a set of libraries + `dotnet new` template pack + AI workflow assets.
- Target frameworks: `net8.0`, `net9.0`, `net10.0`. v4 is **preview** (`-preview` suffix); getting-started requires **.NET SDK 10+**. No upgrade path from v3.
- Layered architecture with strict inward dependency rule:
  - Business layers: **Contracts** ← **Domain** (optional) ← **Application** ← **Infrastructure**.
  - Host layers (composition roots): **API**, **Outbox Relay**, **Subscribe**.
  - Design-time tooling: **CodeGen** (ref-data scaffolding), **Database** (schema lifecycle, seeds, generates `*.g.cs`).
- Key packages: `CoreEx`, `CoreEx.AspNetCore`, `CoreEx.AspNetCore.NSwag`, `CoreEx.Validation`, `CoreEx.RefData`, `CoreEx.Data`, `CoreEx.Database`, `CoreEx.Database.Postgres`, `CoreEx.EntityFrameworkCore`, `CoreEx.Events`, `CoreEx.Azure.Messaging.ServiceBus`, `CoreEx.Caching.FusionCache`, `CoreEx.CodeGen`, `CoreEx.UnitTesting`.

## Capabilities Mapped to Mock Concerns

| Mock concern (src/mock, src/state) | CoreEx capability |
|---|---|
| `ThreatLevel`, `LeviathanStatus`, `SignalSeverity` enums w/ colors/labels | Reference Data (`ReferenceData<TId,TSelf>`), cached, code/Id-indexed, extra properties for color |
| `types.ts` domain model | Contracts layer (technology-agnostic DTOs; `IIdentifier`, `IETag`) |
| Ad-hoc validation / capacity guards | Validation framework + Semantic exceptions → `ProblemDetails` |
| `dispatchUnit`, `triggerAlert` handlers | Application services (thin controllers → orchestration) |
| Signal feed generation | Events (CloudEvents) + Service Bus + transactional outbox |
| No API today | `CoreEx.AspNetCore` WebApi helpers, MVC/Minimal API, OpenAPI/NSwag |

## Scaffolding Facts

- Folder naming convention: `Company.Product.Domain` → `ApexDynamics.TitanWatch.ResponseSys` (`ResponseSys` avoids clashes with the common word `Response`).
- `dotnet new install CoreEx.Template` installs the template pack.
- `dotnet new coreex-ai [--app-folder <path>]` installs AI instructions/skills into `.github/` (use `--app-folder` for monorepo subfolder).
- `dotnet new coreex` scaffolds solution with defaults; emits `src/`, `tests/`, `tools/`, `docker-compose.yml`, config. Compiles cleanly but is an empty shell (no entities/routes).
- `docker-compose.yml` services: Postgres (5432), SQL Server (1433, SB emulator backing), Redis (6379), Service Bus emulator (5672/5300), Aspire dashboard (18888 UI, 4317 OTLP).
- Host add-on templates: `coreex-api`, `coreex-relay`, `coreex-subscribe`; optional `coreex-domain`.
- Design-time run: `dotnet run --project tools/<name>.Database -- all` (apply schema); `dotnet run --project tools/<name>.CodeGen` (generate ref-data stubs).
- AI skill catalog (invoke via `/coreex-<name>` or prompts): `coreex-contract`, `coreex-refdata`, `coreex-db-migration`, `coreex-repository`, `coreex-adapter`, `coreex-app-service`, `coreex-validator`, `coreex-policy`, `coreex-aggregate`, `coreex-api`, `coreex-subscriber`, plus L2 `coreex-api-e2e`, `coreex-subscriber-e2e`.

## Gaps / Considerations

- **Live push:** CoreEx is REST + broker-events, not browser push. Decision = REST polling. Server simulation must expose current state via `GET /roster` and `GET /feed` (paged).
- **Simulation engine:** The mock's 1s heartbeat + Poisson feed + motion model (`tickLeviathans`, `deriveAdvance`, `makeSignalEvent`) must move server-side as an `IHostedService`. This is the largest net-new piece; no CoreEx template covers it directly.
- **CORS:** API host must allow the Vite dev origin (and deployed origin).
- **Front-end seam:** Refactor `useCommandState` behind a data-adapter (MockAdapter vs HttpAdapter) so components are unchanged. `types.ts` becomes the shared contract; generate TS types from OpenAPI.
- **Preview risk:** v4 preview may introduce breaking changes.

## References

- CoreEx repo: https://github.com/Avanade/CoreEx
- README: https://github.com/Avanade/CoreEx/blob/main/README.md
- Getting Started: https://github.com/Avanade/CoreEx/blob/main/docs/getting-started.md
- Layers: https://github.com/Avanade/CoreEx/blob/main/samples/docs/layers.md
- Pattern Catalog: https://github.com/Avanade/CoreEx/blob/main/samples/docs/patterns.md
- AGENTS.md: https://github.com/Avanade/CoreEx/blob/main/AGENTS.md
