namespace ApexDynamics.TitanWatch.ResponseSys.Infrastructure.Mapping;

/// <summary>Provides mapping from <see cref="Persistence.Leviathan"/> to <see cref="Contracts.Leviathan"/>.</summary>
internal partial class LeviathanMapper : Mapper<Persistence.Leviathan, Contracts.Leviathan, LeviathanMapper>
{
    /// <inheritdoc/>
    protected override Contracts.Leviathan OnMap(Persistence.Leviathan source)
    {
        var destination = new Contracts.Leviathan
        {
            Id = source.Id!,
            Codename = source.Codename,
            ClassNumeral = source.ClassNumeral,
            Archetype = source.Archetype,
            Range = source.Range,
            Height = source.Height,
            Speed = source.Speed,
            Hp = source.Hp,
            HpMax = source.HpMax,
            StatusCode = source.StatusCode,
            Lng = source.Lng,
            Lat = source.Lat,
            ThreatCode = source.ThreatCode,
            Heading = source.Heading,
            FromLng = source.FromLng,
            FromLat = source.FromLat,
            ToLng = source.ToLng,
            ToLat = source.ToLat,
            Target = source.Target,
            StartRange = source.StartRange,
            Repel = source.Repel,
            ETag = source.ETag
        };

        OnMapExtend(source, destination);
        return destination;
    }

    /// <summary>Provides the opportunity to extend the <see cref="OnMap" /> method.</summary>
    partial void OnMapExtend(Persistence.Leviathan source, Contracts.Leviathan destination);
}
