namespace ApexDynamics.TitanWatch.ResponseSys.Application.Repositories;

/// <summary>Enables the <see cref="SignalEvent"/> read repository.</summary>
public interface ISignalEventRepository
{
    /// <summary>Gets a paged, most-recent-first collection of <see cref="SignalEvent"/> items.</summary>
    /// <param name="paging">The optional <see cref="PagingArgs"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The paged <see cref="SignalEvent"/> collection result.</returns>
    Task<ItemsResult<SignalEvent>> GetAllAsync(PagingArgs? paging, CancellationToken cancellationToken = default);

    /// <summary>Gets the specified <see cref="SignalEvent"/>.</summary>
    /// <param name="id">The <see cref="SignalEvent.Id"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="SignalEvent"/> where found; otherwise, <c>null</c>.</returns>
    Task<SignalEvent?> GetAsync(string id, CancellationToken cancellationToken = default);
}
