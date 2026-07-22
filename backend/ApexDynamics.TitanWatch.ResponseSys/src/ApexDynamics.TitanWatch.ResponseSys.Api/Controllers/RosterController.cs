namespace ApexDynamics.TitanWatch.ResponseSys.Api.Controllers;

/// <summary>Represents the <c>Leviathan</c> roster (mutating) controller.</summary>
[ApiController, Route("/roster"), OpenApiTag("Roster")]
public class RosterController(CoreEx.AspNetCore.Mvc.WebApi webApi, IDispatchService service) : ControllerBase
{
    private readonly CoreEx.AspNetCore.Mvc.WebApi _webApi = webApi.ThrowIfNull();
    private readonly IDispatchService _service = service.ThrowIfNull();

    /// <summary>Deploys a <see cref="DispatchUnit"/> against the specified <see cref="Leviathan"/>, applying repel knockback and decrementing the unit's available capacity.</summary>
    /// <remarks>Not idempotent — each call is a distinct, non-repeatable command that decrements capacity, so no <c>[IdempotencyKey]</c> is applied.</remarks>
    [HttpPost("{id}/dispatch")]
    [Accepts<DispatchRequest>]
    [ProducesResponseType(typeof(DispatchUnit), 200)]
    [ProducesNotFoundProblem]
    public Task<IActionResult> PostDispatchAsync(string id, CancellationToken cancellationToken = default) => _webApi.PostAsync<DispatchRequest, DispatchUnit>(Request,
        (ro, ct) => _service.DeployAsync(id.Required(), ro.Value, ct), HttpStatusCode.OK, cancellationToken: cancellationToken);
}
