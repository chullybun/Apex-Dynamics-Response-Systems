namespace ApexDynamics.TitanWatch.ResponseSys.Application;

/// <summary>Provides the <see cref="IDispatchUnitService"/> implementation.</summary>
[ScopedService<IDispatchUnitService>]
public class DispatchUnitService(IDispatchUnitRepository repository) : IDispatchUnitService
{
    private readonly IDispatchUnitRepository _repository = repository.ThrowIfNull();

    /// <inheritdoc/>
    public Task<DispatchUnitCollection> GetAllAsync(CancellationToken cancellationToken = default)
        => _repository.GetAllAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<DispatchUnit?> GetAsync(string id, CancellationToken cancellationToken = default)
        => _repository.GetAsync(id, cancellationToken);
}
