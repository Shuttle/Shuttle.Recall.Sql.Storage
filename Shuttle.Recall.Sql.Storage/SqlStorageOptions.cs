using System;

namespace Shuttle.Recall.Sql.Storage;

public class SqlStorageOptions
{
    public const string SectionName = "Shuttle:EventStore:Sql:Storage";

    public string ConnectionStringName { get; set; } = string.Empty;
    public string Schema { get; set; } = "dbo";
    public bool ConfigureDatabase { get; set; } = true;
    public TimeSpan UncommittedTolerance { get; set; } = TimeSpan.FromSeconds(15);
}