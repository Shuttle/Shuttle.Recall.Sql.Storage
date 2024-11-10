using System;
using System.Collections.Generic;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage;

public interface IPrimitiveEventQueryFactory
{
    IQuery GetEventStream(Guid id);
    IQuery GetSequenceNumber(Guid id);
    IQuery RemoveEventStream(Guid id);
    IQuery SaveEvent(PrimitiveEvent primitiveEvent, Guid eventTypeId);
    IQuery Search(PrimitiveEventSpecification specification, IEnumerable<Guid> eventTypeIds);
}