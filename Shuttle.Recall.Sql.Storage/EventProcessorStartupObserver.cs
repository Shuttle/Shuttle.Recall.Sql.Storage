using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Core.Pipelines;
using Shuttle.Core.Threading;

namespace Shuttle.Recall.Sql.Storage
{
    public class EventProcessorStartupObserver : IPipelineObserver<OnAfterConfigureThreadPools>
    {
        private readonly IDatabaseContextService _databaseContextService;

        public EventProcessorStartupObserver(IDatabaseContextService databaseContextService)
        {
            _databaseContextService = Guard.AgainstNull(databaseContextService, nameof(databaseContextService));
        }

        public void Execute(OnAfterConfigureThreadPools pipelineEvent)
        {
            pipelineEvent.Pipeline.State.Get<IProcessorThreadPool>("EventProcessorThreadPool").ProcessorThreadCreated += (sender, args) =>
            {
                _databaseContextService.SetAmbientScope();
            };
        }

        public async Task ExecuteAsync(OnAfterConfigureThreadPools pipelineEvent)
        {
            Execute(pipelineEvent);

            await Task.CompletedTask;
        }
    }
}