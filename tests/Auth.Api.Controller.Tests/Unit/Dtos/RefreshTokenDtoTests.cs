using Auth.Api.Controller.Dtos;

namespace Auth.Api.Controller.Tests.Unit.Dtos;

public class RefreshTokenDtoTests
{
    [Fact]
    public void ShouldCreateEntityWithValidProperties()
    {
        // Arrange
        var refreshToken = "refresh_token";

        // Act
        var dto = new RefreshTokenDto(refreshToken);

        // Assert
        Assert.Equal(refreshToken, dto.RefreshToken);
    }
}