select top {0}
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
	es.SequenceNumber >= @FromSequenceNumber
{1}
order by
	es.SequenceNumber
