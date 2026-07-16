namespace ApexDynamics.TitanWatch.ResponseSys.Application.Validators;

/// <summary>Validates a <see cref="DispatchRequest"/>.</summary>
public class DispatchRequestValidator : Validator<DispatchRequest, DispatchRequestValidator>
{
    /// <summary>Initializes a new instance of the <see cref="DispatchRequestValidator"/> class.</summary>
    public DispatchRequestValidator() => Property(x => x.DispatchUnitId).Mandatory();
}
