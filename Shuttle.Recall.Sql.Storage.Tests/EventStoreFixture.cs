using System.Reflection;
using Castle.Windsor;
using NUnit.Framework;
using Shuttle.Core.Castle;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.Recall.Tests;

namespace Shuttle.Recall.Sql.Storage.Tests
{
    public class EventStoreFixture : Fixture
    {
        [Test]
        public void ExerciseEventStore()
        {
            var container = new WindsorComponentContainer(new WindsorContainer());

	        EventStore.Register(container);

            using (container.Resolve<IDatabaseContextFactory>().Create(EventStoreConnectionStringName))
            {
                RecallFixture.ExerciseEventStore(EventStore.Create(container), EventProcessor.Create(container));
            }
        }
    }
}