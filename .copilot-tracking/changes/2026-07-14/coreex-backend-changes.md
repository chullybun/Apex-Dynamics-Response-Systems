<!-- markdownlint-disable-file -->
# Changes: CoreEx Backend — Phase 1 (Scaffold)

- Related plan: [.copilot-tracking/plans/2026-07-14/coreex-backend-plan.instructions.md](../../plans/2026-07-14/coreex-backend-plan.instructions.md)
- Implementation date: 2026-07-14

## Summary

Executed Phase 1 (Prerequisites, scaffold & build) of the CoreEx backend plan. Verified the local toolchain and scaffolded the empty-shell CoreEx solution under `backend/`. Build succeeds cleanly. Infrastructure startup and `coreex-ai` asset install were intentionally deferred.

## Environment verified

- .NET SDK: 10.0.301
- Container engine: Podman (machine `podman-machine-default` running under WSL); Docker not installed (Podman is preferred per CoreEx guide)
- CoreEx templates: already installed (`coreex`, `coreex-ai`, `coreex-api`, `coreex-domain`, `coreex-relay`, `coreex-subscribe`)

## Added

- `backend/ApexDynamics.TitanWatch.ResponseSys/` — scaffolded CoreEx solution (`dotnet new coreex`, all defaults → Postgres, Service Bus, outbox, reference data), including:
  - `src/ApexDynamics.TitanWatch.ResponseSys.Contracts`
  - `src/ApexDynamics.TitanWatch.ResponseSys.Application` (+ `Repositories`)
  - `src/ApexDynamics.TitanWatch.ResponseSys.Infrastructure` (+ `Repositories`)
  - `tests/ApexDynamics.TitanWatch.ResponseSys.Test.Common` (+ `Data`)
  - `tests/ApexDynamics.TitanWatch.ResponseSys.Test.Unit`
  - `tools/ApexDynamics.TitanWatch.ResponseSys.CodeGen`
  - `tools/ApexDynamics.TitanWatch.ResponseSys.Database` (+ `Data`, `Migrations`)
  - Solution/config files: `ApexDynamics.TitanWatch.ResponseSys.slnx`, `docker-compose.yml`, `Directory.Build.props`, `Directory.Packages.props`, `AGENTS.md`, `CLAUDE.md`, `README.md`, `.editorconfig`, `.gitignore`, `servicebus/`

## Validation

- `dotnet build` → Build succeeded, 0 errors, 1 warning (CS9113 unread parameter `jdr` in scaffolded `Test.Unit/EntryPoint.cs` — benign, part of the template).

## Deferred (not done this session)

- `coreex-ai` AI assets install — writes CoreEx Copilot instructions/prompts/agents into the repo `.github/`; deferred pending user confirmation to avoid cluttering the existing HVE `.github/`.
- `podman compose up -d` infrastructure startup — large multi-image pull (Postgres, SQL Server, Redis, Service Bus emulator, Aspire); deferred to the start of Phase 2 where migrations/ref-data seeding need it.

---

# Changes: CoreEx Backend — Phase 2 (Reference data) + DB layer

- Implementation date: 2026-07-14 (continued)

## Summary

Completed the database layer and the reference-data vertical. `coreex-ai` was installed (CoreEx skills/instructions now present in repo `.github/`). Infrastructure containers started. Authored 9 migrations, seed data, and generated both DbEx persistence models and CoreEx ref-data contracts. Build clean.

## Added / generated

- **Infra**: `podman compose up -d` — Postgres, SQL Server, Redis, Service Bus emulator, Aspire running.
- **Migrations** (`tools/*.Database/Migrations/`, `.pgsql`): schema, outbox, `threat_level`, `leviathan_status`, `signal_severity`, `leviathan`, `signal_event`, `dispatch_unit`, `last_stand_city`.
- **Seed**: `Data/ref-data.seed.yaml` (ref-data rows) + `Data/masters.seed.pgsql` (master/roster data). `dbex.yaml` tables registered.
- **DbEx CodeGen**: `Infrastructure/Persistence/*.g.cs` (7 models) + `Repositories/ResponseSysDbContext.g.cs`.
- **CoreEx CodeGen** (`ref-data.yaml` entities: ThreatLevel [+Color], LeviathanStatus, SignalSeverity): `Contracts/{ThreatLevel,LeviathanStatus,SignalSeverity}.g.cs`, `Api/Controllers/ReferenceDataController.g.cs`, `Application/ReferenceDataService.g.cs`, `Application/Repositories/IReferenceDataRepository.g.cs`, `Infrastructure/Repositories/ReferenceDataRepository.g.cs`, `Infrastructure/Mapping/{ThreatLevel,LeviathanStatus,SignalSeverity}Mapper.g.cs`.
- **Global usings** wired (clean scaffold omits own-namespace imports): `Contracts` → Application/Infrastructure/Api; `Application.Repositories` → Infrastructure; `Infrastructure.Mapping` → Infrastructure.
- **API host + Test.Api** projects exist (added during scaffold continuation).

## Reference-data seeded (3-char Code / sentence-case Text)

- `threat_level` (5): DOR Dormant, STR Stirring, ELV Elevated, CRI Critical, CAT Cataclysm (+ Color hex mirroring src/mock/severity.ts).
- `leviathan_status` (5): SUB Submerged, SUR Surfaced, INB Inbound, LND Landfall, CON Contained.
- `signal_severity` (4): INF Info, WRN Warn, OPS Ops, CRT Crit.

## Validation

- `dotnet run --project tools/*.Database -- all` → Create/Migrate/CodeGen/Schema/Data all Complete; ref-data upserts succeed (5/5/4 rows).
- `dotnet build` → succeeded, 0 errors, 1 benign scaffold warning (CS9113).

## Gotchas captured (for future phases)

- Seed SQL for Postgres must be `.pgsql`, not `.sql` (DbEx provider suffix). `.sql` files are ignored by the Postgres migrator.
- The DbEx migrator's standalone `codegen` command is not enabled; DbEx code-gen runs only inside `dotnet run -- all`.
- CoreEx `*.CodeGen` resolves `ref-data.yaml` relative to the **current directory** — run `dotnet run` from *inside* the CodeGen project folder (or the config path resolves to the solution root and fails).
- The clean `coreex` scaffold omits own-namespace `global using`s; add them alongside the code that needs them (CS0246/CS0103 after CodeGen).
