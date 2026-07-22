namespace ApexDynamics.TitanWatch.ResponseSys.Application;

/// <summary>Provides the <see cref="ILeviathanService"/> implementation.</summary>
[ScopedService<ILeviathanService>]
public class LeviathanService(ILeviathanRepository repository) : ILeviathanService
{
    private readonly ILeviathanRepository _repository = repository.ThrowIfNull();

    /// <inheritdoc/>
    public Task<LeviathanCollection> GetAllAsync(CancellationToken cancellationToken = default)
        => _repository.GetAllAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<Leviathan?> GetAsync(string id, CancellationToken cancellationToken = default)
        => _repository.GetAsync(id, cancellationToken);
}
