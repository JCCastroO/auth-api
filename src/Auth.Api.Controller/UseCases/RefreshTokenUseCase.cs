using Auth.Api.Controller.Dtos;
using Auth.Api.Controller.Requests;
using Auth.Api.Controller.Responses;
using Auth.Api.Controller.Services.Interfaces;
using Auth.Api.Controller.UseCases.Interfaces;
using Auth.Api.Model.Entities;
using Auth.Api.Model.Services.Interfaces;
using Newtonsoft.Json;
using OperationResult;

namespace Auth.Api.Controller.UseCases;

public class RefreshTokenUseCase(
    ICacheService cacheService,
    ITokenService tokenService) : IRefreshTokenUseCase
{
    private const int REFRESH_TOKEN_EXPIRES_ON = 7;
    private readonly ICacheService _cacheService = cacheService;
    private readonly ITokenService _tokenService = tokenService;

    public async Task<Result<RefreshTokenResponse>> Execute(RefreshTokenRequest dto)
    {
        var cacheKey = $"refresh#{dto.RefreshToken}";
        var (cacheDataSuccess, cacheData, cacheDataError) = await _cacheService.GetAsync<RefreshTokenCacheResultDto>(cacheKey);
        if (!cacheDataSuccess && cacheDataError is not null)
            return Result.Error<RefreshTokenResponse>(new Exception("Internal Error"));

        if (cacheData is null)
            return Result.Error<RefreshTokenResponse>(new Exception("Unhautorized"));

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
            return Result.Error<RefreshTokenResponse>(new Exception("Internal Error"));

        var (setCacheSuccess, setCacheError) = await _cacheService.SetAsync(
            $"refresh#{refreshToken}",
            JsonConvert.SerializeObject(new { user.Id, user.Email }),
            TimeSpan.FromDays(REFRESH_TOKEN_EXPIRES_ON));
        if (!setCacheSuccess && setCacheError is not null)
            return Result.Error<RefreshTokenResponse>(new Exception("Internal Error"));

        return Result.Success(new RefreshTokenResponse(accessToken, refreshToken, expiresOn, expiresRefreshOn));
    }
}