using Auth.Api.Controller.Dtos;
using Auth.Api.Controller.Services;
using Auth.Api.Controller.Services.Interfaces;
using Auth.Api.Controller.UseCases;
using Auth.Api.Controller.UseCases.Interfaces;
using Auth.Api.Model.Entities;
using Auth.Api.Model.Repositories.Interfaces;
using NSubstitute;
using OperationResult;

namespace Auth.Api.Controller.Tests.Unit.UseCases;

public class LoginUseCaseTests
{
    private readonly IUserRepository _repository = Substitute.For<IUserRepository>();
    private readonly IEncryptPasswordService _encryptPasswordService = Substitute.For<IEncryptPasswordService>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly ILoginUseCase _sut;

    public LoginUseCaseTests()
        => _sut = new LoginUseCase(_repository, _encryptPasswordService, _tokenService);

    [Fact]
    public async Task ShouldExecuteLoginThenReturnErrorWhenGetByEmailFailed()
    {
        // Arrange
        var dto = new LoginDto("john.doe@email.com", "john@123");

        _repository
            .GetByEmail(Arg.Do<string>(x => x = dto.Email))
            .Returns(Result.Error<UserEntity?>(new Exception("Internal Error")));

        // Act
        var result = await _sut.Execute(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        await _repository.Received().GetByEmail(Arg.Do<string>(x => x = dto.Email));
        _encryptPasswordService.DidNotReceive()
            .Validate(default!, default!);
    }

    [Fact]
    public async Task ShouldExecuteLoginThenReturnErrorWhenUserNotFound()
    {
        // Arrange
        var dto = new LoginDto("john.doe@email.com", "john@123");

        _repository
            .GetByEmail(Arg.Do<string>(x => x = dto.Email))
            .Returns(Result.Success<UserEntity?>(null));

        // Act
        var result = await _sut.Execute(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        await _repository.Received().GetByEmail(Arg.Do<string>(x => x = dto.Email));
        _encryptPasswordService.DidNotReceive()
            .Validate(default!, default!);
    }

    [Fact]
    public async Task ShouldExecuteLoginThenReturnErrorWhenUseAWrongPassword()
    {
        // Arrange
        var dto = new LoginDto("john.doe@email.com", "john@123");

        var user = new UserEntity()
        {
            Name = "John Doe",
            Email = dto.Email,
            Password = _encryptPasswordService.Encrypt("123#john")
        };
        _repository
            .GetByEmail(Arg.Do<string>(x => x = dto.Email))
            .Returns(Result.Success<UserEntity?>(user));

        _encryptPasswordService
            .Validate(Arg.Do<string>(x => x = dto.Password), Arg.Do<string>(x => x = user.Password))
            .Returns(false);

        // Act
        var result = await _sut.Execute(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        await _repository
            .Received()
            .GetByEmail(Arg.Do<string>(x => x = dto.Email));
        _encryptPasswordService
            .Received()
            .Validate(Arg.Do<string>(x => x = dto.Password), Arg.Do<string>(x => x = user.Password));
    }

    [Fact]
    public async Task ShouldExecuteLoginThenReturnSuccessWithAccessToken()
    {
        // Arrange
        var dto = new LoginDto("john.doe@email.com", "john@123");

        var user = new UserEntity()
        {
            Name = "John Doe",
            Email = dto.Email,
            Password = _encryptPasswordService.Encrypt(dto.Password)
        };
        _repository
            .GetByEmail(Arg.Do<string>(x => x = dto.Email))
            .Returns(Result.Success<UserEntity?>(user));

        _encryptPasswordService
            .Validate(Arg.Do<string>(x => x = dto.Password), Arg.Do<string>(x => x = user.Password))
            .Returns(true);

        var accessToken = "access_token";
        var refreshToken = "refresh_token";
        var expiresOn = DateTimeOffset.UtcNow.AddMinutes(60);
        var expiresRefreshOn = DateTimeOffset.UtcNow.AddMinutes(180);
        _tokenService
            .Generate(Arg.Do<UserEntity>(x => x = user))
            .Returns((accessToken, expiresOn));
        _tokenService
            .GenerateRefresh(Arg.Do<UserEntity>(x => x = user))
            .Returns((refreshToken, expiresRefreshOn));

        var response = new LoginResponse(accessToken, refreshToken, expiresOn, expiresRefreshOn);

        // Act
        var result = await _sut.Execute(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(response, result.Value);
        await _repository
            .Received()
            .GetByEmail(Arg.Do<string>(x => x = dto.Email));
        _encryptPasswordService
            .Received()
            .Validate(Arg.Do<string>(x => x = dto.Password), Arg.Do<string>(x => x = user.Password));
        _tokenService
            .Received()
            .Generate(Arg.Do<UserEntity>(x => x = user));
        _tokenService
            .Received()
            .GenerateRefresh(Arg.Do<UserEntity>(x => x = user));
    }
}