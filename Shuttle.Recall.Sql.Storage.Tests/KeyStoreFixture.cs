using System;
using NUnit.Framework;

namespace Shuttle.Recall.Sql.Storage.Tests
{
	public class KeyStoreFixture : Fixture
	{
	    public static readonly Guid OrderId = new Guid("047FF6FB-FB57-4F63-8795-99F252EDA62F");

        [Test]
		public void Should_be_able_to_use_key_store()
		{
			var store = new KeyStore(DatabaseGateway,
				new KeyStoreQueryFactory(new ScriptProvider(new ScriptProviderConfiguration())));

			using (DatabaseContextFactory.Create(EventStoreConnectionStringName))
			{
			    var id = OrderId;

			    var value = string.Concat("value=", id.ToString());
			    var anotherValue = string.Concat("anotherValue=", id.ToString());

			    store.Add(id, value);

			    Assert.Throws<DuplicateKeyException>(() => store.Add(id, value),
			        $"Should not be able to add duplicate key / id = {id} / key = '{value}' / (ensure that your implementation throws a `DuplicateKeyException`)");

			    var idGet = store.Get(value);

			    Assert.IsNotNull(idGet,
			        $"Should be able to retrieve the id of the associated key / id = {id} / key = '{value}'");
			    Assert.AreEqual(id, idGet,
			        $"Should be able to retrieve the correct id of the associated key / id = {id} / key = '{value}' / id retrieved = {idGet}");

			    idGet = store.Get(anotherValue);

			    Assert.IsNull(idGet, $"Should not be able to get id of non-existent / id = {id} / key = '{anotherValue}'");

			    store.Remove(id);

			    idGet = store.Get(value);

			    Assert.IsNull(idGet,
			        $"Should be able to remove association using id (was not removed) / id = {id} / key = '{value}'");

			    store.Add(id, value);
			    store.Remove(value);

			    idGet = store.Get(value);

			    Assert.IsNull(idGet,
			        $"Should be able to remove association using key (was not removed) / id = {id} / key = '{value}'");
			}
        }
	}
}