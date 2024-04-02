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
        private readonly bool _manageEventStoreConnections;
        private readonly string _connectionStringName;
        private const string StateKey = "DatabaseContextObserver:DatabaseContext";
        private readonly IDatabaseContextFactory _databaseContextFactory;

        public DatabaseContextObserver(bool manageEventStoreConnections, string connectionStringName, IDatabaseContextFactory databaseContextFactory)
        {
            _manageEventStoreConnections = manageEventStoreConnections;
            _connectionStringName = Guard.AgainstNullOrEmptyString(connectionStringName, nameof(connectionStringName));
            _databaseContextFactory = Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
        }

        public void Execute(OnBeforeGetStreamEvents pipelineEvent)
        {
            ExecuteAsync(pipelineEvent).GetAwaiter().GetResult();
        }

        public async Task ExecuteAsync(OnBeforeGetStreamEvents pipelineEvent)
        {
            SetDatabaseContext(pipelineEvent);

            await Task.CompletedTask;
        }

        private void SetDatabaseContext(PipelineEvent pipelineEvent)
        {
            var state = Guard.AgainstNull(pipelineEvent, nameof(pipelineEvent)).Pipeline.State;

            var eventStreamBuilder = state.GetEventStreamBuilder();

            if (!(eventStreamBuilder is { ShouldIgnoreConnectionRequest: true }) && _manageEventStoreConnections)
            {
                pipelineEvent.Pipeline.State.Replace(StateKey, _databaseContextFactory.Create(_connectionStringName));
            }
        }

        public void Execute(OnAfterGetStreamEvents pipelineEvent)
        {
            ExecuteAsync(pipelineEvent).GetAwaiter().GetResult();
        }

        public async Task ExecuteAsync(OnAfterGetStreamEvents pipelineEvent)
        {
            ReleaseDatabaseContext(pipelineEvent);

            await Task.CompletedTask;
        }

        private void ReleaseDatabaseContext(PipelineEvent pipelineEvent)
        {
            Guard.AgainstNull(pipelineEvent, nameof(pipelineEvent)).Pipeline.State.Get<IDatabaseContext>(StateKey)?.TryDispose();
        }

        public void Execute(OnBeforeSavePrimitiveEvents pipelineEvent)
        {
            ExecuteAsync(pipelineEvent).GetAwaiter().GetResult();
        }

        public async Task ExecuteAsync(OnBeforeSavePrimitiveEvents pipelineEvent)
        {
            SetDatabaseContext(pipelineEvent);

            await Task.CompletedTask;
        }

        public void Execute(OnAfterSavePrimitiveEvents pipelineEvent)
        {
            ExecuteAsync(pipelineEvent).GetAwaiter().GetResult();
        }

        public async Task ExecuteAsync(OnAfterSavePrimitiveEvents pipelineEvent)
        {
            ReleaseDatabaseContext(pipelineEvent);

            await Task.CompletedTask;
        }

        public void Execute(OnBeforeRemoveEventStream pipelineEvent)
        {
            ExecuteAsync(pipelineEvent).GetAwaiter().GetResult();
        }

        public async Task ExecuteAsync(OnBeforeRemoveEventStream pipelineEvent)
        {
            SetDatabaseContext(pipelineEvent);

            await Task.CompletedTask;
        }

        public void Execute(OnAfterRemoveEventStream pipelineEvent)
        {
            ExecuteAsync(pipelineEvent).GetAwaiter().GetResult();
        }

        public async Task ExecuteAsync(OnAfterRemoveEventStream pipelineEvent)
        {
            ReleaseDatabaseContext(pipelineEvent);

            await Task.CompletedTask;
        }
    }
}