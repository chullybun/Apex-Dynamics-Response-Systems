namespace ApexDynamics.TitanWatch.ResponseSys.Application;

/// <summary>Enables the <see cref="Leviathan"/> read service.</summary>
public interface ILeviathanService
{
    /// <summary>Gets the full <see cref="Leviathan"/> roster.</summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="LeviathanCollection"/>.</returns>
    Task<LeviathanCollection> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets the specified <see cref="Leviathan"/>.</summary>
    /// <param name="id">The <see cref="Leviathan.Id"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Leviathan"/> where found; otherwise, <c>null</c>.</returns>
    Task<Leviathan?> GetAsync(string id, CancellationToken cancellationToken = default);
}
