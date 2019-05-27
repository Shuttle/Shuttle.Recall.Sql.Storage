using System;
using System.Collections.Generic;
using System.Linq;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
    public class PrimitiveEventQueryFactory : IPrimitiveEventQueryFactory
    {
        private readonly IScriptProvider _scriptProvider;

        public PrimitiveEventQueryFactory(IScriptProvider scriptProvider)
        {
            Guard.AgainstNull(scriptProvider, nameof(scriptProvider));

            _scriptProvider = scriptProvider;
        }

        public IQuery RemoveSnapshot(Guid id)
        {
            return new RawQuery(_scriptProvider.Get("EventStore.RemoveSnapshot")).AddParameterValue(EventStoreColumns.Id, id);
        }

        public IQuery RemoveEventStream(Guid id)
        {
            return new RawQuery(_scriptProvider.Get("EventStore.RemoveEventStream")).AddParameterValue(EventStoreColumns.Id, id);
        }

        public IQuery GetEventStream(Guid id)
        {
            return new RawQuery(_scriptProvider.Get("EventStore.GetEventStream")).AddParameterValue(EventStoreColumns.Id, id);
        }

        public IQuery Get(long fromSequenceNumber, long toSequenceNumber, IEnumerable<Type> eventTypes)
        {
            return
                new RawQuery(string.Format(_scriptProvider.Get("EventStore.Get"),
                    eventTypes == null || !eventTypes.Any()
                        ? string.Empty
                        : $"and EventType in ({string.Join(",", eventTypes.Select(eventType => string.Concat("'", eventType, "'")).ToArray())})"))
                    .AddParameterValue(EventStoreColumns.FromSequenceNumber, fromSequenceNumber)
                    .AddParameterValue(EventStoreColumns.ToSequenceNumber, toSequenceNumber);
        }

        public IQuery SaveEvent(PrimitiveEvent primitiveEvent)
        {
            return new RawQuery(_scriptProvider.Get("EventStore.Save"))
                    .AddParameterValue(EventStoreColumns.Id, primitiveEvent.Id)
                    .AddParameterValue(EventStoreColumns.DateRegistered, primitiveEvent.DateRegistered)
                    .AddParameterValue(EventStoreColumns.EventEnvelope, primitiveEvent.EventEnvelope)
                    .AddParameterValue(EventStoreColumns.EventType, primitiveEvent.EventType)
                    .AddParameterValue(EventStoreColumns.Version, primitiveEvent.Version)
                    .AddParameterValue(EventStoreColumns.IsSnapshot, primitiveEvent.IsSnapshot);
        }

        public IQuery SaveSnapshot(PrimitiveEvent primitiveEvent)
        {
            return new RawQuery(_scriptProvider.Get("SnapshotStore.Save"))
                    .AddParameterValue(EventStoreColumns.Id, primitiveEvent.Id)
                    .AddParameterValue(EventStoreColumns.Version, primitiveEvent.Version);
        }
    }
}