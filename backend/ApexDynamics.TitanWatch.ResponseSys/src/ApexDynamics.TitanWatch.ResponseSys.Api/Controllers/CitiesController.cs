namespace ApexDynamics.TitanWatch.ResponseSys.Api.Controllers;

/// <summary>Represents the <c>LastStandCity</c> (read-only) controller.</summary>
[ApiController, Route("/cities")]
public class CitiesController(CoreEx.AspNetCore.Mvc.WebApi webApi, ILastStandCityService service) : ControllerBase
{
    private readonly CoreEx.AspNetCore.Mvc.WebApi _webApi = webApi.ThrowIfNull();
    private readonly ILastStandCityService _service = service.ThrowIfNull();

    /// <summary>Gets all <see cref="LastStandCity"/> items.</summary>
    [HttpGet, HttpHead]
    [ProducesResponseType(typeof(LastStandCity[]), 200)]
    public Task<IActionResult> GetAllAsync(CancellationToken cancellationToken = default)
        => _webApi.GetAsync(Request, (_, ct) => _service.GetAllAsync(ct), cancellationToken: cancellationToken);

    /// <summary>Gets the specified <see cref="LastStandCity"/>.</summary>
    [HttpGet("{id}"), HttpHead("{id}")]
    [ProducesResponseType(typeof(LastStandCity), 200)]
    public Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
        => _webApi.GetAsync(Request, (_, ct) => _service.GetAsync(id.Required(), ct), cancellationToken: cancellationToken);
}
