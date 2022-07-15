using System;
using System.Collections.Generic;
using System.Linq;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
    public class PrimitiveEventQueryFactory : IPrimitiveEventQueryFactory
    {
        private readonly IEventTypeStore _eventTypeStore;
        private readonly IScriptProvider _scriptProvider;

        public PrimitiveEventQueryFactory(IScriptProvider scriptProvider, IEventTypeStore eventTypeStore)
        {
            Guard.AgainstNull(scriptProvider, nameof(scriptProvider));
            Guard.AgainstNull(eventTypeStore, nameof(eventTypeStore));

            _scriptProvider = scriptProvider;
            _eventTypeStore = eventTypeStore;
        }

        public IQuery RemoveSnapshot(Guid id)
        {
            return new RawQuery(_scriptProvider.Get("EventStore.RemoveSnapshot")).AddParameterValue(Columns.Id, id);
        }

        public IQuery RemoveEventStream(Guid id)
        {
            return new RawQuery(_scriptProvider.Get("EventStore.RemoveEventStream")).AddParameterValue(Columns.Id, id);
        }

        public IQuery GetEventStream(Guid id)
        {
            return new RawQuery(_scriptProvider.Get("EventStore.GetEventStream")).AddParameterValue(Columns.Id, id);
        }

        public IQuery Fetch(long fromSequenceNumber, int fetchCount, IEnumerable<Type> eventTypes)
        {
            var eventTypeIds = eventTypes.Select(eventType => _eventTypeStore.GetId(eventType.FullName)).ToList();

            return
                new RawQuery(string.Format(_scriptProvider.Get("EventStore.Fetch"),
                        fetchCount>0
                            ? fetchCount
                                : 1,
                        !eventTypeIds.Any()
                            ? string.Empty
                            : $"and EventTypeId in ({string.Join(",", eventTypeIds.Select(id => string.Concat("'", id, "'")).ToArray())})"))
                    .AddParameterValue(Columns.FromSequenceNumber, fromSequenceNumber);
        }

        public IQuery SaveEvent(PrimitiveEvent primitiveEvent)
        {
            return new RawQuery(_scriptProvider.Get("EventStore.Save"))
                .AddParameterValue(Columns.Id, primitiveEvent.Id)
                .AddParameterValue(Columns.DateRegistered, primitiveEvent.DateRegistered)
                .AddParameterValue(Columns.EventEnvelope, primitiveEvent.EventEnvelope)
                .AddParameterValue(Columns.EventId, primitiveEvent.EventId)
                .AddParameterValue(Columns.EventTypeId, _eventTypeStore.GetId(primitiveEvent.EventType))
                .AddParameterValue(Columns.Version, primitiveEvent.Version)
                .AddParameterValue(Columns.IsSnapshot, primitiveEvent.IsSnapshot);
        }

        public IQuery SaveSnapshot(PrimitiveEvent primitiveEvent)
        {
            return new RawQuery(_scriptProvider.Get("SnapshotStore.Save"))
                .AddParameterValue(Columns.Id, primitiveEvent.Id)
                .AddParameterValue(Columns.Version, primitiveEvent.Version);
        }

        public IQuery GetSequenceNumber(Guid id)
        {
            return new RawQuery(_scriptProvider.Get("EventStore.GetSequenceNumber")).AddParameterValue(Columns.Id, id);
        }
    }
}