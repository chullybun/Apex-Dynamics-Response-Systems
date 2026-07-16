namespace ApexDynamics.TitanWatch.ResponseSys.Application.Simulation;

/// <summary>Provides the pure, side-effect-free motion/HP model for a single simulation tick, extracted from <see cref="SimulationService"/> so its invariants (clamping, landfall wrap, status-band
/// derivation) are directly unit-testable without a database, transaction, or outbox. Ported from the browser mock's <c>tickLeviathans</c>/<c>statusFromRange</c> (<c>src/mock/leviathans.ts</c>).</summary>
public static class LeviathanMotion
{
    /// <summary>Fraction of a track's length covered per (km/h · sim-second); see <c>CLOSURE_RATE</c> in the mock.</summary>
    public const double ClosureRate = 0.05;

    /// <summary>Remaining range (km) below which a leviathan is at landfall; see <c>LANDFALL_RANGE_KM</c>.</summary>
    public const double LandfallRangeKm = 25;

    /// <summary>Remaining range (km) below which a leviathan has surfaced; see <c>SURFACED_RANGE_KM</c>.</summary>
    public const double SurfacedRangeKm = 70;

    /// <summary>Dispatch knockback (track fraction) shed each 1-second tick; see <c>REPEL_DECAY_PER_TICK</c>.</summary>
    public const double RepelDecayPerTick = 0.04;

    /// <summary>The fixed simulation delta applied each tick (seconds).</summary>
    public const double TickSeconds = 1.0;

    /// <summary>Advances a single <see cref="Leviathan"/> one tick in place, returning <c>true</c> when it reached landfall (range wrapped).</summary>
    /// <param name="leviathan">The <see cref="Leviathan"/> to mutate in place.</param>
    /// <param name="random">The <see cref="IRandomSource"/> supplying the tick's HP-decay and speed-drift randomness.</param>
    public static bool Advance(Leviathan leviathan, IRandomSource random)
    {
        random.ThrowIfNull();
        var hpMax = leviathan.ThrowIfNull().HpMax ?? 0;

        // Bounded organic drift (ported from tickLeviathans): HP countdown, speed random-walk, repel decay.
        leviathan.Hp = Clamp((leviathan.Hp ?? 0) - random.Next(1, 29), 0, hpMax);
        var speed = Clamp((leviathan.Speed ?? 0) + ((random.NextDouble() * 8.0) - 4.0), 0, 120);
        leviathan.Speed = Math.Round(speed, 1);
        var repel = Math.Max(0, Math.Round((leviathan.Repel ?? 0) - RepelDecayPerTick, 2));
        leviathan.Repel = repel;

        // Inbound motion: decrement remaining range by speed·CLOSURE_RATE·dt, reduced/held while repelled (frac = max(0, fracAt - repel) intent).
        var startRange = (double)(leviathan.StartRange ?? 0);
        var range = leviathan.Range ?? startRange;
        var advance = speed * ClosureRate * TickSeconds * (1 - Math.Min(repel, 1));
        range -= advance;

        var reachedLandfall = false;
        if (range <= 0)
        {
            range = startRange;   // wrap back to spawn — a completed landfall cycle
            reachedLandfall = true;
        }

        // Recompute live position (straight-line lerp) and status band from the remaining range.
        var frac = startRange > 0 ? Math.Clamp(1 - (range / startRange), 0, 1) : 0;
        leviathan.Range = Math.Round(range, 1);
        leviathan.Lng = (leviathan.FromLng ?? 0) + (((leviathan.ToLng ?? 0) - (leviathan.FromLng ?? 0)) * frac);
        leviathan.Lat = (leviathan.FromLat ?? 0) + (((leviathan.ToLat ?? 0) - (leviathan.FromLat ?? 0)) * frac);
        leviathan.StatusCode = StatusCodeFromRange(range);

        return reachedLandfall;
    }

    /// <summary>Returns the reference-data status code for a remaining range (bands ported from <c>statusFromRange</c>).</summary>
    public static string StatusCodeFromRange(double rangeKm) => rangeKm < LandfallRangeKm ? "LND" : rangeKm < SurfacedRangeKm ? "SUR" : "INB";

    /// <summary>Clamps a value into the inclusive <paramref name="min"/>..<paramref name="max"/> range.</summary>
    private static int Clamp(int value, int min, int max) => Math.Max(min, Math.Min(max, value));

    /// <summary>Clamps a value into the inclusive <paramref name="min"/>..<paramref name="max"/> range.</summary>
    private static double Clamp(double value, double min, double max) => Math.Max(min, Math.Min(max, value));
}
