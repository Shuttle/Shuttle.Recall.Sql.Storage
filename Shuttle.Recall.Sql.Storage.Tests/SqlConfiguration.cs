using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Core.Data.Logging;

namespace Shuttle.Recall.Sql.Storage.Tests;

[SetUpFixture]
public class SqlConfiguration
{
    public static IServiceCollection GetServiceCollection(IServiceCollection? serviceCollection = null)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        return (serviceCollection ?? new ServiceCollection())
            .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build())
            .AddDataAccess(builder =>
            {
                builder.AddConnectionString("RecallFixtureStorage", "Microsoft.Data.SqlClient", Guard.AgainstNullOrEmptyString(configuration.GetConnectionString("RecallFixtureStorage")));
                builder.Options.DatabaseContextFactory.DefaultConnectionStringName = "RecallFixtureStorage";
            })
            .AddDataAccessLogging(builder =>
            {
                builder.Options.DatabaseContext = false;
                builder.Options.DbCommandFactory = true;
            })
            .AddSqlEventStorage(builder =>
            {
                configuration.GetSection(SqlStorageOptions.SectionName).Bind(builder.Options);
            });
    }

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);
    }
}