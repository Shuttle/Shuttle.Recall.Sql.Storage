using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Recall.Sql.Storage
{
    public interface IKeyStore
    {
        bool Contains(string key, Action<EventStreamBuilder> builder = null);
        bool Contains(Guid id, Action<EventStreamBuilder> builder = null);
        Guid? Find(string key, Action<EventStreamBuilder> builder = null);
        void Remove(string key, Action<EventStreamBuilder> builder = null);
        void Remove(Guid id, Action<EventStreamBuilder> builder = null);
        void Add(Guid id, string key, Action<EventStreamBuilder> builder = null);
        void Rekey(string key, string rekey, Action<EventStreamBuilder> builder = null);
        ValueTask<bool> ContainsAsync(string key, Action<EventStreamBuilder> builder = null, CancellationToken cancellationToken = default);
        ValueTask<bool> ContainsAsync(Guid id, Action<EventStreamBuilder> builder = null, CancellationToken cancellationToken = default);
        ValueTask<Guid?> FindAsync(string key, Action<EventStreamBuilder> builder = null, CancellationToken cancellationToken = default);
        Task RemoveAsync(string key, Action<EventStreamBuilder> builder = null, CancellationToken cancellationToken = default);
        Task RemoveAsync(Guid id, Action<EventStreamBuilder> builder = null, CancellationToken cancellationToken = default);
        Task AddAsync(Guid id, string key, Action<EventStreamBuilder> builder = null, CancellationToken cancellationToken = default);
        Task RekeyAsync(string key, string rekey, Action<EventStreamBuilder> builder = null, CancellationToken cancellationToken = default);
    }
}