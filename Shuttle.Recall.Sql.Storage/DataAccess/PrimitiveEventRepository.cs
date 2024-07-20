using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
    public class PrimitiveEventRepository : IPrimitiveEventRepository
    {
        private readonly IDatabaseGateway _databaseGateway;
        private readonly IQueryMapper _queryMapper;
        private readonly IPrimitiveEventQueryFactory _queryFactory;

        public PrimitiveEventRepository(IDatabaseGateway databaseGateway, IQueryMapper queryMapper, IPrimitiveEventQueryFactory queryFactory)
        {
            _databaseGateway = Guard.AgainstNull(databaseGateway, nameof(databaseGateway));
            _queryMapper = Guard.AgainstNull(queryMapper, nameof(queryMapper));
            _queryFactory = Guard.AgainstNull(queryFactory, nameof(queryFactory));
        }

        public async Task RemoveAsync(Guid id)
        {
            await _databaseGateway.ExecuteAsync(_queryFactory.RemoveSnapshot(id)).ConfigureAwait(false);
            await _databaseGateway.ExecuteAsync(_queryFactory.RemoveEventStream(id)).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PrimitiveEvent>> GetAsync(Guid id)
        {
            return await _queryMapper.MapObjectsAsync<PrimitiveEvent>(_queryFactory.GetEventStream(id)).ConfigureAwait(false);
        }

        public async ValueTask<long> SaveAsync(PrimitiveEvent primitiveEvent)
        {
            var result = await _databaseGateway.GetScalarAsync<long>(_queryFactory.SaveEvent(primitiveEvent)).ConfigureAwait(false);

            if (primitiveEvent.IsSnapshot)
            {
                await _databaseGateway.ExecuteAsync(_queryFactory.SaveSnapshot(primitiveEvent)).ConfigureAwait(false);
            }

            return result;
        }

        public async ValueTask<long> GetSequenceNumberAsync(Guid id)
        {
            return await _databaseGateway.GetScalarAsync<long>(_queryFactory.GetSequenceNumber(id));
        }
    }
}