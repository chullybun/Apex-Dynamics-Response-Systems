namespace ApexDynamics.TitanWatch.ResponseSys.Contracts;

/// <summary>Represents the <c>SignalEvent</c> contract.</summary>
[Contract]
public partial class SignalEvent : IIdentifier<string>
{
    /// <inheritdoc/>
    [ReadOnly(true)]
    public string Id { get; set; } = default!;

    /// <summary>Gets or sets the human-readable signal message.</summary>
    public string? Message { get; set; }

    /// <summary>Gets or sets the epoch timestamp in milliseconds.</summary>
    public long? Timestamp { get; set; }

    /// <summary>Gets or sets the originating <see cref="Leviathan.Id"/>.</summary>
    public string? LeviathanId { get; set; }

    /// <summary>Gets or sets the severity code (reference data).</summary>
    [ReferenceData<SignalSeverity>]
    public partial string? SeverityCode { get; set; }
}

/// <summary>Represents the <c>SignalEvent</c> contract collection.</summary>
public class SignalEventCollection : List<SignalEvent> { }
