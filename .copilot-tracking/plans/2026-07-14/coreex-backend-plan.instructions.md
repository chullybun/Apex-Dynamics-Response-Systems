<!-- markdownlint-disable-file -->
---
description: "Phased implementation plan: CoreEx REST backend for Titan Watch"
---

# Implementation Plan: CoreEx REST Backend for Titan Watch

## User Requests

- "I would like to leverage https://github.com/Avanade/coreex to stand-up the backend service using REST-based APIs. Let's understand what is needed to leverage this?" — user, 2026-07-14.
- Decisions (2026-07-14 Q&A): Data provider = **PostgreSQL**; Live updates = **REST polling**; Messaging = **Azure Service Bus + outbox**; Next step = **produce a phased implementation plan**.

## Overview

Replace the in-browser mocked simulation ([src/mock](../../../src/mock), [src/state/useCommandState.ts](../../../src/state/useCommandState.ts)) with a server-authoritative CoreEx microservice exposing REST APIs, backed by PostgreSQL, publishing signal events via Service Bus + transactional outbox. The React client is refactored behind a data-adapter seam and switched from an in-memory mock to HTTP polling. Simulation logic (roster motion, HP countdown, Poisson feed) moves to a server-side hosted service.

Derived objective: keep the existing UI and `types.ts` contract shape intact so the client migration is additive (adapter swap), not a rewrite.

## Context Summary

- Front-end domain model: [src/mock/types.ts](../../../src/mock/types.ts) (`Leviathan`, `SignalEvent`, `DispatchUnit`), [src/mock/severity.ts](../../../src/mock/severity.ts) (`ThreatLevel`), [src/mock/lastStandCities.ts](../../../src/mock/lastStandCities.ts).
- Simulation to port: [src/mock/leviathans.ts](../../../src/mock/leviathans.ts) (`createRoster`, `tickLeviathans`, `deriveAdvance`), [src/mock/feed.ts](../../../src/mock/feed.ts) (`makeSignalEvent`, `scheduleNext`).
- Client state + commands: [src/state/useCommandState.ts](../../../src/state/useCommandState.ts).
- Research: [.copilot-tracking/research/2026-07-14/coreex-backend-research.md](../../research/2026-07-14/coreex-backend-research.md).
- Applicable instructions: [csharp.instructions.md](../../../.github/instructions/coding-standards/csharp/csharp.instructions.md), [csharp-tests.instructions.md](../../../.github/instructions/coding-standards/csharp/csharp-tests.instructions.md), [markdown.instructions.md](../../../.github/instructions/hve-core/markdown.instructions.md) (resolve via extension `.github/` fallback per hve-core-location).

## Implementation Checklist

### Phase 1: Prerequisites, scaffold & infrastructure
<!-- parallelizable: false -->
- [x] Confirm prerequisites (.NET SDK 10+, Podman/Docker) — details: Phase 1 Step 1 — verified .NET 10.0.301, Podman (machine running); Docker absent (Podman preferred)
- [x] Choose monorepo placement (`backend/` subfolder) and solution name `ApexDynamics.TitanWatch.ResponseSys` — details: Phase 1 Step 2
- [x] Install template pack and AI assets — `CoreEx.Template` already installed (coreex* templates present); `coreex-ai` deferred (optional, writes to repo `.github/`)
- [x] Scaffold solution (`dotnet new coreex`, defaults) — details: Phase 1 Step 4 — created Contracts/Application/Infrastructure + Test.Common/Test.Unit + CodeGen/Database
- [x] Verify build (`dotnet build`) — succeeded, 0 errors, 1 benign scaffold warning. Infrastructure startup (`podman compose up -d`) deferred to Phase 2 (large image pull) — details: Phase 1 Step 5

### Phase 2: Reference data
<!-- parallelizable: false -->
- [x] Define `ThreatLevel`, `LeviathanStatus`, `SignalSeverity` in `tools/*.CodeGen/ref-data.yaml` — three-character uppercase `Code`, sentence-case `Text`, plus color/rank metadata — DB migrations + seed done; DbEx persistence models generated
- [x] Run CodeGen + Database migration to generate/seed ref-data — CoreEx CodeGen generated ref-data contracts/controller/service/repository/mappers; global usings wired; build clean

### Phase 3: Contracts, persistence & mapping
<!-- parallelizable: false -->
- [ ] Author Contracts entities (`Leviathan`, `SignalEvent`, `DispatchUnit`, `LastStandCity`) with `IIdentifier`/`IETag` — details: Phase 3 Step 1
- [ ] Add DB migrations (tables), persistence models, EF DbContext, mappers — details: Phase 3 Step 2
- [ ] Seed initial roster + dispatch assets + cities — details: Phase 3 Step 3

### Phase 4: Application services & validation
<!-- parallelizable: false -->
- [ ] `RosterService` (Get, GetById, GetLive) — details: Phase 4 Step 1
- [ ] `DispatchService` (deploy w/ capacity validation + repel effect) — details: Phase 4 Step 2
- [ ] `AlertService` (raise/stand-down) — details: Phase 4 Step 3
- [ ] `FeedService` (paged query) + validators/policies — details: Phase 4 Step 4

### Phase 5: Server-authoritative simulation engine
<!-- parallelizable: false -->
- [ ] Port motion/HP model (`tickLeviathans`, `deriveAdvance`) to a C# `IHostedService` tick loop — details: Phase 5 Step 1
- [ ] Port Poisson feed generator (`makeSignalEvent`, `scheduleNext`) → persist `SignalEvent` + publish via outbox — details: Phase 5 Step 2

### Phase 6: API host, endpoints, OpenAPI & CORS
<!-- parallelizable: false -->
- [ ] Add API host (`dotnet new coreex-api`) — details: Phase 6 Step 1
- [ ] Controllers: roster, dispatch, alert, feed, ref-data — details: Phase 6 Step 2
- [ ] Configure CORS (Vite origin) + OpenAPI/NSwag — details: Phase 6 Step 3

### Phase 7: Outbox Relay & Subscribe hosts
<!-- parallelizable: true -->
- [ ] Add Outbox Relay host (`dotnet new coreex-relay`) — details: Phase 7 Step 1
- [ ] Add Subscribe host (`dotnet new coreex-subscribe`) + subscriber(s) — details: Phase 7 Step 2

### Phase 8: Front-end integration
<!-- parallelizable: false -->
- [ ] Introduce data-adapter seam in `useCommandState` (`MockAdapter` vs `HttpAdapter`) — details: Phase 8 Step 1
- [ ] Implement REST polling for `/roster` + `/feed`; wire commands to `POST` endpoints — details: Phase 8 Step 2
- [ ] Generate TS types from OpenAPI; align with `types.ts` — details: Phase 8 Step 3

### Phase 9: Testing & validation
<!-- parallelizable: false -->
- [ ] API/unit/intra-domain tests (`coreex-test-api`, `dotnet test`) — details: Phase 9 Step 1
- [ ] Front-end adapter tests + `npm test` / `npm run build` — details: Phase 9 Step 2

## Planning Log Reference

- [.copilot-tracking/plans/logs/2026-07-14/coreex-backend-log.md](../logs/2026-07-14/coreex-backend-log.md)

## Dependencies

- CoreEx skills (from `dotnet new coreex-ai`): `coreex-contract`, `coreex-refdata`, `coreex-db-migration`, `coreex-repository`, `coreex-app-service`, `coreex-validator`, `coreex-policy`, `coreex-api`, `coreex-subscriber`, `coreex-api-e2e`, `coreex-test-api`.
- Tooling: .NET SDK 10+, Podman/Docker, `CoreEx.Template`.
- Infrastructure: PostgreSQL, Redis, Azure Service Bus emulator (via generated `docker-compose.yml`).

## Success Criteria

- `dotnet build` and `dotnet test` pass against local infrastructure.
- API exposes `GET /roster`, `GET /roster/{id}`, `POST /roster/{id}/dispatch`, `POST /alert`, `GET /dispatch`, `GET /feed` (paged), and ref-data endpoints, with OpenAPI docs.
- Signal events persist and publish through the transactional outbox to Service Bus.
- The React client, with `HttpAdapter` selected, renders live roster + feed via polling with no component changes; `npm test` and `npm run build` pass.
