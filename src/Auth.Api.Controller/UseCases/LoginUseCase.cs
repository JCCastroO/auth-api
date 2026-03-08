using Auth.Api.Controller.Services.Interfaces;
using Auth.Api.Controller.UseCases.Interfaces;
using Auth.Api.Model.Repositories.Interfaces;
using Auth.Api.Model.Services.Interfaces;
using Auth.Api.Share.Requests;
using Auth.Api.Share.Responses;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OperationResult;

namespace Auth.Api.Controller.UseCases;

public class LoginUseCase(
    ILogger<LoginUseCase> logger,
    IUserRepository repository,
    IEncryptPasswordService encryptPasswordService,
    ITokenService tokenService,
    ICacheService cacheService) : ILoginUseCase
{
    private const int REFRESH_TOKEN_EXPIRES_ON = 7;
    private readonly ILogger<LoginUseCase> _logger = logger;
    private readonly IUserRepository _repository = repository;
    private readonly IEncryptPasswordService _encryptPasswordService = encryptPasswordService;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result<LoginResponse>> Execute(LoginRequest request)
    {
        _logger.LogInformation("Initializing Login. Email: {Email}", request.Email);

        var (userSuccess, user, userError) = await _repository.GetByEmail(request.Email);
        if (!userSuccess && userError is not null)
        {
            _logger.LogError(userError, "An unexpected error occurred on database search for user. Email: {Email}", request.Email);
            return Result.Error<LoginResponse>(new SystemException("Internal Error"));
        }

        if (user is null)
        {
            _logger.LogWarning("User not found. Email: {Email}", request.Email);
            return Result.Error<LoginResponse>(new UnauthorizedAccessException("Unauthorized"));
        }

        if (!_encryptPasswordService.Validate(request.Password, user.Password))
        {
            _logger.LogWarning("Wrong Password. Email: {Email}", request.Email);
            return Result.Error<LoginResponse>(new UnauthorizedAccessException("Unauthorized"));
        }

        var (accessToken, expiresOn) = _tokenService.Generate(user);
        var refreshToken = _tokenService.GenerateRefresh();
        var expiresRefreshOn = DateTimeOffset.UtcNow.AddDays(REFRESH_TOKEN_EXPIRES_ON);

        var (cacheSuccess, cacheError) = await _cacheService.SetAsync(
            $"refresh#{refreshToken}",
            JsonConvert.SerializeObject(new { user.Id, user.Email }),
            TimeSpan.FromDays(REFRESH_TOKEN_EXPIRES_ON));
        if (!cacheSuccess && cacheError is not null)
        {
            _logger.LogError(cacheError, "An unexpected error occurred on set refresh token cache. Email: {Email}", request.Email);
            return Result.Error<LoginResponse>(new SystemException("Internal Error"));
        }

        _logger.LogInformation("Finalizing Login Successfully. Email: {Email}", request.Email);
        return Result.Success(new LoginResponse(accessToken, refreshToken, expiresOn, expiresRefreshOn));
    }
}