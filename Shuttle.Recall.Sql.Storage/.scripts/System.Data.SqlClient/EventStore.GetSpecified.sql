select top {0}
	[Id],
	[Version],
	[EventType],
	[EventEnvelope],
	[SequenceNumber],
	[DateRegistered]
from 
	[dbo].[EventStore] 
where 
	SequenceNumber > @SequenceNumber
	{1}
order by
	[SequenceNumber]
