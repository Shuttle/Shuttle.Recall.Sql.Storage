using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Core.Pipelines;

namespace Shuttle.Recall.Sql.Storage
{
    public class EventStoreHostedService : IHostedService
    {
        private readonly Type _getEventStreamPipelineType = typeof(GetEventStreamPipeline);
        private readonly Type _saveEventStreamPipelineType = typeof(SaveEventStreamPipeline);
        private readonly Type _removeEventStreamPipelineType = typeof(RemoveEventStreamPipeline);

        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOptions<EventStoreOptions> _eventStoreOptions;
        private readonly IPipelineFactory _pipelineFactory;

        public EventStoreHostedService(IOptions<EventStoreOptions> eventStoreOptions, IPipelineFactory pipelineFactory, IDatabaseContextFactory databaseContextFactory)
        {
            _eventStoreOptions = Guard.AgainstNull(eventStoreOptions, nameof(eventStoreOptions));
            _pipelineFactory = Guard.AgainstNull(pipelineFactory, nameof(pipelineFactory));
            _databaseContextFactory = Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));

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

            e.Pipeline.RegisterObserver(new DatabaseContextObserver(_eventStoreOptions, _databaseContextFactory));
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