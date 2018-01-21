using NUnit.Framework;
using Shuttle.Core.Data;
using Shuttle.Recall.Tests;

namespace Shuttle.Recall.Sql.Storage.Tests
{
	public class KeyStoreFixture : Fixture
	{
		[Test]
		public void Should_be_able_to_use_key_store()
		{
			var store = new KeyStore(DatabaseGateway,
				new KeyStoreQueryFactory(new ScriptProvider(new ScriptProviderConfiguration())));

			using (DatabaseContextFactory.Create(EventStoreConnectionStringName))
			{
				RecallFixture.ExcerciseKeyStore(store);
			}
		}
	}
}