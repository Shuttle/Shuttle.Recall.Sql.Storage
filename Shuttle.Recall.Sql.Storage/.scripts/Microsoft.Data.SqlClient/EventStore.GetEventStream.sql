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
	[{schema}].[EventStore] es
inner join
	[{schema}].[EventType] et on et.Id = es.EventTypeId
where
	es.Id = @Id
order by
	[Version]
