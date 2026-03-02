using Auth.Api.Model.Entities;
using Auth.Api.Model.Repositories;
using Auth.Api.Model.Repositories.Interfaces;
using Dapper;

namespace Auth.Api.Model.Tests.Integrated.Repositories;

public class UserRepositoryTests : AppTestContainer
{
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
        await repository.Insert(user);
        var result = await Connection.QueryFirstOrDefaultAsync<UserEntity>("SELECT * FROM users WHERE id = @id", new { user.Id });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.Password, result.Password);
        Assert.Equal(user.CreatedAt, result.CreatedAt, TimeSpan.FromSeconds(5));
        Assert.Equal(user.UpdatedAt, result.UpdatedAt);
    }
}