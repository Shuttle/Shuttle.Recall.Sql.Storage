using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Core.Pipelines;

namespace Shuttle.Recall.Sql.Storage
{
    public class DatabaseContextObserver :
        IPipelineObserver<OnBeforeGetStreamEvents>,
        IPipelineObserver<OnAfterGetStreamEvents>,
        IPipelineObserver<OnBeforeSavePrimitiveEvents>,
        IPipelineObserver<OnAfterSavePrimitiveEvents>,
        IPipelineObserver<OnBeforeRemoveEventStream>,
        IPipelineObserver<OnAfterRemoveEventStream>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;

        private const string StateKey = "Shuttle.Recall.Sql.Storage.DatabaseContextObserver:DatabaseContext";

        public DatabaseContextObserver(IDatabaseContextFactory databaseContextFactory)
        {
            _databaseContextFactory = Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
        }

        public void Execute(OnBeforeGetStreamEvents pipelineEvent)
        {
            ExecuteAsync(pipelineEvent).GetAwaiter().GetResult();
        }

        public async Task ExecuteAsync(OnBeforeGetStreamEvents pipelineEvent)
        {
            await CreateDatabaseContextAsync(pipelineEvent);
        }

        private async Task CreateDatabaseContextAsync(PipelineEvent pipelineEvent)
        {
            pipelineEvent.Pipeline.State.Replace(StateKey, _databaseContextFactory.Create());

            await Task.CompletedTask;
        }

        public void Execute(OnAfterGetStreamEvents pipelineEvent)
        {
            ExecuteAsync(pipelineEvent).GetAwaiter().GetResult();
        }

        public async Task ExecuteAsync(OnAfterGetStreamEvents pipelineEvent)
        {
            await DisposeDatabaseContextAsync(pipelineEvent);
        }

        private async Task DisposeDatabaseContextAsync(PipelineEvent pipelineEvent)
        {
            pipelineEvent.Pipeline.State.Get<IDatabaseContext>(StateKey).Dispose();

            await Task.CompletedTask;
        }

        public void Execute(OnBeforeSavePrimitiveEvents pipelineEvent)
        {
            ExecuteAsync(pipelineEvent).GetAwaiter().GetResult();
        }

        public async Task ExecuteAsync(OnBeforeSavePrimitiveEvents pipelineEvent)
        {
            await CreateDatabaseContextAsync(pipelineEvent);
        }

        public void Execute(OnAfterSavePrimitiveEvents pipelineEvent)
        {
            ExecuteAsync(pipelineEvent).GetAwaiter().GetResult();
        }

        public async Task ExecuteAsync(OnAfterSavePrimitiveEvents pipelineEvent)
        {
            await DisposeDatabaseContextAsync(pipelineEvent);
        }

        public void Execute(OnBeforeRemoveEventStream pipelineEvent)
        {
            ExecuteAsync(pipelineEvent).GetAwaiter().GetResult();
        }

        public async Task ExecuteAsync(OnBeforeRemoveEventStream pipelineEvent)
        {
            await CreateDatabaseContextAsync(pipelineEvent);
        }

        public void Execute(OnAfterRemoveEventStream pipelineEvent)
        {
            ExecuteAsync(pipelineEvent).GetAwaiter().GetResult();
        }

        public async Task ExecuteAsync(OnAfterRemoveEventStream pipelineEvent)
        {
            await DisposeDatabaseContextAsync(pipelineEvent);
        }
    }
}