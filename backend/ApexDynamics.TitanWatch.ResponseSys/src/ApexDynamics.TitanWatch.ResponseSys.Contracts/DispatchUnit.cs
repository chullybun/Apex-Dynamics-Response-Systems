namespace ApexDynamics.TitanWatch.ResponseSys.Contracts;

/// <summary>Represents the <c>DispatchUnit</c> contract.</summary>
[Contract]
public partial class DispatchUnit : IIdentifier<string>
{
    /// <inheritdoc/>
    [ReadOnly(true)]
    public string Id { get; set; } = default!;

    /// <summary>Gets or sets the dispatch unit name.</summary>
    public string? Name { get; set; }

    /// <summary>Gets or sets the number of currently available units.</summary>
    public int? Available { get; set; }

    /// <summary>Gets or sets the total unit capacity.</summary>
    public int? Capacity { get; set; }
}

/// <summary>Represents the <c>DispatchUnit</c> contract collection.</summary>
public class DispatchUnitCollection : List<DispatchUnit> { }
