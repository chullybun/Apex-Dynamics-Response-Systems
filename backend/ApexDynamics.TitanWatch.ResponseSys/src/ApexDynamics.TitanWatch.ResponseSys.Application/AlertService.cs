namespace ApexDynamics.TitanWatch.ResponseSys.Application;

/// <summary>Provides the <see cref="IAlertService"/> implementation.</summary>
/// <remarks>Ports the browser mock's <c>triggerAlert</c> command (<c>src/state/useCommandState.ts</c>): appends a <c>WRN</c> (raised) or <c>OPS</c> (stood down) signal event,
/// committed atomically via the <see cref="IUnitOfWork"/>. The citywide alert is a transient command, not a persisted entity — the caller's own subsequent action determines the
/// active/stood-down transition (mirroring the mock, which does not derive it from prior server state).</remarks>
[ScopedService<IAlertService>]
public class AlertService(IUnitOfWork unitOfWork, ISignalEventRepository signalEventRepository, CoreEx.ExecutionContext executionContext) : IAlertService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork.ThrowIfNull();
    private readonly ISignalEventRepository _signalEventRepository = signalEventRepository.ThrowIfNull();
    private readonly CoreEx.ExecutionContext _executionContext = executionContext.ThrowIfNull();

    /// <inheritdoc/>
    public Task<SignalEvent> RaiseAsync(AlertRequest request, CancellationToken cancellationToken = default)
    {
        request.ThrowIfNull();
        var active = request.Active ?? true;

        return _unitOfWork.TransactionAsync(async ct =>
        {
            var signalEvent = new SignalEvent
            {
                Id = Runtime.NewId(),
                SeverityCode = active ? "WRN" : "OPS",
                Message = active
                    ? "CITYWIDE ALERT RAISED — all sectors to shelter posture."
                    : "Citywide alert stood down — sectors returning to nominal.",
                Timestamp = _executionContext.Timestamp.ToUnixTimeMilliseconds()
            };

            var created = await _signalEventRepository.CreateAsync(signalEvent, ct).ConfigureAwait(false);
            _unitOfWork.Events.Add("titanwatch.signal-event", EventData.CreateEventWith(created, EventAction.Created));
            return created;
        }, cancellationToken);
    }
}
