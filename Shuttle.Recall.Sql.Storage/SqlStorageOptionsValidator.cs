using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Recall.Sql.Storage;

public class SqlStorageOptionsValidator : IValidateOptions<SqlStorageOptions>
{
    public ValidateOptionsResult Validate(string? name, SqlStorageOptions options)
    {
        Guard.AgainstNull(options);

        if (string.IsNullOrWhiteSpace(options.ConnectionStringName))
        {
            return ValidateOptionsResult.Fail(Resources.ConnectionStringException);
        }

        return ValidateOptionsResult.Success;
    }
}