namespace ApexDynamics.TitanWatch.ResponseSys.Test.Api;

/// <summary>Integration tests for the <c>Leviathan</c> roster dispatch command (<c>POST /roster/{id}/dispatch</c>).</summary>
public partial class DispatchMutateTests : WithApiTester<ApexDynamics.TitanWatch.ResponseSys.Api.Program>
{
    [OneTimeSetUp]
    public async Task OneTimeSetUpAsync()
    {
        await Test.MigratePostgresDataAsync<TestData>(["mutate-data.seed.yaml"], DbMigration.ConfigureMigrationArgs).ConfigureAwait(false);
        await Test.ClearFusionCacheAsync().ConfigureAwait(false);

        Test.UseExpectedPostgresOutboxPublisher();
    }
}
