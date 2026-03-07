using Auth.Api.Controller.Requests;
using System.Net;
using System.Net.Http.Json;

namespace Auth.Api.View.Tests.Integrated.Endpoints;

public class AuthEndpointsTests : IClassFixture<PostgreSqlFixture>, IClassFixture<RedisFixture>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(PostgreSqlFixture dbFixture, RedisFixture redisFixture)
    {
        var factory = new AppTestContainer(dbFixture, redisFixture);
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ShoulCallPostAuthRegisterThenResultSuccess()
    {
        // Arrange
        var request = new RegisterUserRequest("John Doe", "john.doe@email.com", "john@123");

        // Act
        var response = await _client.PostAsJsonAsync("api/v1/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ShoulCallPostAuthLoginThenResultSuccess()
    {
        // Arrange
        var request = new LoginRequest("doe.john@email.com", "doe@123");

        // Act
        var response = await _client.PostAsJsonAsync("api/v1/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ShoulCallPostAuthRefreshTokenThenResultSuccess()
    {
        // Arrange
        var request = new RefreshTokenRequest("refresh_token");

        // Act
        var response = await _client.PostAsJsonAsync("api/v1/auth/refresh_token", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}