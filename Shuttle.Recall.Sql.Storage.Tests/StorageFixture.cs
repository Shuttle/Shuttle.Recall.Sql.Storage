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
    public void Should_be_able_to_exercise_event_store()
    {
        Should_be_able_to_exercise_event_store_async(true).GetAwaiter().GetResult();
    }

    [Test]
    public async Task Should_be_able_to_exercise_event_store_async()
    {
        await Should_be_able_to_exercise_event_store_async(false);
    }

    private async Task Should_be_able_to_exercise_event_store_async(bool sync)
    {
        var services = SqlConfiguration.GetServiceCollection(new ServiceCollection().AddSingleton(new Mock<IProjectionRepository>().Object));

        var serviceProvider = services.BuildServiceProvider();
        var databaseGateway = serviceProvider.GetRequiredService<IDatabaseGateway>();

        using (serviceProvider.GetRequiredService<IDatabaseContextFactory>().Create())
        {
            if (sync)
            {
                databaseGateway.Execute(new Query("delete from EventStore where Id = @Id").AddParameter(Columns.Id, OrderId));
                databaseGateway.Execute(new Query("delete from EventStore where Id = @Id").AddParameter(Columns.Id, OrderProcessId));
                databaseGateway.Execute(new Query("delete from SnapshotStore where Id = @Id").AddParameter(Columns.Id, OrderId));
                databaseGateway.Execute(new Query("delete from SnapshotStore where Id = @Id").AddParameter(Columns.Id, OrderProcessId));
            }
            else
            {
                await databaseGateway.ExecuteAsync(new Query("delete from EventStore where Id = @Id").AddParameter(Columns.Id, OrderId));
                await databaseGateway.ExecuteAsync(new Query("delete from EventStore where Id = @Id").AddParameter(Columns.Id, OrderProcessId));
                await databaseGateway.ExecuteAsync(new Query("delete from SnapshotStore where Id = @Id").AddParameter(Columns.Id, OrderId));
                await databaseGateway.ExecuteAsync(new Query("delete from SnapshotStore where Id = @Id").AddParameter(Columns.Id, OrderProcessId));
            }

            if (sync)
            {
                ExerciseStorage(services);
                ExerciseStorageRemoval(services);
            }
            else
            {
                await ExerciseStorageAsync(services);
                await ExerciseStorageRemovalAsync(services);
            }
        }
    }
}