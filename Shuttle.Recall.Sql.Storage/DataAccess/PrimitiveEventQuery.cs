using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
    public class PrimitiveEventQuery : IPrimitiveEventQuery
    {
        private readonly IDatabaseGateway _databaseGateway;
        private readonly IQueryMapper _queryMapper;
        private readonly IPrimitiveEventQueryFactory _queryFactory;

        public PrimitiveEventQuery(IDatabaseGateway databaseGateway, IQueryMapper queryMapper, IPrimitiveEventQueryFactory queryFactory)
        {
            _databaseGateway = Guard.AgainstNull(databaseGateway, nameof(databaseGateway));
            _queryMapper = Guard.AgainstNull(queryMapper, nameof(queryMapper));
            _queryFactory = Guard.AgainstNull(queryFactory, nameof(queryFactory));
        }

        public IEnumerable<PrimitiveEvent> Search(PrimitiveEvent.Specification specification)
        {
            return _queryMapper.MapObjects<PrimitiveEvent>(_queryFactory.Search(specification));
        }

        public async Task<IEnumerable<PrimitiveEvent>> SearchAsync(PrimitiveEvent.Specification specification)
        {
            return await _queryMapper.MapObjectsAsync<PrimitiveEvent>(_queryFactory.Search(specification)).ConfigureAwait(false);
        }
    }
}