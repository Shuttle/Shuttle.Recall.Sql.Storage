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
    public static IServiceCollection GetServiceCollection(bool manageEventStoreConnections, IServiceCollection serviceCollection = null)
    {
        return (serviceCollection ?? new ServiceCollection())
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
                builder.Options.ManageEventStoreConnections = manageEventStoreConnections;
            })
            .AddEventStore();
    }

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);
    }
}