using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlEventStorage(this IServiceCollection services)
        {
            Guard.AgainstNull(services, nameof(services));

            services.TryAddSingleton<IScriptProvider, ScriptProvider>();

            services.TryAddSingleton<IConcurrencyExceptionSpecification, ConcurrencyExceptionSpecification>();

            services.TryAddSingleton<IPrimitiveEventRepository, PrimitiveEventRepository>();
            services.TryAddSingleton<IPrimitiveEventQueryFactory, PrimitiveEventQueryFactory>();
            services.TryAddSingleton<IKeyStoreQueryFactory, KeyStoreQueryFactory>();
            services.TryAddSingleton<IKeyStore, KeyStore>();
            services.TryAddSingleton<IEventTypeStore, EventTypeStore>();
            services.TryAddSingleton<IEventTypeStoreQueryFactory, EventTypeStoreQueryFactory>();

            return services;
        }
    }
}