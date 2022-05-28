using System;
using System.Transactions;
using NUnit.Framework;

namespace Shuttle.Recall.Sql.Storage.Tests
{
    public class KeyStoreFixture : Fixture
    {
        public static readonly Guid Id = new("047FF6FB-FB57-4F63-8795-99F252EDA62F");

        [Test]
        public void Should_be_able_to_use_key_store()
        {
            var store = new KeyStore(DatabaseGateway,
                new KeyStoreQueryFactory(new ScriptProvider(new ScriptProviderConfiguration())));

            using (new TransactionScope())
            using (DatabaseContextFactory.Create(EventStoreConnectionStringName))
            {
                var keyA = string.Concat("a=", Id.ToString());
                var keyB = string.Concat("b=", Id.ToString());

                store.Add(Id, keyA);

                Assert.Throws<DuplicateKeyException>(() => store.Add(Id, keyA),
                    $"Should not be able to add duplicate key / id = {Id} / key = '{keyA}' / (ensure that your implementation throws a `DuplicateKeyException`)");

                var id = store.Get(keyA);

                Assert.That(id, Is.Not.Null,
                    $"Should be able to retrieve the id of the associated key / id = {Id} / key = '{keyA}'");
                Assert.That(id, Is.EqualTo(Id),
                    $"Should be able to retrieve the correct id of the associated key / id = {Id} / key = '{keyA}' / id retrieved = {id}");

                Assert.That(store.Get(keyB), Is.Null,
                    $"Should not be able to get id of non-existent / id = {Id} / key = '{keyB}'");

                store.Remove(Id);

                Assert.That(store.Get(keyA), Is.Null,
                    $"Should be able to remove association using id (was not removed) / id = {Id} / key = '{keyA}'");

                store.Add(Id, keyA);
                store.Remove(keyA);

                Assert.That(store.Get(keyA), Is.Null,
                    $"Should be able to remove association using key (was not removed) / id = {Id} / key = '{keyA}'");

                Assert.That(store.Contains(keyA), Is.False,
                    $"Should not contain key A / key = '{keyA}'");
                Assert.That(store.Contains(keyB), Is.False,
                    $"Should not contain key B / key = '{keyB}'");

                store.Add(Id, keyB);

                Assert.That(store.Contains(keyA), Is.False,
                    $"Should not contain key A / key = '{keyA}'");
                Assert.That(store.Contains(keyB), Is.True,
                    $"Should contain key B / key = '{keyB}'");

                store.Rekey(Id, keyA);

                Assert.That(store.Contains(keyA), Is.True,
                    $"Should contain key A / key = '{keyA}'");
                Assert.That(store.Contains(keyB), Is.False,
                    $"Should not contain key B / key = '{keyB}'");

                store.Add(Id, keyB);

                Assert.That(store.Contains(keyA), Is.True,
                    $"Should contain key A / key = '{keyA}'");
                Assert.That(store.Contains(keyB), Is.True,
                    $"Should contain key B / key = '{keyB}'");

                store.Remove(Id);

                Assert.That(store.Contains(keyA), Is.False,
                    $"Should not contain key A / key = '{keyA}'");
                Assert.That(store.Contains(keyB), Is.False,
                    $"Should not contain key B / key = '{keyB}'");
            }
        }
    }
}