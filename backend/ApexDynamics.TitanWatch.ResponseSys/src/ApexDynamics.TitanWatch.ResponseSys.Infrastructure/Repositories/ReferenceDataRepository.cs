namespace ApexDynamics.TitanWatch.ResponseSys.Infrastructure.Repositories;

/// <summary>Provides the <see cref="ReferenceDataRepository"/> implementation (see generated <c>ReferenceDataRepository.g.cs</c> for entity-specific providers).</summary>
public partial class ReferenceDataRepository(ResponseSysEfDb ef)
{
    private readonly ResponseSysEfDb _ef = ef.ThrowIfNull();
}