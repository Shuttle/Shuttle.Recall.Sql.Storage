namespace Shuttle.Recall.Sql.Storage
{
    public class SqlStorageOptions
    {
        public const string SectionName = "Shuttle:EventStore:Sql:Storage";

        public string ConnectionStringName { get; set; }
    }
}