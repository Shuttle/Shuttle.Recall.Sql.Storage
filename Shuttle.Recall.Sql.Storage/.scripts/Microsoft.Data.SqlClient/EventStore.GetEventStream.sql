declare @Version int

select
    @Version = [Version]
from 
	[dbo].[SnapshotStore] 
where 
	Id = @Id;

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
	es.Id = @Id
and
	(
		@Version is null
		or
		[Version] >= @Version
	)
order by
	[Version]
