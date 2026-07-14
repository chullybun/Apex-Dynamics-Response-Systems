namespace ApexDynamics.TitanWatch.ResponseSys.Infrastructure.Mapping;

/// <summary>Provides mapping from <see cref="Persistence.DispatchUnit"/> to <see cref="Contracts.DispatchUnit"/>.</summary>
internal partial class DispatchUnitMapper : Mapper<Persistence.DispatchUnit, Contracts.DispatchUnit, DispatchUnitMapper>
{
    /// <inheritdoc/>
    protected override Contracts.DispatchUnit OnMap(Persistence.DispatchUnit source)
    {
        var destination = new Contracts.DispatchUnit
        {
            Id = source.Id!,
            Name = source.Name,
            Available = source.Available,
            Capacity = source.Capacity
        };

        OnMapExtend(source, destination);
        return destination;
    }

    /// <summary>Provides the opportunity to extend the <see cref="OnMap" /> method.</summary>
    partial void OnMapExtend(Persistence.DispatchUnit source, Contracts.DispatchUnit destination);
}
