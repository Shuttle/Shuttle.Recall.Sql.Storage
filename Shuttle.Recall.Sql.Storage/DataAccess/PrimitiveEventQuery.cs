using System;
using System.Collections.Generic;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
    public class PrimitiveEventQuery : IPrimitiveEventQuery
    {
        private readonly IDatabaseGateway _databaseGateway;
        private readonly IQueryMapper _queryMapper;
        private readonly IPrimitiveEventQueryFactory _queryFactory;

        public PrimitiveEventQuery(IDatabaseGateway databaseGateway, IQueryMapper queryMapper,
            IPrimitiveEventQueryFactory queryFactory)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));
            Guard.AgainstNull(queryMapper, nameof(queryMapper));
            Guard.AgainstNull(queryFactory, nameof(queryFactory));

            _databaseGateway = databaseGateway;
            _queryMapper = queryMapper;
            _queryFactory = queryFactory;
        }

        public IEnumerable<PrimitiveEvent> Search(PrimitiveEvent.Specification specification)
        {
            return _queryMapper.MapObjects<PrimitiveEvent>(_queryFactory.Search(specification));
        }
    }
}