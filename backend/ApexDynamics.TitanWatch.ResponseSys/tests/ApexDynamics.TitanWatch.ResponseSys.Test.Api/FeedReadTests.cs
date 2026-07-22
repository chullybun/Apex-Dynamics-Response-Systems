namespace ApexDynamics.TitanWatch.ResponseSys.Test.Api;

/// <summary>Integration tests for the read-only, paged <c>SignalEvent</c> feed endpoints (<c>GET /feed</c>, <c>GET /feed/{id}</c>).</summary>
public partial class FeedReadTests : WithApiTester<ApexDynamics.TitanWatch.ResponseSys.Api.Program>
{
    [OneTimeSetUp]
    public async Task OneTimeSetUpAsync()
    {
        await Test.MigratePostgresDataAsync<TestData>(["read-data.seed.yaml"], DbMigration.ConfigureMigrationArgs).ConfigureAwait(false);
        await Test.ClearFusionCacheAsync().ConfigureAwait(false);
    }

    [Test]
    public void Feed_GetAll_ReturnsAllMostRecentFirst()
    {
        var feed = Test.Http<SignalEvent[]>()
            .Run(HttpMethod.Get, "/feed")
            .AssertOK()
            .Value!;

        feed.Should().HaveCount(4);
        feed.Select(s => s.Id).Should().ContainInOrder("sig-4", "sig-3", "sig-2", "sig-1");
        feed.Select(s => s.Timestamp).Should().ContainInOrder(4000L, 3000L, 2000L, 1000L);
        feed[0].Timestamp.Should().Be(4000);
    }

    [Test]
    public void Feed_GetAll_TakeLimitsPageSize()
    {
        var feed = Test.Http<SignalEvent[]>()
            .Run(HttpMethod.Get, "/feed?$take=2")
            .AssertOK()
            .Value!;

        feed.Should().HaveCount(2);
        feed.Select(s => s.Id).Should().ContainInOrder("sig-4", "sig-3");
        feed[0].Timestamp.Should().Be(4000);
    }

    [Test]
    public void Feed_GetAll_SkipOffsetsPage()
    {
        var feed = Test.Http<SignalEvent[]>()
            .Run(HttpMethod.Get, "/feed?$take=2&$skip=2")
            .AssertOK()
            .Value!;

        feed.Should().HaveCount(2);
        feed.Select(s => s.Id).Should().ContainInOrder("sig-2", "sig-1");
    }

    [Test]
    public void Feed_GetAll_CountSetsTotalCountHeader()
    {
        var result = Test.Http<SignalEvent[]>()
            .Run(HttpMethod.Get, "/feed?$take=2&$count=true")
            .AssertOK();

        result.Value!.Should().HaveCount(2);
        // CoreEx paging emits the total-count via the 'X-Paging-Total-Count' response header.
        result.Response.Headers.GetValues("X-Paging-Total-Count").First().Should().Be("4");
    }

    [Test]
    public void Feed_Get_Found()
    {
        var signal = Test.Http<SignalEvent>()
            .Run(HttpMethod.Get, "/feed/sig-1")
            .AssertOK()
            .Value!;

        signal.Id.Should().Be("sig-1");
        signal.Message.Should().Be("Sensor baseline nominal");
        signal.Timestamp.Should().Be(1000);
        signal.SeverityCode.Should().Be("INF");
        signal.LeviathanId.Should().Be("lev-1");
    }

    [Test]
    public void Feed_Get_RefDataSerializesAsSeverity()
    {
        // The [ReferenceData<SignalSeverity>] SeverityCode serializes by its non-'Code' JSON name 'severity'.
        var json = Test.Http()
            .Run(HttpMethod.Get, "/feed/sig-4")
            .AssertOK()
            .GetContent().Should().BeJson()
                .ContainAll(["$.id", "$.message", "$.timestamp", "$.severity"]);

        json.HavePath("$.severity").GetValue<string>().Should().Be("CRT");
    }

    [Test]
    public void Feed_Get_NotFound()
        => Test.Http()
            .Run(HttpMethod.Get, "/feed/does-not-exist")
            .AssertNotFound();
}
