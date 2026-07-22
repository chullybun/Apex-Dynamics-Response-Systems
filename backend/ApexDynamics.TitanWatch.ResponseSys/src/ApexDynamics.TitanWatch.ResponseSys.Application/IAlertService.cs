namespace ApexDynamics.TitanWatch.ResponseSys.Application;

/// <summary>Enables the citywide alert (command) service.</summary>
public interface IAlertService
{
    /// <summary>Raises or stands down the citywide alert.</summary>
    /// <param name="request">The <see cref="AlertRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The published <see cref="SignalEvent"/>.</returns>
    Task<SignalEvent> RaiseAsync(AlertRequest request, CancellationToken cancellationToken = default);
}
