using Auth.Api.Controller.Services;
using Auth.Api.Controller.Services.Interfaces;
using Auth.Api.Model.Entities;

namespace Auth.Api.Controller.Tests.Unit.Services;

public class TokenServiceTests
{
    private readonly ITokenService _service = new TokenService("key", "app", 60, 64);

    [Fact]
    public void ShouldGenerateAnJsonWebToken()
    {
        // Arrange
        var email = "john.doe@email.com";
        var user = new UserEntity()
        {
            Name = "Jhon Doe",
            Email = email,
            Password = "jhon@123"
        };

        // Act
        var (accessToken, expiresOn) = _service.Generate(user);

        // Assert
        Assert.NotNull(accessToken);
        Assert.NotEmpty(accessToken);
        Assert.NotEqual(DateTimeOffset.UtcNow, expiresOn);
    }

    [Fact]
    public void ShouldGenerateAnRefreshJsonWebToken()
    {
        // Arrange

        // Act
        var refreshToken = _service.GenerateRefresh();

        // Assert
        Assert.NotNull(refreshToken);
        Assert.NotEmpty(refreshToken);
    }
}