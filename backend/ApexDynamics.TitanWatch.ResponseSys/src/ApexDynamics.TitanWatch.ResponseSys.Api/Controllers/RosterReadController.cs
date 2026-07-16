namespace ApexDynamics.TitanWatch.ResponseSys.Api.Controllers;

/// <summary>Represents the <c>Leviathan</c> roster (read-only) controller.</summary>
[ApiController, Route("/roster"), OpenApiTag("Roster")]
public class RosterReadController(CoreEx.AspNetCore.Mvc.WebApi webApi, ILeviathanService service) : ControllerBase
{
    private readonly CoreEx.AspNetCore.Mvc.WebApi _webApi = webApi.ThrowIfNull();
    private readonly ILeviathanService _service = service.ThrowIfNull();

    /// <summary>Gets the full <see cref="Leviathan"/> roster.</summary>
    [HttpGet, HttpHead]
    [ProducesResponseType(typeof(Leviathan[]), 200)]
    public Task<IActionResult> GetAllAsync(CancellationToken cancellationToken = default)
        => _webApi.GetAsync(Request, (_, ct) => _service.GetAllAsync(ct), cancellationToken: cancellationToken);

    /// <summary>Gets the specified <see cref="Leviathan"/>.</summary>
    [HttpGet("{id}"), HttpHead("{id}")]
    [ProducesResponseType(typeof(Leviathan), 200)]
    public Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
        => _webApi.GetAsync(Request, (_, ct) => _service.GetAsync(id.Required(), ct), cancellationToken: cancellationToken);
}
