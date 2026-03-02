using Auth.Api.Model.Entities;
using Auth.Api.Model.Repositories.Interfaces;
using Dapper;
using System.Data;

namespace Auth.Api.Model.Repositories;

public class UserRepository(IDbConnection connection) : IUserRepository
{
    private readonly IDbConnection _connection = connection;

    public async Task Insert(UserEntity user)
        => await _connection.ExecuteAsync("""
            INSERT INTO users (id, name, email, password, created_at, updated_at)
            VALUES (@Id, @Name, @Email, @Password, @CreatedAt, @UpdatedAt)
            """, user);
}