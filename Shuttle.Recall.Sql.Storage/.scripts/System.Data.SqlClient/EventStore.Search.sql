select top {0}
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
{1}
order by
	es.SequenceNumber
