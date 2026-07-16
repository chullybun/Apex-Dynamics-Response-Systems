namespace ApexDynamics.TitanWatch.ResponseSys.Application.Simulation;

/// <summary>Provides the server-authoritative <see cref="ISimulationService"/> implementation.</summary>
/// <remarks>Ports the browser mock (<c>src/mock/leviathans.ts</c> and <c>src/mock/feed.ts</c>) to a persisted, server-driven model: each tick applies bounded organic drift to the
/// roster, advances inbound motion along the <c>from → to</c> track, wraps on landfall (emitting a <c>CRT</c> signal), and generates ambient signal-feed events. All database writes and
/// the corresponding outbox event publications are committed atomically via the <see cref="IUnitOfWork"/>.</remarks>
[ScopedService<ISimulationService>]
public class SimulationService(IUnitOfWork unitOfWork, ILeviathanRepository leviathanRepository, ISignalEventRepository signalEventRepository, CoreEx.ExecutionContext executionContext, IRandomSource random) : ISimulationService
{
    #region Ported simulation constants

    /// <summary>Expected signal-feed arrivals per tick (~24/min ≈ 0.4/sec); see <c>scheduleNext</c> in the mock.</summary>
    private const double _feedLambdaPerTick = 24.0 / 60.0;

    #endregion

    #region Ported phrase pools (feed.ts)

    private static readonly string[] _sensors = ["COASTAL ARRAY", "ORBITAL UPLINK", "SEISMIC GRID", "SONAR PICKET", "THERMAL DRONE", "CIVIL DEFENSE NET"];
    private static readonly string[] _sectors = ["ALPHA", "BRAVO", "CHARLIE", "DELTA", "ECHO", "FOXTROT"];

    private static readonly string[] _warnClauses =
    [
        "energy signature spiking past safe thresholds",
        "hull-displacement wake widening fast",
        "bioelectric surge scrambling nearby sensors",
        "dive profile flattening toward an attack run",
        "thermal bloom consistent with an imminent surface",
        "harmonic roar registering across the seismic grid"
    ];

    private static readonly string[] _linkedOpsClauses =
    [
        "interception lattice realigning to its heading",
        "shore batteries walking fire onto its track",
        "mech wing vectoring for a flanking push",
        "barrier grid charging along the projected path"
    ];

    private static readonly string[] _sectorOpsClauses =
    [
        "Mobilizing the rapid-response wing",
        "Hardening the seawall cordon",
        "Staging evacuation corridors",
        "Routing reserve power to the barrier grid"
    ];

    #endregion

    private readonly IUnitOfWork _unitOfWork = unitOfWork.ThrowIfNull();
    private readonly ILeviathanRepository _leviathanRepository = leviathanRepository.ThrowIfNull();
    private readonly ISignalEventRepository _signalEventRepository = signalEventRepository.ThrowIfNull();
    private readonly CoreEx.ExecutionContext _executionContext = executionContext.ThrowIfNull();
    private readonly IRandomSource _random = random.ThrowIfNull();

    /// <inheritdoc/>
    public async Task TickAsync(CancellationToken cancellationToken = default)
    {
        // Consistent per-tick UTC timestamp (epoch milliseconds) for event and change stamping.
        var timestampMs = _executionContext.Timestamp.ToUnixTimeMilliseconds();

        // Load the current roster (read outside the transaction; the mutation + event publish are committed atomically below).
        var roster = await _leviathanRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);

        await _unitOfWork.TransactionAsync(async ct =>
        {
            // 1. Advance the roster: drift, inbound motion, landfall wrap.
            foreach (var leviathan in roster)
            {
                var reachedLandfall = LeviathanMotion.Advance(leviathan, _random);
                await _leviathanRepository.UpdateAsync(leviathan, ct).ConfigureAwait(false);

                if (reachedLandfall)
                    await PublishSignalAsync(BuildLandfallEvent(leviathan, timestampMs), ct).ConfigureAwait(false);
            }

            // 2. Generate ambient signal-feed events (~0.4/sec).
            var count = PoissonSample(_feedLambdaPerTick);
            for (var i = 0; i < count; i++)
                await PublishSignalAsync(BuildSignalEvent(roster, timestampMs), ct).ConfigureAwait(false);

            return true;
        }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Builds the <c>CRT</c> landfall signal event for the supplied leviathan (ported from <c>reportLandfall</c>).</summary>
    private static SignalEvent BuildLandfallEvent(Leviathan leviathan, long timestampMs) => new()
    {
        SeverityCode = "CRT",
        Message = $"{leviathan.Codename} reached landfall at {leviathan.Target} — repelled. Track reacquired.",
        Timestamp = timestampMs,
        LeviathanId = leviathan.Id
    };

    /// <summary>Builds a single ambient signal-feed event with weighted severity and optional leviathan link (ported from <c>makeSignalEvent</c>).</summary>
    private static SignalEvent BuildSignalEvent(LeviathanCollection roster, long timestampMs)
    {
        // Weight toward INFO with occasional WARN/OPS for texture.
        var roll = Random.Shared.NextDouble();
        var severityCode = roll < 0.6 ? "INF" : roll < 0.82 ? "WRN" : "OPS";

        var linkChance = severityCode == "WRN" ? 0.7 : 0.35;
        var leviathan = roster.Count > 0 && Random.Shared.NextDouble() < linkChance ? Pick(roster) : null;

        return new SignalEvent
        {
            SeverityCode = severityCode,
            Message = MessageFor(severityCode, leviathan),
            Timestamp = timestampMs,
            LeviathanId = leviathan?.Id
        };
    }

    /// <summary>Composes the human-readable message for a severity/leviathan combination (ported from <c>messageFor</c>).</summary>
    private static string MessageFor(string severityCode, Leviathan? leviathan)
    {
        var sensor = Pick(_sensors);
        var sector = Pick(_sectors);

        if (leviathan is not null)
        {
            return severityCode switch
            {
                "WRN" => $"{sensor}: {leviathan.Codename} tracking toward Sector {sector} — {Pick(_warnClauses)}",
                "OPS" => $"Dispatch update on {leviathan.Codename}: {Pick(_linkedOpsClauses)}",
                _ => $"{sensor} contact: {leviathan.Codename} signature stable in Sector {sector}."
            };
        }

        return severityCode switch
        {
            "WRN" => $"{sensor}: anomalous reading in Sector {sector} — {Pick(_warnClauses)}",
            "OPS" => $"Operations: {Pick(_sectorOpsClauses)} for Sector {sector}.",
            _ => $"{sensor} nominal across Sector {sector}."
        };
    }

    /// <summary>Assigns the identity, persists the <see cref="SignalEvent"/>, and enqueues its <c>Created</c> outbox event within the current transaction.</summary>
    private async Task PublishSignalAsync(SignalEvent signalEvent, CancellationToken cancellationToken)
    {
        signalEvent.Id = Runtime.NewId();   // service-assigned identity — deterministic and present before publish.

        var created = await _signalEventRepository.CreateAsync(signalEvent, cancellationToken).ConfigureAwait(false);

        // Publish the domain event to the destination topic; the CloudEvents subject is derived automatically by the IEventFormatter from the SignalEvent type.
        _unitOfWork.Events.Add("titanwatch.signal-event", EventData.CreateEventWith(created, EventAction.Created));
    }

    /// <summary>Draws a Poisson-distributed count for the supplied rate (Knuth's algorithm) for organic feed arrivals.</summary>
    private static int PoissonSample(double lambda)
    {
        var limit = Math.Exp(-lambda);
        var product = 1.0;
        var k = 0;

        do
        {
            k++;
            product *= Random.Shared.NextDouble();
        }
        while (product > limit);

        return k - 1;
    }

    /// <summary>Picks a pseudo-random element from the supplied list.</summary>
    private static T Pick<T>(IReadOnlyList<T> items) => items[Random.Shared.Next(items.Count)];
}
