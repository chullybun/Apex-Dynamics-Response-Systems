namespace ApexDynamics.TitanWatch.ResponseSys.Api.Services;

/// <summary>Server-authoritative simulation driver: on each timer tick it advances the roster and generates signal-feed events via the Application-layer <see cref="ISimulationService"/>.</summary>
/// <remarks>Extends <see cref="TimerHostedServiceBase"/>, which wraps every <see cref="OnExecuteAsync"/> invocation in a fresh DI scope with a scoped <see cref="CoreEx.ExecutionContext"/>.
/// Pause/resume, health reporting, first-interval jitter, and unhandled-exception handling are provided by the base and surfaced via <c>MapHostedServices()</c>.</remarks>
public class SimulationHostedService : TimerHostedServiceBase
{
    /// <summary>Initializes a new instance of the <see cref="SimulationHostedService"/> class.</summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/>.</param>
    /// <param name="logger">The <see cref="ILogger{TCategoryName}"/>.</param>
    public SimulationHostedService(IServiceProvider serviceProvider, ILogger<SimulationHostedService> logger) : base(serviceProvider, logger)
    {
        ServiceName = "Simulation";
        Interval = TimeSpan.FromMilliseconds(1000);   // one server-authoritative tick per second (config-overridable via CoreEx:Host:Services:Simulation:Interval).
    }

    /// <inheritdoc/>
    protected override async Task<bool> OnExecuteAsync(CoreEx.ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        // Resolve the scoped simulation service from the per-tick scope and run a single tick.
        var service = executionContext.ServiceProvider!.GetRequiredService<ISimulationService>();
        await service.TickAsync(cancellationToken).ConfigureAwait(false);

        return false;   // wait the configured Interval before the next tick.
    }
}
