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

        var fixtureConfiguration = new FixtureConfiguration(services)
            .WithRemoveIdsCallback(async (serviceProvider, ids) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<SqlStorageOptions>>().Value;

                await using (var databaseContext = serviceProvider.GetRequiredService<IDatabaseContextFactory>().Create())
                {
                    await databaseContext.ExecuteAsync(new Query($@"
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{options.Schema}].[PrimitiveEvent]') AND type in (N'U'))
BEGIN
    DELETE FROM [{options.Schema}].[PrimitiveEvent] WHERE Id IN ({string.Join(',', ids.Select(id => $"'{id}'"))})
END
")
                    );
                }
            });

        await ExerciseStorageAsync(fixtureConfiguration);
    }
}