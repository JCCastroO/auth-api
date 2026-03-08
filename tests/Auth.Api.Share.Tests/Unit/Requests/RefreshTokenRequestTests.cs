using Auth.Api.Share.Requests;

namespace Auth.Api.Share.Tests.Unit.Requests;

public class RefreshTokenRequestTests
{
    [Fact]
    public void ShouldCreateEntityWithValidProperties()
    {
        // Arrange
        var refreshToken = "refresh_token";

        // Act
        var request = new RefreshTokenRequest(refreshToken);

        // Assert
        Assert.Equal(refreshToken, request.RefreshToken);
    }
}