using Auth.Api.Model.Entities;
using Auth.Api.Model.Repositories.Interfaces;
using Dapper;
using OperationResult;
using System.Data;

namespace Auth.Api.Model.Repositories;

public class UserRepository(IDbConnection connection) : IUserRepository
{
    private readonly IDbConnection _connection = connection;

    public async Task<Result<UserEntity?>> GetByEmail(string email)
    {
        try
        {
            var result = await _connection.QueryFirstOrDefaultAsync<UserEntity?>(
                "SELECT id, name, email, password, created_at, updated_at FROM users WHERE email = @email",
                new { email });

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Error<UserEntity?>(ex);
        }
    }

    public async Task<Result> Insert(UserEntity user)
    {
        try
        {
            await _connection.ExecuteAsync("""
            INSERT INTO users (id, name, email, password, created_at, updated_at)
            VALUES (@Id, @Name, @Email, @Password, @CreatedAt, @UpdatedAt)
            """, user);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error(ex);
        }
    }
}