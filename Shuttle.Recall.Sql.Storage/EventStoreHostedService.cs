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
    private readonly DatabaseContextObserver _databaseContextObserver;
    private readonly Type _getEventStreamPipelineType = typeof(GetEventStreamPipeline);

    private readonly IPipelineFactory _pipelineFactory;
    private readonly Type _removeEventStreamPipelineType = typeof(RemoveEventStreamPipeline);
    private readonly Type _saveEventStreamPipelineType = typeof(SaveEventStreamPipeline);

    public EventStoreHostedService(IOptions<SqlStorageOptions> sqlStorageOptions, IPipelineFactory pipelineFactory, IDatabaseContextService databaseContextService, IDatabaseContextFactory databaseContextFactory)
    {
        _pipelineFactory = Guard.AgainstNull(pipelineFactory);

        _databaseContextObserver = new(Guard.AgainstNull(Guard.AgainstNull(sqlStorageOptions).Value), databaseContextService, databaseContextFactory);

        pipelineFactory.PipelineCreated += OnPipelineCreated;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _pipelineFactory.PipelineCreated -= OnPipelineCreated;

        await Task.CompletedTask;
    }

    private void OnPipelineCreated(object? sender, PipelineEventArgs e)
    {
        var pipelineType = e.Pipeline.GetType();

        if (pipelineType != _getEventStreamPipelineType &&
            pipelineType != _saveEventStreamPipelineType &&
            pipelineType != _removeEventStreamPipelineType)
        {
            return;
        }

        e.Pipeline.AddObserver(_databaseContextObserver);
    }
}