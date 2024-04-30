using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
    public class PrimitiveEventQueryFactory : IPrimitiveEventQueryFactory
    {
        private readonly SqlStorageOptions _sqlStorageOptions;
        private readonly IScriptProvider _scriptProvider;
        private readonly IEventTypeStore _eventTypeStore;

        public PrimitiveEventQueryFactory(IOptions<SqlStorageOptions> sqlStorageOptions, IScriptProvider scriptProvider, IEventTypeStore eventTypeStore)
        {
            Guard.AgainstNull(sqlStorageOptions, nameof(sqlStorageOptions));

            _sqlStorageOptions = Guard.AgainstNull(sqlStorageOptions.Value, nameof(sqlStorageOptions.Value));
            _scriptProvider = Guard.AgainstNull(scriptProvider, nameof(scriptProvider));
            _eventTypeStore = Guard.AgainstNull(eventTypeStore, nameof(eventTypeStore));
        }

        public IQuery RemoveSnapshot(Guid id)
        {
            return new Query(_scriptProvider.Get(_sqlStorageOptions.ConnectionStringName, "EventStore.RemoveSnapshot")).AddParameter(Columns.Id, id);
        }

        public IQuery RemoveEventStream(Guid id)
        {
            return new Query(_scriptProvider.Get(_sqlStorageOptions.ConnectionStringName, "EventStore.RemoveEventStream")).AddParameter(Columns.Id, id);
        }

        public IQuery GetEventStream(Guid id)
        {
            return new Query(_scriptProvider.Get(_sqlStorageOptions.ConnectionStringName, "EventStore.GetEventStream")).AddParameter(Columns.Id, id);
        }

        public IQuery Search(PrimitiveEvent.Specification specification)
        {
            Guard.AgainstNull(specification, nameof(specification));

            var eventTypeIds = specification.EventTypes.Select(eventType => _eventTypeStore.GetId(eventType.FullName)).ToList();

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

        public IQuery SaveEvent(PrimitiveEvent primitiveEvent)
        {
            return new Query(_scriptProvider.Get(_sqlStorageOptions.ConnectionStringName, "EventStore.Save"))
                .AddParameter(Columns.Id, primitiveEvent.Id)
                .AddParameter(Columns.DateRegistered, primitiveEvent.DateRegistered)
                .AddParameter(Columns.EventEnvelope, primitiveEvent.EventEnvelope)
                .AddParameter(Columns.EventId, primitiveEvent.EventId)
                .AddParameter(Columns.EventTypeId, _eventTypeStore.GetId(primitiveEvent.EventType))
                .AddParameter(Columns.Version, primitiveEvent.Version)
                .AddParameter(Columns.IsSnapshot, primitiveEvent.IsSnapshot);
        }

        public IQuery SaveSnapshot(PrimitiveEvent primitiveEvent)
        {
            return new Query(_scriptProvider.Get(_sqlStorageOptions.ConnectionStringName, "SnapshotStore.Save"))
                .AddParameter(Columns.Id, primitiveEvent.Id)
                .AddParameter(Columns.Version, primitiveEvent.Version);
        }

        public IQuery GetSequenceNumber(Guid id)
        {
            return new Query(_scriptProvider.Get(_sqlStorageOptions.ConnectionStringName, "EventStore.GetSequenceNumber")).AddParameter(Columns.Id, id);
        }
    }
}