using System.Data.Common;
using Moq;
using NUnit.Framework;
using Shuttle.Core.Data;
using Shuttle.Recall.Tests;

namespace Shuttle.Recall.Sql.Storage.Tests
{
	[TestFixture]
	public class Fixture
	{
		[SetUp]
		public void TestSetUp()
		{
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", System.Data.SqlClient.SqlClientFactory.Instance);

            var connectionConfigurationProvider = new Mock<IConnectionConfigurationProvider>();

            connectionConfigurationProvider.Setup(m => m.Get(It.IsAny<string>())).Returns(
                new ConnectionConfiguration(
                    "Shuttle",
                    "System.Data.SqlClient",
                    "Data Source=.\\sqlexpress;Initial Catalog=Shuttle;Integrated Security=SSPI;"));

            DatabaseContextFactory = new DatabaseContextFactory(
                connectionConfigurationProvider.Object,
                new DbConnectionFactory(),
                new DbCommandFactory(),
                new ThreadStaticDatabaseContextCache());

			ClearDataStore();
		}

		public DatabaseGateway DatabaseGateway { get; } = new DatabaseGateway();
		public IDatabaseContextFactory DatabaseContextFactory { get; private set; }

		public string EventStoreConnectionStringName = "EventStore";

		[TearDown]
		protected void ClearDataStore()
		{
			using (DatabaseContextFactory.Create(EventStoreConnectionStringName))
			{
				DatabaseGateway.ExecuteUsing(RawQuery.Create("delete from EventStore where Id = @Id").AddParameterValue(Columns.Id, RecallFixture.OrderId));
				DatabaseGateway.ExecuteUsing(RawQuery.Create("delete from EventStore where Id = @Id").AddParameterValue(Columns.Id, RecallFixture.OrderProcessId));
			    DatabaseGateway.ExecuteUsing(RawQuery.Create("delete from KeyStore where Id = @Id").AddParameterValue(Columns.Id, RecallFixture.OrderId));
				DatabaseGateway.ExecuteUsing(RawQuery.Create("delete from KeyStore where Id = @Id").AddParameterValue(Columns.Id, RecallFixture.OrderProcessId));
                DatabaseGateway.ExecuteUsing(RawQuery.Create("delete from SnapshotStore where Id = @Id").AddParameterValue(Columns.Id, RecallFixture.OrderId));
                DatabaseGateway.ExecuteUsing(RawQuery.Create("delete from SnapshotStore where Id = @Id").AddParameterValue(Columns.Id, RecallFixture.OrderProcessId));
            }
        }
	}
}