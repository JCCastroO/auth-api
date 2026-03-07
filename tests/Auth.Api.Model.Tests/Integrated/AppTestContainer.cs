using Dapper;
using Npgsql;
using StackExchange.Redis;
using System.Data;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Auth.Api.Model.Tests.Integrated;

public class AppTestContainer : IAsyncLifetime
{
    private PostgreSqlContainer _container = default!;
    private RedisContainer _redisContainer = default!;

    protected IDbConnection Connection = default!;
    protected IConnectionMultiplexer RedisConnection = default!;

    public async Task InitializeAsync()
    {
        _container = new PostgreSqlBuilder("postgres:alpine3.23")
            .WithDatabase("test_db")
            .WithUsername("test")
            .WithPassword("test")
            .Build();

        _redisContainer = new RedisBuilder("redis:7-alpine")
           .WithCleanUp(true)
           .Build();

        await _container.StartAsync();
        await _redisContainer.StartAsync();

        Connection = new NpgsqlConnection(_container.GetConnectionString());
        RedisConnection = ConnectionMultiplexer.Connect(_redisContainer.GetConnectionString());

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