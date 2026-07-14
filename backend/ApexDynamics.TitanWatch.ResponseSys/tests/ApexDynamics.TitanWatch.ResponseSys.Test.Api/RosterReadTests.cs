namespace ApexDynamics.TitanWatch.ResponseSys.Test.Api;

/// <summary>Integration tests for the read-only <c>Leviathan</c> roster endpoints (<c>GET /roster</c>, <c>GET /roster/{id}</c>).</summary>
public partial class RosterReadTests : WithApiTester<ApexDynamics.TitanWatch.ResponseSys.Api.Program>
{
    [OneTimeSetUp]
    public async Task OneTimeSetUpAsync()
    {
        await Test.MigratePostgresDataAsync<TestData>(["read-data.seed.yaml"], DbMigration.ConfigureMigrationArgs).ConfigureAwait(false);
        await Test.ClearFusionCacheAsync().ConfigureAwait(false);
    }

    [Test]
    public void Roster_GetAll_ReturnsAllOrderedByCodename()
    {
        // Asserts the production masters roster (lev-1..lev-4), which the test migrate seeds.
        var roster = Test.Http<Leviathan[]>()
            .Run(HttpMethod.Get, "/roster")
            .AssertOK()
            .Value!;

        roster.Should().HaveCount(4);
        roster.Select(l => l.Codename).Should().ContainInOrder("Gorathos", "Nyxmora", "Terrakon", "Vespyra");
    }

    [Test]
    public void Roster_Get_Found()
    {
        var leviathan = Test.Http<Leviathan>()
            .Run(HttpMethod.Get, "/roster/lev-1")
            .AssertOK()
            .Value!;

        leviathan.Id.Should().Be("lev-1");
        leviathan.Codename.Should().Be("Gorathos");
        leviathan.ThreatCode.Should().Be("CAT");
        leviathan.StatusCode.Should().Be("LND");
        leviathan.Hp.Should().Be(6050);
        leviathan.HpMax.Should().Be(11000);
        leviathan.ETag.Should().NotBeNullOrEmpty();
    }

    [Test]
    public void Roster_Get_RefDataSerializesAsCodes()
    {
        // The [ReferenceData<>] properties serialize by their non-'Code' JSON name (status/threat),
        // and the PostgreSQL xmin-derived ETag is emitted as 'etag'.
        var json = Test.Http()
            .Run(HttpMethod.Get, "/roster/lev-1")
            .AssertOK()
            .GetContent().Should().BeJson()
                .ContainAll(["$.id", "$.codename", "$.threat", "$.status", "$.etag"]);

        json.HavePath("$.threat").GetValue<string>().Should().Be("CAT");
        json.HavePath("$.status").GetValue<string>().Should().Be("LND");
        json.HavePath("$.codename").GetValue<string>().Should().Be("Gorathos");
        json.HavePath("$.etag").GetValue<string>().Should().NotBeNullOrEmpty();
    }

    [Test]
    public void Roster_Get_NotFound()
        => Test.Http()
            .Run(HttpMethod.Get, "/roster/does-not-exist")
            .AssertNotFound();
}
