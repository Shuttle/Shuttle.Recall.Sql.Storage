using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage;

public class Columns
{
    public static readonly Column<Guid> Id = new("Id", DbType.Guid);
    public static readonly Column<Guid> EventTypeId = new("EventTypeId", DbType.Guid);
    public static readonly Column<Guid> EventId = new("EventId", DbType.Guid);
    public static readonly Column<Guid?> CorrelationId = new("CorrelationId", DbType.Guid);
    public static readonly Column<string> TypeName = new("TypeName", DbType.AnsiString);
    public static readonly Column<byte[]> EventEnvelope = new("EventEnvelope", DbType.Binary);
    public static readonly Column<long> SequenceNumberStart = new("SequenceNumberStart", DbType.Int64);
    public static readonly Column<long> SequenceNumberEnd = new("SequenceNumberEnd", DbType.Int64);
    public static readonly Column<DateTime> DateRegistered = new("DateRegistered", DbType.DateTime);
    public static readonly Column<string> UniqueKey = new("UniqueKey", DbType.AnsiString);
    public static readonly Column<string> Rekey = new("Rekey", DbType.AnsiString);
    public static readonly Column<int> Version = new("Version", DbType.Int32);
}