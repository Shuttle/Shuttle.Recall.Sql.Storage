using System;
using System.Collections.Generic;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
    public class PrimitiveEventRepository : IPrimitiveEventRepository
    {
        private readonly IDatabaseGateway _databaseGateway;
        private readonly IQueryMapper _queryMapper;
        private readonly IPrimitiveEventQueryFactory _queryFactory;

        public PrimitiveEventRepository(IDatabaseGateway databaseGateway, IQueryMapper queryMapper,
            IPrimitiveEventQueryFactory queryFactory)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));
            Guard.AgainstNull(queryMapper, nameof(queryMapper));
            Guard.AgainstNull(queryFactory, nameof(queryFactory));

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

        public IEnumerable<PrimitiveEvent> Get(long fromSequenceNumber, long toSequenceNumber, IEnumerable<Type> eventTypes)
        {
            return _queryMapper.MapObjects<PrimitiveEvent>(_queryFactory.Get(fromSequenceNumber, toSequenceNumber, eventTypes));
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