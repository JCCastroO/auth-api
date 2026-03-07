using Auth.Api.Controller.Requests;

namespace Auth.Api.Controller.Tests.Unit.Requests;

public class LoginRequestTests
{
    [Fact]
    public void ShouldCreateEntityWithValidProperties()
    {
        // Arrange
        var email = "john.doe@email.com";
        var password = "john@123";

        // Act
        var request = new LoginRequest(email, password);

        // Assert
        Assert.Equal(email, request.Email);
        Assert.Equal(password, request.Password);
    }
}