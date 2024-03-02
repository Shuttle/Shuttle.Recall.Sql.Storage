using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Recall.Sql.Storage
{
    public interface IKeyStore
    {
        bool Contains(string key);
        bool Contains(Guid id);
        Guid? Find(string key);
        void Remove(string key);
        void Remove(Guid id);
        void Add(Guid id, string key);
        void Rekey(string key, string rekey);
        ValueTask<bool> ContainsAsync(string key, CancellationToken cancellationToken = default);
        ValueTask<bool> ContainsAsync(Guid id, CancellationToken cancellationToken = default);
        ValueTask<Guid?> FindAsync(string key, CancellationToken cancellationToken = default);
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
        Task RemoveAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(Guid id, string key, CancellationToken cancellationToken = default);
        Task RekeyAsync(string key, string rekey, CancellationToken cancellationToken = default);
    }
}