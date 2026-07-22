namespace ApexDynamics.TitanWatch.ResponseSys.Application.Simulation;

/// <summary>Enables the server-authoritative kaiju simulation engine that advances the roster and generates signal-feed events.</summary>
public interface ISimulationService
{
    /// <summary>Executes a single simulation tick: advances every <see cref="Leviathan"/> along its inbound track and generates any <see cref="SignalEvent"/> feed items, persisting the changes and publishing the corresponding domain events transactionally.</summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    Task TickAsync(CancellationToken cancellationToken = default);
}
