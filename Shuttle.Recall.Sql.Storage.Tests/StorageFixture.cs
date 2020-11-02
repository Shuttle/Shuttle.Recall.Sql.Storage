using Castle.Windsor;
using Moq;
using NUnit.Framework;
using Shuttle.Core.Castle;
using Shuttle.Core.Container;
using Shuttle.Recall.Tests;

namespace Shuttle.Recall.Sql.Storage.Tests
{
    public class StorageFixture : Fixture
    {
        [Test]
        public void ExerciseStorage()
        {
            var container = new WindsorComponentContainer(new WindsorContainer());

            container.RegisterInstance(new Mock<IProjectionRepository>().Object);

            EventStore.Register(container);

            using (DatabaseContextFactory.Create(EventStoreConnectionStringName))
            {
                RecallFixture.ExerciseStorage(EventStore.Create(container));
            }
        }
    }
}