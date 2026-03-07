using Auth.Api.Controller.Dtos;

namespace Auth.Api.Controller.Tests.Unit.Dtos;

public class RefreshTokenCacheResultDtoTests
{
    [Fact]
    public void ShouldCreateEntityWithValidProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "john.doe@email.com";

        // Act
        var dto = new RefreshTokenCacheResultDto(userId, email);

        // Assert
        Assert.Equal(userId, dto.Id);
        Assert.Equal(email, dto.Email);
    }
}