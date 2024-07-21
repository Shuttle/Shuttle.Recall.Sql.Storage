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

            services.AddSingleton<IScriptProvider, ScriptProvider>();
            services.AddSingleton<IConcurrencyExceptionSpecification, ConcurrencyExceptionSpecification>();
            services.AddSingleton<IPrimitiveEventRepository, PrimitiveEventRepository>();
            services.AddSingleton<IPrimitiveEventQuery, PrimitiveEventQuery>();
            services.AddSingleton<IPrimitiveEventQueryFactory, PrimitiveEventQueryFactory>();
            services.AddSingleton<IKeyStoreQueryFactory, KeyStoreQueryFactory>();
            services.AddSingleton<IKeyStore, KeyStore>();
            services.AddSingleton<IEventTypeStore, EventTypeStore>();
            services.AddSingleton<IEventTypeStoreQueryFactory, EventTypeStoreQueryFactory>();

            services.AddHostedService<EventStoreHostedService>();

            return services;
        }
    }
}