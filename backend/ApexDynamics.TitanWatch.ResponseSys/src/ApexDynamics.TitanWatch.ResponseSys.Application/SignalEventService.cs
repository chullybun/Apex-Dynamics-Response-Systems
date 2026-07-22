namespace ApexDynamics.TitanWatch.ResponseSys.Application;

/// <summary>Provides the <see cref="ISignalEventService"/> implementation.</summary>
[ScopedService<ISignalEventService>]
public class SignalEventService(ISignalEventRepository repository) : ISignalEventService
{
    private readonly ISignalEventRepository _repository = repository.ThrowIfNull();

    /// <inheritdoc/>
    public Task<ItemsResult<SignalEvent>> GetAllAsync(PagingArgs? paging, CancellationToken cancellationToken = default)
        => _repository.GetAllAsync(paging, cancellationToken);

    /// <inheritdoc/>
    public Task<SignalEvent?> GetAsync(string id, CancellationToken cancellationToken = default)
        => _repository.GetAsync(id, cancellationToken);
}
