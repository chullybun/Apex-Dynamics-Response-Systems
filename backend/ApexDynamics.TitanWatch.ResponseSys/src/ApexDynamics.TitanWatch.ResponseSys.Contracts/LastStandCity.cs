namespace ApexDynamics.TitanWatch.ResponseSys.Contracts;

/// <summary>Represents the <c>LastStandCity</c> contract.</summary>
[Contract]
public partial class LastStandCity : IIdentifier<string>
{
    /// <inheritdoc/>
    [ReadOnly(true)]
    public string Id { get; set; } = default!;

    /// <summary>Gets or sets the city name.</summary>
    public string? Name { get; set; }

    /// <summary>Gets or sets the defended side.</summary>
    public string? Side { get; set; }

    /// <summary>Gets or sets the city population.</summary>
    public int? Population { get; set; }
}

/// <summary>Represents the <c>LastStandCity</c> contract collection.</summary>
public class LastStandCityCollection : List<LastStandCity> { }
