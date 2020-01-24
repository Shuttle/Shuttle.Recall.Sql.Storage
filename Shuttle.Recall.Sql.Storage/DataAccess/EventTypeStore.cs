using System;
using System.Collections.Generic;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
    public class EventTypeStore : IEventTypeStore
    {
        private readonly IDatabaseGateway _databaseGateway;
        private readonly IEventTypeStoreQueryFactory _queryFactory;
        private readonly Dictionary<string, Guid> _cache = new Dictionary<string, Guid>();
        private readonly object _lock = new object();

        public EventTypeStore(IDatabaseGateway databaseGateway, IEventTypeStoreQueryFactory queryFactory)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));
            Guard.AgainstNull(queryFactory, nameof(queryFactory));

            _databaseGateway = databaseGateway;
            _queryFactory = queryFactory;
        }

        public Guid GetId(string typeName)
        {
            lock (_lock)
            {
                var key = typeName.ToLower();

                if (!_cache.ContainsKey(key))
                {
                    _cache.Add(key, _databaseGateway.GetScalarUsing<Guid>(_queryFactory.GetId(typeName)));
                }

                return _cache[key];
            }
        }
    }
}