IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EventType]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[EventType](
		[Id] [uniqueidentifier] NOT NULL,
		[TypeName] [varchar](160) NOT NULL,
	 CONSTRAINT [PK_EventType] PRIMARY KEY NONCLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[EventType]') AND name = N'IX_EventType')
BEGIN
	CREATE UNIQUE CLUSTERED INDEX [IX_EventType] ON [dbo].[EventType]
	(
		[TypeName] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END
GO

IF (COL_LENGTH('[dbo].[EventStore]', 'EventType') IS NOT NULL)
BEGIN
	declare @sql nvarchar(max)

	set @sql = '
	insert into EventType
	(
		Id,
		TypeName
	)
	select
		newid(),
		EventType
	from
	(
		select distinct
			EventType
		from
			EventStore
		where
			not exists
			(
				select NULL from EventType where TypeName = EventType
			)
	) t'
	exec sp_executesql @sql
END
GO

IF (COL_LENGTH('[dbo].[EventStore]', 'EventId') IS NULL)
BEGIN
	ALTER TABLE
		[dbo].[EventStore]
	ADD 
		EventId [uniqueidentifier] NULL;
END
GO

IF (COL_LENGTH('[dbo].[EventStore]', 'EventTypeId') IS NULL)
BEGIN
	ALTER TABLE
		[dbo].[EventStore]
	ADD 
		EventTypeId [uniqueidentifier] NULL
END
GO

IF (COL_LENGTH('[dbo].[EventStore]', 'EventId') IS NOT NULL)
BEGIN
	declare @sql nvarchar(max)

	set @sql = '
	UPDATE
		[dbo].[EventStore]
	SET
		EventId = ''00000000-0000-0000-0000-000000000000''
	WHERE
		EventId IS NULL'
	exec sp_executesql @sql
END
GO

IF (COL_LENGTH('[dbo].[EventStore]', 'EventType') IS NOT NULL AND (COL_LENGTH('[dbo].[EventStore]', 'EventTypeId') IS NOT NULL))
BEGIN
	declare @sql nvarchar(max)

	set @sql = '
	UPDATE
		[dbo].[EventStore]
	SET
		EventTypeId = et.Id
	FROM
		[dbo].[EventStore] es
	INNER JOIN
		[dbo].[EventType] et ON (et.TypeName = es.EventType)
	WHERE
		es.EventTypeId IS NULL'
	exec sp_executesql @sql
END
GO

IF (COL_LENGTH('[dbo].[EventStore]', 'EventTypeId') IS NOT NULL)
BEGIN
	ALTER TABLE
		[dbo].[EventStore]
	ALTER COLUMN
		EventTypeId [uniqueidentifier] NOT NULL
END
GO

IF (COL_LENGTH('[dbo].[EventStore]', 'EventId') IS NOT NULL)
BEGIN
	ALTER TABLE
		[dbo].[EventStore]
	ALTER COLUMN
		EventId [uniqueidentifier] NOT NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EventStore_EventType]') AND parent_object_id = OBJECT_ID(N'[dbo].[EventStore]'))
	ALTER TABLE 
		[dbo].[EventStore] WITH CHECK 
	ADD CONSTRAINT 
		[FK_EventStore_EventType] 
	FOREIGN KEY
	(
		[EventTypeId]
	)
	REFERENCES [dbo].[EventType] 
	(
		[Id]
	)
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EventStore_EventType]') AND parent_object_id = OBJECT_ID(N'[dbo].[EventStore]'))
	ALTER TABLE 
		[dbo].[EventStore] 
	CHECK CONSTRAINT 
		[FK_EventStore_EventType]
GO

IF (COL_LENGTH('[dbo].[EventStore]', 'EventType') IS NOT NULL)
BEGIN
	ALTER TABLE
		[dbo].[EventStore]
	DROP COLUMN
		EventType
END
GO