namespace ApexDynamics.TitanWatch.ResponseSys.Application;

/// <summary>Enables the <see cref="LastStandCity"/> read service.</summary>
public interface ILastStandCityService
{
    /// <summary>Gets all <see cref="LastStandCity"/> items.</summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="LastStandCityCollection"/>.</returns>
    Task<LastStandCityCollection> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets the specified <see cref="LastStandCity"/>.</summary>
    /// <param name="id">The <see cref="LastStandCity.Id"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="LastStandCity"/> where found; otherwise, <c>null</c>.</returns>
    Task<LastStandCity?> GetAsync(string id, CancellationToken cancellationToken = default);
}
