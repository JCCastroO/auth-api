using Auth.Api.Controller.Services;
using Auth.Api.Model.Entities;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using System.Data;
using Testcontainers.PostgreSql;

namespace Auth.Api.View.Tests.Integrated;

public class AppTestContainer(PostgreSqlFixture dbFixture) : WebApplicationFactory<Program>
{
    private readonly PostgreSqlFixture _dbFixture = dbFixture;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
        => builder.ConfigureServices(services =>
        {
            services.RemoveAll<IDbConnection>();

            services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(_dbFixture.Container.GetConnectionString()));
        });
}

public class PostgreSqlFixture : IAsyncLifetime
{
    public PostgreSqlContainer Container = default!;

    public async Task InitializeAsync()
    {
        Container = new PostgreSqlBuilder("postgres:alpine3.23")
            .WithDatabase("test_db")
            .WithUsername("test")
            .WithPassword("test")
            .Build();

        await Container.StartAsync();

        await CreateSchemaAsync();
    }

    public async Task DisposeAsync()
    {
        await Container.StopAsync();
    }

    private async Task CreateSchemaAsync()
    {
        await using var connection = new NpgsqlConnection(Container.GetConnectionString());
        await connection.ExecuteAsync("""
            CREATE TABLE users (
                id UUID PRIMARY KEY,
                name VARCHAR NOT NULL,
                email VARCHAR NOT NULL,
                password VARCHAR NOT NULL,
                created_at TIMESTAMP NOT NULL,
                updated_at TIMESTAMP
            );
            """);

        var encryptService = new EncryptPasswordService();

        var user = new UserEntity()
        {
            Name = "Doe John",
            Email = "doe.john@email.com",
            Password = encryptService.Encrypt("doe@123")
        };
        await connection.ExecuteAsync("""
            INSERT INTO users (id, name, email, password, created_at, updated_at)
            VALUES (@Id, @Name, @Email, @Password, @CreatedAt, @UpdatedAt)
            """, user);
    }
}