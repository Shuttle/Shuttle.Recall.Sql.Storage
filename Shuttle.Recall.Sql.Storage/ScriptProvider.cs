using Shuttle.Core.Contract;

namespace Shuttle.Recall.Sql.Storage
{
	public class ScriptProvider : IScriptProvider
	{
		private readonly Core.Data.IScriptProvider _scriptProvider;

		public ScriptProvider(IScriptProviderConfiguration configuration)
		{
			Guard.AgainstNull(configuration, nameof(configuration));

			_scriptProvider = new Core.Data.ScriptProvider(new ScriptProviderConfiguration
			{
				ResourceNameFormat = string.IsNullOrEmpty(configuration.ResourceNameFormat)
					? "Shuttle.Recall.Sql.Storage..scripts.{ProviderName}.{ScriptName}.sql"
					: configuration.ResourceNameFormat,
				ResourceAssembly = configuration.ResourceAssembly ?? typeof(PrimitiveEventRepository).Assembly,
				FileNameFormat = configuration.FileNameFormat,
				ScriptFolder = configuration.ScriptFolder
			});
		}

		public string Get(string scriptName)
		{
			return _scriptProvider.Get(scriptName);
		}

		public string Get(string scriptName, params object[] parameters)
		{
			return _scriptProvider.Get(scriptName, parameters);
		}
	}
}