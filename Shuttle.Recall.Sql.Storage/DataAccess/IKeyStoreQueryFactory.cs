using System;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
	public interface IKeyStoreQueryFactory
	{
		IQuery Get(string key);
		IQuery Add(Guid id, string key);
		IQuery Remove(string key);
	    IQuery Remove(Guid id);
	    IQuery Contains(string key);
	    IQuery Rekey(string key, string rekey);
	    IQuery Contains(Guid id);
	}
}