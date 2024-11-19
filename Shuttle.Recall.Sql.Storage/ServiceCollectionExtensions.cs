using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data.ThreadDatabaseContextScope;
using Shuttle.Recall.Sql.Storage.DataAccess;
using Shuttle.Recall.Sql.Storage.SqlServer;

namespace Shuttle.Recall.Sql.Storage;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqlEventStorage(this IServiceCollection services, Action<SqlStorageBuilder>? builder = null)
    {
        var sqlStorageBuilder = new SqlStorageBuilder(Guard.AgainstNull(services));

        builder?.Invoke(sqlStorageBuilder);

        services.TryAddSingleton<IValidateOptions<SqlStorageOptions>, SqlStorageOptionsValidator>();

        services.AddOptions<SqlStorageOptions>().Configure(options =>
        {
            options.ConnectionStringName = sqlStorageBuilder.Options.ConnectionStringName;
            options.Schema = sqlStorageBuilder.Options.Schema;
        });

        services.AddSingleton<IConcurrencyExceptionSpecification, ConcurrencyExceptionSpecification>();
        services.AddSingleton<IPrimitiveEventRepository, PrimitiveEventRepository>();
        services.AddSingleton<IPrimitiveEventQuery, PrimitiveEventQuery>();
        services.AddSingleton<IPrimitiveEventQueryFactory, PrimitiveEventQueryFactory>();
        services.AddSingleton<IIdKeyQueryFactory, IdKeyQueryFactory>();
        services.AddSingleton<IIdKeyRepository, IdKeyRepository>();
        services.AddSingleton<IEventTypeRepository, EventTypeRepository>();
        services.AddSingleton<IEventTypeQueryFactory, EventTypeQueryFactory>();
        services.AddThreadDatabaseContextScope();

        services.AddHostedService<EventStoreHostedService>();

        return services;
    }
}