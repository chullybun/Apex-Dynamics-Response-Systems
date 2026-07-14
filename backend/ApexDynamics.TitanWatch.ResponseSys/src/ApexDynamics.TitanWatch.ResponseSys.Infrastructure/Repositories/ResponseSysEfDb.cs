namespace ApexDynamics.TitanWatch.ResponseSys.Infrastructure.Repositories;

/// <summary>Provides the <see cref="ResponseSysDbContext"/> <see cref="EfDb{TDbContext}"/> wrapper.</summary>
public sealed class ResponseSysEfDb(ResponseSysDbContext dbContext) : EfDb<ResponseSysDbContext>(dbContext, _options)
{
    private static readonly EfDbOptions _options = new();
}