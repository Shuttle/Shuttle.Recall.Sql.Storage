using System;

namespace Shuttle.Recall.Sql.Storage
{
    public interface IEventTypeStore
    {
        Guid GetId(string typeName);
    }
}