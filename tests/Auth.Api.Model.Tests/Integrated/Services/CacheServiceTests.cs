using Auth.Api.Model.Services;

namespace Auth.Api.Model.Tests.Integrated.Services;

public class CacheServiceTests : AppTestContainer
{
    [Fact]
    public async Task ShouldSetDataOnCacheDatabase()
    {
        // Arrange
        var key = "KEY";
        var data = "DATA@DATA";
        var expires = TimeSpan.FromDays(7);

        var service = new CacheService(RedisConnection);

        // Act
        var result = await service.SetAsync(key, data, expires);

        // Assert
        Assert.True(result.IsSuccess);
    }
}