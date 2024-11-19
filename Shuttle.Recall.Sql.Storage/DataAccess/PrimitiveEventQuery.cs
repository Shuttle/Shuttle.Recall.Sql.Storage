using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage;

public class PrimitiveEventQuery : IPrimitiveEventQuery
{
    private readonly IDatabaseContextService _databaseContextService;
    private readonly IEventTypeRepository _eventTypeRepository;
    private readonly IPrimitiveEventQueryFactory _queryFactory;
    private readonly IQueryMapper _queryMapper;

    public PrimitiveEventQuery(IDatabaseContextService databaseContextService, IQueryMapper queryMapper, IPrimitiveEventQueryFactory queryFactory, IEventTypeRepository eventTypeRepository)
    {
        _databaseContextService = Guard.AgainstNull(databaseContextService);
        _queryMapper = Guard.AgainstNull(queryMapper);
        _queryFactory = Guard.AgainstNull(queryFactory);
        _eventTypeRepository = Guard.AgainstNull(eventTypeRepository);
    }

    public async Task<IEnumerable<PrimitiveEvent>> SearchAsync(PrimitiveEventSpecification specification)
    {
        var databaseContext = _databaseContextService.Active;

        var eventTypeIds = new List<Guid>();

        foreach (var eventType in specification.EventTypes)
        {
            eventTypeIds.Add(await _eventTypeRepository.GetIdAsync(databaseContext, Guard.AgainstNullOrEmptyString(eventType.FullName)));
        }

        return await _queryMapper.MapObjectsAsync<PrimitiveEvent>(databaseContext, _queryFactory.Search(specification, eventTypeIds)).ConfigureAwait(false);
    }
}