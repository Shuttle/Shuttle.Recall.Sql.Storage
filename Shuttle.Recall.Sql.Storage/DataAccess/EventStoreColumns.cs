using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
	public class EventStoreColumns
	{
		public static readonly MappedColumn<Guid> Id = new MappedColumn<Guid>("Id", DbType.Guid);
		public static readonly MappedColumn<int> Version = new MappedColumn<int>("Version", DbType.Int32);
		public static readonly MappedColumn<string> EventType = new MappedColumn<string>("EventType", DbType.AnsiString, 160);
		public static readonly MappedColumn<byte[]> EventEnvelope = new MappedColumn<byte[]>("EventEnvelope", DbType.Binary);
		public static readonly MappedColumn<long> SequenceNumber = new MappedColumn<long>("SequenceNumber", DbType.Int64);
		public static readonly MappedColumn<DateTime> DateRegistered = new MappedColumn<DateTime>("DateRegistered", DbType.DateTime);
		public static readonly MappedColumn<bool> IsSnapshot = new MappedColumn<bool>("IsSnapshot", DbType.Boolean);
	}
}