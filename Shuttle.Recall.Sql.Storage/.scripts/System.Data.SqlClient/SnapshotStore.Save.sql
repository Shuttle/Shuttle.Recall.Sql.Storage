if exists (select null from [dbo].[SnapshotStore] where Id = @Id)
	update [dbo].[SnapshotStore]  set [Version] = @Version
else
	insert into [dbo].[SnapshotStore] 
	(
		[Id],
		[Version]
	)
	values
	(
		@Id,
		@Version
	)