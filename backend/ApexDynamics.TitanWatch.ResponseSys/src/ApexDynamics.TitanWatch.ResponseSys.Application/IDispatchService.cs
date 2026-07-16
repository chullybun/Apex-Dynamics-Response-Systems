namespace ApexDynamics.TitanWatch.ResponseSys.Application;

/// <summary>Enables the <c>DispatchUnit</c> deployment (command) service.</summary>
public interface IDispatchService
{
    /// <summary>Deploys the requested <see cref="DispatchUnit"/> against the specified <see cref="Leviathan"/>.</summary>
    /// <param name="leviathanId">The targeted <see cref="Leviathan.Id"/>.</param>
    /// <param name="request">The <see cref="DispatchRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The updated <see cref="DispatchUnit"/>.</returns>
    Task<DispatchUnit> DeployAsync(string leviathanId, DispatchRequest request, CancellationToken cancellationToken = default);
}
