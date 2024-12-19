using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage;

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
SELECT
	es.[Id],
	es.[Version],
	es.[CorrelationId],
	et.[TypeName] EventType,
	es.[EventEnvelope],
	es.[EventId],
	es.[SequenceNumber],
	es.[DateRegistered]
FROM 
	[{_sqlStorageOptions.Schema}].[PrimitiveEvent] es
INNER JOIN
	[{_sqlStorageOptions.Schema}].[EventType] et ON et.Id = es.EventTypeId
WHERE
	es.Id = @Id
ORDER BY
	[Version]
")
            .AddParameter(Columns.Id, id);
    }

    public IQuery Search(PrimitiveEventSpecification specification, IEnumerable<Guid> eventTypeIds)
    {
        Guard.AgainstNull(specification);

        return
            new Query($@"
SELECT {(specification.MaximumRows > 0 ? $"TOP {specification.MaximumRows}" : string.Empty)}
	es.[Id],
	es.[Version],
	es.[CorrelationId],
	et.[TypeName] EventType,
	es.[EventEnvelope],
	es.[EventId],
	es.[SequenceNumber],
	es.[DateRegistered]
FROM 
	[{_sqlStorageOptions.Schema}].[PrimitiveEvent] es
INNER JOIN
	[{_sqlStorageOptions.Schema}].[EventType] et ON et.Id = es.EventTypeId
WHERE 
(
    @SequenceNumberStart = 0
    OR
	es.SequenceNumber >= @SequenceNumberStart
)
AND
(
    @SequenceNumberEnd = 0
    OR
	es.SequenceNumber <= @SequenceNumberEnd
)
{(
    !eventTypeIds.Any()
        ? string.Empty
        : $"AND EventTypeId IN ({string.Join(",", eventTypeIds.Select(id => string.Concat("'", id, "'")).ToArray())})"
)}
{(
    !specification.HasIds
        ? string.Empty
        : $"AND Id IN ({string.Join(",", specification.Ids.Select(id => string.Concat("'", id, "'")).ToArray())})"
)}
{(
    !specification.HasSequenceNumbers
        ? string.Empty
        : $"AND SequenceNumber IN ({string.Join(",", specification.SequenceNumbers)})"
)}
ORDER BY
	es.SequenceNumber
")
                .AddParameter(Columns.SequenceNumberStart, specification.SequenceNumberStart)
                .AddParameter(Columns.SequenceNumberEnd, specification.SequenceNumberEnd);
    }

    public IQuery GetUncommittedSequenceNumberStart()
    {
        return new Query($@"
UPDATE
    [{_sqlStorageOptions.Schema}].[PrimitiveEvent]
SET
    DateCommitted = GETUTCDATE()
WHERE
    DateCommitted IS NULL

SELECT
    MIN(SequenceNumber)
FROM
    [{_sqlStorageOptions.Schema}].[PrimitiveEvent] (NOLOCK)
WHERE
    DateCommitted IS NULL
");
    }

    public IQuery Commit(Guid id)
    {
        return new Query($@"
UPDATE
    [{_sqlStorageOptions.Schema}].[PrimitiveEvent]
SET
    DateCommitted = GETUTCDATE()
WHERE
    Id = @Id
AND
    DateCommitted IS NULL
")
            .AddParameter(Columns.Id, id);
    }

    public IQuery SaveEvent(PrimitiveEvent primitiveEvent, Guid eventTypeId)
    {
        return new Query($@"
INSERT INTO [{_sqlStorageOptions.Schema}].[PrimitiveEvent] 
(
	[Id],
	[Version],
	[CorrelationId],
	[EventTypeId],
	[EventEnvelope],
	[EventId],
    [DateRegistered]
)
VALUES
(
	@Id,
	@Version,
	@CorrelationId,
	@EventTypeId,
	@EventEnvelope,
	@EventId,
    GETUTCDATE()
);

SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
")
            .AddParameter(Columns.Id, primitiveEvent.Id)
            .AddParameter(Columns.Version, primitiveEvent.Version)
            .AddParameter(Columns.CorrelationId, primitiveEvent.CorrelationId)
            .AddParameter(Columns.EventEnvelope, primitiveEvent.EventEnvelope)
            .AddParameter(Columns.EventId, primitiveEvent.EventId)
            .AddParameter(Columns.EventTypeId, eventTypeId);
    }

    public IQuery GetSequenceNumber(Guid id)
    {
        return new Query($"SELECT MAX(SequenceNumber) FROM [{_sqlStorageOptions.Schema}].[PrimitiveEvent] WHERE Id = @Id").AddParameter(Columns.Id, id);
    }
}