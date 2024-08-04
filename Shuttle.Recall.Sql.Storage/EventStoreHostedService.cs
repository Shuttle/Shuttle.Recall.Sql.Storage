using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Core.Pipelines;
using Shuttle.Core.Threading;

namespace Shuttle.Recall.Sql.Storage
{
    public class EventStoreHostedService : IHostedService
    {
        private readonly Type _getEventStreamPipelineType = typeof(GetEventStreamPipeline);
        private readonly Type _saveEventStreamPipelineType = typeof(SaveEventStreamPipeline);
        private readonly Type _removeEventStreamPipelineType = typeof(RemoveEventStreamPipeline);

        private readonly IPipelineFactory _pipelineFactory;

        private readonly DatabaseContextObserver _databaseContextObserver;

        public EventStoreHostedService(IOptions<SqlStorageOptions> sqlStorageOptions, IPipelineFactory pipelineFactory, IDatabaseContextService databaseContextService, IDatabaseContextFactory databaseContextFactory)
        {
            _pipelineFactory = Guard.AgainstNull(pipelineFactory, nameof(pipelineFactory));

            _databaseContextObserver = new DatabaseContextObserver(Guard.AgainstNull(sqlStorageOptions, nameof(sqlStorageOptions)).Value, databaseContextFactory);

            pipelineFactory.PipelineCreated += OnPipelineCreated;
        }

        private void OnPipelineCreated(object sender, PipelineEventArgs e)
        {
            var pipelineType = e.Pipeline.GetType();

            if (pipelineType != _getEventStreamPipelineType &&
                pipelineType != _saveEventStreamPipelineType &&
                pipelineType != _removeEventStreamPipelineType)
            {
                return;
            }

            e.Pipeline.RegisterObserver(_databaseContextObserver);
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
    }
}