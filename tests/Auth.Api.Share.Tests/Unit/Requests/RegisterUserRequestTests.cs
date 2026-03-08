using Auth.Api.Share.Requests;

namespace Auth.Api.Share.Tests.Unit.Requests;

public class RegisterUserRequestTests
{
    [Fact]
    public void ShouldCreateEntityWithValidProperties()
    {
        // Arrange
        var name = "John Doe";
        var email = "john.doe@email.com";
        var password = "john@123";

        // Act
        var request = new RegisterUserRequest(name, email, password);

        // Assert
        Assert.Equal(name, request.Name);
        Assert.Equal(email, request.Email);
        Assert.Equal(password, request.Password);
    }
}