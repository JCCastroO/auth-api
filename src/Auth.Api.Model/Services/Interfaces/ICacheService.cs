using OperationResult;

namespace Auth.Api.Model.Services.Interfaces;

public interface ICacheService
{
    Task<Result> SetAsync(string key, string data, TimeSpan expire);

    Task<Result<T?>> GetAsync<T>(string key);

    Task<Result> RemoveAsync(string key);
}