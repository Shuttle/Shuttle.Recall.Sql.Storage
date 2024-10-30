BEGIN TRANSACTION
GO
IF COL_LENGTH('dbo.EventStore', 'IsSnapshot') IS NOT NULL
BEGIN
    ALTER TABLE dbo.EventStore
    DROP COLUMN IsSnapshot;
END
GO
IF COL_LENGTH('dbo.EventStore', 'CorrelationId') IS NULL
BEGIN
    ALTER TABLE dbo.EventStore
    ADD [CorrelationId] uniqueidentifier NULL;
END
GO
COMMIT
GO