namespace ApexDynamics.TitanWatch.ResponseSys.Contracts;

/// <summary>Represents the request body for raising or standing down the citywide alert.</summary>
[Contract]
public partial class AlertRequest
{
    /// <summary>Gets or sets whether the alert is being raised (<c>true</c>, the default) or stood down (<c>false</c>).</summary>
    public bool? Active { get; set; }
}
