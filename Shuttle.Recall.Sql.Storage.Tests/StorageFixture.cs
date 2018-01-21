using Castle.Windsor;
using NUnit.Framework;
using Shuttle.Core.Castle;
using Shuttle.Core.Container;
using Shuttle.Core.Data;
using Shuttle.Recall.Tests;

namespace Shuttle.Recall.Sql.Storage.Tests
{
    public class StorageFixture : Fixture
    {
        [Test]
        public void ExerciseStorage()
        {
            var container = new WindsorComponentContainer(new WindsorContainer());

            EventStore.Register(container);

            using (container.Resolve<IDatabaseContextFactory>().Create(EventStoreConnectionStringName))
            {
                RecallFixture.ExerciseStorage(EventStore.Create(container));
            }
        }
    }
}