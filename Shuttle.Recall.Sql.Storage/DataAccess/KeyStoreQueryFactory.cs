using System;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
    public class KeyStoreQueryFactory : IKeyStoreQueryFactory
    {
        private readonly IScriptProvider _scriptProvider;

        public KeyStoreQueryFactory(IScriptProvider scriptProvider)
        {
            Guard.AgainstNull(scriptProvider, nameof(scriptProvider));

            _scriptProvider = scriptProvider;
        }

        public IQuery Get(string key)
        {
            return RawQuery.Create(_scriptProvider.Get("KeyStore.Get"))
                    .AddParameterValue(Columns.Key, key);
        }

        public IQuery Add(Guid id, string key)
        {
            return
                RawQuery.Create(_scriptProvider.Get("KeyStore.Add"))
                    .AddParameterValue(Columns.Key, key)
                    .AddParameterValue(Columns.Id, id);
        }

        public IQuery Remove(string key)
        {
            return RawQuery.Create(_scriptProvider.Get("KeyStore.RemoveKey"))
                    .AddParameterValue(Columns.Key, key);
        }

        public IQuery Remove(Guid id)
        {
            return RawQuery.Create(_scriptProvider.Get("KeyStore.RemoveId"))
                    .AddParameterValue(Columns.Id, id);
        }

        public IQuery Contains(string key)
        {
            return RawQuery.Create(_scriptProvider.Get("KeyStore.Contains"))
                    .AddParameterValue(Columns.Key, key);
        }
    }
}