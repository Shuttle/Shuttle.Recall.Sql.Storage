using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Shuttle.Core.Data;
using Shuttle.Recall.Tests;

namespace Shuttle.Recall.Sql.Storage.Tests;

public class StorageFixture : RecallFixture
{
    [Test]
    public async Task Should_be_able_to_exercise_event_store_async()
    {
        var services = SqlConfiguration.GetServiceCollection(new ServiceCollection().AddSingleton(new Mock<IProjectionRepository>().Object));

        var serviceProvider = services.BuildServiceProvider();
        var databaseContextFactory = serviceProvider.GetRequiredService<IDatabaseContextFactory>();

        await using (var databaseContext = databaseContextFactory.Create())
        {
            await databaseContext.ExecuteAsync(new Query("delete from EventStore where Id = @Id").AddParameter(Columns.Id, OrderId));
            await databaseContext.ExecuteAsync(new Query("delete from EventStore where Id = @Id").AddParameter(Columns.Id, OrderProcessId));
        }

        await ExerciseStorageAsync(services);
        await ExerciseStorageRemovalAsync(services);
    }
}