select
	[Id],
	[Version],
	[EventType],
	[EventEnvelope],
	[SequenceNumber],
	[DateRegistered]
from 
	[dbo].[EventStore] 
where 
	SequenceNumber >= @FromSequenceNumber
and
	SequenceNumber <= @ToSequenceNumber
	{0}
order by
	[SequenceNumber]
