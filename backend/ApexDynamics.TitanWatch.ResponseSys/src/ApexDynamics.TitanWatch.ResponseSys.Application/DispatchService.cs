namespace ApexDynamics.TitanWatch.ResponseSys.Application;

/// <summary>Provides the <see cref="IDispatchService"/> implementation.</summary>
/// <remarks>Ports the browser mock's <c>dispatchUnit</c> command (<c>src/state/useCommandState.ts</c>): validates the requested <see cref="Contracts.DispatchUnit"/> has remaining
/// capacity, decrements it, applies bounded repel knockback to the targeted <see cref="Contracts.Leviathan"/>, and appends an <c>OPS</c> signal event — all committed atomically via the
/// <see cref="IUnitOfWork"/>.</remarks>
[ScopedService<IDispatchService>]
public class DispatchService(IUnitOfWork unitOfWork, ILeviathanRepository leviathanRepository, IDispatchUnitRepository dispatchUnitRepository, ISignalEventRepository signalEventRepository, CoreEx.ExecutionContext executionContext) : IDispatchService
{
    /// <summary>The repel increment applied to the targeted leviathan per dispatch (ported from <c>REPEL_KNOCKBACK_FRAC</c>).</summary>
    private const double _repelKnockbackFraction = 0.18;

    /// <summary>The maximum repel value a dispatch can push a leviathan to (ported from <c>REPEL_KNOCKBACK_MAX</c>).</summary>
    private const double _repelKnockbackMax = 0.6;

    private readonly IUnitOfWork _unitOfWork = unitOfWork.ThrowIfNull();
    private readonly ILeviathanRepository _leviathanRepository = leviathanRepository.ThrowIfNull();
    private readonly IDispatchUnitRepository _dispatchUnitRepository = dispatchUnitRepository.ThrowIfNull();
    private readonly ISignalEventRepository _signalEventRepository = signalEventRepository.ThrowIfNull();
    private readonly CoreEx.ExecutionContext _executionContext = executionContext.ThrowIfNull();

    /// <inheritdoc/>
    public async Task<DispatchUnit> DeployAsync(string leviathanId, DispatchRequest request, CancellationToken cancellationToken = default)
    {
        request.ThrowIfNull();
        await DispatchRequestValidator.Default.ValidateAndThrowAsync(request, cancellationToken).ConfigureAwait(false);

        var leviathan = await _leviathanRepository.GetAsync(leviathanId.ThrowIfNullOrEmpty(), cancellationToken).ConfigureAwait(false);
        NotFoundException.ThrowIfDefault(leviathan);

        var unit = await _dispatchUnitRepository.GetAsync(request.DispatchUnitId!, cancellationToken).ConfigureAwait(false);
        NotFoundException.ThrowIfDefault(unit);

        if (unit!.Available is null or <= 0)
            throw new BusinessException($"Dispatch denied: {unit.Name} at zero capacity.").WithErrorCode("dispatch-unit-exhausted").WithKey(unit.Id);

        unit.Available--;
        leviathan!.Repel = Math.Min(_repelKnockbackMax, (leviathan.Repel ?? 0) + _repelKnockbackFraction);

        return await _unitOfWork.TransactionAsync(async ct =>
        {
            await _leviathanRepository.UpdateAsync(leviathan, ct).ConfigureAwait(false);
            await _dispatchUnitRepository.UpdateAsync(unit, ct).ConfigureAwait(false);

            await PublishSignalAsync(new SignalEvent
            {
                SeverityCode = "OPS",
                Message = $"{unit.Name} engaged {leviathan.Codename?.ToUpperInvariant()} — repelled toward open water ({unit.Available}/{unit.Capacity} remaining).",
                Timestamp = _executionContext.Timestamp.ToUnixTimeMilliseconds(),
                LeviathanId = leviathan.Id
            }, ct).ConfigureAwait(false);

            return unit;
        }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Assigns the identity, persists the <see cref="SignalEvent"/>, and enqueues its <c>Created</c> outbox event within the current transaction.</summary>
    private async Task PublishSignalAsync(SignalEvent signalEvent, CancellationToken cancellationToken)
    {
        signalEvent.Id = Runtime.NewId();

        var created = await _signalEventRepository.CreateAsync(signalEvent, cancellationToken).ConfigureAwait(false);
        _unitOfWork.Events.Add("titanwatch.signal-event", EventData.CreateEventWith(created, EventAction.Created));
    }
}
