namespace ApexDynamics.TitanWatch.ResponseSys.Infrastructure.Mapping;

/// <summary>Provides mapping from <see cref="Persistence.LastStandCity"/> to <see cref="Contracts.LastStandCity"/>.</summary>
internal partial class LastStandCityMapper : Mapper<Persistence.LastStandCity, Contracts.LastStandCity, LastStandCityMapper>
{
    /// <inheritdoc/>
    protected override Contracts.LastStandCity OnMap(Persistence.LastStandCity source)
    {
        var destination = new Contracts.LastStandCity
        {
            Id = source.Id!,
            Name = source.Name,
            Side = source.Side,
            Population = source.Population
        };

        OnMapExtend(source, destination);
        return destination;
    }

    /// <summary>Provides the opportunity to extend the <see cref="OnMap" /> method.</summary>
    partial void OnMapExtend(Persistence.LastStandCity source, Contracts.LastStandCity destination);
}
