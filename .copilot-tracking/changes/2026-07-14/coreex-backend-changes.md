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

---

# Changes: CoreEx Backend — Leviathan read vertical (Phase 3/4 slice)

- Implementation date: 2026-07-14 (continued)

## Summary

Implemented the read side of the `Leviathan` entity end-to-end and verified it at runtime against the live Postgres database. Build clean; `GET /roster` returns the seeded 4-leviathan roster with reference-data codes.

## Added

- `src/Contracts/Leviathan.cs` — `[Contract]` `Leviathan` (+`LeviathanCollection`), `IIdentifier<string>`+`IETag`, flat coordinates, `[ReferenceData<LeviathanStatus>] StatusCode` → JSON `status`, `[ReferenceData<ThreatLevel>] ThreatCode` → JSON `threat`.
- `src/Infrastructure/Mapping/LeviathanMapper.cs` — one-way Persistence→Contract mapper (read slice).
- `src/Application/Repositories/ILeviathanRepository.cs` + `src/Infrastructure/Repositories/LeviathanRepository.cs` — `[ScopedService<>]`, EfDb query, logical-delete filter, order-by codename, get-by-id.
- `src/Application/ILeviathanService.cs` + `src/Application/LeviathanService.cs` — `[ScopedService<>]`, GetAll + GetById.
- `src/Api/Controllers/RosterController.cs` — thin `WebApi` controller: `GET /roster`, `GET /roster/{id}` (+HEAD).
- `src/Api/GlobalUsing.cs` — added `global using ...Application;`.

## Validation (runtime)

- `dotnet build` → 0 errors (1 benign scaffold warning + 1 pre-existing generated-mapper CS8601).
- `GET /roster` → 200, 4 leviathans (Gorathos, Vespyra, Terrakon, Nyxmora).
- `GET /roster/lev-1` → 200; JSON `{"id":"lev-1","codename":"Gorathos",...,"status":"LND",...,"threat":"CAT",...,"etag":"..."}`. Ref-data codes serialize as `status`/`threat` — aligns with front-end `src/mock/types.ts` field names.
- EF SQL verified: logical-delete predicate + order-by codename + parameterized get-by-id.

## Key operational finding — running hosts standalone

The `coreex` API/Relay/Subscribe hosts expect `ConnectionStrings:Postgres` and `ConnectionStrings:redis` to be injected by **Aspire**. Running a host bare via `dotnet run` yields env `Production` with no connection strings → `InvalidOperationException: ConnectionString is missing` and an HTTP 500 at controller activation (DI cannot build `ResponseSysEfDb`). To smoke-test a host standalone, provide them, e.g.:

```powershell
$env:ConnectionStrings__Postgres = 'Host=127.0.0.1;Port=5432;Database=responsesys;Username=postgres;Password=yourStrong#!Password'
$env:ConnectionStrings__redis = 'localhost:6379'
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet run --project src/ApexDynamics.TitanWatch.ResponseSys.Api
```

The proper long-term path is an Aspire AppHost (or appsettings.Development.json / user-secrets) so hosts resolve connection strings without manual env vars.

## Deferred / notes

- `LeviathanMapper` is one-way (read slice); promote to `BiDirectionMapper` when mutate endpoints are added.
- `GetLive` (sim-derived position/range/status) is Phase 5.
- Remaining entities (SignalEvent, DispatchUnit, LastStandCity) not yet contracted/served.

---

# Changes: CoreEx Backend — SignalEvent / DispatchUnit / LastStandCity read verticals

- Implementation date: 2026-07-14 (continued)

## Summary

Added read-only verticals for the remaining three entities (21 files), mirroring the verified Leviathan recipe. All four read endpoints are now runtime-verified against Postgres. User added `Api/appsettings.Development.json` (Aspire Npgsql + Redis connection strings), so hosts now run standalone with just `ASPNETCORE_ENVIRONMENT=Development` (supersedes the env-var workaround).

## Added (per entity: contract, mapper, repo iface+impl, service iface+impl, controller)

- **SignalEvent** → `FeedController` `GET /feed` (**paged**, `[Paging(supportsCount:true)]`, `ORDER BY timestamp DESC`, `ToMappedItemsResultAsync`) + `GET /feed/{id}`. Contract: `IIdentifier<string>` (no ETag), `[ReferenceData<SignalSeverity>] SeverityCode` → JSON `severity`.
- **DispatchUnit** → `DispatchController` `GET /dispatch` (ORDER BY name) + `/{id}`. Fields Name/Available/Capacity.
- **LastStandCity** → `CitiesController` `GET /cities` (ORDER BY name) + `/{id}`. Fields Name/Side/Population.
- No `global using` changes needed (Leviathan work already covered them).

## Validation (runtime, Development env via appsettings.Development.json)

- `dotnet build` → 0 errors (2 pre-existing warnings).
- `GET /dispatch` → 4 units: Deploy Mechs 4/4, Evac Sector 8/8, Raise Barrier 3/3, Scramble Jets 6/6 (matches mock INITIAL_DISPATCH).
- `GET /cities` → BREMERTON (left, 412000), OLYMPIA (right, 318000) (matches mock LAST_STAND_CITIES).
- `GET /feed?$take=2` → 200, empty `[]` — EXPECTED: `signal_event` has no seed rows; the feed is produced at runtime by the Phase 5 simulation engine. Paged query executes (`LIMIT/OFFSET`, `ORDER BY timestamp DESC` confirmed in SQL log).

## Read side complete

All four entities now have GET endpoints: `/roster`, `/roster/{id}`, `/feed` (paged), `/feed/{id}`, `/dispatch`, `/dispatch/{id}`, `/cities`, `/cities/{id}`, plus generated ref-data endpoints. Remaining: command endpoints (dispatch/alert), the simulation engine (to populate the feed + live roster movement), CORS, and the front-end HttpAdapter.

---

# Changes: CoreEx Backend — API read integration tests

- Implementation date: 2026-07-14 (continued)

## Summary

Added UnitTestEx `*.Test.Api` intra-domain integration tests for all four GET verticals as a stabilization checkpoint before the command/simulation work. `dotnet test` → **25/25 pass** (includes the scaffolded Swagger/Health tests). Dev DB restored to clean masters-only seed afterward via `-- all`.

## Added / modified

- `tests/*.Test.Api/RosterReadTests.cs`, `DispatchReadTests.cs`, `CitiesReadTests.cs`, `FeedReadTests.cs` (partial `HostTests`).
- `tests/*.Test.Common/Data/read-data.seed.yaml` — seeds `signal_event` (4 rows, ascending timestamps, severities INF/WRN/OPS/CRT); the other 3 entities assert against the production masters seed.
- `tests/*.Test.Api/GlobalUsing.cs` — added `global using ...Contracts;`.

## Coverage

- Roster: list (4, ordered by codename), get-by-id (codename + `threat`=CAT + `status`=LND + hp + etag), JSON ref-data shape (`threat`/`status`/`etag`), 404.
- Dispatch: list (4, name asc), get-by-id (name/available/capacity), 404.
- Cities: list (2, name asc), get-by-id (name/side/population), 404.
- Feed: list (4, timestamp DESC), `$take`, `$skip`, `$take&$count=true` → `X-Paging-Total-Count`, get-by-id (`severity`=INF), 404.

## Key findings

- The Test.Api migrate path (`MigratePostgresDataAsync<TestData>(..., DbMigration.ConfigureMigrationArgs)`) **DOES apply the production Data phase** — `masters.seed.pgsql` + `ref-data.seed.yaml` — and resets the `responsesys` schema each run. So test seed files should add ONLY tables with no production seed (here: `signal_event`); re-seeding masters/ref-data causes duplicate-key failures.
- CoreEx paging total-count header is **`X-Paging-Total-Count`** (not `x-total-count`).
- Running Test.Api mutates the shared `responsesys` DB (reset + re-seed). Restore a clean feed-empty state with `dotnet run --project tools/*.Database -- all`.

## Validation

- `dotnet test tests/*.Test.Api` → Passed: 25, Failed: 0, Skipped: 0.
- Dev DB restored (`-- all`): ref-data 5/5/4 rows; masters re-seeded; feed empty again.
