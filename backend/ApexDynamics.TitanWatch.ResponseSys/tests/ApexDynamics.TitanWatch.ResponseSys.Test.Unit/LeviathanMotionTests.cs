namespace ApexDynamics.TitanWatch.ResponseSys.Test.Unit;

/// <summary>Unit tests for <see cref="LeviathanMotion"/> — the pure per-tick motion/HP model extracted from <see cref="SimulationService"/> so its invariants are verifiable without a
/// database, transaction, or outbox.</summary>
[TestFixture]
public class LeviathanMotionTests
{
    [Test]
    public void GivenHpNearFloor_Advance_ClampsHpToZero()
    {
        // Arrange — decay draw (28, the maximum) exceeds the remaining Hp (10).
        var leviathan = CreateLeviathan(hp: 10, hpMax: 100, speed: 0, repel: 0, range: 500, startRange: 500);
        var random = new FixedRandomSource(nextInt: 28, nextDouble: 0.5);

        // Act
        LeviathanMotion.Advance(leviathan, random);

        // Assert
        leviathan.Hp.Should().Be(0);
    }

    [Test]
    public void GivenSpeedNearUpperBound_Advance_ClampsSpeedTo120()
    {
        // Arrange — speed delta formula is (NextDouble * 8.0) - 4.0; NextDouble=1.0 yields +4.0, pushing 118 over the 120 ceiling.
        var leviathan = CreateLeviathan(hp: 50, hpMax: 100, speed: 118, repel: 0, range: 500, startRange: 500);
        var random = new FixedRandomSource(nextInt: 1, nextDouble: 1.0);

        // Act
        LeviathanMotion.Advance(leviathan, random);

        // Assert
        leviathan.Speed.Should().Be(120);
    }

    [Test]
    public void GivenSpeedNearLowerBound_Advance_ClampsSpeedToZero()
    {
        // Arrange — NextDouble=0.0 yields a -4.0 delta, pushing 2 below the 0 floor.
        var leviathan = CreateLeviathan(hp: 50, hpMax: 100, speed: 2, repel: 0, range: 500, startRange: 500);
        var random = new FixedRandomSource(nextInt: 1, nextDouble: 0.0);

        // Act
        LeviathanMotion.Advance(leviathan, random);

        // Assert
        leviathan.Speed.Should().Be(0);
    }

    [Test]
    public void GivenRepelNearFloor_Advance_ClampsRepelToZero()
    {
        // Arrange — decay-per-tick (0.04) exceeds the remaining repel (0.01).
        var leviathan = CreateLeviathan(hp: 50, hpMax: 100, speed: 0, repel: 0.01, range: 500, startRange: 500);
        var random = new FixedRandomSource(nextInt: 1, nextDouble: 0.5);

        // Act
        LeviathanMotion.Advance(leviathan, random);

        // Assert
        leviathan.Repel.Should().Be(0);
    }

    [Test]
    public void GivenRangeAboveZeroAfterAdvance_Advance_ReturnsFalseAndDoesNotWrap()
    {
        // Arrange — speed=0 so the tick's own advance is zero; range stays well above the landfall threshold.
        var leviathan = CreateLeviathan(hp: 50, hpMax: 100, speed: 0, repel: 0, range: 500, startRange: 500);
        var random = new FixedRandomSource(nextInt: 1, nextDouble: 0.5);

        // Act
        var reachedLandfall = LeviathanMotion.Advance(leviathan, random);

        // Assert
        reachedLandfall.Should().BeFalse();
    }

    [Test]
    public void GivenRangeAtOrBelowZeroAfterAdvance_Advance_WrapsToStartRangeAndReturnsTrue()
    {
        // Arrange — speed=0 keeps the tick's own advance at zero, so the seeded Range (already at 0) stays at 0 → range <= 0 triggers the wrap.
        var leviathan = CreateLeviathan(hp: 50, hpMax: 100, speed: 0, repel: 0, range: 0, startRange: 500);
        var random = new FixedRandomSource(nextInt: 1, nextDouble: 0.5);

        // Act
        var reachedLandfall = LeviathanMotion.Advance(leviathan, random);

        // Assert
        reachedLandfall.Should().BeTrue();
        leviathan.Range.Should().Be(500);
    }

    [TestCase(24.0, "LND")]
    [TestCase(69.9, "SUR")]
    [TestCase(70.0, "INB")]
    public void GivenRemainingRange_StatusCodeFromRange_ReturnsExpectedBand(double rangeKm, string expected) => LeviathanMotion.StatusCodeFromRange(rangeKm).Should().Be(expected);

    [Test]
    public void GivenTrackHalfCovered_Advance_LerpsPositionToMidpoint()
    {
        // Arrange — StartRange=100, Range=50 (before this tick) with speed=0 so the advance leaves Range at 50 → frac=0.5 across a 0..10 track.
        var leviathan = CreateLeviathan(hp: 50, hpMax: 100, speed: 0, repel: 0, range: 50, startRange: 100);
        leviathan.FromLng = 0; leviathan.FromLat = 0; leviathan.ToLng = 10; leviathan.ToLat = 10;
        var random = new FixedRandomSource(nextInt: 1, nextDouble: 0.5);

        // Act
        LeviathanMotion.Advance(leviathan, random);

        // Assert
        leviathan.Lng.Should().Be(5);
        leviathan.Lat.Should().Be(5);
    }

    /// <summary>Builds a minimal <see cref="Leviathan"/> fixture for motion tests.</summary>
    private static Leviathan CreateLeviathan(int hp, int hpMax, double speed, double repel, double range, int startRange) => new()
    {
        Id = "lev-test",
        Hp = hp,
        HpMax = hpMax,
        Speed = speed,
        Repel = repel,
        Range = range,
        StartRange = startRange,
        FromLng = 0,
        FromLat = 0,
        ToLng = 0,
        ToLat = 0
    };

    /// <summary>A deterministic <see cref="IRandomSource"/> fake that returns the same configured values on every call.</summary>
    private sealed class FixedRandomSource(int nextInt, double nextDouble) : IRandomSource
    {
        public int Next(int minValue, int maxValue) => nextInt;

        public double NextDouble() => nextDouble;
    }
}
