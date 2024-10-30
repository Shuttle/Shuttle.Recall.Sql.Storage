using System;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage;

public interface IKeyStoreQueryFactory
{
    IQuery Add(Guid id, string key);
    IQuery Contains(string key);
    IQuery Contains(Guid id);
    IQuery Get(string key);
    IQuery Rekey(string key, string rekey);
    IQuery Remove(string key);
    IQuery Remove(Guid id);
}