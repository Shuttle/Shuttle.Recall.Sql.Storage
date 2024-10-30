using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage;

public class KeyStore : IKeyStore
{
    private readonly IDatabaseContextFactory _databaseContextFactory;
    private readonly IKeyStoreQueryFactory _queryFactory;
    private readonly SqlStorageOptions _sqlStorageOptions;

    public KeyStore(IOptions<SqlStorageOptions> sqlStorageOptions, IDatabaseContextFactory databaseContextFactory, IKeyStoreQueryFactory queryFactory)
    {
        _sqlStorageOptions = Guard.AgainstNull(Guard.AgainstNull(sqlStorageOptions).Value);
        _databaseContextFactory = Guard.AgainstNull(databaseContextFactory);
        _queryFactory = Guard.AgainstNull(queryFactory);
    }

    public async ValueTask<bool> ContainsAsync(string key, CancellationToken cancellationToken = default)
    {
        await using var databaseContext = _databaseContextFactory.Create(_sqlStorageOptions.ConnectionStringName);
        return await databaseContext.GetScalarAsync<int>(_queryFactory.Contains(key), cancellationToken).ConfigureAwait(false) == 1;
    }

    public async ValueTask<bool> ContainsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var databaseContext = _databaseContextFactory.Create(_sqlStorageOptions.ConnectionStringName);
        return await databaseContext.GetScalarAsync<int>(_queryFactory.Contains(id), cancellationToken).ConfigureAwait(false) == 1;
    }

    public async ValueTask<Guid?> FindAsync(string key, CancellationToken cancellationToken = default)
    {
        await using var databaseContext = _databaseContextFactory.Create(_sqlStorageOptions.ConnectionStringName);
        return await databaseContext.GetScalarAsync<Guid?>(_queryFactory.Get(key), cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await using var databaseContext = _databaseContextFactory.Create(_sqlStorageOptions.ConnectionStringName);
        await databaseContext.ExecuteAsync(_queryFactory.Remove(key), cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var databaseContext = _databaseContextFactory.Create(_sqlStorageOptions.ConnectionStringName);
        await databaseContext.ExecuteAsync(_queryFactory.Remove(id), cancellationToken).ConfigureAwait(false);
    }

    public async Task AddAsync(Guid id, string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var databaseContext = _databaseContextFactory.Create(_sqlStorageOptions.ConnectionStringName);
            await databaseContext.ExecuteAsync(_queryFactory.Add(id, key), cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (ex.Message.ToLower().Contains("violation of primary key constraint"))
            {
                throw new DuplicateKeyException(id, key);
            }

            throw;
        }
    }

    public async Task RekeyAsync(string key, string rekey, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var databaseContext = _databaseContextFactory.Create(_sqlStorageOptions.ConnectionStringName);
            await databaseContext.ExecuteAsync(_queryFactory.Rekey(key, rekey), cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (ex.Message.ToLower().Contains("violation of primary key constraint"))
            {
                throw new DuplicateKeyException(await FindAsync(key, cancellationToken) ?? Guid.Empty, key);
            }

            throw;
        }
    }
}