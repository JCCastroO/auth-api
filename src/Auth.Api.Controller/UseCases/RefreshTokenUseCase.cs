using Auth.Api.Controller.Services.Interfaces;
using Auth.Api.Controller.UseCases.Interfaces;
using Auth.Api.Model.Entities;
using Auth.Api.Model.Services.Interfaces;
using Auth.Api.Share.Dtos;
using Auth.Api.Share.Requests;
using Auth.Api.Share.Responses;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OperationResult;

namespace Auth.Api.Controller.UseCases;

public class RefreshTokenUseCase(
    ILogger<RefreshTokenUseCase> logger,
    ICacheService cacheService,
    ITokenService tokenService) : IRefreshTokenUseCase
{
    private const int REFRESH_TOKEN_EXPIRES_ON = 7;
    private readonly ILogger<RefreshTokenUseCase> _logger = logger;
    private readonly ICacheService _cacheService = cacheService;
    private readonly ITokenService _tokenService = tokenService;

    public async Task<Result<RefreshTokenResponse>> Execute(RefreshTokenRequest request)
    {
        _logger.LogInformation("Initializing Refresh Token.");

        var cacheKey = $"refresh#{request.RefreshToken}";
        var (cacheDataSuccess, cacheData, cacheDataError) = await _cacheService.GetAsync<RefreshTokenCacheResultDto>(cacheKey);
        if (!cacheDataSuccess && cacheDataError is not null)
        {
            _logger.LogError(cacheDataError, "An unexpected error occurred on cache data search. CacheKey: {CacheKey}", cacheKey);
            return Result.Error<RefreshTokenResponse>(new SystemException("Internal Error"));
        }

        if (cacheData is null)
        {
            _logger.LogWarning("Data not found. CacheKey: {CacheKey}", cacheKey);
            return Result.Error<RefreshTokenResponse>(new UnauthorizedAccessException("Unauthorized"));
        }

        var user = new UserEntity
        {
            Id = cacheData.Id,
            Email = cacheData.Email
        };

        var (accessToken, expiresOn) = _tokenService.Generate(user);
        var refreshToken = _tokenService.GenerateRefresh();
        var expiresRefreshOn = DateTimeOffset.UtcNow.AddDays(REFRESH_TOKEN_EXPIRES_ON);

        var (removeCacheSuccess, removeCacheError) = await _cacheService.RemoveAsync(cacheKey);
        if (!removeCacheSuccess && removeCacheError is not null)
        {
            _logger.LogError(removeCacheError, "An unexpected error occurred on remove old cache data. CacheKey: {CacheKey}", cacheKey);
            return Result.Error<RefreshTokenResponse>(new SystemException("Internal Error"));
        }

        var (setCacheSuccess, setCacheError) = await _cacheService.SetAsync(
            $"refresh#{refreshToken}",
            JsonConvert.SerializeObject(new { user.Id, user.Email }),
            TimeSpan.FromDays(REFRESH_TOKEN_EXPIRES_ON));
        if (!setCacheSuccess && setCacheError is not null)
        {
            _logger.LogError(setCacheError, "An unexpected error occurred on set refresh token cache. Email: {Email}", user.Email);
            return Result.Error<RefreshTokenResponse>(new SystemException("Internal Error"));
        }

        _logger.LogInformation("Finalizing Refresh Token Successfully. Email: {Email}", user.Email);
        return Result.Success(new RefreshTokenResponse(accessToken, refreshToken, expiresOn, expiresRefreshOn));
    }
}