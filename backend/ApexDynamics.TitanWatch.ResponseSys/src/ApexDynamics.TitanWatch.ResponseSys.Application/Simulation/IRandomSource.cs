namespace ApexDynamics.TitanWatch.ResponseSys.Application.Simulation;

/// <summary>Provides an injectable source of randomness so simulation logic can be exercised deterministically in unit tests.</summary>
public interface IRandomSource
{
    /// <summary>Returns a random integer greater than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/>.</summary>
    int Next(int minValue, int maxValue);

    /// <summary>Returns a random double greater than or equal to <c>0.0</c> and less than <c>1.0</c>.</summary>
    double NextDouble();
}
