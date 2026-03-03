using Auth.Api.Controller.Dtos;

namespace Auth.Api.Controller.Tests.Unit.Dtos;

public class LoginDtoTests
{
    [Fact]
    public void ShouldCreateEntityWithValidProperties()
    {
        // Arrange
        var email = "john.doe@email.com";
        var password = "john@123";

        // Act
        var dto = new LoginDto(email, password);

        // Assert
        Assert.Equal(email, dto.Email);
        Assert.Equal(password, dto.Password);
    }
}