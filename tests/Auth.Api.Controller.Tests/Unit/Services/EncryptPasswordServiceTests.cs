using Auth.Api.Controller.Services;

namespace Auth.Api.Controller.Tests.Unit.Services;

public class EncryptPasswordServiceTests
{
    [Fact]
    public void ShouldEncryptPassword()
    {
        // Arrange
        var service = new EncryptPasswordService();
        var password = "john@123";

        // Act
        var result = service.Encrypt(password);

        // Assert
        Assert.NotEqual(password, result);
    }

    [Fact]
    public void ShouldValidEncryptedPassword()
    {
        // Arrange
        var service = new EncryptPasswordService();
        var password = "john@123";
        var hash = service.Encrypt(password);

        // Act
        var result = service.Validate(password, hash);

        // Assert
        Assert.True(result);
    }
}