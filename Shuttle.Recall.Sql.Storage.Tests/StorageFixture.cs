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
        var provider = SqlConfiguration.BuildServiceProvider(new ServiceCollection().AddSingleton(new Mock<IProjectionRepository>().Object));

        var databaseGateway = provider.GetRequiredService<IDatabaseGateway>();

        using (provider.GetRequiredService<IDatabaseContextFactory>().Create())
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
                ExerciseStorage(provider.GetRequiredService<IEventStore>());
                ExerciseStorageRemoval(provider.GetRequiredService<IEventStore>());
            }
            else
            {
                await ExerciseStorageAsync(provider.GetRequiredService<IEventStore>());
                await ExerciseStorageRemovalAsync(provider.GetRequiredService<IEventStore>());
            }
        }
    }
}