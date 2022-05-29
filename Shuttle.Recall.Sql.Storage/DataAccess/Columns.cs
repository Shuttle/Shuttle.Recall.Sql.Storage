using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
	public class Columns
	{
		public static readonly MappedColumn<Guid> Id = new MappedColumn<Guid>("Id", DbType.Guid);
		public static readonly MappedColumn<Guid> EventTypeId = new MappedColumn<Guid>("EventTypeId", DbType.Guid);
		public static readonly MappedColumn<Guid> EventId = new MappedColumn<Guid>("EventId", DbType.Guid);
        public static readonly MappedColumn<string> TypeName = new MappedColumn<string>("TypeName", DbType.AnsiString);
        public static readonly MappedColumn<byte[]> EventEnvelope = new MappedColumn<byte[]>("EventEnvelope", DbType.Binary);
		public static readonly MappedColumn<long> FromSequenceNumber = new MappedColumn<long>("FromSequenceNumber", DbType.Int64);
		public static readonly MappedColumn<DateTime> DateRegistered = new MappedColumn<DateTime>("DateRegistered", DbType.DateTime);
		public static readonly MappedColumn<bool> IsSnapshot = new MappedColumn<bool>("IsSnapshot", DbType.Boolean);
        public static readonly MappedColumn<string> Key = new MappedColumn<string>("Key", DbType.AnsiString);
        public static readonly MappedColumn<string> Rekey = new MappedColumn<string>("Rekey", DbType.AnsiString);
        public static readonly MappedColumn<int> Version = new MappedColumn<int>("Version", DbType.Int32);
	}
}