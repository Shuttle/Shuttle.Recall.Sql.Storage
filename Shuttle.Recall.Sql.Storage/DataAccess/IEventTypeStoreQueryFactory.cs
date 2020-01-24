using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
    public interface IEventTypeStoreQueryFactory
    {
        IQuery GetId(string typeName);
    }
}