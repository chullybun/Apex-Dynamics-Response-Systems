namespace ApexDynamics.TitanWatch.ResponseSys.Infrastructure.Repositories;

/// <summary>Provides the <see cref="Contracts.Leviathan"/> read repository.</summary>
[ScopedService<ILeviathanRepository>]
public class LeviathanRepository(ResponseSysEfDb ef) : ILeviathanRepository
{
    private readonly ResponseSysEfDb _ef = ef.ThrowIfNull();

    /// <inheritdoc/>
    public Task<Contracts.LeviathanCollection> GetAllAsync(CancellationToken cancellationToken = default)
        => _ef.Model<Persistence.Leviathan>().Query()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Codename)
            .ToMappedItemsAsync<Persistence.Leviathan, Contracts.LeviathanCollection, Contracts.Leviathan>(LeviathanMapper.Map, cancellationToken);

    /// <inheritdoc/>
    public async Task<Contracts.Leviathan?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var items = await _ef.Model<Persistence.Leviathan>().Query()
            .Where(x => x.Id == id && !x.IsDeleted)
            .ToMappedItemsAsync<Persistence.Leviathan, Contracts.LeviathanCollection, Contracts.Leviathan>(LeviathanMapper.Map, cancellationToken)
            .ConfigureAwait(false);

        return items.FirstOrDefault();
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(Contracts.Leviathan leviathan, CancellationToken cancellationToken = default)
    {
        leviathan.ThrowIfNull();

        // Read the current persistence model (preserving identity, track geometry, and change-log columns), then apply only the simulation-mutated live state.
        var model = await _ef.Model<Persistence.Leviathan>().GetAsync(leviathan.Id, cancellationToken).ConfigureAwait(false);
        if (model is null)
            return;

        model.Hp = leviathan.Hp;
        model.Speed = leviathan.Speed;
        model.Repel = leviathan.Repel;
        model.Range = leviathan.Range;
        model.Lng = leviathan.Lng;
        model.Lat = leviathan.Lat;
        model.StatusCode = leviathan.StatusCode;

        await _ef.Model<Persistence.Leviathan>().UpdateAsync(model, cancellationToken).ConfigureAwait(false);
    }
}
