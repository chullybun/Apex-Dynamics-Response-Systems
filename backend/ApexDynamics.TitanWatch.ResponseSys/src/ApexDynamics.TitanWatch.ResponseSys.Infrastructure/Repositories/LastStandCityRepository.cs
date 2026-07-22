namespace ApexDynamics.TitanWatch.ResponseSys.Infrastructure.Repositories;

/// <summary>Provides the <see cref="Contracts.LastStandCity"/> read repository.</summary>
[ScopedService<ILastStandCityRepository>]
public class LastStandCityRepository(ResponseSysEfDb ef) : ILastStandCityRepository
{
    private readonly ResponseSysEfDb _ef = ef.ThrowIfNull();

    /// <inheritdoc/>
    public Task<Contracts.LastStandCityCollection> GetAllAsync(CancellationToken cancellationToken = default)
        => _ef.Model<Persistence.LastStandCity>().Query()
            .OrderBy(x => x.Name)
            .ToMappedItemsAsync<Persistence.LastStandCity, Contracts.LastStandCityCollection, Contracts.LastStandCity>(LastStandCityMapper.Map, cancellationToken);

    /// <inheritdoc/>
    public async Task<Contracts.LastStandCity?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var items = await _ef.Model<Persistence.LastStandCity>().Query()
            .Where(x => x.Id == id)
            .ToMappedItemsAsync<Persistence.LastStandCity, Contracts.LastStandCityCollection, Contracts.LastStandCity>(LastStandCityMapper.Map, cancellationToken)
            .ConfigureAwait(false);

        return items.FirstOrDefault();
    }
}
