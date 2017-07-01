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
        IQuery GetSpecified(long fromSequenceNumber, IEnumerable<Type> eventTypes, int limit);
        IQuery SaveEvent(PrimitiveEvent primitiveEvent);
        IQuery SaveSnapshot(PrimitiveEvent primitiveEvent);
    }
}