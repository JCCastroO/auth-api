using OperationResult;

namespace Auth.Api.Model.Services.Interfaces;

public interface ICacheService
{
    Task<Result> SetAsync(string key, string data, TimeSpan expire);
}