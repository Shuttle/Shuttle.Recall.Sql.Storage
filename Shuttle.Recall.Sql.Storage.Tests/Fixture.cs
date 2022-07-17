using System.Data.Common;
using Microsoft.Extensions.Options;
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

            var connectionStringOptions = new Mock<IOptionsMonitor<ConnectionStringOptions>>();

            connectionStringOptions.Setup(m => m.Get(It.IsAny<string>())).Returns(new ConnectionStringOptions
            {
	            Name = "shuttle",
	            ProviderName = "System.Data.SqlClient",
	            ConnectionString = "server=.;database=Shuttle;user id=sa;password=Pass!000"
			});

            ConnectionStringOptions = connectionStringOptions.Object;

            DatabaseContextCache = new ThreadStaticDatabaseContextCache();

            DatabaseGateway = new DatabaseGateway(DatabaseContextCache);
            
            DatabaseContextFactory = new DatabaseContextFactory(
	            ConnectionStringOptions,
	            new DbConnectionFactory(),
	            new DbCommandFactory(Options.Create(new CommandOptions())),
	            DatabaseContextCache);
			
            ClearDataStore();
		}

		public DatabaseGateway DatabaseGateway { get; private set; }
		public IDatabaseContextCache DatabaseContextCache { get; private set; }
		public IDatabaseContextFactory DatabaseContextFactory { get; private set; }
		public IOptionsMonitor<ConnectionStringOptions> ConnectionStringOptions { get; private set; }

		public string EventStoreConnectionStringName = "EventStore";

		[TearDown]
		protected void ClearDataStore()
		{
			using (DatabaseContextFactory.Create(EventStoreConnectionStringName))
			{
				DatabaseGateway.Execute(RawQuery.Create("delete from EventStore where Id = @Id").AddParameterValue(Columns.Id, RecallFixture.OrderId));
				DatabaseGateway.Execute(RawQuery.Create("delete from EventStore where Id = @Id").AddParameterValue(Columns.Id, RecallFixture.OrderProcessId));
			    DatabaseGateway.Execute(RawQuery.Create("delete from KeyStore where Id = @Id").AddParameterValue(Columns.Id, RecallFixture.OrderId));
				DatabaseGateway.Execute(RawQuery.Create("delete from KeyStore where Id = @Id").AddParameterValue(Columns.Id, RecallFixture.OrderProcessId));
                DatabaseGateway.Execute(RawQuery.Create("delete from SnapshotStore where Id = @Id").AddParameterValue(Columns.Id, RecallFixture.OrderId));
                DatabaseGateway.Execute(RawQuery.Create("delete from SnapshotStore where Id = @Id").AddParameterValue(Columns.Id, RecallFixture.OrderProcessId));
            }
        }
	}
}