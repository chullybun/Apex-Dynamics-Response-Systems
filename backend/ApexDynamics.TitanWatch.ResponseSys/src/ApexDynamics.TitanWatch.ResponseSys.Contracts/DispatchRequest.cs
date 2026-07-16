namespace ApexDynamics.TitanWatch.ResponseSys.Contracts;

/// <summary>Represents the request body for deploying a <see cref="DispatchUnit"/> against a targeted <see cref="Leviathan"/>.</summary>
[Contract]
public partial class DispatchRequest
{
    /// <summary>Gets or sets the <see cref="DispatchUnit.Id"/> to deploy.</summary>
    public string? DispatchUnitId { get; set; }
}
