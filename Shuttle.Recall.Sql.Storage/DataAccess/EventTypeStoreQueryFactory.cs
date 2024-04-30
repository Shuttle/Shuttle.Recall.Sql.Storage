using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
    public class EventTypeStoreQueryFactory : IEventTypeStoreQueryFactory
    {
        private readonly SqlStorageOptions _sqlStorageOptions;
        private readonly IScriptProvider _scriptProvider;

        public EventTypeStoreQueryFactory(IOptions<SqlStorageOptions> sqlStorageOptions, IScriptProvider scriptProvider)
        {
            Guard.AgainstNull(sqlStorageOptions, nameof(sqlStorageOptions));

            _sqlStorageOptions = Guard.AgainstNull(sqlStorageOptions.Value, nameof(sqlStorageOptions.Value));
            _scriptProvider = Guard.AgainstNull(scriptProvider, nameof(scriptProvider));
        }

        public IQuery GetId(string typeName)
        {
            return new Query(_scriptProvider.Get(_sqlStorageOptions.ConnectionStringName, "EventTypeStore.GetId")).AddParameter(Columns.TypeName, typeName);
        }
    }
}