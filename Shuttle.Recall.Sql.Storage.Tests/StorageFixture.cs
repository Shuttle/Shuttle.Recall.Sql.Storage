using Moq;
using Ninject;
using NUnit.Framework;
using Shuttle.Core.Container;
using Shuttle.Core.Data;
using Shuttle.Core.Ninject;
using Shuttle.Recall.Tests;

namespace Shuttle.Recall.Sql.Storage.Tests
{
    public class StorageFixture : Fixture
    {
        [Test]
        public void ExerciseStorage()
        {
            var container = new NinjectComponentContainer(new StandardKernel());

            container.RegisterInstance(new Mock<IProjectionRepository>().Object);

            container.RegisterEventStore();
            container.RegisterDataAccess();
            container.RegisterEventStoreStorage();

            using (DatabaseContextFactory.Create(EventStoreConnectionStringName))
            {
                RecallFixture.ExerciseStorage(container.Resolve<IEventStore>());
            }
        }
    }
}