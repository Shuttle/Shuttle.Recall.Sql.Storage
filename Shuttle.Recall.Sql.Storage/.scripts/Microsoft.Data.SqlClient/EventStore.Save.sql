﻿insert into [dbo].[EventStore] 
(
	[Id],
	[Version],
	[EventTypeId],
	[EventEnvelope],
	[EventId],
	[IsSnapshot],
	[DateRegistered]	
)
values
(
	@Id,
	@Version,
	@EventTypeId,
	@EventEnvelope,
	@EventId,
	@IsSnapshot,
	@DateRegistered
)