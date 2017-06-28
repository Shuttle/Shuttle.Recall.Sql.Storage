insert into [dbo].[EventStore] 
(
	[Id],
	[Version],
	[EventType],
	[EventEnvelope],
	[IsSnapshot],
	[DateRegistered]	
)
values
(
	@Id,
	@Version,
	@EventType,
	@EventEnvelope,
	@IsSnapshot,
	@DateRegistered
)