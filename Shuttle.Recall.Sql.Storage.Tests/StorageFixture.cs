using System;
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
        Should_be_able_to_exercise_event_store_async(false, true).GetAwaiter().GetResult();
    }

    [Test]
    public async Task Should_be_able_to_exercise_event_store_async()
    {
        await Should_be_able_to_exercise_event_store_async(false, false);
    }

    [Test]
    public void Should_be_able_to_exercise_event_store_with_existing_database_context()
    {
        Should_be_able_to_exercise_event_store_async(true, true).GetAwaiter().GetResult();
    }

    [Test]
    public async Task Should_be_able_to_exercise_event_store_with_existing_database_context_async()
    {
        await Should_be_able_to_exercise_event_store_async(true, false);
    }

    private async Task Should_be_able_to_exercise_event_store_async(bool createDatabaseContext, bool sync)
    {
        var services = SqlConfiguration.GetServiceCollection(new ServiceCollection().AddSingleton(new Mock<IProjectionRepository>().Object));

        var serviceProvider = services.BuildServiceProvider();
        var databaseGateway = serviceProvider.GetRequiredService<IDatabaseGateway>();
        var databaseContextFactory = serviceProvider.GetRequiredService<IDatabaseContextFactory>();

        await using (databaseContextFactory.Create())
        {
            await databaseGateway.ExecuteAsync(new Query("delete from EventStore where Id = @Id").AddParameter(Columns.Id, OrderId));
            await databaseGateway.ExecuteAsync(new Query("delete from EventStore where Id = @Id").AddParameter(Columns.Id, OrderProcessId));
            await databaseGateway.ExecuteAsync(new Query("delete from SnapshotStore where Id = @Id").AddParameter(Columns.Id, OrderId));
            await databaseGateway.ExecuteAsync(new Query("delete from SnapshotStore where Id = @Id").AddParameter(Columns.Id, OrderProcessId));
        }

        IDisposable scope = null;
        IDatabaseContext databaseContext = null;

        if (!createDatabaseContext)
        {
            databaseContext = databaseContextFactory.Create();
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

        databaseContext?.Dispose();
        scope?.Dispose();
    }
}