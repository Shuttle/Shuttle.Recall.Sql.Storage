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
        private const string DisposeDatabaseContextStateKey = "Shuttle.Recall.Sql.Storage.DatabaseContextObserver:DisposeDatabaseContext";

        public DatabaseContextObserver(SqlStorageOptions sqlStorageOptions, IDatabaseContextService databaseContextService, IDatabaseContextFactory databaseContextFactory)
        {
            _sqlStorageOptions = Guard.AgainstNull(sqlStorageOptions, nameof(sqlStorageOptions));
            _databaseContextService = Guard.AgainstNull(databaseContextService, nameof(databaseContextService));
            _databaseContextFactory = Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
        }

        public async Task ExecuteAsync(IPipelineContext<OnBeforeGetStreamEvents> pipelineContext)
        {
            await CreateDatabaseContextAsync(pipelineContext);
        }
        private async Task CreateDatabaseContextAsync(IPipelineContext pipelineContext)
        {
            var hasDatabaseContext = _databaseContextService.Contains(_sqlStorageOptions.ConnectionStringName);

            Guard.AgainstNull(pipelineContext).Pipeline.State.Add(DisposeDatabaseContextStateKey, !hasDatabaseContext);

            if (!hasDatabaseContext)
            {
                pipelineContext.Pipeline.State.Add(DatabaseContextStateKey, _databaseContextFactory.Create(_sqlStorageOptions.ConnectionStringName));
            }

            await Task.CompletedTask;
        }

        public async Task ExecuteAsync(IPipelineContext<OnAfterGetStreamEvents> pipelineContext)
        {
            await DisposeDatabaseContextAsync(pipelineContext);
        }

        private async Task DisposeDatabaseContextAsync(IPipelineContext pipelineContext)
        {
            if (Guard.AgainstNull(pipelineContext).Pipeline.State.Get<bool>(DisposeDatabaseContextStateKey))
            {
                await Guard.AgainstNull(pipelineContext.Pipeline.State.Get<IDatabaseContext>(DatabaseContextStateKey)).TryDisposeAsync();
            }

            pipelineContext.Pipeline.State.Remove(DatabaseContextStateKey);
            pipelineContext.Pipeline.State.Remove(DisposeDatabaseContextStateKey);

            await Task.CompletedTask;
        }

        public async Task ExecuteAsync(IPipelineContext<OnBeforeSavePrimitiveEvents> pipelineContext)
        {
            await CreateDatabaseContextAsync(pipelineContext);
        }

        public async Task ExecuteAsync(IPipelineContext<OnAfterSavePrimitiveEvents> pipelineContext)
        {
            await DisposeDatabaseContextAsync(pipelineContext);
        }

        public async Task ExecuteAsync(IPipelineContext<OnBeforeRemoveEventStream> pipelineContext)
        {
            await CreateDatabaseContextAsync(pipelineContext);
        }

        public async Task ExecuteAsync(IPipelineContext<OnAfterRemoveEventStream> pipelineContext)
        {
            await DisposeDatabaseContextAsync(pipelineContext);
        }
    }
}