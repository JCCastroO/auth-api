using Dapper;
using Npgsql;
using System.Data;
using Testcontainers.PostgreSql;

namespace Auth.Api.Model.Tests.Integrated;

public class AppTestContainer : IAsyncLifetime
{
    private PostgreSqlContainer _container = default!;

    protected IDbConnection Connection = default!;

    public async Task InitializeAsync()
    {
        _container = new PostgreSqlBuilder("postgres:alpine3.23")
            .WithDatabase("test_db")
            .WithUsername("test")
            .WithPassword("test")
            .Build();

        await _container.StartAsync();

        Connection = new NpgsqlConnection(_container.GetConnectionString());

        Connection.Open();
        await CreateSchemaAsync();
    }

    public async Task DisposeAsync()
    {
        Connection.Dispose();
        await _container.DisposeAsync();
    }

    private async Task CreateSchemaAsync()
    {
        await Connection.ExecuteAsync("""
            CREATE TABLE users (
                id UUID PRIMARY KEY,
                name VARCHAR NOT NULL,
                email VARCHAR NOT NULL,
                password VARCHAR NOT NULL,
                created_at TIMESTAMP NOT NULL,
                updated_at TIMESTAMP
            );
            """);
    }
}