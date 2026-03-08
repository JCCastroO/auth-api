using Auth.Api.Share.Dtos;

namespace Auth.Api.Share.Tests.Unit.Dtos;

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