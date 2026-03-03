using Auth.Api.Controller.Dtos;
using Auth.Api.Controller.Services.Interfaces;
using Auth.Api.Controller.UseCases.Interfaces;
using Auth.Api.Model.Repositories.Interfaces;
using OperationResult;

namespace Auth.Api.Controller.UseCases;

public class LoginUseCase(
    IUserRepository repository,
    IEncryptPasswordService encryptPasswordService,
    ITokenService tokenService) : ILoginUseCase
{
    private readonly IUserRepository _repository = repository;
    private readonly IEncryptPasswordService _encryptPasswordService = encryptPasswordService;
    private readonly ITokenService _tokenService = tokenService;

    public async Task<Result<LoginResponse>> Execute(LoginDto dto)
    {
        var (userSuccess, user, userError) = await _repository.GetByEmail(dto.Email);
        if (!userSuccess && userError is not null)
            return Result.Error<LoginResponse>(new Exception("Internal Error"));

        if (user is null)
            return Result.Error<LoginResponse>(new Exception("Unhautorized"));

        if (!_encryptPasswordService.Validate(dto.Password, user.Password))
            return Result.Error<LoginResponse>(new Exception("Unhautorized"));

        var (accessToken, expiresOn) = _tokenService.Generate(user);
        var (refreshToken, expiresRefreshOn) = _tokenService.GenerateRefresh(user);

        return Result.Success(new LoginResponse(accessToken, refreshToken, expiresOn, expiresRefreshOn));
    }
}