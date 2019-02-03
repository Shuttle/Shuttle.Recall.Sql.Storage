﻿using Moq;
using Shuttle.Core.Container;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage.Tests
{
    public class Bootstrap : IComponentRegistryBootstrap
    {
        public void Register(IComponentRegistry registry)
        {
            Guard.AgainstNull(registry, nameof(registry));

#if (NETCOREAPP2_1 || NETSTANDARD2_0)
            var connectionConfigurationProvider = new Mock<IConnectionConfigurationProvider>();

            connectionConfigurationProvider.Setup(m => m.Get(It.IsAny<string>())).Returns((string name) =>
                new ConnectionConfiguration(
                    name,
                    "System.Data.SqlClient",
                    "Data Source=.\\sqlexpress;Initial Catalog=shuttle;Integrated Security=SSPI;"));

            registry.RegisterInstance(connectionConfigurationProvider.Object);
#else
            registry.Register<IConnectionConfigurationProvider, ConnectionConfigurationProvider>();
#endif
        }
    }
}