declare @Version int

select
    @Version = [Version]
from 
	[dbo].[SnapshotStore] 
where 
	Id = @Id

if (@Version is null)
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
		Id = @Id
	order by
		[Version]
else
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
		Id = @Id
	and
		[Version] >= @Version
	order by
		[Version]
