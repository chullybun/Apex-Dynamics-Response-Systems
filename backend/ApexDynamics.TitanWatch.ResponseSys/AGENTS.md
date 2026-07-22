# ApexDynamics.TitanWatch.ResponseSys -- CoreEx Application Services

This represents the **CoreEx domain-based application services** for the `ResponseSys` domain.

**AI assistance:** To install CoreEx AI workflow assets (instructions, prompts, agents) for this solution, run `dotnet new coreex-ai` at the **repo root**:

```bash
# Single-repo (most common):
dotnet new coreex-ai

# Monorepo (CoreEx under a subfolder):
dotnet new coreex-ai --app-folder <relative-path-from-root>
```

Once installed, run `/coreex-scaffold` to add missing hosts or `/coreex-expert` for architecture guidance.

> After bumping the CoreEx NuGet version in `Directory.Packages.props`:
> - Re-run `dotnet new coreex-ai --force` to update instruction and prompt files to the new version.
> - Run `/coreex-docs-sync` to refresh the local `.github/docs/coreex/` cache.

---

## Project Structure

```
ApexDynamics.TitanWatch.ResponseSys/
+-- src/
|   +-- ApexDynamics.TitanWatch.ResponseSys.Contracts/        # Public contracts: entities, DTOs, event schemas
|   +-- ApexDynamics.TitanWatch.ResponseSys.Application/      # Business logic: services, validators, repository interfaces
|   +-- ApexDynamics.TitanWatch.ResponseSys.Domain/           # (coreex-domain addon, optional) Aggregates, value objects, domain events
|   +-- ApexDynamics.TitanWatch.ResponseSys.Infrastructure/   # EF Core repositories, outbox, external integrations
+-- tools/
|   +-- ApexDynamics.TitanWatch.ResponseSys.Database/         # (data-provider != None) Database migrations (DbEx)
|   +-- ApexDynamics.TitanWatch.ResponseSys.CodeGen/          # (refdata-enabled && data-provider != None) Ref-data code gen
+-- tests/
|   +-- ApexDynamics.TitanWatch.ResponseSys.Test.Common/      # Shared test infrastructure: TestData marker, embedded seed data
|   +-- ApexDynamics.TitanWatch.ResponseSys.Test.Unit/        # Fast isolated unit tests (validators, services, no I/O)
+-- Directory.Packages.props       # Central NuGet version management (no versions in .csproj)
```

---

## Feature Configuration

- **Data provider:** PostgreSQL (`CoreEx.Database.Postgres`, `CoreEx.EntityFrameworkCore`)
- **Reference data:** Enabled -- `src/ApexDynamics.TitanWatch.ResponseSys.Application/ReferenceDataService.cs` and `tools/ApexDynamics.TitanWatch.ResponseSys.CodeGen/`
- **Domain project:** Optional -- add `dotnet new coreex-domain -n ApexDynamics.TitanWatch.ResponseSys` when domain complexity warrants DDD
- **Railway-Oriented Programming:** Disabled -- standard exception-based error handling
- **Transactional outbox:** Enabled -- events committed atomically with data via the outbox table
- **Messaging:** Azure Service Bus (`CoreEx.Azure.Messaging.ServiceBus`)

---

## Relevant Docs

`dotnet new coreex-ai` installs the following under `.github/docs/coreex/` (refresh later with `/coreex-docs-sync`):

- `.github/docs/coreex/layers.md` -- full layered architecture and dependency rules
- `.github/docs/coreex/patterns.md` -- CoreEx request/response and event patterns
- `.github/docs/coreex/application-scaffolding-guide.md` -- choosing the smallest safe CoreEx solution shape before adding code
- `.github/docs/coreex/contracts-layer.md` -- entities, DTOs, event schemas
- `.github/docs/coreex/application-layer.md` -- services, validators, repository interfaces
- `.github/docs/coreex/infrastructure-layer.md` -- EF Core, outbox, external integrations
- `.github/docs/coreex/testing.md` -- test project setup, `WithGenericTester`, `WithApiTester`
- `.github/docs/coreex/local-dev.md` -- running locally with .NET Aspire
- `.github/docs/coreex/tooling.md` -- Database and CodeGen tool projects

