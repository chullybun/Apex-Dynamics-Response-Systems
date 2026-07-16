namespace ApexDynamics.TitanWatch.ResponseSys.Application.Simulation;

/// <summary>Provides the default <see cref="IRandomSource"/>, backed by <see cref="Random.Shared"/>.</summary>
[ScopedService<IRandomSource>]
public sealed class RandomSource : IRandomSource
{
    /// <inheritdoc/>
    public int Next(int minValue, int maxValue) => Random.Shared.Next(minValue, maxValue);

    /// <inheritdoc/>
    public double NextDouble() => Random.Shared.NextDouble();
}
