using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Common.Common.Db
{
    public sealed class PostgresDatabaseManager
    {
        private readonly ILogger<PostgresDatabaseManager> _logger;
        private readonly string _adminConnectionString;

        public PostgresDatabaseManager(ILogger<PostgresDatabaseManager> logger, IConfiguration configuration)
        {
            _logger = logger;
            _adminConnectionString = configuration.GetConnectionString("PostgresAdmin")
                ?? throw new InvalidOperationException("ConnectionStrings:PostgresAdmin is required");
        }

        public async Task EnsureDatabasesExistAsync(IEnumerable<string> databaseNames)
        {
            await using var admin = new NpgsqlConnection(_adminConnectionString);
            await admin.OpenAsync();

            foreach (var dbName in databaseNames)
            {
                await using var cmd = admin.CreateCommand();
                cmd.CommandText = $"SELECT 1 FROM pg_database WHERE datname = @name";
                cmd.Parameters.AddWithValue("name", dbName);
                var exists = await cmd.ExecuteScalarAsync() is not null;

                if (!exists)
                {
                    _logger.LogInformation("Creating database {Database}", dbName);
                    await using var create = admin.CreateCommand();
                    create.CommandText = $"CREATE DATABASE \"{dbName}\"";
                    await create.ExecuteNonQueryAsync();
                }
            }
        }

        public string BuildDatabaseConnectionString(string templateConnectionString, string databaseName)
        {
            var builder = new NpgsqlConnectionStringBuilder(templateConnectionString)
            {
                Database = databaseName
            };
            return builder.ConnectionString;
        }
    }
}
