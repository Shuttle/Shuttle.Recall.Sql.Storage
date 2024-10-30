using System;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage;

public interface IEventTypeStore
{
    Task<Guid> GetIdAsync(IDatabaseContext databaseContext, string typeName, CancellationToken cancellationToken = default);
}