namespace ApexDynamics.TitanWatch.ResponseSys.Test.Api;

public partial class AlertMutateTests
{
    [Test]
    public void Alert_Raise_Success()
    {
        var signalEvent = Test.Http<SignalEvent>()
            .ExpectPostgresOutboxEvents(e => e
                .AssertWithValue("titanwatch.signal-event", "apexdynamics.titanwatch.responsesys.signalevent.created.v1"))
            .Run(HttpMethod.Post, "/alert", new AlertRequest { Active = true })
            .AssertOK()
            .Value!;

        signalEvent.SeverityCode.Should().Be("WRN");
        signalEvent.Message.Should().Be("CITYWIDE ALERT RAISED — all sectors to shelter posture.");
    }

    [Test]
    public void Alert_StandDown_Success()
    {
        var signalEvent = Test.Http<SignalEvent>()
            .ExpectPostgresOutboxEvents(e => e
                .AssertWithValue("titanwatch.signal-event", "apexdynamics.titanwatch.responsesys.signalevent.created.v1"))
            .Run(HttpMethod.Post, "/alert", new AlertRequest { Active = false })
            .AssertOK()
            .Value!;

        signalEvent.SeverityCode.Should().Be("OPS");
        signalEvent.Message.Should().Be("Citywide alert stood down — sectors returning to nominal.");
    }

    [Test]
    public void Alert_Raise_DefaultsActiveTrueWhenOmitted()
    {
        var signalEvent = Test.Http<SignalEvent>()
            .ExpectPostgresOutboxEvents(e => e
                .AssertWithValue("titanwatch.signal-event", "apexdynamics.titanwatch.responsesys.signalevent.created.v1"))
            .Run(HttpMethod.Post, "/alert", new AlertRequest())
            .AssertOK()
            .Value!;

        signalEvent.SeverityCode.Should().Be("WRN");
    }
}
