namespace ApexDynamics.TitanWatch.ResponseSys.Api.Controllers;

/// <summary>Represents the citywide alert (mutating) controller.</summary>
[ApiController, Route("/alert"), OpenApiTag("Alert")]
public class AlertController(CoreEx.AspNetCore.Mvc.WebApi webApi, IAlertService service) : ControllerBase
{
    private readonly CoreEx.AspNetCore.Mvc.WebApi _webApi = webApi.ThrowIfNull();
    private readonly IAlertService _service = service.ThrowIfNull();

    /// <summary>Raises or stands down the citywide alert, logging the transition to the signal feed.</summary>
    [HttpPost]
    [Accepts<AlertRequest>]
    [ProducesResponseType(typeof(SignalEvent), 200)]
    public Task<IActionResult> PostAsync(CancellationToken cancellationToken = default) => _webApi.PostAsync<AlertRequest, SignalEvent>(Request,
        (ro, ct) => _service.RaiseAsync(ro.Value, ct), HttpStatusCode.OK, cancellationToken: cancellationToken);
}
