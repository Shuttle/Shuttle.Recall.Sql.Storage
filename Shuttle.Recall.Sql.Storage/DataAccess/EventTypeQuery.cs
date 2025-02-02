using Shuttle.Core.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;

namespace Shuttle.Recall.Sql.Storage;

public class EventTypeQuery : IEventTypeQuery
{
    private readonly IQueryMapper _queryMapper;
    private readonly IEventTypeQueryFactory _queryFactory;

    public EventTypeQuery(IQueryMapper queryMapper, IEventTypeQueryFactory queryFactory)
    {
        _queryMapper = Guard.AgainstNull(queryMapper);
        _queryFactory = Guard.AgainstNull(queryFactory);
    }

    public async Task<IEnumerable<EventType>> SearchAsync(EventType.Specification specification, CancellationToken cancellationToken = default)
    {
        return await _queryMapper.MapObjectsAsync<EventType>(_queryFactory.Search(specification), cancellationToken).ConfigureAwait(false);
    }
}