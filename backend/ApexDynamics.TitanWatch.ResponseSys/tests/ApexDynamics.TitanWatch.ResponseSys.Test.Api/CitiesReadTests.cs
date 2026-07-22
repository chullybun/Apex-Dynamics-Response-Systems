namespace ApexDynamics.TitanWatch.ResponseSys.Test.Api;

/// <summary>Integration tests for the read-only <c>LastStandCity</c> endpoints (<c>GET /cities</c>, <c>GET /cities/{id}</c>).</summary>
public partial class CitiesReadTests : WithApiTester<ApexDynamics.TitanWatch.ResponseSys.Api.Program>
{
    [OneTimeSetUp]
    public async Task OneTimeSetUpAsync()
    {
        await Test.MigratePostgresDataAsync<TestData>(["read-data.seed.yaml"], DbMigration.ConfigureMigrationArgs).ConfigureAwait(false);
        await Test.ClearFusionCacheAsync().ConfigureAwait(false);
    }

    [Test]
    public void Cities_GetAll_ReturnsAllOrderedByName()
    {
        // Asserts the production masters cities, which the test migrate seeds.
        var cities = Test.Http<LastStandCity[]>()
            .Run(HttpMethod.Get, "/cities")
            .AssertOK()
            .Value!;

        cities.Should().HaveCount(2);
        cities.Select(c => c.Name).Should().ContainInOrder("BREMERTON", "OLYMPIA");
    }

    [Test]
    public void Cities_Get_Found()
    {
        var city = Test.Http<LastStandCity>()
            .Run(HttpMethod.Get, "/cities/bremerton")
            .AssertOK()
            .Value!;

        city.Id.Should().Be("bremerton");
        city.Name.Should().Be("BREMERTON");
        city.Side.Should().Be("left");
        city.Population.Should().Be(412000);
    }

    [Test]
    public void Cities_Get_NotFound()
        => Test.Http()
            .Run(HttpMethod.Get, "/cities/does-not-exist")
            .AssertNotFound();
}
