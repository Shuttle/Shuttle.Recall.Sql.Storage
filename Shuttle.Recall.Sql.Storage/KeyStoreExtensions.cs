using System;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;

namespace Shuttle.Recall.Sql.Storage;

public static class KeyStoreExtensions
{
    public static async Task AddAsync(this IKeyStore keyStore, Guid id, string key, CancellationToken cancellationToken = default)
    {
        await Guard.AgainstNull(keyStore).AddAsync(id, key, cancellationToken).ConfigureAwait(false);
    }

    public static async ValueTask<bool> ContainsAsync(this IKeyStore keyStore, string key, CancellationToken cancellationToken = default)
    {
        return await Guard.AgainstNull(keyStore).ContainsAsync(key, cancellationToken).ConfigureAwait(false);
    }

    public static async ValueTask<bool> ContainsAsync(this IKeyStore keyStore, Guid id, CancellationToken cancellationToken = default)
    {
        return await Guard.AgainstNull(keyStore).ContainsAsync(id, cancellationToken).ConfigureAwait(false);
    }

    public static async ValueTask<Guid?> FindAsync(this IKeyStore keyStore, string key, CancellationToken cancellationToken = default)
    {
        return await Guard.AgainstNull(keyStore).FindAsync(key, cancellationToken).ConfigureAwait(false);
    }

    public static async Task RekeyAsync(this IKeyStore keyStore, string key, string rekey, CancellationToken cancellationToken = default)
    {
        await Guard.AgainstNull(keyStore).RekeyAsync(key, rekey, cancellationToken).ConfigureAwait(false);
    }

    public static async Task RemoveAsync(this IKeyStore keyStore, string key, CancellationToken cancellationToken = default)
    {
        await Guard.AgainstNull(keyStore).RemoveAsync(key, cancellationToken).ConfigureAwait(false);
    }

    public static async Task RemoveAsync(this IKeyStore keyStore, Guid id, CancellationToken cancellationToken = default)
    {
        await Guard.AgainstNull(keyStore).RemoveAsync(id, cancellationToken).ConfigureAwait(false);
    }
}