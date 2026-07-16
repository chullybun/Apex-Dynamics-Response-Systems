namespace ApexDynamics.TitanWatch.ResponseSys.Infrastructure.Repositories;

/// <summary>Provides the <see cref="Contracts.DispatchUnit"/> read repository.</summary>
[ScopedService<IDispatchUnitRepository>]
public class DispatchUnitRepository(ResponseSysEfDb ef) : IDispatchUnitRepository
{
    private readonly ResponseSysEfDb _ef = ef.ThrowIfNull();

    /// <inheritdoc/>
    public Task<Contracts.DispatchUnitCollection> GetAllAsync(CancellationToken cancellationToken = default)
        => _ef.Model<Persistence.DispatchUnit>().Query()
            .OrderBy(x => x.Name)
            .ToMappedItemsAsync<Persistence.DispatchUnit, Contracts.DispatchUnitCollection, Contracts.DispatchUnit>(DispatchUnitMapper.Map, cancellationToken);

    /// <inheritdoc/>
    public async Task<Contracts.DispatchUnit?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var items = await _ef.Model<Persistence.DispatchUnit>().Query()
            .Where(x => x.Id == id)
            .ToMappedItemsAsync<Persistence.DispatchUnit, Contracts.DispatchUnitCollection, Contracts.DispatchUnit>(DispatchUnitMapper.Map, cancellationToken)
            .ConfigureAwait(false);

        return items.FirstOrDefault();
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(Contracts.DispatchUnit dispatchUnit, CancellationToken cancellationToken = default)
    {
        dispatchUnit.ThrowIfNull();

        // Read the current persistence model (preserving identity and name), then apply only the mutable available count.
        var model = await _ef.Model<Persistence.DispatchUnit>().GetAsync(dispatchUnit.Id, cancellationToken).ConfigureAwait(false);
        if (model is null)
            return;

        model.Available = dispatchUnit.Available;

        await _ef.Model<Persistence.DispatchUnit>().UpdateAsync(model, cancellationToken).ConfigureAwait(false);
    }
}
