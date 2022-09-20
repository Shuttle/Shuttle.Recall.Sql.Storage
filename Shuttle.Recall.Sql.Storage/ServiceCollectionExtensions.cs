using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shuttle.Core.Contract;

namespace Shuttle.Recall.Sql.Storage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlEventStorage(this IServiceCollection services)
        {
            Guard.AgainstNull(services, nameof(services));

            services.TryAddSingleton<IScriptProvider, ScriptProvider>();

            services.TryAddSingleton<IConcurrencyExceptionSpecification, ConcurrencyExceptionSpecification>();

            services.AddSingleton<IPrimitiveEventRepository, PrimitiveEventRepository>();
            services.AddSingleton<IPrimitiveEventQuery, PrimitiveEventQuery>();
            services.AddSingleton<IPrimitiveEventQueryFactory, PrimitiveEventQueryFactory>();
            services.AddSingleton<IKeyStoreQueryFactory, KeyStoreQueryFactory>();
            services.AddSingleton<IKeyStore, KeyStore>();
            services.AddSingleton<IEventTypeStore, EventTypeStore>();
            services.AddSingleton<IEventTypeStoreQueryFactory, EventTypeStoreQueryFactory>();

            return services;
        }
    }
}