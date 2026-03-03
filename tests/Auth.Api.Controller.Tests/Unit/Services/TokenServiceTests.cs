using Auth.Api.Controller.Services;
using Auth.Api.Controller.Services.Interfaces;
using Auth.Api.Model.Entities;

namespace Auth.Api.Controller.Tests.Unit.Services;

public class TokenServiceTests
{
    private readonly ITokenService _service = new TokenService();

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
        var email = "john.doe@email.com";
        var user = new UserEntity()
        {
            Name = "Jhon Doe",
            Email = email,
            Password = "jhon@123"
        };

        // Act
        var (refreshToken, expiresRefreshOn) = _service.GenerateRefresh(user);

        // Assert
        Assert.NotNull(refreshToken);
        Assert.NotEmpty(refreshToken);
        Assert.NotEqual(DateTimeOffset.UtcNow, expiresRefreshOn);
    }
}