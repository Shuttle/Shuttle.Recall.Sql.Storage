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
        IPipelineObserver<OnAfterRemoveEventStream>
    {
        private readonly IDatabaseContextService _databaseContextService;
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly SqlStorageOptions _sqlStorageOptions;

        private const string DatabaseContextStateKey = "Shuttle.Recall.Sql.Storage.DatabaseContextObserver:DatabaseContext";
        private const string CloseConnectionStateKey = "Shuttle.Recall.Sql.Storage.DatabaseContextObserver:CloseConnection";

        public DatabaseContextObserver(SqlStorageOptions sqlStorageOptions, IDatabaseContextService databaseContextService, IDatabaseContextFactory databaseContextFactory)
        {
            _sqlStorageOptions = Guard.AgainstNull(sqlStorageOptions, nameof(sqlStorageOptions));
            _databaseContextService = Guard.AgainstNull(databaseContextService, nameof(databaseContextService));
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
            var hasExistingConnection = _databaseContextService.Contains(_sqlStorageOptions.ConnectionStringName);

            pipelineEvent.Pipeline.State.Add(CloseConnectionStateKey, !hasExistingConnection);

            if (!hasExistingConnection)
            {
                pipelineEvent.Pipeline.State.Add(DatabaseContextStateKey, _databaseContextFactory.Create(_sqlStorageOptions.ConnectionStringName));
            }

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
            if (pipelineEvent.Pipeline.State.Get<bool>(CloseConnectionStateKey))
            {
                await pipelineEvent.Pipeline.State.Get<IDatabaseContext>(DatabaseContextStateKey).TryDisposeAsync();
            }

            pipelineEvent.Pipeline.State.Remove(DatabaseContextStateKey);
            pipelineEvent.Pipeline.State.Remove(CloseConnectionStateKey);

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