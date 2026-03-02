using Auth.Api.Controller.Dtos;

namespace Auth.Api.Controller.Tests.Unit.Dtos;

public class RegisterUserDtoTests
{
    [Fact]
    public void ShouldCreateEntityWithValidProperties()
    {
        // Arrange
        var name = "John Doe";
        var email = "john.doe@email.com";
        var password = "john@123";

        // Act
        var dto = new RegisterUserDto(name, email, password);

        // Assert
        Assert.Equal(name, dto.Name);
        Assert.Equal(email, dto.Email);
        Assert.Equal(password, dto.Password);
    }
}