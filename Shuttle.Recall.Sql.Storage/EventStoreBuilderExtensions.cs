using Microsoft.Extensions.DependencyInjection.Extensions;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
    public static class EventStoreBuilderExtensions
    {
        public static EventStoreBuilder UseSqlStorage(this EventStoreBuilder eventStoreBuilder)
        {
            Guard.AgainstNull(eventStoreBuilder, nameof(eventStoreBuilder));

            eventStoreBuilder.Services.TryAddSingleton<IScriptProvider, ScriptProvider>();

            eventStoreBuilder.Services
                .TryAddSingleton<IConcurrencyExceptionSpecification, ConcurrencyExceptionSpecification>();

            eventStoreBuilder.Services.AddDataAccess();

            eventStoreBuilder.Services.TryAddSingleton<IPrimitiveEventRepository, PrimitiveEventRepository>();
            eventStoreBuilder.Services.TryAddSingleton<IPrimitiveEventQueryFactory, PrimitiveEventQueryFactory>();
            eventStoreBuilder.Services.TryAddSingleton<IKeyStoreQueryFactory, KeyStoreQueryFactory>();
            eventStoreBuilder.Services.TryAddSingleton<IKeyStore, KeyStore>();
            eventStoreBuilder.Services.TryAddSingleton<IEventTypeStore, EventTypeStore>();
            eventStoreBuilder.Services.TryAddSingleton<IEventTypeStoreQueryFactory, EventTypeStoreQueryFactory>();

            return eventStoreBuilder;
        }
    }
}