using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Core.Pipelines;
using Shuttle.Core.Reflection;

namespace Shuttle.Recall.Sql.Storage
{
    public class DatabaseContextObserver :
        IPipelineObserver<OnBeforeGetStreamEvents>,
        IPipelineObserver<OnAfterGetStreamEvents>,
        IPipelineObserver<OnBeforeSavePrimitiveEvents>,
        IPipelineObserver<OnAfterSavePrimitiveEvents>,
        IPipelineObserver<OnBeforeRemoveEventStream>,
        IPipelineObserver<OnAfterRemoveEventStream>,
        IPipelineObserver<OnPipelineException>
    {
        private const string DatabaseContextStateKey = "Shuttle.Recall.Sql.Storage.DatabaseContextObserver:DatabaseContext";
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly SqlStorageOptions _sqlStorageOptions;

        public DatabaseContextObserver(SqlStorageOptions sqlStorageOptions, IDatabaseContextFactory databaseContextFactory)
        {
            _sqlStorageOptions = Guard.AgainstNull(sqlStorageOptions, nameof(sqlStorageOptions));
            _databaseContextFactory = Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
        }

        public void Execute(OnAfterGetStreamEvents pipelineEvent)
        {
            ExecuteAsync(pipelineEvent).GetAwaiter().GetResult();
        }

        public async Task ExecuteAsync(OnAfterGetStreamEvents pipelineEvent)
        {
            await DisposeDatabaseContextAsync(pipelineEvent);
        }

        public void Execute(OnAfterRemoveEventStream pipelineEvent)
        {
            ExecuteAsync(pipelineEvent).GetAwaiter().GetResult();
        }

        public async Task ExecuteAsync(OnAfterRemoveEventStream pipelineEvent)
        {
            await DisposeDatabaseContextAsync(pipelineEvent);
        }

        public void Execute(OnAfterSavePrimitiveEvents pipelineEvent)
        {
            ExecuteAsync(pipelineEvent).GetAwaiter().GetResult();
        }

        public async Task ExecuteAsync(OnAfterSavePrimitiveEvents pipelineEvent)
        {
            await DisposeDatabaseContextAsync(pipelineEvent);
        }

        public void Execute(OnBeforeGetStreamEvents pipelineEvent)
        {
            ExecuteAsync(pipelineEvent).GetAwaiter().GetResult();
        }

        public async Task ExecuteAsync(OnBeforeGetStreamEvents pipelineEvent)
        {
            await CreateDatabaseContextAsync(pipelineEvent);
        }

        public void Execute(OnBeforeRemoveEventStream pipelineEvent)
        {
            ExecuteAsync(pipelineEvent).GetAwaiter().GetResult();
        }

        public async Task ExecuteAsync(OnBeforeRemoveEventStream pipelineEvent)
        {
            await CreateDatabaseContextAsync(pipelineEvent);
        }

        public void Execute(OnBeforeSavePrimitiveEvents pipelineEvent)
        {
            ExecuteAsync(pipelineEvent).GetAwaiter().GetResult();
        }

        public async Task ExecuteAsync(OnBeforeSavePrimitiveEvents pipelineEvent)
        {
            await CreateDatabaseContextAsync(pipelineEvent);
        }

        public void Execute(OnPipelineException pipelineEvent)
        {
            ExecuteAsync(pipelineEvent).GetAwaiter().GetResult();
        }

        public async Task ExecuteAsync(OnPipelineException pipelineEvent)
        {
            await DisposeDatabaseContextAsync(pipelineEvent);
        }

        private async Task CreateDatabaseContextAsync(PipelineEvent pipelineEvent)
        {
            pipelineEvent.Pipeline.State.Add(DatabaseContextStateKey, _databaseContextFactory.Create(_sqlStorageOptions.ConnectionStringName));

            await Task.CompletedTask;
        }

        private async Task DisposeDatabaseContextAsync(PipelineEvent pipelineEvent)
        {
            var databaseContext = pipelineEvent.Pipeline.State.Get<IDatabaseContext>(DatabaseContextStateKey);

            if (databaseContext != null)
            {
                await databaseContext.TryDisposeAsync();
            }

            pipelineEvent.Pipeline.State.Remove(DatabaseContextStateKey);
        }
    }
}