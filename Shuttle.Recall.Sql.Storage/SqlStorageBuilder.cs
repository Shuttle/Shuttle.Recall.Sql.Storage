using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Recall.Sql.Storage
{
    public class SqlStorageBuilder
    {
        private SqlStorageOptions _sqlStorageOptions = new SqlStorageOptions();

        public SqlStorageOptions Options
        {
            get => _sqlStorageOptions;
            set => _sqlStorageOptions = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IServiceCollection Services { get; }

        public SqlStorageBuilder(IServiceCollection services)
        {
            Guard.AgainstNull(services, nameof(services));

            Services = services;
        }
    }
}