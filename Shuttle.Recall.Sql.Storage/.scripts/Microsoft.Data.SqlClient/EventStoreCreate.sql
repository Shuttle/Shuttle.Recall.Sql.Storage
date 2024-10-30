IF ('{schema}' <> 'dbo' AND NOT EXISTS (SELECT * FROM sys.schemas WHERE name = '{schema}'))
BEGIN
    EXEC('CREATE SCHEMA {schema}');
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{schema}].[EventType]') AND type in (N'U'))
BEGIN
	CREATE TABLE [{schema}].[EventType]
    (
		[Id] [uniqueidentifier] NOT NULL,
		[TypeName] [varchar](160) NOT NULL,
	    CONSTRAINT [PK_EventType] PRIMARY KEY NONCLUSTERED 
	    (
		    [Id] ASC
	    )
        WITH 
        (
            PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON
        ) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[{schema}].[EventType]') AND name = N'IX_EventType')
BEGIN
	CREATE UNIQUE CLUSTERED INDEX [IX_EventType] ON [{schema}].[EventType]
	(
		[TypeName] ASC
	)
    WITH 
    (
        PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON
    ) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{schema}].[EventStore]') AND type in (N'U'))
BEGIN
	CREATE TABLE [{schema}].[EventStore]
    (
		[Id] [uniqueidentifier] NOT NULL,
		[Version] [int] NOT NULL,
		[CorrelationId] [uniqueidentifier] NULL,
		[EventEnvelope] [varbinary](max) NOT NULL,
		[EventId] [uniqueidentifier] NOT NULL,
		[EventTypeId] [uniqueidentifier] NOT NULL,
		[SequenceNumber] [bigint] IDENTITY(1,1) NOT NULL,
		[DateRegistered] [datetime2] NOT NULL,
	    CONSTRAINT [PK_EventStore] PRIMARY KEY CLUSTERED 
	    (
		    [Id] ASC,
		    [Version] ASC
	    )
	    WITH 
	    (
		    PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[{schema}].[EventStore]') AND name = N'IX_EventStore')
	CREATE NONCLUSTERED INDEX [IX_EventStore] ON [{schema}].[EventStore]
	(
		[SequenceNumber] ASC
	)
	WITH 
	(
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON
	) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{schema}].[DF_EventStore_DateRegistered]') AND type = 'D')
BEGIN
	ALTER TABLE [{schema}].[EventStore] ADD CONSTRAINT [DF_EventStore_DateRegistered] DEFAULT (getdate()) FOR [DateRegistered]
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[{schema}].[FK_EventStore_EventType]') AND parent_object_id = OBJECT_ID(N'[{schema}].[EventStore]'))
	ALTER TABLE 
		[{schema}].[EventStore]  
	WITH CHECK ADD CONSTRAINT 
		[FK_EventStore_EventType] 
	FOREIGN KEY
	(
		[EventTypeId]
	)
	REFERENCES [{schema}].[EventType] 
	(
		[Id]
	)
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[{schema}].[FK_EventStore_EventType]') AND parent_object_id = OBJECT_ID(N'[{schema}].[EventStore]'))
	ALTER TABLE 
		[{schema}].[EventStore] 
	CHECK CONSTRAINT 
		[FK_EventStore_EventType]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{schema}].[KeyStore]') AND type in (N'U'))
BEGIN
	CREATE TABLE [{schema}].[KeyStore]
    (
		[Key] [varchar](160) NOT NULL,
		[Id] [uniqueidentifier] NOT NULL,
	    CONSTRAINT [PK_KeyStore] PRIMARY KEY CLUSTERED 
	    (
		    [Key] ASC
	    )
        WITH 
        (
            PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 50
        ) ON [PRIMARY]
	) ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[{schema}].[EventStore]') AND name = N'IX_EventStore')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_EventStore] ON [{schema}].[EventStore]
	(
		[SequenceNumber] ASC
	)
    WITH 
    (
        PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON
    ) ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[{schema}].[KeyStore]') AND name = N'IX_KeyStore')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_KeyStore] ON [{schema}].[KeyStore]
	(
		[Id] ASC
	)
    WITH 
    (
        PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON
    ) ON [PRIMARY]
END