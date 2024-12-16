using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Shuttle.Core.Data;
using Shuttle.Recall.Tests;

namespace Shuttle.Recall.Sql.Storage.Tests;

public class StorageFixture : RecallFixture
{
    [Test]
    public async Task Should_be_able_to_exercise_event_store_async()
    {
        var services = SqlConfiguration.GetServiceCollection();

        var serviceProvider = services.BuildServiceProvider();
        var databaseContextFactory = serviceProvider.GetRequiredService<IDatabaseContextFactory>();
        var options = serviceProvider.GetRequiredService<IOptions<SqlStorageOptions>>().Value;

        var fixtureConfiguration = new FixtureConfiguration(services)
            .WithRemoveIdsCallback(async ids =>
            {
                await using (var databaseContext = databaseContextFactory.Create())
                {
                    await databaseContext.ExecuteAsync(new Query($"DELETE FROM [{options.Schema}].[PrimitiveEvent] WHERE Id IN ({string.Join(',', ids.Select(id => $"'{id}'"))})"));
                }
            });

        await ExerciseStorageAsync(fixtureConfiguration);
    }
}