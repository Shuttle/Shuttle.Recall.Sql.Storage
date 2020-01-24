using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
    public class EventTypeStoreQueryFactory : IEventTypeStoreQueryFactory
    {
        private readonly IScriptProvider _scriptProvider;

        public EventTypeStoreQueryFactory(IScriptProvider scriptProvider)
        {
            Guard.AgainstNull(scriptProvider, nameof(scriptProvider));

            _scriptProvider = scriptProvider;
        }

        public IQuery GetId(string typeName)
        {
            return new RawQuery(_scriptProvider.Get("EventTypeStore.GetId")).AddParameterValue(Columns.TypeName, typeName);
        }
    }
}