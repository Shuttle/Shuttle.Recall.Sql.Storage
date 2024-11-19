using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage;

public interface IEventTypeQueryFactory
{
    IQuery GetId(string typeName);
}