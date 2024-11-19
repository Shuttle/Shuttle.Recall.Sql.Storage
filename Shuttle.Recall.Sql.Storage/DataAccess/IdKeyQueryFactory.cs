using System;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage;

public class IdKeyQueryFactory : IIdKeyQueryFactory
{
    private readonly SqlStorageOptions _sqlStorageOptions;

    public IdKeyQueryFactory(IOptions<SqlStorageOptions> sqlStorageOptions)
    {
        _sqlStorageOptions = Guard.AgainstNull(Guard.AgainstNull(sqlStorageOptions).Value);
    }

    public IQuery Get(string key)
    {
        return new Query($"select Id from [{_sqlStorageOptions.Schema}].IdKey where [Key] = @Key")
            .AddParameter(Columns.UniqueKey, key);
    }

    public IQuery Add(Guid id, string key)
    {
        return
            new Query($@"
insert into [{_sqlStorageOptions.Schema}].IdKey
(
    [UniqueKey],
	[Id]
)
values
(
	@UniqueKey,
	@Id
)
")
                .AddParameter(Columns.UniqueKey, key)
                .AddParameter(Columns.Id, id);
    }

    public IQuery Remove(string key)
    {
        return new Query($"delete from [{_sqlStorageOptions.Schema}].IdKey where [UniqueKey] = @UniqueKey")
            .AddParameter(Columns.UniqueKey, key);
    }

    public IQuery Remove(Guid id)
    {
        return new Query($"delete from [{_sqlStorageOptions.Schema}].IdKey where [Id] = @Id")
            .AddParameter(Columns.Id, id);
    }

    public IQuery Contains(string key)
    {
        return new Query($"if exists (select null from [{_sqlStorageOptions.Schema}].IdKey where [UniqueKey] = @UniqueKey) select 1 else select 0")
            .AddParameter(Columns.UniqueKey, key);
    }

    public IQuery Rekey(string key, string rekey)
    {
        return
            new Query(@$"
update
	[{_sqlStorageOptions.Schema}].IdKey
set
	[UniqueKey] = @Rekey
where
	[UniqueKey] = @Key
")
                .AddParameter(Columns.UniqueKey, key)
                .AddParameter(Columns.Rekey, rekey);
    }

    public IQuery Contains(Guid id)
    {
        return new Query($"if exists (select null from [{_sqlStorageOptions.Schema}].IdKey where [Id] = @Id) select 1 else select 0")
            .AddParameter(Columns.Id, id);
    }
}