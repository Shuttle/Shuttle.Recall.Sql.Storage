using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage.DataAccess;

public class IdKeyRepository : IIdKeyRepository
{
    private readonly IDatabaseContextService _databaseContextService;
    private readonly IIdKeyQueryFactory _queryFactory;

    public IdKeyRepository(IDatabaseContextService databaseContextService, IIdKeyQueryFactory queryFactory)
    {
        _databaseContextService = Guard.AgainstNull(databaseContextService);
        _queryFactory = Guard.AgainstNull(queryFactory);
    }

    public async ValueTask<bool> ContainsAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _databaseContextService.Active.GetScalarAsync<int>(_queryFactory.Contains(key), cancellationToken).ConfigureAwait(false) == 1;
    }

    public async ValueTask<bool> ContainsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _databaseContextService.Active.GetScalarAsync<int>(_queryFactory.Contains(id), cancellationToken).ConfigureAwait(false) == 1;
    }

    public async ValueTask<Guid?> FindAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _databaseContextService.Active.GetScalarAsync<Guid?>(_queryFactory.Get(key), cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _databaseContextService.Active.ExecuteAsync(_queryFactory.Remove(key), cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _databaseContextService.Active.ExecuteAsync(_queryFactory.Remove(id), cancellationToken).ConfigureAwait(false);
    }

    public async Task AddAsync(Guid id, string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _databaseContextService.Active.ExecuteAsync(_queryFactory.Add(id, key), cancellationToken).ConfigureAwait(false);
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
            await _databaseContextService.Active.ExecuteAsync(_queryFactory.Rekey(key, rekey), cancellationToken).ConfigureAwait(false);
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