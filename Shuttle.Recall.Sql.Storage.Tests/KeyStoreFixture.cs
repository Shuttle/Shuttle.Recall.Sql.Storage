using System;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage.Tests;

public class KeyStoreFixture
{
    public static readonly Guid Id = new("047FF6FB-FB57-4F63-8795-99F252EDA62F");

    [Test]
    public async Task Should_be_able_to_use_key_store_async()
    {
        var services = SqlConfiguration.GetServiceCollection();

        var serviceProvider = services.BuildServiceProvider();

        var databaseContextFactory = serviceProvider.GetRequiredService<IDatabaseContextFactory>();
        var keyStore = serviceProvider.GetRequiredService<IKeyStore>();

        await using (var databaseContext = databaseContextFactory.Create())
        {
            await databaseContext.ExecuteAsync(new Query("delete from [dbo].[KeyStore] where Id = @Id").AddParameter(Columns.Id, Id));
        }

        using (new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var keyA = string.Concat("a=", Id.ToString());
            var keyB = string.Concat("b=", Id.ToString());

                await keyStore.AddAsync(Id, keyA);

            Assert.ThrowsAsync<DuplicateKeyException>(async () => await keyStore.AddAsync(Id, keyA), $"Should not be able to add duplicate key / id = {Id} / key = '{keyA}' / (ensure that your implementation throws a `DuplicateKeyException`)");

            var id = await keyStore.FindAsync(keyA);

            Assert.That(id, Is.Not.Null, $"Should be able to retrieve the id of the associated key / id = {Id} / key = '{keyA}'");
            Assert.That(id, Is.EqualTo(Id), $"Should be able to retrieve the correct id of the associated key / id = {Id} / key = '{keyA}' / id retrieved = {id}");

            Assert.That(await keyStore.FindAsync(keyB), Is.Null, $"Should not be able to get id of non-existent / id = {Id} / key = '{keyB}'");

            await keyStore.RemoveAsync(Id);

            Assert.That(await keyStore.FindAsync(keyA), Is.Null, $"Should be able to remove association using id (was not removed) / id = {Id} / key = '{keyA}'");

            await keyStore.AddAsync(Id, keyA);
            await keyStore.RemoveAsync(keyA);

            Assert.That(await keyStore.FindAsync(keyA), Is.Null, $"Should be able to remove association using key (was not removed) / id = {Id} / key = '{keyA}'");

            Assert.That(await keyStore.ContainsAsync(keyA), Is.False, $"Should not contain key A / key = '{keyA}'");
            Assert.That(await keyStore.ContainsAsync(keyB), Is.False, $"Should not contain key B / key = '{keyB}'");

            await keyStore.AddAsync(Id, keyB);

            Assert.That(await keyStore.ContainsAsync(keyA), Is.False, $"Should not contain key A / key = '{keyA}'");
            Assert.That(await keyStore.ContainsAsync(keyB), Is.True, $"Should contain key B / key = '{keyB}'");

            await keyStore.RekeyAsync(keyB, keyA);

            Assert.That(await keyStore.ContainsAsync(keyA), Is.True, $"Should contain key A / key = '{keyA}'");
            Assert.That(await keyStore.ContainsAsync(keyB), Is.False, $"Should not contain key B / key = '{keyB}'");

            await keyStore.AddAsync(Id, keyB);

            Assert.That(await keyStore.ContainsAsync(keyA), Is.True, $"Should contain key A / key = '{keyA}'");
            Assert.That(await keyStore.ContainsAsync(keyB), Is.True, $"Should contain key B / key = '{keyB}'");

            Assert.That(async () => await keyStore.RekeyAsync(keyA, keyB), Throws.TypeOf<DuplicateKeyException>());

            await keyStore.RemoveAsync(Id);

            Assert.That(await keyStore.ContainsAsync(keyA), Is.False, $"Should not contain key A / key = '{keyA}'");
            Assert.That(await keyStore.ContainsAsync(keyB), Is.False, $"Should not contain key B / key = '{keyB}'");
        }
    }
}