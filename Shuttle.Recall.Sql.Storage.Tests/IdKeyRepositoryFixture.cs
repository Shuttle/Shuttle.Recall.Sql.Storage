using System;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Shuttle.Core.Data;
using Shuttle.Recall.Sql.Storage.DataAccess;
using Shuttle.Recall.Tests;

namespace Shuttle.Recall.Sql.Storage.Tests;

public class IdKeyRepositoryFixture
{
    public static readonly Guid Id = new("047FF6FB-FB57-4F63-8795-99F252EDA62F");

    [Test]
    public async Task Should_be_able_to_use_repository_async()
    {
        var services = SqlConfiguration.GetServiceCollection()
                .AddEventStore()
                .ConfigureLogging(nameof(IdKeyRepositoryFixture));

        var serviceProvider = services.BuildServiceProvider();

        var databaseContextFactory = serviceProvider.GetRequiredService<IDatabaseContextFactory>();
        var repository = serviceProvider.GetRequiredService<IIdKeyRepository>();
        var options = serviceProvider.GetRequiredService<IOptions<SqlStorageOptions>>().Value;

        await serviceProvider.StartHostedServicesAsync().ConfigureAwait(false);

        await using (var databaseContext = databaseContextFactory.Create())
        {
            await databaseContext.ExecuteAsync(new Query(@$"
IF OBJECT_ID ('{options.Schema}.IdKey', 'U') IS NOT NULL 
BEGIN
    DELETE FROM [{options.Schema}].[IdKey] WHERE Id = @Id
END
").AddParameter(Columns.Id, Id));
        }

        using (new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        await using (databaseContextFactory.Create())
        {
            var keyA = string.Concat("a=", Id.ToString());
            var keyB = string.Concat("b=", Id.ToString());

            await repository.AddAsync(Id, keyA);

            Assert.ThrowsAsync<DuplicateKeyException>(async () => await repository.AddAsync(Id, keyA), $"Should not be able to add duplicate key / id = {Id} / key = '{keyA}' / (ensure that your implementation throws a `DuplicateKeyException`)");

            var id = await repository.FindAsync(keyA);

            Assert.That(id, Is.Not.Null, $"Should be able to retrieve the id of the associated key / id = {Id} / key = '{keyA}'");
            Assert.That(id, Is.EqualTo(Id), $"Should be able to retrieve the correct id of the associated key / id = {Id} / key = '{keyA}' / id retrieved = {id}");

            Assert.That(await repository.FindAsync(keyB), Is.Null, $"Should not be able to get id of non-existent / id = {Id} / key = '{keyB}'");

            await repository.RemoveAsync(Id);

            Assert.That(await repository.FindAsync(keyA), Is.Null, $"Should be able to remove association using id (was not removed) / id = {Id} / key = '{keyA}'");

            await repository.AddAsync(Id, keyA);
            await repository.RemoveAsync(keyA);

            Assert.That(await repository.FindAsync(keyA), Is.Null, $"Should be able to remove association using key (was not removed) / id = {Id} / key = '{keyA}'");

            Assert.That(await repository.ContainsAsync(keyA), Is.False, $"Should not contain key A / key = '{keyA}'");
            Assert.That(await repository.ContainsAsync(keyB), Is.False, $"Should not contain key B / key = '{keyB}'");

            await repository.AddAsync(Id, keyB);

            Assert.That(await repository.ContainsAsync(keyA), Is.False, $"Should not contain key A / key = '{keyA}'");
            Assert.That(await repository.ContainsAsync(keyB), Is.True, $"Should contain key B / key = '{keyB}'");

            await repository.RekeyAsync(keyB, keyA);

            Assert.That(await repository.ContainsAsync(keyA), Is.True, $"Should contain key A / key = '{keyA}'");
            Assert.That(await repository.ContainsAsync(keyB), Is.False, $"Should not contain key B / key = '{keyB}'");

            await repository.AddAsync(Id, keyB);

            Assert.That(await repository.ContainsAsync(keyA), Is.True, $"Should contain key A / key = '{keyA}'");
            Assert.That(await repository.ContainsAsync(keyB), Is.True, $"Should contain key B / key = '{keyB}'");

            Assert.That(async () => await repository.RekeyAsync(keyA, keyB), Throws.TypeOf<DuplicateKeyException>());

            await repository.RemoveAsync(Id);

            Assert.That(await repository.ContainsAsync(keyA), Is.False, $"Should not contain key A / key = '{keyA}'");
            Assert.That(await repository.ContainsAsync(keyB), Is.False, $"Should not contain key B / key = '{keyB}'");
        }
    }
}