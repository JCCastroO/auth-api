using Auth.Api.Model.Entities;
using Auth.Api.Model.Repositories;
using Dapper;

namespace Auth.Api.Model.Tests.Integrated.Repositories;

public class UserRepositoryTests : AppTestContainer
{
    [Fact]
    public async Task ShouldNotFoundUserWhenQueryUserByEmail()
    {
        // Arrange
        var name = "john doe";
        var email = "john.doe@email.com";
        var password = "jhon@123";
        var user = new UserEntity()
        {
            Name = name,
            Email = email,
            Password = password,
        };

        var repository = new UserRepository(Connection);

        // Act
        var (success, result, _) = await repository.GetByEmail(user.Email);

        // Assert
        Assert.True(success);
        Assert.Null(result);
    }

    [Fact]
    public async Task ShouldReturnErrorWhenQueryUserByEmail()
    {
        // Arrange
        var name = "john doe";
        var email = "john.doe@email.com";
        var password = "jhon@123";
        var user = new UserEntity()
        {
            Name = name,
            Email = email,
            Password = password,
        };

        var repository = new UserRepository(default!);

        // Act
        await Connection.ExecuteAsync("""
            INSERT INTO users (id, name, email, password, created_at, updated_at)
            VALUES (@Id, @Name, @Email, @Password, @CreatedAt, @UpdatedAt)
            """, user);
        var (success, result, error) = await repository.GetByEmail(user.Email);

        // Assert
        Assert.False(success);
        Assert.Null(result);
        Assert.NotNull(error);
    }

    [Fact]
    public async Task ShouldGetUserByEmail()
    {
        // Arrange
        var name = "john doe";
        var email = "john.doe@email.com";
        var password = "jhon@123";
        var user = new UserEntity()
        {
            Name = name,
            Email = email,
            Password = password,
        };

        var repository = new UserRepository(Connection);

        // Act
        await Connection.ExecuteAsync("""
            INSERT INTO users (id, name, email, password, created_at, updated_at)
            VALUES (@Id, @Name, @Email, @Password, @CreatedAt, @UpdatedAt)
            """, user);
        var (success, result, _) = await repository.GetByEmail(user.Email);

        // Assert
        Assert.True(success);
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.Password, result.Password);
        Assert.Equal(user.CreatedAt, result.CreatedAt, TimeSpan.FromSeconds(5));
        Assert.Equal(user.UpdatedAt, result.UpdatedAt);
    }

    [Fact]
    public async Task ShouldNotInsertNewUserOnDatabase()
    {
        // Arrange
        var name = "john doe";
        var email = "john.doe@email.com";
        var password = "jhon@123";
        var user = new UserEntity()
        {
            Name = name,
            Email = email,
            Password = password,
        };

        var repository = new UserRepository(default!);

        // Act
        var insertResult = await repository.Insert(user);
        var result = await Connection.QueryFirstOrDefaultAsync<UserEntity>("SELECT * FROM users WHERE id = @id", new { user.Id });

        // Assert
        Assert.False(insertResult.IsSuccess);
        Assert.Null(result);
    }

    [Fact]
    public async Task ShouldInsertNewUserOnDatabase()
    {
        // Arrange
        var name = "john doe";
        var email = "john.doe@email.com";
        var password = "jhon@123";
        var user = new UserEntity()
        {
            Name = name,
            Email = email,
            Password = password,
        };

        var repository = new UserRepository(Connection);

        // Act
        var insertResult = await repository.Insert(user);
        var result = await Connection.QueryFirstOrDefaultAsync<UserEntity>("SELECT * FROM users WHERE id = @id", new { user.Id });

        // Assert
        Assert.True(insertResult.IsSuccess);
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.Password, result.Password);
        Assert.Equal(user.CreatedAt, result.CreatedAt, TimeSpan.FromSeconds(5));
        Assert.Equal(user.UpdatedAt, result.UpdatedAt);
    }
}