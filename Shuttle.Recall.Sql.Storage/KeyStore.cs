using System;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Recall.Sql.Storage
{
    public class KeyStore : IKeyStore
    {
        private readonly IDatabaseGateway _databaseGateway;
        private readonly IKeyStoreQueryFactory _queryFactory;

        public KeyStore(IDatabaseGateway databaseGateway, IKeyStoreQueryFactory queryFactory)
        {
            Guard.AgainstNull(databaseGateway, "databaseGateway");
            Guard.AgainstNull(queryFactory, "queryFactory");

            _databaseGateway = databaseGateway;
            _queryFactory = queryFactory;
        }

        public bool Contains(string key)
        {
            return _databaseGateway.GetScalarUsing<int>(_queryFactory.Contains(key)) == 1;
        }

        public Guid? Get(string key)
        {
            return _databaseGateway.GetScalarUsing<Guid?>(_queryFactory.Get(key));
        }

        public void Remove(string key)
        {
            _databaseGateway.ExecuteUsing(_queryFactory.Remove(key));
        }

        public void Remove(Guid id)
        {
            _databaseGateway.ExecuteUsing(_queryFactory.Remove(id));
        }

        public void Add(Guid id, string key)
        {
            _databaseGateway.ExecuteUsing(_queryFactory.Add(id, key));
        }
    }
}