# ResponseSys API Host -- AI Agent Guide

This is the **ASP.NET Core API host** for the `ResponseSys` domain, part of the `ApexDynamics.TitanWatch.ResponseSys` microservice. It exposes REST endpoints backed by the CoreEx layered application core.

> **Before answering any CoreEx question:** check whether `.github/docs/coreex/` is populated at the solution root. If empty, run `/coreex-docs-sync` first. The package guide at `.github/docs/coreex/agents/CoreEx.AspNetCore.md` is especially relevant to this project.

---

## Host Responsibilities

This project is a **thin host** -- it wires up DI, configures the pipeline, and delegates all business logic to the `ApexDynamics.TitanWatch.ResponseSys.Application` layer. It should contain:

- `Program.cs` -- startup, DI registration, middleware pipeline
- Controllers -- route handlers that call Application services; no business logic here
- No direct database access; no direct Service Bus calls

Read `.github/docs/coreex/hosts-layer.md` for the full host-layer guide.

---

## Adding a Controller

1. Create a new file in the project, e.g. `Controllers/ProductController.cs`
2. Inherit from `ControllerBase` and apply `[Route("api/[controller]")]`
3. Inject the Application service interface (from `ApexDynamics.TitanWatch.ResponseSys.Application`)
4. Map HTTP verbs to service methods -- return `IActionResult` using `ApexDynamics.TitanWatch.ResponseSys.Application` result types
5. CoreEx provides `WebApiPublisher` / `HttpWebApi` helpers for standardised response handling

Consult `.github/docs/coreex/agents/CoreEx.AspNetCore.md` for controller patterns and `WebApiPublisher` usage.

---

## OpenAPI / NSwag

The host registers NSwag via `services.AddOpenApiDocument(...)` with CoreEx defaults applied (`s.AddCoreExConfiguration()`). The Swagger UI is available at `/swagger` in development.

Consult `.github/docs/coreex/agents/CoreEx.AspNetCore.NSwag.md` for NSwag customisation patterns.

---

## Caching

The host wires FusionCache with both in-memory (L1) and Redis distributed (L2) backing. The CoreEx `IHybridCache` abstraction is registered as `AddFusionHybridCache()`.

- Use `IHybridCache` in Application services -- never take direct `IMemoryCache` or `IDistributedCache` dependencies
- Idempotency support is provided via `AddHybridCacheIdempotencyProvider()`

Consult `.github/docs/coreex/agents/CoreEx.Caching.FusionCache.md` for caching patterns.

---

## This Host's Feature Configuration

- **Data provider:** PostgreSQL -- `builder.AddNpgsqlDataSource("Postgres")` (Aspire connection name)
- **Transactional outbox:** Enabled -- events are written to the DB outbox by `ResponseSysOutboxPublisher`; the Relay host reads and forwards them
- **Reference data:** Enabled -- `ReferenceDataOrchestrator<ReferenceDataService>` is registered; reference data is hydrated via `ReferenceDataRepository`

---

## Key Packages

| Package | Purpose |
|---|---|
| `CoreEx.AspNetCore` | Web API base types, middleware, `WebApiPublisher` |
| `CoreEx.AspNetCore.NSwag` | NSwag OpenAPI integration |
| `CoreEx.Caching.FusionCache` | FusionCache `IHybridCache` integration |
| `CoreEx.Database.Postgres` | PostgreSQL database access |
| `CoreEx.EntityFrameworkCore` | EF Core integration (`EfDb`, `IEfDbContext`) |
| `CoreEx.RefData` | Reference data orchestration |

---

## Relevant Docs

- `.github/docs/coreex/hosts-layer.md` -- host startup patterns
- `.github/docs/coreex/patterns.md` -- CoreEx request/response patterns
- `.github/docs/coreex/testing.md` -- integration test setup for the API host
- `.github/docs/coreex/local-dev.md` -- running locally with .NET Aspire
- `.github/docs/coreex/agents/CoreEx.AspNetCore.md` -- Web API patterns
- `.github/docs/coreex/agents/CoreEx.Caching.FusionCache.md` -- caching
- `.github/docs/coreex/agents/CoreEx.EntityFrameworkCore.md` -- EF Core patterns
- `.github/docs/coreex/agents/CoreEx.RefData.md` -- reference data patterns
