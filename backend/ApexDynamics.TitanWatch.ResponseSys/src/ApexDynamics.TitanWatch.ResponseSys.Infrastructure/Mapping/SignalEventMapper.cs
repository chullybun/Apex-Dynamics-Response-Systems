namespace ApexDynamics.TitanWatch.ResponseSys.Infrastructure.Mapping;

/// <summary>Provides mapping from <see cref="Persistence.SignalEvent"/> to <see cref="Contracts.SignalEvent"/>.</summary>
internal partial class SignalEventMapper : Mapper<Persistence.SignalEvent, Contracts.SignalEvent, SignalEventMapper>
{
    /// <inheritdoc/>
    protected override Contracts.SignalEvent OnMap(Persistence.SignalEvent source)
    {
        var destination = new Contracts.SignalEvent
        {
            Id = source.Id!,
            Message = source.Message,
            Timestamp = source.Timestamp,
            LeviathanId = source.LeviathanId,
            SeverityCode = source.SeverityCode
        };

        OnMapExtend(source, destination);
        return destination;
    }

    /// <summary>Provides the opportunity to extend the <see cref="OnMap" /> method.</summary>
    partial void OnMapExtend(Persistence.SignalEvent source, Contracts.SignalEvent destination);
}
