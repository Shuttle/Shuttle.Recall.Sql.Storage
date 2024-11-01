using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage;

public class PrimitiveEventRepository : IPrimitiveEventRepository
{
    private readonly IDatabaseContextService _databaseContextService;
    private readonly IEventTypeStore _eventTypeStore;
    private readonly IPrimitiveEventQueryFactory _queryFactory;
    private readonly IQueryMapper _queryMapper;

    public PrimitiveEventRepository(IDatabaseContextService databaseContextService, IQueryMapper queryMapper, IPrimitiveEventQueryFactory queryFactory, IEventTypeStore eventTypeStore)
    {
        _databaseContextService = Guard.AgainstNull(databaseContextService);
        _queryMapper = Guard.AgainstNull(queryMapper);
        _queryFactory = Guard.AgainstNull(queryFactory);
        _eventTypeStore = Guard.AgainstNull(eventTypeStore);
    }

    public async Task RemoveAsync(Guid id)
    {
        await _databaseContextService.Active.ExecuteAsync(_queryFactory.RemoveEventStream(id)).ConfigureAwait(false);
    }

    public async ValueTask<long> SaveAsync(IEnumerable<PrimitiveEvent> primitiveEvents)
    {
        var databaseContext = _databaseContextService.Active;
        
        long result = 0;

        foreach (var primitiveEvent in primitiveEvents)
        {
            var eventTypeId = await _eventTypeStore.GetIdAsync(databaseContext, primitiveEvent.EventType).ConfigureAwait(false);

            result = await databaseContext.GetScalarAsync<long>(_queryFactory.SaveEvent(primitiveEvent, eventTypeId)).ConfigureAwait(false);
        }

        return result;
    }


    public async Task<IEnumerable<PrimitiveEvent>> GetAsync(Guid id)
    {
        return await _queryMapper.MapObjectsAsync<PrimitiveEvent>(_databaseContextService.Active, _queryFactory.GetEventStream(id)).ConfigureAwait(false);
    }

    public async ValueTask<long> GetSequenceNumberAsync(Guid id)
    {
        return await _databaseContextService.Active.GetScalarAsync<long>(_queryFactory.GetSequenceNumber(id));
    }
}