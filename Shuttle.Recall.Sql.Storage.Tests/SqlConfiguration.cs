using System;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shuttle.Core.Data;
using Shuttle.Core.Data.Logging;

namespace Shuttle.Recall.Sql.Storage.Tests;

[SetUpFixture]
public class SqlConfiguration
{
    public static IServiceProvider BuildServiceProvider(IServiceCollection serviceCollection = null)
    {
        var services = serviceCollection ?? new ServiceCollection();

        services
            .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build())
            .AddDataAccess(builder =>
            {
                builder.AddConnectionString("Shuttle", "Microsoft.Data.SqlClient", "server=.;database=Shuttle;user id=sa;password=Pass!000;TrustServerCertificate=true");
                builder.Options.DatabaseContextFactory.DefaultConnectionStringName = "Shuttle";
            })
            .AddDataAccessLogging(builder =>
            {
                builder.Options.DatabaseContext = false;
                builder.Options.DbCommandFactory = true;
            })
            .AddSqlEventStorage(builder =>
            {
                builder.Options.ConnectionStringName = "Shuttle";
            })
            .AddEventStore()
;

        return services.BuildServiceProvider();
    }

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);
    }
}