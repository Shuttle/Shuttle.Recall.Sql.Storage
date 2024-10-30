using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage;

public class EventTypeStoreQueryFactory : IEventTypeStoreQueryFactory
{
    private readonly IScriptProvider _scriptProvider;
    private readonly SqlStorageOptions _sqlStorageOptions;

    public EventTypeStoreQueryFactory(IOptions<SqlStorageOptions> sqlStorageOptions, IScriptProvider scriptProvider)
    {
        _sqlStorageOptions = Guard.AgainstNull(Guard.AgainstNull(sqlStorageOptions).Value);
        _scriptProvider = Guard.AgainstNull(scriptProvider);
    }

    public IQuery GetId(string typeName)
    {
        return new Query(_scriptProvider.Get(_sqlStorageOptions.ConnectionStringName, "EventTypeStore.GetId")).AddParameter(Columns.TypeName, typeName);
    }
}