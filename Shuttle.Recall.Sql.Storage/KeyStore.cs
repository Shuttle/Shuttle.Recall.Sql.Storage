using System;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
    public class KeyStore : IKeyStore
    {
        private readonly IDatabaseGateway _databaseGateway;
        private readonly IKeyStoreQueryFactory _queryFactory;

        public KeyStore(IDatabaseGateway databaseGateway, IKeyStoreQueryFactory queryFactory)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));
            Guard.AgainstNull(queryFactory, nameof(queryFactory));

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
            try
            {
                _databaseGateway.ExecuteUsing(_queryFactory.Add(id, key));
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("violation of primary key constraint"))
                {
                    throw new DuplicateKeyException(id, key);
                }

                throw;
            }
        }
    }
}