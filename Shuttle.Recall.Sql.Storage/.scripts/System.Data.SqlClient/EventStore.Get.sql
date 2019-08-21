select
	es.[Id],
	es.[Version],
	et.[TypeName] EventType,
	es.[EventEnvelope],
	es.[EventId],
	es.[SequenceNumber],
	es.[DateRegistered]
from 
	[dbo].[EventStore] es
inner join
	[dbo].[EventType] et on et.Id = es.EventTypeId
where 
	es.SequenceNumber >= @FromSequenceNumber
and
	es.SequenceNumber <= @ToSequenceNumber
	{0}
order by
	es.SequenceNumber
