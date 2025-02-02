using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage;

public class EventTypeQueryFactory : IEventTypeQueryFactory
{
    private readonly SqlStorageOptions _sqlStorageOptions;

    public EventTypeQueryFactory(IOptions<SqlStorageOptions> sqlStorageOptions)
    {
        _sqlStorageOptions = Guard.AgainstNull(Guard.AgainstNull(sqlStorageOptions).Value);
    }

    public IQuery GetId(string typeName)
    {
        return new Query($@"
MERGE
	[{_sqlStorageOptions.Schema}].EventType t
USING
	(
		SELECT @TypeName
	) s (TypeName)
ON
	s.TypeName = t.TypeName
WHEN NOT MATCHED BY TARGET THEN
	INSERT
	(
		Id,
		TypeName
	)
	VALUES
	(
		newid(),
		@TypeName
	);

SELECT
	Id
FROM
	[{_sqlStorageOptions.Schema}].EventType
WHERE
	TypeName = @TypeName
")
            .AddParameter(Columns.TypeName, typeName);
    }

    public IQuery Search(EventType.Specification specification)
    {
        Guard.AgainstNull(specification);

        return new Query($@"
SELECT {(specification.MaximumRows > 0 ? $"TOP {specification.MaximumRows}" : string.Empty)}
    Id,
    TypeName
FROM
    [{_sqlStorageOptions.Schema}].EventType
WHERE
(
    @TypeNameMatch IS NULL
    OR 
    TypeName LIKE '%' + @TypeNameMatch + '%'
)
")
            .AddParameter(Columns.TypeNameMatch, specification.TypeNameMatch);
    }
}