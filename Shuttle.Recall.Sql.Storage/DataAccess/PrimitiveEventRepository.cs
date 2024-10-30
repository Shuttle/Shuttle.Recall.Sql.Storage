using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage;

public class PrimitiveEventRepository : IPrimitiveEventRepository
{
    private readonly IEventTypeStore _eventTypeStore;
    private readonly IDatabaseContextFactory _databaseContextFactory;
    private readonly IPrimitiveEventQueryFactory _queryFactory;
    private readonly IQueryMapper _queryMapper;
    private readonly SqlStorageOptions _sqlStorageOptions;

    public PrimitiveEventRepository(IOptions<SqlStorageOptions> sqlStorageOptions, IDatabaseContextFactory databaseContextFactory, IQueryMapper queryMapper, IPrimitiveEventQueryFactory queryFactory, IEventTypeStore eventTypeStore)
    {
        _sqlStorageOptions = Guard.AgainstNull(Guard.AgainstNull(sqlStorageOptions).Value);
        _databaseContextFactory = Guard.AgainstNull(databaseContextFactory);
        _queryMapper = Guard.AgainstNull(queryMapper);
        _queryFactory = Guard.AgainstNull(queryFactory);
        _eventTypeStore = Guard.AgainstNull(eventTypeStore);
    }

    public async Task RemoveAsync(Guid id)
    {
        await using var databaseContext = _databaseContextFactory.Create(_sqlStorageOptions.ConnectionStringName);
        await databaseContext.ExecuteAsync(_queryFactory.RemoveEventStream(id)).ConfigureAwait(false);
    }

    public async ValueTask<long> SaveAsync(IEnumerable<PrimitiveEvent> primitiveEvents)
    {
        await using var databaseContext = _databaseContextFactory.Create(_sqlStorageOptions.ConnectionStringName);
        await using (var transaction = await databaseContext.BeginTransactionAsync())
        {
            long result = 0;

            foreach (var primitiveEvent in primitiveEvents)
            {
                var eventTypeId = await _eventTypeStore.GetIdAsync(databaseContext, primitiveEvent.EventType).ConfigureAwait(false);

                result = await databaseContext.GetScalarAsync<long>(_queryFactory.SaveEvent(primitiveEvent, eventTypeId)).ConfigureAwait(false);
            }

            await transaction.CommitTransactionAsync();

            return result;
        }
    }


    public async Task<IEnumerable<PrimitiveEvent>> GetAsync(Guid id)
    {
        await using var databaseContext = _databaseContextFactory.Create(_sqlStorageOptions.ConnectionStringName);
        return await _queryMapper.MapObjectsAsync<PrimitiveEvent>(databaseContext, _queryFactory.GetEventStream(id)).ConfigureAwait(false);
    }

    public async ValueTask<long> GetSequenceNumberAsync(Guid id)
    {
        await using var databaseContext = _databaseContextFactory.Create(_sqlStorageOptions.ConnectionStringName);
        return await databaseContext.GetScalarAsync<long>(_queryFactory.GetSequenceNumber(id));
    }
}