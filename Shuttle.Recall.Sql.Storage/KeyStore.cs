using System;
using System.Threading;
using System.Threading.Tasks;
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
            return _databaseGateway.GetScalar<int>(_queryFactory.Contains(key)) == 1;
        }

        public bool Contains(Guid id)
        {
            return _databaseGateway.GetScalar<int>(_queryFactory.Contains(id)) == 1;
        }

        public Guid? Find(string key)
        {
            return _databaseGateway.GetScalar<Guid?>(_queryFactory.Get(key));
        }

        public void Remove(string key)
        {
            _databaseGateway.Execute(_queryFactory.Remove(key));
        }

        public void Remove(Guid id)
        {
            _databaseGateway.Execute(_queryFactory.Remove(id));
        }

        public void Add(Guid id, string key)
        {
            AddAsync(id, key, CancellationToken.None, true).GetAwaiter().GetResult();
        }

        public void Rekey(string key, string rekey)
        {
            RekeyAsync(key, rekey, CancellationToken.None, true).GetAwaiter().GetResult();
        }

        public async ValueTask<bool> ContainsAsync(string key, CancellationToken cancellationToken = default)
        {
            return await _databaseGateway.GetScalarAsync<int>(_queryFactory.Contains(key), cancellationToken).ConfigureAwait(false) == 1;
        }

        public async ValueTask<bool> ContainsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _databaseGateway.GetScalarAsync<int>(_queryFactory.Contains(id), cancellationToken).ConfigureAwait(false) == 1;
        }

        public async ValueTask<Guid?> FindAsync(string key, CancellationToken cancellationToken = default)
        {
            return await _databaseGateway.GetScalarAsync<Guid?>(_queryFactory.Get(key), cancellationToken).ConfigureAwait(false);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _databaseGateway.ExecuteAsync(_queryFactory.Remove(key), cancellationToken).ConfigureAwait(false);
        }

        public async Task RemoveAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await _databaseGateway.ExecuteAsync(_queryFactory.Remove(id), cancellationToken).ConfigureAwait(false);
        }

        public async Task AddAsync(Guid id, string key, CancellationToken cancellationToken = default)
        {
            await AddAsync(id, key, cancellationToken, false).ConfigureAwait(false);
        }

        private async Task AddAsync(Guid id, string key, CancellationToken cancellationToken, bool sync)
        {
            try
            {
                if (sync)
                {
                    _databaseGateway.Execute(_queryFactory.Add(id, key));
                }
                else
                {
                    await _databaseGateway.ExecuteAsync(_queryFactory.Add(id, key), cancellationToken).ConfigureAwait(false);
                }
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

        public async Task RekeyAsync(string key, string rekey, CancellationToken cancellationToken = default)
        {
            await RekeyAsync(key, rekey, cancellationToken, false).ConfigureAwait(false);
        }

        private async Task RekeyAsync(string key, string rekey, CancellationToken cancellationToken, bool sync)
        {
            try
            {
                if (sync)
                {
                    _databaseGateway.Execute(_queryFactory.Rekey(key, rekey));
                }
                else
                {
                    await _databaseGateway.ExecuteAsync(_queryFactory.Rekey(key, rekey), cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("violation of primary key constraint"))
                {
                    throw new DuplicateKeyException(Find(key) ?? Guid.Empty, key);
                }

                throw;
            }
        }
    }
}