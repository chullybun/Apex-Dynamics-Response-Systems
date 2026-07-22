namespace ApexDynamics.TitanWatch.ResponseSys.Application.Repositories;

/// <summary>Enables the <see cref="DispatchUnit"/> read repository.</summary>
public interface IDispatchUnitRepository
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

    /// <summary>Updates the mutable <see cref="DispatchUnit.Available"/> count for the specified <see cref="DispatchUnit"/>.</summary>
    /// <param name="dispatchUnit">The <see cref="DispatchUnit"/> carrying the updated available count.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    Task UpdateAsync(DispatchUnit dispatchUnit, CancellationToken cancellationToken = default);
}
