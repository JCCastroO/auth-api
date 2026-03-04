using Auth.Api.Controller.Dtos;
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
        var dto = new RegisterUserDto("John Doe", "john.doe@email.com", "john@123");

        // Act
        var response = await _client.PostAsJsonAsync("api/v1/auth/register", dto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ShoulCallPostAuthLoginThenResultSuccess()
    {
        // Arrange
        var dto = new LoginDto("doe.john@email.com", "doe@123");

        // Act
        var response = await _client.PostAsJsonAsync("api/v1/auth/login", dto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}