using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage;

public class PrimitiveEventQuery : IPrimitiveEventQuery
{
    private readonly IDatabaseContextService _databaseContextService;
    private readonly IEventTypeRepository _eventTypeRepository;
    private readonly IPrimitiveEventQueryFactory _queryFactory;
    private readonly IQueryMapper _queryMapper;
    private readonly double _uncommittedToleranceSeconds;

    public PrimitiveEventQuery(IOptions<SqlStorageOptions> sqlStorageOptions, IDatabaseContextService databaseContextService, IQueryMapper queryMapper, IPrimitiveEventQueryFactory queryFactory, IEventTypeRepository eventTypeRepository)
    {
        _uncommittedToleranceSeconds = Guard.AgainstNull(Guard.AgainstNull(sqlStorageOptions).Value).UncommittedTolerance.TotalSeconds;
        _databaseContextService = Guard.AgainstNull(databaseContextService);
        _queryMapper = Guard.AgainstNull(queryMapper);
        _queryFactory = Guard.AgainstNull(queryFactory);
        _eventTypeRepository = Guard.AgainstNull(eventTypeRepository);
    }

    public async Task<IEnumerable<PrimitiveEvent>> SearchAsync(PrimitiveEvent.Specification specification)
    {
        var databaseContext = _databaseContextService.Active;

        var eventTypeIds = new List<Guid>();

        foreach (var eventType in specification.EventTypes)
        {
            eventTypeIds.Add(await _eventTypeRepository.GetIdAsync(databaseContext, eventType));
        }

        var sequenceNumberEnd = await databaseContext.GetScalarAsync<long?>(_queryFactory.GetUncommittedSequenceNumberStart(_uncommittedToleranceSeconds));

        if (sequenceNumberEnd.HasValue && sequenceNumberEnd < specification.SequenceNumberEnd)
        {
            specification.WithSequenceNumberEnd(sequenceNumberEnd.Value - 1);
        }

        return await _queryMapper.MapObjectsAsync<PrimitiveEvent>(_queryFactory.Search(specification, eventTypeIds)).ConfigureAwait(false);
    }
}