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
	    public IDatabaseContextCache DatabaseContextCache { get; private set; }

		[SetUp]
		public void TestSetUp()
		{
			DatabaseGateway = new DatabaseGateway();
            DatabaseContextCache = new ThreadStaticDatabaseContextCache();

#if (!NETCOREAPP2_1 && !NETSTANDARD2_0)
            DatabaseContextFactory = new DatabaseContextFactory(
                new ConnectionConfigurationProvider(),
                new DbConnectionFactory(), 
                new DbCommandFactory(), 
                new ThreadStaticDatabaseContextCache());
#else
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", System.Data.SqlClient.SqlClientFactory.Instance);

            var connectionConfigurationProvider = new Mock<IConnectionConfigurationProvider>();

		    connectionConfigurationProvider.Setup(m => m.Get(It.IsAny<string>())).Returns(
		        new ConnectionConfiguration(
		            "Shuttle",
		            "System.Data.SqlClient",
		            "Data Source=.\\sqlexpress;Initial Catalog=shuttle;Integrated Security=SSPI;"));

		    DatabaseContextFactory = new DatabaseContextFactory(
		        connectionConfigurationProvider.Object,
		        new DbConnectionFactory(),
		        new DbCommandFactory(),
		        new ThreadStaticDatabaseContextCache());
#endif

            ClearDataStore();
		}

		public DatabaseGateway DatabaseGateway { get; private set; }
		public IDatabaseContextFactory DatabaseContextFactory { get; private set; }

		public string EventStoreConnectionStringName = "EventStore";
		public string EventStoreProjectionConnectionStringName = "EventStoreProjection";

		[TearDown]
		protected void ClearDataStore()
		{
			using (DatabaseContextFactory.Create(EventStoreConnectionStringName))
			{
				DatabaseGateway.ExecuteUsing(RawQuery.Create("delete from EventStore where Id = @Id").AddParameterValue(EventStoreColumns.Id, RecallFixture.OrderId));
				DatabaseGateway.ExecuteUsing(RawQuery.Create("delete from EventStore where Id = @Id").AddParameterValue(EventStoreColumns.Id, RecallFixture.OrderProcessId));
			    DatabaseGateway.ExecuteUsing(RawQuery.Create("delete from KeyStore where Id = @Id").AddParameterValue(EventStoreColumns.Id, RecallFixture.OrderId));
				DatabaseGateway.ExecuteUsing(RawQuery.Create("delete from KeyStore where Id = @Id").AddParameterValue(EventStoreColumns.Id, RecallFixture.OrderProcessId));
                DatabaseGateway.ExecuteUsing(RawQuery.Create("delete from SnapshotStore where Id = @Id").AddParameterValue(EventStoreColumns.Id, RecallFixture.OrderId));
                DatabaseGateway.ExecuteUsing(RawQuery.Create("delete from SnapshotStore where Id = @Id").AddParameterValue(EventStoreColumns.Id, RecallFixture.OrderProcessId));
            }
        }
	}
}