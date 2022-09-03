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
        IQuery Search(PrimitiveEvent.Specification specification);
        IQuery SaveEvent(PrimitiveEvent primitiveEvent);
        IQuery SaveSnapshot(PrimitiveEvent primitiveEvent);
        IQuery GetSequenceNumber(Guid id);
    }
}