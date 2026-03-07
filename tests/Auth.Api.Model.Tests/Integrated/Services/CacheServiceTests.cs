using Auth.Api.Model.Services;
using Newtonsoft.Json;

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

    [Fact]
    public async Task ShouldGetDataOnCacheDatabase()
    {
        // Arrange
        var service = new CacheService(RedisConnection);

        var key = "refresh#refresh_token";
        var data = JsonConvert.SerializeObject(new { Id = Guid.NewGuid().ToString(), Email = "john.doe@email.com" });

        // Act
        await service.SetAsync(key, data, TimeSpan.FromDays(7));
        var result = await service.GetAsync<object>(key);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }
}