namespace ApexDynamics.TitanWatch.ResponseSys.Test.Api;

/// <summary>Integration tests for the read-only <c>DispatchUnit</c> endpoints (<c>GET /dispatch</c>, <c>GET /dispatch/{id}</c>).</summary>
public partial class DispatchReadTests : WithApiTester<ApexDynamics.TitanWatch.ResponseSys.Api.Program>
{
    [OneTimeSetUp]
    public async Task OneTimeSetUpAsync()
    {
        await Test.MigratePostgresDataAsync<TestData>(["read-data.seed.yaml"], DbMigration.ConfigureMigrationArgs).ConfigureAwait(false);
        await Test.ClearFusionCacheAsync().ConfigureAwait(false);
    }

    [Test]
    public void Dispatch_GetAll_ReturnsAllOrderedByName()
    {
        // Asserts the production masters dispatch units, which the test migrate seeds.
        var units = Test.Http<DispatchUnit[]>()
            .Run(HttpMethod.Get, "/dispatch")
            .AssertOK()
            .Value!;

        units.Should().HaveCount(4);
        units.Select(u => u.Name).Should().ContainInOrder("Deploy Mechs", "Evac Sector", "Raise Barrier", "Scramble Jets");
    }

    [Test]
    public void Dispatch_Get_Found()
    {
        var unit = Test.Http<DispatchUnit>()
            .Run(HttpMethod.Get, "/dispatch/scramble-jets")
            .AssertOK()
            .Value!;

        unit.Id.Should().Be("scramble-jets");
        unit.Name.Should().Be("Scramble Jets");
        unit.Available.Should().Be(6);
        unit.Capacity.Should().Be(6);
    }

    [Test]
    public void Dispatch_Get_NotFound()
        => Test.Http()
            .Run(HttpMethod.Get, "/dispatch/does-not-exist")
            .AssertNotFound();
}
