namespace ApexDynamics.TitanWatch.ResponseSys.Test.Api;

public partial class DispatchMutateTests
{
    [Test]
    public void Dispatch_Deploy_Success()
    {
        // lev-1 / scramble-jets (capacity 6) are exclusive to this test — no other test mutates this pair.
        var unit = Test.Http<DispatchUnit>()
            .ExpectPostgresOutboxEvents(e => e
                .AssertMetadata("titanwatch.signal-event", "apexdynamics.titanwatch.responsesys.signalevent.created.v1"))
            .Run(HttpMethod.Post, "/roster/lev-1/dispatch", new DispatchRequest { DispatchUnitId = "scramble-jets" })
            .AssertOK()
            .Value!;

        unit.Id.Should().Be("scramble-jets");
        unit.Available.Should().Be(5);
        unit.Capacity.Should().Be(6);

        // Verify persistence, not just the echo.
        Test.Http<DispatchUnit>()
            .Run(HttpMethod.Get, "/dispatch/scramble-jets")
            .AssertOK()
            .Value!.Available.Should().Be(5);

        Test.Http<Leviathan>()
            .Run(HttpMethod.Get, "/roster/lev-1")
            .AssertOK()
            .Value!.Repel.Should().Be(0.18);
    }

    [Test]
    public void Dispatch_Deploy_LeviathanNotFound()
        => Test.Http()
            .ExpectNoPostgresOutboxEvents()
            .Run(HttpMethod.Post, "/roster/does-not-exist/dispatch", new DispatchRequest { DispatchUnitId = "deploy-mechs" })
            .AssertNotFound();

    [Test]
    public void Dispatch_Deploy_DispatchUnitNotFound()
        // lev-2 is read-only here (the lookup succeeds; the failure is on the dispatch unit, before any mutation).
        => Test.Http()
            .ExpectNoPostgresOutboxEvents()
            .Run(HttpMethod.Post, "/roster/lev-2/dispatch", new DispatchRequest { DispatchUnitId = "does-not-exist" })
            .AssertNotFound();

    [Test]
    public void Dispatch_Deploy_ValidationError_MissingDispatchUnitId()
        // Validation runs before any repository access, so lev-3 is never touched.
        => Test.Http()
            .ExpectNoPostgresOutboxEvents()
            .Run(HttpMethod.Post, "/roster/lev-3/dispatch", new DispatchRequest())
            .AssertBadRequest();

    [Test]
    public void Dispatch_Deploy_Exhausted()
    {
        // raise-barrier (capacity 3) + lev-4 are exclusive to this test.
        for (var i = 0; i < 3; i++)
        {
            Test.Http<DispatchUnit>()
                .ExpectPostgresOutboxEvents(e => e
                    .AssertMetadata("titanwatch.signal-event", "apexdynamics.titanwatch.responsesys.signalevent.created.v1"))
                .Run(HttpMethod.Post, "/roster/lev-4/dispatch", new DispatchRequest { DispatchUnitId = "raise-barrier" })
                .AssertOK();
        }

        Test.Http()
            .ExpectNoPostgresOutboxEvents()
            .Run(HttpMethod.Post, "/roster/lev-4/dispatch", new DispatchRequest { DispatchUnitId = "raise-barrier" })
            .AssertBadRequest();
    }
}
