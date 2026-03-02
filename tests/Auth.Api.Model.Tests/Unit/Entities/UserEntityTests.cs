using Auth.Api.Model.Entities;

namespace Auth.Api.Model.Tests.Unit.Entities;

public class UserEntityTests
{
    [Fact]
    public void ShouldCreateEntityWithValidProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "John Doe";
        var email = "john.doe@email.com";
        var password = "john@123";
        var createdAt = DateTimeOffset.UtcNow;

        // Act
        var user = new UserEntity(id, name, email, password);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(id, user.Id);
        Assert.Equal(name, user.Name);
        Assert.Equal(email, user.Email);
        Assert.Equal(password, user.Password);
        Assert.Equal(createdAt, user.CreatedAt, TimeSpan.FromSeconds(5));
        Assert.Null(user.UpdatedAt);
    }
}