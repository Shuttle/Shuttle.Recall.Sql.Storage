using System.Threading.Tasks;
using System.Threading;
using Shuttle.Core.Contract;
using System;

namespace Shuttle.Recall.Sql.Storage
{
    public static class KeyStoreExtensions
    {
        public static async ValueTask<bool> ContainsAsync(this IKeyStore keyStore, string key, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(keyStore, nameof(keyStore));

            return await keyStore.ContainsAsync(key, null, cancellationToken).ConfigureAwait(false);
        }

        public static async ValueTask<bool> ContainsAsync(this IKeyStore keyStore, Guid id, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(keyStore, nameof(keyStore));
            
            return await keyStore.ContainsAsync(id, null, cancellationToken).ConfigureAwait(false);
        }

        public static async ValueTask<Guid?> FindAsync(this IKeyStore keyStore, string key, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(keyStore, nameof(keyStore));
            
            return await keyStore.FindAsync(key, null, cancellationToken).ConfigureAwait(false);
        }

        public static async Task RemoveAsync(this IKeyStore keyStore, string key, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(keyStore, nameof(keyStore));
            
            await keyStore.RemoveAsync(key, null, cancellationToken).ConfigureAwait(false);
        }

        public static async Task RemoveAsync(this IKeyStore keyStore, Guid id, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(keyStore, nameof(keyStore));
            
            await keyStore.RemoveAsync(id, null, cancellationToken).ConfigureAwait(false);
        }

        public static async Task AddAsync(this IKeyStore keyStore, Guid id, string key, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(keyStore, nameof(keyStore));

            await keyStore.AddAsync(id, key, null, cancellationToken).ConfigureAwait(false);
        }

        public static async Task RekeyAsync(this IKeyStore keyStore, string key, string rekey, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(keyStore, nameof(keyStore));

            await keyStore.RekeyAsync(key, rekey, null, cancellationToken).ConfigureAwait(false);
        }
    }
}