﻿using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Recall.Sql.Storage
{
	public class Bootstrap : IComponentRegistryBootstrap
	{
		public void Register(IComponentRegistry registry)
		{
			Guard.AgainstNull(registry, "registry");

			registry.AttemptRegister<IScriptProviderConfiguration, ScriptProviderConfiguration>();
			registry.AttemptRegister<IScriptProvider, ScriptProvider>();

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
		}
	}
}