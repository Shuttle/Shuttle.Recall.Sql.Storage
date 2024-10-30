using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage;

public class PrimitiveEventQuery : IPrimitiveEventQuery
{
    private readonly IEventTypeStore _eventTypeStore;
    private readonly IDatabaseContextFactory _databaseContextFactory;
    private readonly IPrimitiveEventQueryFactory _queryFactory;
    private readonly IQueryMapper _queryMapper;
    private readonly SqlStorageOptions _sqlStorageOptions;

    public PrimitiveEventQuery(IOptions<SqlStorageOptions> sqlStorageOptions, IDatabaseContextFactory databaseContextFactory, IQueryMapper queryMapper, IPrimitiveEventQueryFactory queryFactory, IEventTypeStore eventTypeStore)
    {
        _sqlStorageOptions = Guard.AgainstNull(Guard.AgainstNull(sqlStorageOptions).Value);
        _databaseContextFactory = Guard.AgainstNull(databaseContextFactory);
        _queryMapper = Guard.AgainstNull(queryMapper);
        _queryFactory = Guard.AgainstNull(queryFactory);
        _eventTypeStore = Guard.AgainstNull(eventTypeStore);
    }

    public async Task<IEnumerable<PrimitiveEvent>> SearchAsync(PrimitiveEvent.Specification specification)
    {
        await using var databaseContext = _databaseContextFactory.Create(_sqlStorageOptions.ConnectionStringName);
        await using var transaction = await databaseContext.BeginTransactionAsync();
        
        var eventTypeIds = new List<Guid>();

        foreach (var eventType in specification.EventTypes)
        {
            eventTypeIds.Add(await _eventTypeStore.GetIdAsync(databaseContext, Guard.AgainstNullOrEmptyString(eventType.FullName)));
        }

        await transaction.CommitTransactionAsync();

        return await _queryMapper.MapObjectsAsync<PrimitiveEvent>(databaseContext, _queryFactory.Search(specification, eventTypeIds)).ConfigureAwait(false);
    }
}