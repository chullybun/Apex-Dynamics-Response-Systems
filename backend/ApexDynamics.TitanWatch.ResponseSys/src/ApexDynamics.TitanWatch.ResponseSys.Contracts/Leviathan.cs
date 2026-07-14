namespace ApexDynamics.TitanWatch.ResponseSys.Contracts;

/// <summary>Represents the <c>Leviathan</c> contract.</summary>
[Contract]
public partial class Leviathan : IIdentifier<string>, IETag
{
    /// <inheritdoc/>
    [ReadOnly(true)]
    public string Id { get; set; } = default!;

    /// <summary>Gets or sets the operational call sign (e.g. <c>Gorathos</c>).</summary>
    public string? Codename { get; set; }

    /// <summary>Gets or sets the Roman-numeral severity class (e.g. <c>IV</c>).</summary>
    public string? ClassNumeral { get; set; }

    /// <summary>Gets or sets the behavioral archetype (e.g. <c>Abyssal Colossus</c>).</summary>
    public string? Archetype { get; set; }

    /// <summary>Gets or sets the live remaining range to landfall (km).</summary>
    public double? Range { get; set; }

    /// <summary>Gets or sets the height in meters.</summary>
    public int? Height { get; set; }

    /// <summary>Gets or sets the travel speed in km/h.</summary>
    public double? Speed { get; set; }

    /// <summary>Gets or sets the remaining containment hit points.</summary>
    public int? Hp { get; set; }

    /// <summary>Gets or sets the maximum hit points (for HP-bar ratios).</summary>
    public int? HpMax { get; set; }

    /// <summary>Gets or sets the operational status code (reference data).</summary>
    [ReferenceData<LeviathanStatus>]
    public partial string? StatusCode { get; set; }

    /// <summary>Gets or sets the longitude (WGS84) spawn position.</summary>
    public double? Lng { get; set; }

    /// <summary>Gets or sets the latitude (WGS84) spawn position.</summary>
    public double? Lat { get; set; }

    /// <summary>Gets or sets the threat level code (reference data).</summary>
    [ReferenceData<ThreatLevel>]
    public partial string? ThreatCode { get; set; }

    /// <summary>Gets or sets the projected travel bearing in degrees (0 = north, clockwise to 360).</summary>
    public double? Heading { get; set; }

    /// <summary>Gets or sets the inbound-track spawn point longitude (open water).</summary>
    public double? FromLng { get; set; }

    /// <summary>Gets or sets the inbound-track spawn point latitude (open water).</summary>
    public double? FromLat { get; set; }

    /// <summary>Gets or sets the inbound-track landfall target longitude.</summary>
    public double? ToLng { get; set; }

    /// <summary>Gets or sets the inbound-track landfall target latitude.</summary>
    public double? ToLat { get; set; }

    /// <summary>Gets or sets the human-readable landfall target (e.g. <c>SEATTLE</c>).</summary>
    public string? Target { get; set; }

    /// <summary>Gets or sets the engagement range (km) at spawn.</summary>
    public int? StartRange { get; set; }

    /// <summary>Gets or sets the dispatch knockback (track fraction the leviathan is currently shoved back).</summary>
    public double? Repel { get; set; }

    /// <inheritdoc/>
    [ReadOnly(true)]
    public string? ETag { get; set; }
}

/// <summary>Represents the <c>Leviathan</c> contract collection.</summary>
public class LeviathanCollection : List<Leviathan> { }
