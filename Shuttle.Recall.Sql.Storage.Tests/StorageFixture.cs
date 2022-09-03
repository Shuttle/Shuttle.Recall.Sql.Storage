using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Shuttle.Core.Data;
using Shuttle.Recall.Tests;

namespace Shuttle.Recall.Sql.Storage.Tests
{
    public class StorageFixture : Fixture
    {
        [Test]
        public void ExerciseStorage()
        {
            var services = new ServiceCollection();

            services.AddSingleton(new Mock<IProjectionRepository>().Object);

            services.AddDataAccess();
            services.AddEventStore();
            services.AddSqlEventStorage();

            using (DatabaseContextFactory.Create(EventStoreConnectionStringName))
            {
                RecallFixture.ExerciseStorage(services.BuildServiceProvider().GetRequiredService<IEventStore>());
            }
        }
    }
}