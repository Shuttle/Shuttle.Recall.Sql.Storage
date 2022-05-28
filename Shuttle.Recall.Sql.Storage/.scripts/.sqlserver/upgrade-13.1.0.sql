BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.EventStore
	DROP CONSTRAINT FK_EventStore_EventType
GO
ALTER TABLE dbo.EventType SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.EventStore
	DROP CONSTRAINT DF_EventStore_DateRegistered
GO
CREATE TABLE dbo.Tmp_EventStore
	(
	Id uniqueidentifier NOT NULL,
	Version int NOT NULL,
	EventEnvelope varbinary(MAX) NOT NULL,
	EventId uniqueidentifier NOT NULL,
	EventTypeId uniqueidentifier NOT NULL,
	IsSnapshot bit NOT NULL,
	SequenceNumber bigint NOT NULL IDENTITY (1, 1),
	DateRegistered datetime2(7) NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_EventStore SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_EventStore ADD CONSTRAINT
	DF_EventStore_DateRegistered DEFAULT (getutcdate()) FOR DateRegistered
GO
SET IDENTITY_INSERT dbo.Tmp_EventStore ON
GO
IF EXISTS(SELECT * FROM dbo.EventStore)
	 EXEC('INSERT INTO dbo.Tmp_EventStore (Id, Version, EventEnvelope, EventId, EventTypeId, IsSnapshot, SequenceNumber, DateRegistered)
		SELECT Id, Version, EventEnvelope, EventId, EventTypeId, IsSnapshot, SequenceNumber, CONVERT(datetime2(7), DateRegistered) FROM dbo.EventStore WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_EventStore OFF
GO
DROP TABLE dbo.EventStore
GO
EXECUTE sp_rename N'dbo.Tmp_EventStore', N'EventStore', 'OBJECT' 
GO
ALTER TABLE dbo.EventStore ADD CONSTRAINT
	PK_EventStore PRIMARY KEY CLUSTERED 
	(
	Id,
	Version
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX IX_EventStore ON dbo.EventStore
	(
	SequenceNumber
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE dbo.EventStore ADD CONSTRAINT
	FK_EventStore_EventType FOREIGN KEY
	(
	EventTypeId
	) REFERENCES dbo.EventType
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
