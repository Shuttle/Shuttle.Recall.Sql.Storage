using Shuttle.Core.Container;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
	public static class ComponentRegistryExtensions
	{
		public static void RegisterEventStoreStorage(this IComponentRegistry registry)
		{
			Guard.AgainstNull(registry, nameof(registry));

			registry.AttemptRegister<IScriptProviderConfiguration, ScriptProviderConfiguration>();
			registry.AttemptRegister<IScriptProvider, ScriptProvider>();

            registry.AttemptRegister<IConcurrencyExceptionSpecification, ConcurrencyExceptionSpecification>();

			registry.AttemptRegister<IDatabaseContextCache, ThreadStaticDatabaseContextCache>();
			registry.AttemptRegister<IDatabaseContextFactory, DatabaseContextFactory>();
			registry.AttemptRegister<IDbConnectionFactory, DbConnectionFactory>();
			registry.AttemptRegister<IDbCommandFactory, DbCommandFactory>();
			registry.AttemptRegister<IDatabaseGateway, DatabaseGateway>();
			registry.AttemptRegister<IQueryMapper, QueryMapper>();
			registry.AttemptRegister<IPrimitiveEventRepository, PrimitiveEventRepository>();
			registry.AttemptRegister<IPrimitiveEventQueryFactory, PrimitiveEventQueryFactory>();
			registry.AttemptRegister<IKeyStoreQueryFactory, KeyStoreQueryFactory>();
			registry.AttemptRegister<IKeyStore, KeyStore>();
			registry.AttemptRegister<IEventTypeStore, EventTypeStore>();
			registry.AttemptRegister<IEventTypeStoreQueryFactory, EventTypeStoreQueryFactory>();
		}
	}
}