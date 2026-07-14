namespace ApexDynamics.TitanWatch.ResponseSys.Application;

/// <summary>Provides the <see cref="ILastStandCityService"/> implementation.</summary>
[ScopedService<ILastStandCityService>]
public class LastStandCityService(ILastStandCityRepository repository) : ILastStandCityService
{
    private readonly ILastStandCityRepository _repository = repository.ThrowIfNull();

    /// <inheritdoc/>
    public Task<LastStandCityCollection> GetAllAsync(CancellationToken cancellationToken = default)
        => _repository.GetAllAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<LastStandCity?> GetAsync(string id, CancellationToken cancellationToken = default)
        => _repository.GetAsync(id, cancellationToken);
}
