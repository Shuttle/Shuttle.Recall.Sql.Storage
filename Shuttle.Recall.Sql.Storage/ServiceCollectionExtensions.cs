using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Recall.Sql.Storage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlEventStorage(this IServiceCollection services, Action<SqlStorageBuilder> builder = null)
        {
            Guard.AgainstNull(services, nameof(services));

            var dataAccessBuilder = new SqlStorageBuilder(services);

            builder?.Invoke(dataAccessBuilder);

            services.TryAddSingleton<IValidateOptions<SqlStorageOptions>, SqlStorageOptionsValidator>();

            services.AddOptions<SqlStorageOptions>().Configure(options =>
            {
                options.ConnectionStringName = dataAccessBuilder.Options.ConnectionStringName;
            });

            services.TryAddSingleton<IScriptProvider, ScriptProvider>();
            services.TryAddSingleton<IConcurrencyExceptionSpecification, ConcurrencyExceptionSpecification>();
            services.TryAddSingleton<IPrimitiveEventRepository, PrimitiveEventRepository>();
            services.TryAddSingleton<IPrimitiveEventQuery, PrimitiveEventQuery>();
            services.TryAddSingleton<IPrimitiveEventQueryFactory, PrimitiveEventQueryFactory>();
            services.TryAddSingleton<IKeyStoreQueryFactory, KeyStoreQueryFactory>();
            services.TryAddSingleton<IKeyStore, KeyStore>();
            services.TryAddSingleton<IEventTypeStore, EventTypeStore>();
            services.TryAddSingleton<IEventTypeStoreQueryFactory, EventTypeStoreQueryFactory>();

            services.AddHostedService<EventStoreHostedService>();

            return services;
        }
    }
}