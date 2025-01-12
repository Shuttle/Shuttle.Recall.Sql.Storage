using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Recall.Sql.Storage;

public interface IEventTypeQuery
{
    Task<IEnumerable<EventType>> SearchAsync(EventType.Specification specification, CancellationToken cancellationToken = default);
}