namespace ApexDynamics.TitanWatch.ResponseSys.Application;

/// <summary>Enables the <see cref="DispatchUnit"/> read service.</summary>
public interface IDispatchUnitService
{
    /// <summary>Gets all <see cref="DispatchUnit"/> items.</summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="DispatchUnitCollection"/>.</returns>
    Task<DispatchUnitCollection> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets the specified <see cref="DispatchUnit"/>.</summary>
    /// <param name="id">The <see cref="DispatchUnit.Id"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="DispatchUnit"/> where found; otherwise, <c>null</c>.</returns>
    Task<DispatchUnit?> GetAsync(string id, CancellationToken cancellationToken = default);
}
