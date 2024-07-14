using System;
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
        private readonly IDatabaseContextService _databaseContextService;
        private readonly Type _eventProcessorStartupPipeline = typeof(EventProcessorStartupPipeline);
        private readonly Type _getEventStreamPipelineType = typeof(GetEventStreamPipeline);
        private readonly Type _saveEventStreamPipelineType = typeof(SaveEventStreamPipeline);
        private readonly Type _removeEventStreamPipelineType = typeof(RemoveEventStreamPipeline);

        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IPipelineFactory _pipelineFactory;
        private readonly SqlStorageOptions _sqlStorageOptions;
        private readonly EventProcessorStartupObserver _eventProcessorStartupObserver;

        public EventStoreHostedService(IOptions<SqlStorageOptions> sqlStorageOptions , IPipelineFactory pipelineFactory, IDatabaseContextFactory databaseContextFactory, IDatabaseContextService databaseContextService)
        {
            _sqlStorageOptions = Guard.AgainstNull(sqlStorageOptions, nameof(sqlStorageOptions)).Value;
            _pipelineFactory = Guard.AgainstNull(pipelineFactory, nameof(pipelineFactory));
            _databaseContextFactory = Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
            _databaseContextService = Guard.AgainstNull(databaseContextService, nameof(databaseContextService));
            
            _eventProcessorStartupObserver = new EventProcessorStartupObserver(_databaseContextService);

            pipelineFactory.PipelineCreated += OnPipelineCreated;
        }

        private void OnPipelineCreated(object sender, PipelineEventArgs e)
        {
            var pipelineType = e.Pipeline.GetType();

            if (pipelineType == _eventProcessorStartupPipeline)
            {
                e.Pipeline.RegisterObserver(_eventProcessorStartupObserver);

                return;
            }

            if (pipelineType != _getEventStreamPipelineType &&
                pipelineType != _saveEventStreamPipelineType &&
                pipelineType != _removeEventStreamPipelineType)
            {
                return;
            }

            e.Pipeline.RegisterObserver(new DatabaseContextObserver(_sqlStorageOptions.ManageEventStoreConnections, _sqlStorageOptions.ConnectionStringName, _databaseContextFactory));
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