﻿using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Core.Pipelines;
using Shuttle.Core.Reflection;

namespace Shuttle.Recall.Sql.Storage;

public class DatabaseContextObserver :
    IPipelineObserver<OnBeforeGetStreamEvents>,
    IPipelineObserver<OnAfterGetStreamEvents>,
    IPipelineObserver<OnBeforeSavePrimitiveEvents>,
    IPipelineObserver<OnAfterSavePrimitiveEvents>,
    IPipelineObserver<OnBeforeSaveEventStreamCompleted>,
    IPipelineObserver<OnAfterSaveEventStreamCompleted>,
    IPipelineObserver<OnBeforeRemoveEventStream>,
    IPipelineObserver<OnAfterRemoveEventStream>
{
    private const string DatabaseContextStateKey = "Shuttle.Recall.Sql.Storage.DatabaseContextObserver:DatabaseContext";
    private readonly IDatabaseContextFactory _databaseContextFactory;
    private readonly IDatabaseContextService _databaseContextService;
    private readonly SqlStorageOptions _sqlStorageOptions;

    public DatabaseContextObserver(SqlStorageOptions sqlStorageOptions, IDatabaseContextService databaseContextService, IDatabaseContextFactory databaseContextFactory)
    {
        _sqlStorageOptions = Guard.AgainstNull(sqlStorageOptions);
        _databaseContextService = Guard.AgainstNull(databaseContextService);
        _databaseContextFactory = Guard.AgainstNull(databaseContextFactory);
    }

    public async Task ExecuteAsync(IPipelineContext<OnAfterGetStreamEvents> pipelineContext)
    {
        await DisposeDatabaseContextAsync(pipelineContext);
    }

    public async Task ExecuteAsync(IPipelineContext<OnAfterRemoveEventStream> pipelineContext)
    {
        await DisposeDatabaseContextAsync(pipelineContext);
    }

    public async Task ExecuteAsync(IPipelineContext<OnAfterSaveEventStreamCompleted> pipelineContext)
    {
        await DisposeDatabaseContextAsync(pipelineContext);
    }

    public async Task ExecuteAsync(IPipelineContext<OnAfterSavePrimitiveEvents> pipelineContext)
    {
        await DisposeDatabaseContextAsync(pipelineContext);
    }

    public async Task ExecuteAsync(IPipelineContext<OnBeforeGetStreamEvents> pipelineContext)
    {
        await CreateDatabaseContextAsync(pipelineContext);
    }

    public async Task ExecuteAsync(IPipelineContext<OnBeforeRemoveEventStream> pipelineContext)
    {
        await CreateDatabaseContextAsync(pipelineContext);
    }

    public async Task ExecuteAsync(IPipelineContext<OnBeforeSaveEventStreamCompleted> pipelineContext)
    {
        await CreateDatabaseContextAsync(pipelineContext);
    }

    public async Task ExecuteAsync(IPipelineContext<OnBeforeSavePrimitiveEvents> pipelineContext)
    {
        await CreateDatabaseContextAsync(pipelineContext);
    }

    private async Task CreateDatabaseContextAsync(IPipelineContext pipelineContext)
    {
        pipelineContext.Pipeline.State.Replace(DatabaseContextStateKey,
            !_databaseContextService.Contains(_sqlStorageOptions.ConnectionStringName)
                ? _databaseContextFactory.Create(_sqlStorageOptions.ConnectionStringName)
                : null);

        await Task.CompletedTask;
    }

    private async Task DisposeDatabaseContextAsync(IPipelineContext pipelineContext)
    {
        await (pipelineContext.Pipeline.State.Get<IDatabaseContext>(DatabaseContextStateKey)?.TryDisposeAsync() ?? Task.CompletedTask);

        pipelineContext.Pipeline.State.Remove(DatabaseContextStateKey);
    }
}