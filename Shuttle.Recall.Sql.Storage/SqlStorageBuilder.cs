using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Recall.Sql.Storage;

public class SqlStorageBuilder
{
    private SqlStorageOptions _sqlStorageOptions = new();

    public SqlStorageBuilder(IServiceCollection services)
    {
        Services = Guard.AgainstNull(services);
    }

    public SqlStorageOptions Options
    {
        get => _sqlStorageOptions;
        set => _sqlStorageOptions = Guard.AgainstNull(value);
    }

    public IServiceCollection Services { get; }

    public SqlStorageBuilder UseSqlServer()
    {
        Services.AddSingleton<IPrimitiveEventQueryFactory, PrimitiveEventQueryFactory>();
        Services.AddSingleton<IEventTypeQueryFactory, EventTypeQueryFactory>();

        return this;
    }
}