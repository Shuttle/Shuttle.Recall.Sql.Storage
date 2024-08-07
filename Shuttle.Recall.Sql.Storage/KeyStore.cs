﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
    public class KeyStore : IKeyStore
    {
        private readonly ConnectionFactory _connectionFactory;

        private readonly IDatabaseGateway _databaseGateway;
        private readonly IKeyStoreQueryFactory _queryFactory;

        public KeyStore(IOptions<SqlStorageOptions> sqlStorageOptions, IDatabaseContextService databaseContextService, IDatabaseContextFactory databaseContextFactory, IDatabaseGateway databaseGateway, IKeyStoreQueryFactory queryFactory)
        {
            _databaseGateway = Guard.AgainstNull(databaseGateway, nameof(databaseGateway));
            _queryFactory = Guard.AgainstNull(queryFactory, nameof(queryFactory));

            _connectionFactory = new ConnectionFactory(sqlStorageOptions, Guard.AgainstNull(databaseContextService, nameof(databaseContextService)), databaseContextFactory);
        }

        public bool Contains(string key, Action<EventStreamBuilder> builder = null)
        {
            using var connection = _connectionFactory.GetConnection(builder);
            return _databaseGateway.GetScalar<int>(_queryFactory.Contains(key)) == 1;
        }

        public bool Contains(Guid id, Action<EventStreamBuilder> builder = null)
        {
            using var connection = _connectionFactory.GetConnection(builder);
            return _databaseGateway.GetScalar<int>(_queryFactory.Contains(id)) == 1;
        }

        public Guid? Find(string key, Action<EventStreamBuilder> builder = null)
        {
            using var connection = _connectionFactory.GetConnection(builder);
            return _databaseGateway.GetScalar<Guid?>(_queryFactory.Get(key));
        }

        public void Remove(string key, Action<EventStreamBuilder> builder = null)
        {
            using var connection = _connectionFactory.GetConnection(builder);
            _databaseGateway.Execute(_queryFactory.Remove(key));
        }

        public void Remove(Guid id, Action<EventStreamBuilder> builder = null)
        {
            using var connection = _connectionFactory.GetConnection(builder);
            _databaseGateway.Execute(_queryFactory.Remove(id));
        }

        public void Add(Guid id, string key, Action<EventStreamBuilder> builder = null)
        {
            AddAsync(id, key, builder, CancellationToken.None, true).GetAwaiter().GetResult();
        }

        public void Rekey(string key, string rekey, Action<EventStreamBuilder> builder = null)
        {
            RekeyAsync(key, rekey, builder, CancellationToken.None, true).GetAwaiter().GetResult();
        }

        public async ValueTask<bool> ContainsAsync(string key, Action<EventStreamBuilder> builder = null, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.GetConnection(builder);
            return await _databaseGateway.GetScalarAsync<int>(_queryFactory.Contains(key), cancellationToken).ConfigureAwait(false) == 1;
        }

        public async ValueTask<bool> ContainsAsync(Guid id, Action<EventStreamBuilder> builder = null, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.GetConnection(builder);
            return await _databaseGateway.GetScalarAsync<int>(_queryFactory.Contains(id), cancellationToken).ConfigureAwait(false) == 1;
        }

        public async ValueTask<Guid?> FindAsync(string key, Action<EventStreamBuilder> builder = null, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.GetConnection(builder);
            return await _databaseGateway.GetScalarAsync<Guid?>(_queryFactory.Get(key), cancellationToken).ConfigureAwait(false);
        }

        public async Task RemoveAsync(string key, Action<EventStreamBuilder> builder = null, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.GetConnection(builder);
            await _databaseGateway.ExecuteAsync(_queryFactory.Remove(key), cancellationToken).ConfigureAwait(false);
        }

        public async Task RemoveAsync(Guid id, Action<EventStreamBuilder> builder = null, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.GetConnection(builder);
            await _databaseGateway.ExecuteAsync(_queryFactory.Remove(id), cancellationToken).ConfigureAwait(false);
        }

        public async Task AddAsync(Guid id, string key, Action<EventStreamBuilder> builder = null, CancellationToken cancellationToken = default)
        {
            await AddAsync(id, key, builder, cancellationToken, false).ConfigureAwait(false);
        }

        public async Task RekeyAsync(string key, string rekey, Action<EventStreamBuilder> builder = null, CancellationToken cancellationToken = default)
        {
            await RekeyAsync(key, rekey, builder, cancellationToken, false).ConfigureAwait(false);
        }

        private async Task AddAsync(Guid id, string key, Action<EventStreamBuilder> builder, CancellationToken cancellationToken, bool sync)
        {
            try
            {
                using var connection = _connectionFactory.GetConnection(builder);

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

        private async Task RekeyAsync(string key, string rekey, Action<EventStreamBuilder> builder, CancellationToken cancellationToken, bool sync)
        {
            try
            {
                using var connection = _connectionFactory.GetConnection(builder);

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
                    throw new DuplicateKeyException(await FindAsync(key, builder, cancellationToken) ?? Guid.Empty, key);
                }

                throw;
            }
        }

        private class ConnectionFactory
        {
            private readonly IDatabaseContextService _databaseContextService;
            private readonly IDatabaseContextFactory _databaseContextFactory;

            private readonly SqlStorageOptions _sqlStorageOptions;

            public ConnectionFactory(IOptions<SqlStorageOptions> sqlStorageOptions, IDatabaseContextService databaseContextService, IDatabaseContextFactory databaseContextFactory)
            {
                _sqlStorageOptions = sqlStorageOptions.Value;
                _databaseContextService = databaseContextService;
                _databaseContextFactory = databaseContextFactory;
            }

            public IDisposable GetConnection(Action<EventStreamBuilder> builder = null)
            {
                var eventStreamBuilder = new EventStreamBuilder();

                builder?.Invoke(eventStreamBuilder);

                var databaseContext = _databaseContextService.Find(candidate => candidate.Name.Equals(_sqlStorageOptions.ConnectionStringName));

                if (databaseContext == null)
                {
                    return _databaseContextFactory.Create(_sqlStorageOptions.ConnectionStringName);
                }

                var activeDatabaseContext = _databaseContextService.HasActive ? _databaseContextService.Active : null;

                if (!databaseContext.IsActive)
                {
                    _databaseContextService.Activate(databaseContext);
                }

                return new ActivateDatabaseContext(_databaseContextService, activeDatabaseContext);
            }

            private class ActivateDatabaseContext : IDisposable
            {
                private readonly IDatabaseContextService _databaseContextService;
                private readonly IDatabaseContext _databaseContext;

                public ActivateDatabaseContext(IDatabaseContextService databaseContextService, IDatabaseContext databaseContext)
                {
                    _databaseContextService = databaseContextService;
                    _databaseContext = databaseContext;
                }

                public void Dispose()
                {
                    if (_databaseContext == null || _databaseContext.IsActive)
                    {
                        return;
                    }

                    _databaseContextService.Activate(_databaseContext);
                }
            }

        }
    }
}