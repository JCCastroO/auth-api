using Auth.Api.Controller.Dtos;
using System.Net;
using System.Net.Http.Json;

namespace Auth.Api.View.Tests.Integrated.Endpoints;

public class AuthEndpointsTests : IClassFixture<PostgreSqlFixture>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(PostgreSqlFixture dbFixture)
    {
        var factory = new AppTestContainer(dbFixture);
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
}