using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
	public class KeyStoreColumns
	{
		public static readonly MappedColumn<Guid> Id = new MappedColumn<Guid>("Id", DbType.Guid);
		public static readonly MappedColumn<string> Key = new MappedColumn<string>("Key", DbType.AnsiString);
	}
}