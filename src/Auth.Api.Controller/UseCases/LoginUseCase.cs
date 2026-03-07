using Auth.Api.Controller.Requests;
using Auth.Api.Controller.Responses;
using Auth.Api.Controller.Services.Interfaces;
using Auth.Api.Controller.UseCases.Interfaces;
using Auth.Api.Model.Repositories.Interfaces;
using Auth.Api.Model.Services.Interfaces;
using Newtonsoft.Json;
using OperationResult;

namespace Auth.Api.Controller.UseCases;

public class LoginUseCase(
    IUserRepository repository,
    IEncryptPasswordService encryptPasswordService,
    ITokenService tokenService,
    ICacheService cacheService) : ILoginUseCase
{
    private const int REFRESH_TOKEN_EXPIRES_ON = 7;
    private readonly IUserRepository _repository = repository;
    private readonly IEncryptPasswordService _encryptPasswordService = encryptPasswordService;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result<LoginResponse>> Execute(LoginRequest dto)
    {
        var (userSuccess, user, userError) = await _repository.GetByEmail(dto.Email);
        if (!userSuccess && userError is not null)
            return Result.Error<LoginResponse>(new Exception("Internal Error"));

        if (user is null)
            return Result.Error<LoginResponse>(new Exception("Unhautorized"));

        if (!_encryptPasswordService.Validate(dto.Password, user.Password))
            return Result.Error<LoginResponse>(new Exception("Unhautorized"));

        var (accessToken, expiresOn) = _tokenService.Generate(user);
        var refreshToken = _tokenService.GenerateRefresh();
        var expiresRefreshOn = DateTimeOffset.UtcNow.AddDays(REFRESH_TOKEN_EXPIRES_ON);

        var (cacheSuccess, cacheError) = await _cacheService.SetAsync(
            $"refresh#{refreshToken}",
            JsonConvert.SerializeObject(new { user.Id, user.Email }),
            TimeSpan.FromDays(REFRESH_TOKEN_EXPIRES_ON));
        if (!cacheSuccess && cacheError is not null)
            return Result.Error<LoginResponse>(new Exception("Internal Error"));

        return Result.Success(new LoginResponse(accessToken, refreshToken, expiresOn, expiresRefreshOn));
    }
}