namespace ApexDynamics.TitanWatch.ResponseSys.Application.Repositories;

/// <summary>Enables the <see cref="Leviathan"/> read repository.</summary>
public interface ILeviathanRepository
{
    /// <summary>Gets all (non-deleted) <see cref="Leviathan"/> items.</summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="LeviathanCollection"/>.</returns>
    Task<LeviathanCollection> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets the specified <see cref="Leviathan"/>.</summary>
    /// <param name="id">The <see cref="Leviathan.Id"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Leviathan"/> where found; otherwise, <c>null</c>.</returns>
    Task<Leviathan?> GetAsync(string id, CancellationToken cancellationToken = default);
}
