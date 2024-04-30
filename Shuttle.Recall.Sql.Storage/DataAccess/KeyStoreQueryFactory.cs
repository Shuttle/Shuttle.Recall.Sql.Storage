using System;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
    public class KeyStoreQueryFactory : IKeyStoreQueryFactory
    {
        private readonly SqlStorageOptions _sqlStorageOptions;
        private readonly IScriptProvider _scriptProvider;

        public KeyStoreQueryFactory(IOptions<SqlStorageOptions> sqlStorageOptions, IScriptProvider scriptProvider)
        {
            Guard.AgainstNull(sqlStorageOptions, nameof(sqlStorageOptions));

            _sqlStorageOptions = Guard.AgainstNull(sqlStorageOptions.Value, nameof(sqlStorageOptions.Value));
            _scriptProvider = Guard.AgainstNull(scriptProvider, nameof(scriptProvider));
        }

        public IQuery Get(string key)
        {
            return new Query(_scriptProvider.Get(_sqlStorageOptions.ConnectionStringName, "KeyStore.Get"))                    .AddParameter(Columns.Key, key);
        }

        public IQuery Add(Guid id, string key)
        {
            return
                new Query(_scriptProvider.Get(_sqlStorageOptions.ConnectionStringName, "KeyStore.Add"))
                    .AddParameter(Columns.Key, key)
                    .AddParameter(Columns.Id, id);
        }

        public IQuery Remove(string key)
        {
            return new Query(_scriptProvider.Get(_sqlStorageOptions.ConnectionStringName, "KeyStore.RemoveKey"))
                    .AddParameter(Columns.Key, key);
        }

        public IQuery Remove(Guid id)
        {
            return new Query(_scriptProvider.Get(_sqlStorageOptions.ConnectionStringName, "KeyStore.RemoveId"))
                    .AddParameter(Columns.Id, id);
        }

        public IQuery Contains(string key)
        {
            return new Query(_scriptProvider.Get(_sqlStorageOptions.ConnectionStringName, "KeyStore.ContainsKey"))
                    .AddParameter(Columns.Key, key);
        }

        public IQuery Rekey(string key, string rekey)
        {
            return
                new Query(_scriptProvider.Get(_sqlStorageOptions.ConnectionStringName, "KeyStore.Rekey"))
                    .AddParameter(Columns.Key, key)
                    .AddParameter(Columns.Rekey, rekey);
        }

        public IQuery Contains(Guid id)
        {
            return new Query(_scriptProvider.Get(_sqlStorageOptions.ConnectionStringName, "KeyStore.ContainsId"))
                .AddParameter(Columns.Id, id);
        }
    }
}