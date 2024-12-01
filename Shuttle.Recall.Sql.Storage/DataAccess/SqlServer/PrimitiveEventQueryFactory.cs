using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage.SqlServer;

public class PrimitiveEventQueryFactory : IPrimitiveEventQueryFactory
{
    private readonly SqlStorageOptions _sqlStorageOptions;

    public PrimitiveEventQueryFactory(IOptions<SqlStorageOptions> sqlStorageOptions)
    {
        _sqlStorageOptions = Guard.AgainstNull(Guard.AgainstNull(sqlStorageOptions).Value);
    }

    public IQuery RemoveEventStream(Guid id)
    {
        return new Query($"delete from [{_sqlStorageOptions.Schema}].[PrimitiveEvent] where Id = @Id").AddParameter(Columns.Id, id);
    }

    public IQuery GetEventStream(Guid id)
    {
        return new Query($@"
select
	es.[Id],
	es.[Version],
	es.[CorrelationId],
	et.[TypeName] EventType,
	es.[EventEnvelope],
	es.[EventId],
	es.[SequenceNumber],
	es.[DateRegistered]
from 
	[{_sqlStorageOptions.Schema}].[PrimitiveEvent] es
inner join
	[{_sqlStorageOptions.Schema}].[EventType] et on et.Id = es.EventTypeId
where
	es.Id = @Id
order by
	[Version]
")
            .AddParameter(Columns.Id, id);
    }

    public IQuery Search(PrimitiveEventSpecification specification, IEnumerable<Guid> eventTypeIds)
    {
        Guard.AgainstNull(specification);

        return
            new Query($@"
select top {(specification.MaximumRows > 0 ? specification.MaximumRows : 1)}
	es.[Id],
	es.[Version],
	es.[CorrelationId],
	et.[TypeName] EventType,
	es.[EventEnvelope],
	es.[EventId],
	es.[SequenceNumber],
	es.[DateRegistered]
from 
	[{_sqlStorageOptions.Schema}].[PrimitiveEvent] es
inner join
	[{_sqlStorageOptions.Schema}].[EventType] et on et.Id = es.EventTypeId
where 
(
    @SequenceNumberStart = 0
    or
	es.SequenceNumber >= @SequenceNumberStart
)
{(
    !eventTypeIds.Any()
    ? string.Empty
    : $"and EventTypeId in ({string.Join(",", eventTypeIds.Select(id => string.Concat("'", id, "'")).ToArray())})"
)}
{(
    !specification.Ids.Any()
        ? string.Empty
        : $"and Id in ({string.Join(",", specification.Ids.Select(id => string.Concat("'", id, "'")).ToArray())})"
)}
order by
	es.SequenceNumber
")
                .AddParameter(Columns.SequenceNumberStart, specification.SequenceNumberStart);
    }

    public IQuery SaveEvent(PrimitiveEvent primitiveEvent, Guid eventTypeId)
    {
        return new Query($@"
insert into [{_sqlStorageOptions.Schema}].[PrimitiveEvent] 
(
	[Id],
	[Version],
	[CorrelationId],
	[EventTypeId],
	[EventEnvelope],
	[EventId],
	[DateRegistered]	
)
values
(
	@Id,
	@Version,
	@CorrelationId,
	@EventTypeId,
	@EventEnvelope,
	@EventId,
	@DateRegistered
);

select cast(scope_identity() as bigint);
")
            .AddParameter(Columns.Id, primitiveEvent.Id)
            .AddParameter(Columns.Version, primitiveEvent.Version)
            .AddParameter(Columns.CorrelationId, primitiveEvent.CorrelationId)
            .AddParameter(Columns.DateRegistered, primitiveEvent.DateRegistered)
            .AddParameter(Columns.EventEnvelope, primitiveEvent.EventEnvelope)
            .AddParameter(Columns.EventId, primitiveEvent.EventId)
            .AddParameter(Columns.EventTypeId, eventTypeId);
    }

    public IQuery GetSequenceNumber(Guid id)
    {
        return new Query($"select max(SequenceNumber) from [{_sqlStorageOptions.Schema}].[PrimitiveEvent] where Id = @Id").AddParameter(Columns.Id, id);
    }
}