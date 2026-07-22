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

    /// <inheritdoc/>
    public async Task<Contracts.SignalEvent> CreateAsync(Contracts.SignalEvent signalEvent, CancellationToken cancellationToken = default)
    {
        signalEvent.ThrowIfNull();

        var model = new Persistence.SignalEvent
        {
            Id = signalEvent.Id,
            SeverityCode = signalEvent.SeverityCode,
            Message = signalEvent.Message,
            Timestamp = signalEvent.Timestamp,
            LeviathanId = signalEvent.LeviathanId
        };

        var result = await _ef.Model<Persistence.SignalEvent>().CreateAsync(model, cancellationToken).ConfigureAwait(false);
        return SignalEventMapper.Map(result.Value);
    }
}
