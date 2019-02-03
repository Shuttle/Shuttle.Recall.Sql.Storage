using System;
using System.Collections.Generic;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
    public interface IPrimitiveEventQueryFactory
    {
        IQuery RemoveSnapshot(Guid id);
        IQuery RemoveEventStream(Guid id);
        IQuery GetEventStream(Guid id);
        IQuery Get(long fromSequenceNumber, long toSequenceNumber, IEnumerable<Type> eventTypes);
        IQuery SaveEvent(PrimitiveEvent primitiveEvent);
        IQuery SaveSnapshot(PrimitiveEvent primitiveEvent);
    }
}