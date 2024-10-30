insert into [{schema}].[EventStore] 
(
	[Id],
	[Version],
	[CorrelationId],
	[EventTypeId],
	[EventEnvelope],
	[EventId],
	[DateRegistered]	
)
values
(
	@Id,
	@Version,
	@CorrelationId,
	@EventTypeId,
	@EventEnvelope,
	@EventId,
	@DateRegistered
);

select cast(scope_identity() as bigint);