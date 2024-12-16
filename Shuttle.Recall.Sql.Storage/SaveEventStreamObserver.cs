using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Core.Pipelines;

namespace Shuttle.Recall.Sql.Storage;

public class SaveEventStreamObserver : IPipelineObserver<OnSaveEventStreamCompleted>
{
    private readonly IPrimitiveEventQueryFactory _primitiveEventQueryFactory;
    private readonly SqlStorageOptions _sqlStorageOptions;
    private readonly IDatabaseContextService _databaseContextService;

    public SaveEventStreamObserver(IOptions<SqlStorageOptions> sqlServerStorageOptions, IDatabaseContextService databaseContextService, IPrimitiveEventQueryFactory primitiveEventQueryFactory)
    {
        _sqlStorageOptions = Guard.AgainstNull(Guard.AgainstNull(sqlServerStorageOptions).Value);
        _databaseContextService = Guard.AgainstNull(databaseContextService);
        _primitiveEventQueryFactory = Guard.AgainstNull(primitiveEventQueryFactory);
    }

    public async Task ExecuteAsync(IPipelineContext<OnSaveEventStreamCompleted> pipelineContext)
    {
        var state = Guard.AgainstNull(pipelineContext).Pipeline.State;

        await _databaseContextService.Active.ExecuteAsync(_primitiveEventQueryFactory.Commit(state.GetEventStream().Id));
    }
}