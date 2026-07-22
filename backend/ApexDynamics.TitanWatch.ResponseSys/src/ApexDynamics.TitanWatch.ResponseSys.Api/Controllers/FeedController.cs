namespace ApexDynamics.TitanWatch.ResponseSys.Api.Controllers;

/// <summary>Represents the <c>SignalEvent</c> feed (read-only) controller.</summary>
[ApiController, Route("/feed")]
public class FeedController(CoreEx.AspNetCore.Mvc.WebApi webApi, ISignalEventService service) : ControllerBase
{
    private readonly CoreEx.AspNetCore.Mvc.WebApi _webApi = webApi.ThrowIfNull();
    private readonly ISignalEventService _service = service.ThrowIfNull();

    /// <summary>Gets a paged, most-recent-first <see cref="SignalEvent"/> feed.</summary>
    [HttpGet, HttpHead]
    [Paging(supportsCount: true)]
    [ProducesResponseType(typeof(SignalEvent[]), 200)]
    public Task<IActionResult> GetAllAsync(CancellationToken cancellationToken = default)
        => _webApi.GetAsync(Request, (ro, ct) => _service.GetAllAsync(ro.PagingArgs, ct), cancellationToken: cancellationToken);

    /// <summary>Gets the specified <see cref="SignalEvent"/>.</summary>
    [HttpGet("{id}"), HttpHead("{id}")]
    [ProducesResponseType(typeof(SignalEvent), 200)]
    public Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
        => _webApi.GetAsync(Request, (_, ct) => _service.GetAsync(id.Required(), ct), cancellationToken: cancellationToken);
}
