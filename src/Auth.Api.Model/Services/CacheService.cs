using Auth.Api.Model.Services.Interfaces;
using OperationResult;
using StackExchange.Redis;

namespace Auth.Api.Model.Services;

public class CacheService(IConnectionMultiplexer connection) : ICacheService
{
    private readonly IDatabase _database = connection.GetDatabase();

    public async Task<Result> SetAsync(string key, string data, TimeSpan expire)
    {
        try
        {
            await _database.StringSetAsync(key, data, expire);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error(ex);
        }
    }
}