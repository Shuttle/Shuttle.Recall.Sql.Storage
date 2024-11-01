using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage;

public class PrimitiveEventQuery : IPrimitiveEventQuery
{
    private readonly IDatabaseContextService _databaseContextService;
    private readonly IEventTypeStore _eventTypeStore;
    private readonly IPrimitiveEventQueryFactory _queryFactory;
    private readonly IQueryMapper _queryMapper;

    public PrimitiveEventQuery(IDatabaseContextService databaseContextService, IQueryMapper queryMapper, IPrimitiveEventQueryFactory queryFactory, IEventTypeStore eventTypeStore)
    {
        _databaseContextService = Guard.AgainstNull(databaseContextService);
        _queryMapper = Guard.AgainstNull(queryMapper);
        _queryFactory = Guard.AgainstNull(queryFactory);
        _eventTypeStore = Guard.AgainstNull(eventTypeStore);
    }

    public async Task<IEnumerable<PrimitiveEvent>> SearchAsync(PrimitiveEvent.Specification specification)
    {
        var databaseContext = _databaseContextService.Active;

        var eventTypeIds = new List<Guid>();

        foreach (var eventType in specification.EventTypes)
        {
            eventTypeIds.Add(await _eventTypeStore.GetIdAsync(databaseContext, Guard.AgainstNullOrEmptyString(eventType.FullName)));
        }

        return await _queryMapper.MapObjectsAsync<PrimitiveEvent>(databaseContext, _queryFactory.Search(specification, eventTypeIds)).ConfigureAwait(false);
    }
}