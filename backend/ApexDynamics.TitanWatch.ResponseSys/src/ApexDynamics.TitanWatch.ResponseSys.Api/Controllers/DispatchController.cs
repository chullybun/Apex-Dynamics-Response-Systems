namespace ApexDynamics.TitanWatch.ResponseSys.Api.Controllers;

/// <summary>Represents the <c>DispatchUnit</c> (read-only) controller.</summary>
[ApiController, Route("/dispatch")]
public class DispatchController(CoreEx.AspNetCore.Mvc.WebApi webApi, IDispatchUnitService service) : ControllerBase
{
    private readonly CoreEx.AspNetCore.Mvc.WebApi _webApi = webApi.ThrowIfNull();
    private readonly IDispatchUnitService _service = service.ThrowIfNull();

    /// <summary>Gets all <see cref="DispatchUnit"/> items.</summary>
    [HttpGet, HttpHead]
    [ProducesResponseType(typeof(DispatchUnit[]), 200)]
    public Task<IActionResult> GetAllAsync(CancellationToken cancellationToken = default)
        => _webApi.GetAsync(Request, (_, ct) => _service.GetAllAsync(ct), cancellationToken: cancellationToken);

    /// <summary>Gets the specified <see cref="DispatchUnit"/>.</summary>
    [HttpGet("{id}"), HttpHead("{id}")]
    [ProducesResponseType(typeof(DispatchUnit), 200)]
    public Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
        => _webApi.GetAsync(Request, (_, ct) => _service.GetAsync(id.Required(), ct), cancellationToken: cancellationToken);
}
