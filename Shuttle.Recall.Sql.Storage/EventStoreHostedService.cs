using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Core.Pipelines;

namespace Shuttle.Recall.Sql.Storage;

public class EventStoreHostedService : IHostedService
{
    private readonly IDatabaseContextFactory _databaseContextFactory;
    private readonly DatabaseContextObserver _databaseContextObserver;
    private readonly Type _getEventStreamPipelineType = typeof(GetEventStreamPipeline);

    private readonly IPipelineFactory _pipelineFactory;
    private readonly Type _removeEventStreamPipelineType = typeof(RemoveEventStreamPipeline);
    private readonly Type _saveEventStreamPipelineType = typeof(SaveEventStreamPipeline);
    private readonly SaveEventStreamObserver _saveEventStreamObserver;
    private readonly SqlStorageOptions _sqlStorageOptions;

    public EventStoreHostedService(IOptions<SqlStorageOptions> sqlStorageOptions, IPipelineFactory pipelineFactory, IDatabaseContextService databaseContextService, IDatabaseContextFactory databaseContextFactory, IPrimitiveEventQueryFactory primitiveEventQueryFactory)
    {
        _pipelineFactory = Guard.AgainstNull(pipelineFactory);
        _databaseContextFactory = Guard.AgainstNull(databaseContextFactory);

        _sqlStorageOptions = Guard.AgainstNull(Guard.AgainstNull(sqlStorageOptions).Value);
        _databaseContextObserver = new(_sqlStorageOptions, databaseContextService, databaseContextFactory);
        _saveEventStreamObserver = new(sqlStorageOptions, databaseContextService, primitiveEventQueryFactory);

        pipelineFactory.PipelineCreated += OnPipelineCreated;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_sqlStorageOptions.ConfigureDatabase)
        {
            return;
        }

        var retry = true;
        var retryCount = 0;

        while (retry)
        {
            try
            {
                await using (var databaseContext = _databaseContextFactory.Create(_sqlStorageOptions.ConnectionStringName))
                {
                    await databaseContext.ExecuteAsync(new Query($@"
EXEC sp_getapplock @Resource = '{typeof(EventStoreHostedService).FullName}', @LockMode = 'Exclusive', @LockOwner = 'Session', @LockTimeout = 15000;

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = '{_sqlStorageOptions.Schema}')
BEGIN
    EXEC('CREATE SCHEMA {_sqlStorageOptions.Schema}');
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{_sqlStorageOptions.Schema}].[EventType]') AND type in (N'U'))
BEGIN
    CREATE TABLE [{_sqlStorageOptions.Schema}].[EventType]
    (
	    [Id] [uniqueidentifier] NOT NULL,
	    [TypeName] [nvarchar](1024) NOT NULL,
        CONSTRAINT [PK_EventType] PRIMARY KEY CLUSTERED 
        (
	        [Id] ASC
        ) 
        WITH 
        (
            PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
        ) ON [PRIMARY]
    ) ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{_sqlStorageOptions.Schema}].[IdKey]') AND type in (N'U'))
BEGIN
    CREATE TABLE [{_sqlStorageOptions.Schema}].[IdKey]
    (
	    [UniqueKey] [nvarchar](450) NOT NULL,
	    [Id] [uniqueidentifier] NOT NULL,
        CONSTRAINT [PK_IdKey] PRIMARY KEY CLUSTERED 
        (
	        [UniqueKey] ASC
        )
        WITH 
        (
            PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
        ) ON [PRIMARY]
    ) ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{_sqlStorageOptions.Schema}].[PrimitiveEvent]') AND type in (N'U'))
BEGIN
    CREATE TABLE [{_sqlStorageOptions.Schema}].[PrimitiveEvent]
    (
	    [Id] [uniqueidentifier] NOT NULL,
	    [Version] [int] NOT NULL,
	    [CorrelationId] [uniqueidentifier] NULL,
	    [EventEnvelope] [varbinary](max) NOT NULL,
	    [EventId] [uniqueidentifier] NOT NULL,
	    [EventTypeId] [uniqueidentifier] NOT NULL,
	    [DateRegistered] [datetime2](7) NOT NULL,
	    [SequenceNumber] [bigint] IDENTITY(1,1) NOT NULL,
	    [DateCommitted] [datetime2](7) NULL,
        CONSTRAINT [PK_PrimitiveEvent] PRIMARY KEY CLUSTERED 
        (
	        [Id] ASC,
	        [Version] ASC
        )
        WITH 
        (
            PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
        ) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[{_sqlStorageOptions.Schema}].[EventType]') AND name = N'IX_EventType')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_EventType] ON [{_sqlStorageOptions.Schema}].[EventType]
    (
	    [TypeName] ASC
    )
    WITH 
    (
        PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
    ) ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[{_sqlStorageOptions.Schema}].[PrimitiveEvent]') AND name = N'IX_PrimitiveEvent_DateCommitted_Filtered_Null')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_PrimitiveEvent_DateCommitted_Filtered_Null] ON [{_sqlStorageOptions.Schema}].[PrimitiveEvent]
    (
	    [Id] ASC,
	    [DateCommitted] ASC
    )
    WHERE 
        ([DateCommitted] IS NULL)
    WITH 
    (
        PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
    ) ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[{_sqlStorageOptions.Schema}].[PrimitiveEvent]') AND name = N'IX_PrimitiveEvent_EventTypeId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_PrimitiveEvent_EventTypeId] ON [{_sqlStorageOptions.Schema}].[PrimitiveEvent]
    (
	    [EventTypeId] ASC
    )
    WITH 
    (
        PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
    ) ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{_sqlStorageOptions.Schema}].[DF_PrimitiveEvent_DateRegistered]') AND type = 'D')
BEGIN
    ALTER TABLE [{_sqlStorageOptions.Schema}].[PrimitiveEvent] 
        ADD CONSTRAINT DF_PrimitiveEvent_DateRegistered DEFAULT (GETUTCDATE()) FOR [DateRegistered]
END

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[{_sqlStorageOptions.Schema}].[FK_PrimitiveEvent_EventType_EventTypeId]') AND parent_object_id = OBJECT_ID(N'[{_sqlStorageOptions.Schema}].[PrimitiveEvent]'))
BEGIN
    ALTER TABLE [{_sqlStorageOptions.Schema}].[PrimitiveEvent] 
        WITH CHECK ADD CONSTRAINT [FK_PrimitiveEvent_EventType_EventTypeId] FOREIGN KEY([EventTypeId]) REFERENCES [{_sqlStorageOptions.Schema}].[EventType] ([Id])
    ON DELETE CASCADE
END

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[{_sqlStorageOptions.Schema}].[FK_PrimitiveEvent_EventType_EventTypeId]') AND parent_object_id = OBJECT_ID(N'[{_sqlStorageOptions.Schema}].[PrimitiveEvent]'))
BEGIN
    ALTER TABLE [{_sqlStorageOptions.Schema}].[PrimitiveEvent] CHECK CONSTRAINT [FK_PrimitiveEvent_EventType_EventTypeId]
END

EXEC sp_releaseapplock @Resource = '{typeof(EventStoreHostedService).FullName}', @LockOwner = 'Session';
"), cancellationToken: cancellationToken);
                }

                retry = false;
            }
            catch
            {
                retryCount++;

                if (retryCount > 3)
                {
                    throw;
                }
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _pipelineFactory.PipelineCreated -= OnPipelineCreated;

        await Task.CompletedTask;
    }

    private void OnPipelineCreated(object? sender, PipelineEventArgs e)
    {
        var pipelineType = e.Pipeline.GetType();

        if (pipelineType == _saveEventStreamPipelineType)
        {
            e.Pipeline.AddObserver(_saveEventStreamObserver);
        }

        if (pipelineType != _getEventStreamPipelineType &&
            pipelineType != _saveEventStreamPipelineType &&
            pipelineType != _removeEventStreamPipelineType)
        {
            return;
        }

        e.Pipeline.AddObserver(_databaseContextObserver);
    }
}