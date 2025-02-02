using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shuttle.Recall.Sql.Storage;

public interface IPrimitiveEventQuery
{
    Task<IEnumerable<PrimitiveEvent>> SearchAsync(PrimitiveEvent.Specification specification);
}