using System;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shuttle.Core.Data;
using Shuttle.Core.Reflection;

namespace Shuttle.Recall.Sql.Storage.Tests;

public class KeyStoreFixture
{
    public static readonly Guid Id = new("047FF6FB-FB57-4F63-8795-99F252EDA62F");

    [Test]
    public void Should_be_able_to_use_key_store()
    {
        Should_be_able_to_use_key_store_async(false, true).GetAwaiter().GetResult();
    }

    [Test]
    public async Task Should_be_able_to_use_key_store_async()
    {
        await Should_be_able_to_use_key_store_async(false, false);
    }

    [Test]
    public void Should_be_able_to_use_key_store_with_managed_connections()
    {
        Should_be_able_to_use_key_store_async(true, true).GetAwaiter().GetResult();
    }

    [Test]
    public async Task Should_be_able_to_use_key_store_with_managed_connections_async()
    {
        await Should_be_able_to_use_key_store_async(true, false);
    }

    private async Task Should_be_able_to_use_key_store_async(bool manageEventStoreConnections, bool sync)
    {
        var services = SqlConfiguration.GetServiceCollection();

        var serviceProvider = services.BuildServiceProvider();

        var databaseContextService = serviceProvider.GetRequiredService<IDatabaseContextService>();
        var databaseContextFactory = serviceProvider.GetRequiredService<IDatabaseContextFactory>();
        var databaseGateway = serviceProvider.GetRequiredService<IDatabaseGateway>();
        var keyStore = serviceProvider.GetRequiredService<IKeyStore>();

        await using (databaseContextFactory.Create())
        {
            var query = new Query("delete from [dbo].[KeyStore] where Id = @Id").AddParameter(Columns.Id, Id);

            if (sync)
            {
                databaseGateway.Execute(query);
            }
            else
            {
                await databaseGateway.ExecuteAsync(query);
            }
        }

        using (new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            IDisposable scope = null;
            IDatabaseContext databaseContext = null;

            if (!manageEventStoreConnections)
            {
                databaseContext = databaseContextFactory.Create();
            }

            var keyA = string.Concat("a=", Id.ToString());
            var keyB = string.Concat("b=", Id.ToString());

            if (sync)
            {
                keyStore.Add(Id, keyA);
            }
            else
            {
                await keyStore.AddAsync(Id, keyA);
            }

            Assert.Throws<DuplicateKeyException>(() => keyStore.Add(Id, keyA), $"Should not be able to add duplicate key / id = {Id} / key = '{keyA}' / (ensure that your implementation throws a `DuplicateKeyException`)");

            var id = sync ? keyStore.Find(keyA) : await keyStore.FindAsync(keyA);

            Assert.That(id, Is.Not.Null, $"Should be able to retrieve the id of the associated key / id = {Id} / key = '{keyA}'");
            Assert.That(id, Is.EqualTo(Id), $"Should be able to retrieve the correct id of the associated key / id = {Id} / key = '{keyA}' / id retrieved = {id}");

            Assert.That(sync ? keyStore.Find(keyB) : await keyStore.FindAsync(keyB), Is.Null, $"Should not be able to get id of non-existent / id = {Id} / key = '{keyB}'");

            if (sync)
            {
                keyStore.Remove(Id);
            }
            else
            {
                await keyStore.RemoveAsync(Id);
            }

            Assert.That(keyStore.Find(keyA), Is.Null, $"Should be able to remove association using id (was not removed) / id = {Id} / key = '{keyA}'");

            if (sync)
            {
                keyStore.Add(Id, keyA);
                keyStore.Remove(keyA);
            }
            else
            {
                await keyStore.AddAsync(Id, keyA);
                await keyStore.RemoveAsync(keyA);
            }

            Assert.That(sync ? keyStore.Find(keyA) : await keyStore.FindAsync(keyA), Is.Null, $"Should be able to remove association using key (was not removed) / id = {Id} / key = '{keyA}'");

            Assert.That(sync ? keyStore.Contains(keyA) : await keyStore.ContainsAsync(keyA), Is.False, $"Should not contain key A / key = '{keyA}'");
            Assert.That(sync ? keyStore.Contains(keyB) : await keyStore.ContainsAsync(keyB), Is.False, $"Should not contain key B / key = '{keyB}'");

            if (sync)
            {
                keyStore.Add(Id, keyB);
            }
            else
            {
                await keyStore.AddAsync(Id, keyB);
            }

            Assert.That(sync ? keyStore.Contains(keyA) : await keyStore.ContainsAsync(keyA), Is.False, $"Should not contain key A / key = '{keyA}'");
            Assert.That(sync ? keyStore.Contains(keyB) : await keyStore.ContainsAsync(keyB), Is.True, $"Should contain key B / key = '{keyB}'");

            if (sync)
            {
                keyStore.Rekey(keyB, keyA);
            }
            else
            {
                await keyStore.RekeyAsync(keyB, keyA);
            }

            Assert.That(sync ? keyStore.Contains(keyA) : await keyStore.ContainsAsync(keyA), Is.True, $"Should contain key A / key = '{keyA}'");
            Assert.That(sync ? keyStore.Contains(keyB) : await keyStore.ContainsAsync(keyB), Is.False, $"Should not contain key B / key = '{keyB}'");

            if (sync)
            {
                keyStore.Add(Id, keyB);
            }
            else
            {
                await keyStore.AddAsync(Id, keyB);
            }

            Assert.That(sync ? keyStore.Contains(keyA) : await keyStore.ContainsAsync(keyA), Is.True, $"Should contain key A / key = '{keyA}'");
            Assert.That(sync ? keyStore.Contains(keyB) : await keyStore.ContainsAsync(keyB), Is.True, $"Should contain key B / key = '{keyB}'");

            if (sync)
            {
                Assert.That(() => keyStore.Rekey(keyA, keyB), Throws.TypeOf<DuplicateKeyException>());
            }
            else
            {
                Assert.That(async () => await keyStore.RekeyAsync(keyA, keyB), Throws.TypeOf<DuplicateKeyException>());
            }

            if (sync)
            {
                keyStore.Remove(Id);
            }
            else
            {
                await keyStore.RemoveAsync(Id);
            }

            Assert.That(sync ? keyStore.Contains(keyA) : await keyStore.ContainsAsync(keyA), Is.False, $"Should not contain key A / key = '{keyA}'");
            Assert.That(sync ? keyStore.Contains(keyB) : await keyStore.ContainsAsync(keyB), Is.False, $"Should not contain key B / key = '{keyB}'");

            await databaseContext.TryDisposeAsync();
            await scope.TryDisposeAsync();
        }
    }
}