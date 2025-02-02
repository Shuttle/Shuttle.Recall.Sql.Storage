using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage;

public class EventTypeRepository : IEventTypeRepository
{
    private readonly Dictionary<string, Guid> _cache = new();
    private readonly SemaphoreSlim _lock = new(1,1);
    private readonly IEventTypeQueryFactory _queryFactory;

    public EventTypeRepository(IEventTypeQueryFactory queryFactory)
    {
        _queryFactory = Guard.AgainstNull(queryFactory);
    }

    public async Task<Guid> GetIdAsync(IDatabaseContext databaseContext, string typeName, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            var key = typeName.ToLower();

            if (!_cache.ContainsKey(key))
            {
                _cache.Add(key, await databaseContext.GetScalarAsync<Guid>(_queryFactory.GetId(typeName), cancellationToken: cancellationToken));
            }

            return _cache[key];
        }
        finally
        {
            _lock.Release();
        }
    }
}