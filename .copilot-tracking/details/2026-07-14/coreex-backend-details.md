<!-- markdownlint-disable-file -->
# Implementation Details: CoreEx REST Backend for Titan Watch

## Context References

- Plan: [.copilot-tracking/plans/2026-07-14/coreex-backend-plan.instructions.md](../../plans/2026-07-14/coreex-backend-plan.instructions.md)
- Research: [.copilot-tracking/research/2026-07-14/coreex-backend-research.md](../../research/2026-07-14/coreex-backend-research.md)
- Log: [.copilot-tracking/plans/logs/2026-07-14/coreex-backend-log.md](../../plans/logs/2026-07-14/coreex-backend-log.md)

## Phase 1: Prerequisites, scaffold & infrastructure

### Step 1 — Prerequisites
- Verify `dotnet --list-sdks` shows 10.x. Verify Podman or Docker is running.
- Windows: `git config --global core.autocrlf input` (per CoreEx guide).
- Success: SDK 10+ present; container engine available.

### Step 2 — Placement & naming
- Recommend backend in a `backend/` subfolder of this repo to keep front-end + backend together: `mkdir backend && cd backend`.
- Solution folder name = solution name = `ApexDynamics.TitanWatch.ResponseSys` (`Company.Product.Domain`; `ResponseSys` avoids clashes with the common word `Response`). Create it inside `backend/`.
- Success: empty solution folder with correct name; `git init` already covered by repo root.

### Step 3 — Templates & AI assets
- `dotnet new install CoreEx.Template`
- `dotnet new list --tag CoreEx` (verify `coreex*` templates).
- From repo root: `dotnet new coreex-ai --app-folder backend/ApexDynamics.TitanWatch.ResponseSys` (scopes CoreEx AI instructions to the backend path).
- Success: `coreex*` templates listed; `.github/` gains CoreEx instructions/prompts/agents.

### Step 4 — Scaffold
- From inside the solution folder: `dotnet new coreex` (accept defaults → Postgres, ServiceBus, outbox true, refdata true). Add `--rop-enabled true` only if adopting Result<T> now (deferred).
- Emits `src/`, `tests/`, `tools/`, `docker-compose.yml`, config.
- Success: `dotnet restore` + `dotnet build` succeed (empty shell).

### Step 5 — Infrastructure & build
- `podman compose up -d` (or `docker compose up -d`).
- Optional verify: `dotnet run --project tools/ApexDynamics.TitanWatch.ResponseSys.Database -- all`; `dotnet run --project tools/ApexDynamics.TitanWatch.ResponseSys.CodeGen`.
- Success: containers healthy (Postgres 5432, Redis 6379, SB emulator, Aspire 18888); `dotnet build` clean.
- Dependencies: Steps 1–4.

## Phase 2: Reference data

### Step 1 — ref-data.yaml
- Convention (user-directed): every reference-data type uses a **three-character uppercase `Code`** and a **sentence-case `Text`**. `Code` is the wire value; `Text` is the display label. Codes are scoped per type, so repeats across types are acceptable.
- Add reference-data types in `tools/*.CodeGen/ref-data.yaml`:
  - `ThreatLevel` — with a numeric sort order (rank) and an extra `Color` property mirroring [src/mock/severity.ts](../../../src/mock/severity.ts) hex values:

    | Code | Text | Rank | Color |
    |------|-----------|------|---------|
    | DOR | Dormant | 0 | #10b981 |
    | STR | Stirring | 1 | #facc15 |
    | ELV | Elevated | 2 | #f97316 |
    | CRI | Critical | 3 | #ef4444 |
    | CAT | Cataclysm | 4 | #b91c1c |

  - `LeviathanStatus` — SUB/Submerged, SUR/Surfaced, INB/Inbound, LND/Landfall, CON/Contained.
  - `SignalSeverity` — INF/Info, WRN/Warn, OPS/Ops, CRT/Crit.
- Client mapping note: the existing UI tokens ([src/mock/types.ts](../../../src/mock/types.ts) uses full uppercase strings like `SUBMERGED`, `WARN`, `Cataclysm`) are mapped from ref-data `Code`/`Text` in the front-end `HttpAdapter` (Phase 8), keeping the API contract on 3-char codes without changing component rendering.
- Reference: `coreex-refdata` skill.

### Step 2 — Generate + seed
- `dotnet run --project tools/*.CodeGen` to scaffold contract/controller/service/repository/mapper.
- `dotnet run --project tools/*.Database -- all` to migrate + seed ref-data rows.
- Success: ref-data endpoints resolvable; seed rows present.
- Dependencies: Phase 1.

## Phase 3: Contracts, persistence & mapping

### Step 1 — Contracts entities
- `Leviathan` (id, codename, classNumeral, archetype, range, height, speed, hp, hpMax, status[RefData], lng/lat, threat[RefData], heading, from/to, target, startRange, repel), `IIdentifier<string>` + `IETag`.
- `SignalEvent` (id, severity[RefData], message, timestamp, leviathanId?).
- `DispatchUnit` (name, available, capacity).
- `LastStandCity` (name, side, population).
- Mirror shapes to [src/mock/types.ts](../../../src/mock/types.ts) so the client contract is preserved.
- Reference: `coreex-contract` skill.

### Step 2 — Persistence + mapping
- Migrations for `leviathan`, `signal_event`, `dispatch_unit`, `last_stand_city` (plus outbox infra from scaffold).
- Persistence models + EF DbContext (`*.g.cs` via Database tool), CoreEx mappers Contracts↔Model.
- Reference: `coreex-db-migration`, `coreex-repository`, `coreex-adapter`.

### Step 3 — Seed
- Seed the 4-leviathan roster (from [src/mock/leviathans.ts](../../../src/mock/leviathans.ts) `ROSTER`), initial dispatch assets ([src/state/useCommandState.ts](../../../src/state/useCommandState.ts) `INITIAL_DISPATCH`), and Last-Stand cities.
- Success: `GET /roster` (once API exists) returns seeded roster.
- Dependencies: Phase 2.

## Phase 4: Application services & validation

### Step 1 — RosterService
- `GetAll`, `GetById`, `GetLive` (applies current sim-derived position/range/status).

### Step 2 — DispatchService
- `Deploy(leviathanId, unitName)`: validate capacity (>0) else `BusinessException`/validation error; decrement capacity; apply repel knockback; append OPS signal event.
- Port `REPEL_KNOCKBACK_*` constants.

### Step 3 — AlertService
- `Raise(active)`: toggle citywide alert; append WARN/OPS signal event.

### Step 4 — FeedService + validators
- `GetFeed` paged (CoreEx paging), newest-first, capped semantics.
- Validators for command inputs; policies for capacity guard.
- Success: services unit-tested; validation yields `ProblemDetails`.
- Dependencies: Phase 3.

## Phase 5: Server-authoritative simulation engine

### Step 1 — Tick loop hosted service
- `IHostedService` on ~1s cadence porting [src/mock/leviathans.ts](../../../src/mock/leviathans.ts) `tickLeviathans` (HP countdown, speed drift, repel decay) and `deriveAdvance`/`fracAt`/`posFromFrac`/`rangeFromFrac`/`statusFromRange` for position/range/status.
- Persist derived state so `GET /roster` reflects live world across clients.
- Note: determinism seed (`ROSTER_SEED`) → seed C# PRNG equivalently.

### Step 2 — Feed generator
- Port [src/mock/feed.ts](../../../src/mock/feed.ts) `makeSignalEvent` phrase pools + `scheduleNext` Poisson arrivals into a background producer; persist `SignalEvent` and publish CloudEvents via the transactional outbox.
- Success: roster mutates over time; feed rows accumulate; events land in outbox.
- Dependencies: Phase 4.

## Phase 6: API host, endpoints, OpenAPI & CORS

### Step 1 — API host
- `dotnet new coreex-api -n ApexDynamics.TitanWatch.ResponseSys.Api --data-provider Postgres --refdata-enabled true --outbox-enabled true` (or `coreex-api-e2e` skill).

### Step 2 — Controllers
- `GET /roster`, `GET /roster/{id}`, `POST /roster/{id}/dispatch`, `POST /alert`, `GET /dispatch`, `GET /feed` (`[Paging]`), ref-data endpoints. Thin controllers delegating to Application services.

### Step 3 — CORS + OpenAPI
- CORS policy allowing Vite origin (`http://localhost:5173`) and deployed origin.
- `CoreEx.AspNetCore.NSwag` OpenAPI generation.
- Success: Swagger UI lists endpoints; browser calls succeed cross-origin.
- Dependencies: Phase 5.

## Phase 7: Outbox Relay & Subscribe hosts

### Step 1 — Relay
- `dotnet new coreex-relay -n ...Relay --data-provider Postgres --messaging-provider ServiceBus`. Forwards outbox → Service Bus.

### Step 2 — Subscribe
- `dotnet new coreex-subscribe -n ...Subscribe --data-provider Postgres --messaging-provider ServiceBus --refdata-enabled true`; add subscriber(s) as needed (e.g. audit/replication).
- Success: relay drains outbox; subscriber integration tests run.
- Dependencies: Phase 6 (parallelizable across the two host adds).

## Phase 8: Front-end integration

### Step 1 — Adapter seam
- Refactor [src/state/useCommandState.ts](../../../src/state/useCommandState.ts) internals to call an injectable `CommandDataAdapter`. Provide `MockAdapter` (current behavior) and `HttpAdapter` (REST). No component prop changes.

### Step 2 — Polling + commands
- `HttpAdapter` polls `GET /roster` + `GET /feed` on an interval (respect `prefers-reduced-motion` for rate). Map `dispatchUnit`/`triggerAlert`/`select` to `POST`/local as appropriate.

### Step 3 — Types
- Generate TS types from OpenAPI; reconcile with [src/mock/types.ts](../../../src/mock/types.ts) (keep as the shared contract).
- Success: app runs against backend with `HttpAdapter`; mock still works with `MockAdapter`.
- Dependencies: Phase 6.

## Phase 9: Testing & validation

### Step 1 — Backend tests
- `coreex-test-api` skill; `dotnet test tests/*.Test.Unit` (fast) and full `dotnet test` (with containers).

### Step 2 — Front-end tests
- Add adapter tests; run `npm test` and `npm run build`.
- Success: all suites green.
- Dependencies: Phases 1–8.
