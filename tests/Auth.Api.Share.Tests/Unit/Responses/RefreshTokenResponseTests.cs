using Auth.Api.Share.Responses;

namespace Auth.Api.Share.Tests.Unit.Responses;

public class RefreshTokenResponseTests
{
    [Fact]
    public void ShouldCreateEntityWithValidProperties()
    {
        // Arrange
        var accessToken = "access_token";
        var refreshToken = "refresh_token";
        var expiresOn = DateTimeOffset.UtcNow.AddMinutes(60);
        var expiresRefreshOn = DateTimeOffset.UtcNow.AddMinutes(180);

        // Act
        var response = new RefreshTokenResponse(accessToken, refreshToken, expiresOn, expiresRefreshOn);

        // Assert
        Assert.Equal(accessToken, response.AccessToken);
        Assert.Equal(refreshToken, response.RefreshToken);
        Assert.Equal(expiresOn, response.ExpiresOn);
        Assert.Equal(expiresRefreshOn, response.ExpiresRefreshOn);
    }
}