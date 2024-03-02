using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
	public class Columns
	{
		public static readonly Column<Guid> Id = new Column<Guid>("Id", DbType.Guid);
		public static readonly Column<Guid> EventTypeId = new Column<Guid>("EventTypeId", DbType.Guid);
		public static readonly Column<Guid> EventId = new Column<Guid>("EventId", DbType.Guid);
        public static readonly Column<string> TypeName = new Column<string>("TypeName", DbType.AnsiString);
        public static readonly Column<byte[]> EventEnvelope = new Column<byte[]>("EventEnvelope", DbType.Binary);
		public static readonly Column<long> FromSequenceNumber = new Column<long>("FromSequenceNumber", DbType.Int64);
		public static readonly Column<DateTime> DateRegistered = new Column<DateTime>("DateRegistered", DbType.DateTime);
		public static readonly Column<bool> IsSnapshot = new Column<bool>("IsSnapshot", DbType.Boolean);
        public static readonly Column<string> Key = new Column<string>("Key", DbType.AnsiString);
        public static readonly Column<string> Rekey = new Column<string>("Rekey", DbType.AnsiString);
        public static readonly Column<int> Version = new Column<int>("Version", DbType.Int32);
	}
}