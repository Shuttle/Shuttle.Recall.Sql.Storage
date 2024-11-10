using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage;

public class PrimitiveEventQueryFactory : IPrimitiveEventQueryFactory
{
    private readonly IEventTypeStore _eventTypeStore;
    private readonly IScriptProvider _scriptProvider;
    private readonly SqlStorageOptions _sqlStorageOptions;

    public PrimitiveEventQueryFactory(IOptions<SqlStorageOptions> sqlStorageOptions, IScriptProvider scriptProvider, IEventTypeStore eventTypeStore)
    {
        _sqlStorageOptions = Guard.AgainstNull(Guard.AgainstNull(sqlStorageOptions).Value);
        _scriptProvider = Guard.AgainstNull(scriptProvider);
        _eventTypeStore = Guard.AgainstNull(eventTypeStore);
    }

    public IQuery RemoveEventStream(Guid id)
    {
        return new Query(_scriptProvider.Get(_sqlStorageOptions.ConnectionStringName, "EventStore.RemoveEventStream")).AddParameter(Columns.Id, id);
    }

    public IQuery GetEventStream(Guid id)
    {
        return new Query(_scriptProvider.Get(_sqlStorageOptions.ConnectionStringName, "EventStore.GetEventStream")).AddParameter(Columns.Id, id);
    }

    public IQuery Search(PrimitiveEventSpecification specification, IEnumerable<Guid> eventTypeIds)
    {
        Guard.AgainstNull(specification);

        var whereEventTypeIds = !eventTypeIds.Any()
            ? string.Empty
            : $" and EventTypeId in ({string.Join(",", eventTypeIds.Select(id => string.Concat("'", id, "'")).ToArray())})";

        var whereIds = !specification.Ids.Any()
            ? string.Empty
            : $" and Id in ({string.Join(",", specification.Ids.Select(id => string.Concat("'", id, "'")).ToArray())})";

        return
            new Query(string.Format(_scriptProvider.Get(_sqlStorageOptions.ConnectionStringName, "EventStore.Search"),
                    specification.Count > 0
                        ? specification.Count
                        : 1,
                    $"{whereEventTypeIds}{whereIds}"))
                .AddParameter(Columns.FromSequenceNumber, specification.SequenceNumberStart);
    }

    public IQuery SaveEvent(PrimitiveEvent primitiveEvent, Guid eventTypeId)
    {
        return new Query(_scriptProvider.Get(_sqlStorageOptions.ConnectionStringName, "EventStore.Save"))
            .AddParameter(Columns.Id, primitiveEvent.Id)
            .AddParameter(Columns.Version, primitiveEvent.Version)
            .AddParameter(Columns.CorrelationId, primitiveEvent.CorrelationId)
            .AddParameter(Columns.DateRegistered, primitiveEvent.DateRegistered)
            .AddParameter(Columns.EventEnvelope, primitiveEvent.EventEnvelope)
            .AddParameter(Columns.EventId, primitiveEvent.EventId)
            .AddParameter(Columns.EventTypeId, eventTypeId);
    }

    public IQuery GetSequenceNumber(Guid id)
    {
        return new Query(_scriptProvider.Get(_sqlStorageOptions.ConnectionStringName, "EventStore.GetSequenceNumber")).AddParameter(Columns.Id, id);
    }
}