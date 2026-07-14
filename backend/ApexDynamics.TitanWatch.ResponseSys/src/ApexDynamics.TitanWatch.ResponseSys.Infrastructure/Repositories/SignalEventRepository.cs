namespace ApexDynamics.TitanWatch.ResponseSys.Infrastructure.Repositories;

/// <summary>Provides the <see cref="Contracts.SignalEvent"/> read repository.</summary>
[ScopedService<ISignalEventRepository>]
public class SignalEventRepository(ResponseSysEfDb ef) : ISignalEventRepository
{
    private readonly ResponseSysEfDb _ef = ef.ThrowIfNull();

    /// <inheritdoc/>
    public Task<ItemsResult<Contracts.SignalEvent>> GetAllAsync(PagingArgs? paging, CancellationToken cancellationToken = default)
        => _ef.Model<Persistence.SignalEvent>().Query()
            .OrderByDescending(x => x.Timestamp)
            .ToMappedItemsResultAsync<Persistence.SignalEvent, Contracts.SignalEvent>(SignalEventMapper.Map, paging, cancellationToken: cancellationToken);

    /// <inheritdoc/>
    public async Task<Contracts.SignalEvent?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var items = await _ef.Model<Persistence.SignalEvent>().Query()
            .Where(x => x.Id == id)
            .ToMappedItemsAsync<Persistence.SignalEvent, Contracts.SignalEventCollection, Contracts.SignalEvent>(SignalEventMapper.Map, cancellationToken)
            .ConfigureAwait(false);

        return items.FirstOrDefault();
    }
}
