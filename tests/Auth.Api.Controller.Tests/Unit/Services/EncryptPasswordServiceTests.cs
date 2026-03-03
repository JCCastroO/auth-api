using Auth.Api.Controller.Services;
using Auth.Api.Controller.Services.Interfaces;

namespace Auth.Api.Controller.Tests.Unit.Services;

public class EncryptPasswordServiceTests
{
    private readonly IEncryptPasswordService _service = new EncryptPasswordService();

    [Fact]
    public void ShouldEncryptPassword()
    {
        // Arrange
        var password = "john@123";

        // Act
        var result = _service.Encrypt(password);

        // Assert
        Assert.NotEqual(password, result);
    }

    [Fact]
    public void ShouldValidEncryptedPassword()
    {
        // Arrange
        var password = "john@123";
        var hash = _service.Encrypt(password);

        // Act
        var result = _service.Validate(password, hash);

        // Assert
        Assert.True(result);
    }
}