using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage;

public class ScriptProvider : IScriptProvider
{
    private readonly Core.Data.IScriptProvider _scriptProvider;
    private readonly SqlStorageOptions _sqlStorageOptions;

    public ScriptProvider(IOptionsMonitor<ConnectionStringOptions> connectionStringOptions, IOptions<ScriptProviderOptions> options, IOptions<SqlStorageOptions> sqlStorageOptions)
    {
        Guard.AgainstNull(Guard.AgainstNull(options).Value);
        _sqlStorageOptions = Guard.AgainstNull(Guard.AgainstNull(sqlStorageOptions).Value);

        _scriptProvider = new Core.Data.ScriptProvider(connectionStringOptions, Options.Create(new ScriptProviderOptions
        {
            ResourceNameFormat = string.IsNullOrEmpty(options.Value.ResourceNameFormat)
                ? "Shuttle.Recall.Sql.Storage..scripts.{ProviderName}.{ScriptName}.sql"
                : options.Value.ResourceNameFormat,
            ResourceAssembly = options.Value.ResourceAssembly ?? typeof(KeyStore).Assembly,
            FileNameFormat = options.Value.FileNameFormat,
            ScriptFolder = options.Value.ScriptFolder
        }));
    }

    public string Get(string connectionStringName, string scriptName)
    {
        return _scriptProvider.Get(connectionStringName, scriptName).Replace("{schema}", _sqlStorageOptions.Schema);
    }
}