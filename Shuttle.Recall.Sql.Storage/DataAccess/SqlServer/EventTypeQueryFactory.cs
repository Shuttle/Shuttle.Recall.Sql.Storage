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
merge
	[{_sqlStorageOptions.Schema}].EventType t
using
	(
		select @TypeName
	) s (TypeName)
on
	s.TypeName = t.TypeName
when not matched by target then
	insert
	(
		Id,
		TypeName
	)
	values
	(
		newid(),
		@TypeName
	);

select
	Id
from
	[{_sqlStorageOptions.Schema}].EventType
where
	TypeName = @TypeName
")
            .AddParameter(Columns.TypeName, typeName);
    }
}