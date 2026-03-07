using Auth.Api.Model.Services.Interfaces;
using Newtonsoft.Json;
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

    public async Task<Result<T?>> GetAsync<T>(string key)
    {
        try
        {
            var data = await _database.StringGetAsync(key);
            if (data.IsNull)
                return Result.Success<T?>(default);

            var result = JsonConvert.DeserializeObject<T>(data!);
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Error<T?>(ex);
        }
    }

    public async Task<Result> RemoveAsync(string key)
    {
        try
        {
            await _database.KeyDeleteAsync(key);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error(ex);
        }
    }
}