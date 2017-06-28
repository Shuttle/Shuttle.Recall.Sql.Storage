using System;
using System.Collections.Generic;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Recall.Sql.Storage
{
    public class PrimitiveEventRepository : IPrimitiveEventRepository
    {
        private readonly IDatabaseGateway _databaseGateway;
        private readonly IQueryMapper _queryMapper;
        private readonly IPrimitiveEventQueryFactory _queryFactory;

        public PrimitiveEventRepository(IDatabaseGateway databaseGateway, IQueryMapper queryMapper, IPrimitiveEventQueryFactory queryFactory)
        {
            Guard.AgainstNull(databaseGateway, "databaseGateway");
            Guard.AgainstNull(queryMapper, "queryMapper");
            Guard.AgainstNull(queryFactory, "queryFactory");

            _databaseGateway = databaseGateway;
            _queryMapper = queryMapper;
            _queryFactory = queryFactory;
        }

        public void Remove(Guid id)
        {
            _databaseGateway.ExecuteUsing(_queryFactory.RemoveSnapshot(id));
            _databaseGateway.ExecuteUsing(_queryFactory.RemoveEventStream(id));
        }

        public IEnumerable<PrimitiveEvent> Get(Guid id)
        {
            return _queryMapper.MapObjects<PrimitiveEvent>(_queryFactory.GetEventStream(id));
        }

        public IEnumerable<PrimitiveEvent> Get(long fromSequenceNumber, IEnumerable<Type> eventTypes, int limit)
        {
            return _queryMapper.MapObjects<PrimitiveEvent>(_queryFactory.GetProjectionEvents(fromSequenceNumber, eventTypes, limit));
        }

        public void Save(PrimitiveEvent primitiveEvent)
        {
            _databaseGateway.ExecuteUsing(_queryFactory.SaveEvent(primitiveEvent));

            if (primitiveEvent.IsSnapshot)
            {
                _databaseGateway.ExecuteUsing(_queryFactory.SaveSnapshot(primitiveEvent));
            }
        }
    }
}