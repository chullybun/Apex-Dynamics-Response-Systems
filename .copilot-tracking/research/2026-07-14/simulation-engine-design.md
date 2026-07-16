<!-- markdownlint-disable-file -->
# Research/Design: Server-Authoritative Simulation Engine (Phase 5)

- Date: 2026-07-14
- Related plan: [../../plans/2026-07-14/coreex-backend-plan.instructions.md](../../plans/2026-07-14/coreex-backend-plan.instructions.md)

## Goal

Port the browser simulation to a server-authoritative background engine so `GET /roster` returns live-moving leviathans and `GET /feed` fills with generated signal events (published via the transactional outbox). Mirrors the mock: [src/mock/leviathans.ts](../../../src/mock/leviathans.ts) (`tickLeviathans`, `deriveAdvance`, `fracAt`, `posFromFrac`, `rangeFromFrac`, `statusFromRange`, constants) and [src/mock/feed.ts](../../../src/mock/feed.ts) (`makeSignalEvent`, phrase pools, `scheduleNext` Poisson arrivals).

## Architecture (respects CoreEx layering)

- **Api host** (`src/...Api/`): a `SimulationHostedService : BackgroundService` — runtime driver only. Registered via `builder.Services.AddHostedService<SimulationHostedService>()`. Per tick it creates a DI scope (`IServiceScopeFactory`), sets an `ExecutionContext` (system user `"simulation"`), resolves the Application service, calls it, disposes the scope. Guarded by config flag so tests/other hosts don't run it.
- **Application** (`src/...Application/Simulation/`): `ISimulationService` + `SimulationService` (`[ScopedService<ISimulationService>]`) holding the orchestration + ported pure helpers (motion model, phrase pools). Methods:
  - `TickRosterAsync(double simSeconds, ct)` — drift HP/speed/repel, compute live pos/range/status from `simSeconds`, persist; on landfall crossing emit a CRIT signal event.
  - `GenerateSignalsAsync(int count, ct)` — create N signal events (weighted severity, optional leviathan link) + publish CloudEvents via outbox.
- **Infrastructure** (repositories): add MUTATION methods — `LeviathanRepository.UpdateAsync/SaveRange` and `SignalEventRepository.CreateAsync`. Promote `LeviathanMapper` to bidirectional (contract↔persistence) OR update persistence models directly via EfDb. Feed create + event publish must run inside `IUnitOfWork` so the outbox enqueue commits atomically with the row insert.

## Ported simulation model

- Constants (from leviathans.ts): `CLOSURE_RATE=0.05`, `LANDFALL_RANGE_KM=25`, `SURFACED_RANGE_KM=70`, `REPEL_KNOCKBACK_*`, HP drift `rand(1,28)`, speed drift `rand(-4,4)` clamp `[0,120]`, repel decay `0.04`/tick.
- Sim clock: service tracks accumulated `simSeconds` (monotonic, +dt each tick). `frac = ((speed*CLOSURE_RATE*simSeconds) % startRange)/startRange`, minus `repel`, clamp ≥0; wraps (loops forever). Use the live `speed` for closure (server simplification vs mock's frozen track speed — acceptable).
- `pos = lerp(from,to,frac)`; `range = startRange*(1-frac)`; `status = range<25?LND : range<70?SUR : INB`. Persist `lng/lat/range/status_code`.
- Landfall detection: when a leviathan's range crosses below `LANDFALL_RANGE_KM` (or frac wraps), emit CRIT `"{CODENAME} reached landfall at {target} — repelled. Track reacquired..."`.

## Ported feed model

- Phrase pools from feed.ts (SENSORS, SECTORS, WARN_CLAUSES, LINKED_OPS_CLAUSES, SECTOR_OPS_CLAUSES).
- Severity weighting: roll<0.6 INFO, <0.82 WARN, else OPS (codes INF/WRN/OPS; landfall uses CRT). Optional leviathan link (WARN 0.7, else 0.35).
- Arrival: rate ~24/min. Per 1s tick draw event count ~ Poisson(rate/60) (or exponential inter-arrival). Each event: id, severity_code, message, timestamp=epoch ms (BIGINT), leviathan_id?. Insert row + `IEventPublisher.Publish(EventData)` (→ outbox) within UoW; commit.
- Feed growth: table grows unbounded; `GET /feed` is paged so acceptable for prototype. Optional: trim events older than N (deferred).

## Config (appsettings)

- `Simulation:Enabled` (bool, default true), `Simulation:TickIntervalMs` (1000), `Simulation:FeedRatePerMinute` (24). Bind via IOptions or read IConfiguration. Disable in Test.Api (tests seed their own signal_event and assert deterministic data — sim must NOT run during tests).

## Key CoreEx gotchas to handle

- **ExecutionContext required** for background DB writes (CreatedBy/UpdatedBy stamping). Set a system `ExecutionContext` inside the per-tick scope.
- **Scoped services** (repositories, EfDb, IUnitOfWork, IEventPublisher) must be resolved from a fresh scope each tick — do not capture singletons.
- **Outbox publish needs UoW** — insert + publish + commit in one `IUnitOfWork` flow so the outbox row is transactional.
- **Test isolation** — the hosted service must be OFF under the Test.Api host (config flag; tests already pass with an empty feed + their own seed).

## Verification plan

- Run Api host (Development). Poll `GET /roster` twice → hp decreasing, lng/lat/range/status changing. Poll `GET /feed?$take=5&$count=true` → row count grows over time, newest-first, severities varied. Confirm outbox rows accrue (optional: query `responsesys.outbox`).
- `dotnet build` clean; `dotnet test Test.Api` still 25/25 (sim disabled in tests).
- Restore clean DB (`-- all`) afterward.

## References

- coreex-host-setup.instructions.md (hosted services, AddHostedService); coreex-application-services.instructions.md (UoW, IEventPublisher, ExecutionContext); coreex-repositories.instructions.md (EfDb mutation).
