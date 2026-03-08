using Auth.Api.Controller.Services.Interfaces;
using Auth.Api.Controller.UseCases;
using Auth.Api.Controller.UseCases.Interfaces;
using Auth.Api.Model.Entities;
using Auth.Api.Model.Repositories.Interfaces;
using Auth.Api.Model.Services.Interfaces;
using Auth.Api.Share.Requests;
using Auth.Api.Share.Responses;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using OperationResult;

namespace Auth.Api.Controller.Tests.Unit.UseCases;

public class LoginUseCaseTests
{
    private readonly ILogger<LoginUseCase> _logger = Substitute.For<ILogger<LoginUseCase>>();
    private readonly IUserRepository _repository = Substitute.For<IUserRepository>();
    private readonly IEncryptPasswordService _encryptPasswordService = Substitute.For<IEncryptPasswordService>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly ICacheService _cacheService = Substitute.For<ICacheService>();
    private readonly ILoginUseCase _sut;

    public LoginUseCaseTests()
        => _sut = new LoginUseCase(_logger, _repository, _encryptPasswordService, _tokenService, _cacheService);

    [Fact]
    public async Task ShouldExecuteLoginThenReturnErrorWhenGetByEmailFailed()
    {
        // Arrange
        var request = new LoginRequest("john.doe@email.com", "john@123");

        _repository
            .GetByEmail(Arg.Do<string>(x => x = request.Email))
            .Returns(Result.Error<UserEntity?>(new Exception("Internal Error")));

        // Act
        var result = await _sut.Execute(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        await _repository.Received().GetByEmail(Arg.Do<string>(x => x = request.Email));
        _encryptPasswordService.DidNotReceive()
            .Validate(default!, default!);
        _tokenService
            .DidNotReceive()
            .Generate(default!);
        _tokenService
            .DidNotReceive()
            .GenerateRefresh();
        await _cacheService
            .DidNotReceive()
            .SetAsync(default!, default!, default!);
    }

    [Fact]
    public async Task ShouldExecuteLoginThenReturnErrorWhenUserNotFound()
    {
        // Arrange
        var request = new LoginRequest("john.doe@email.com", "john@123");

        _repository
            .GetByEmail(Arg.Do<string>(x => x = request.Email))
            .Returns(Result.Success<UserEntity?>(null));

        // Act
        var result = await _sut.Execute(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        await _repository.Received().GetByEmail(Arg.Do<string>(x => x = request.Email));
        _encryptPasswordService.DidNotReceive()
            .Validate(default!, default!);
        _tokenService
            .DidNotReceive()
            .Generate(default!);
        _tokenService
            .DidNotReceive()
            .GenerateRefresh();
        await _cacheService
            .DidNotReceive()
            .SetAsync(default!, default!, default!);
    }

    [Fact]
    public async Task ShouldExecuteLoginThenReturnErrorWhenUseAWrongPassword()
    {
        // Arrange
        var request = new LoginRequest("john.doe@email.com", "john@123");

        var user = new UserEntity()
        {
            Name = "John Doe",
            Email = request.Email,
            Password = _encryptPasswordService.Encrypt("123#john")
        };
        _repository
            .GetByEmail(Arg.Do<string>(x => x = request.Email))
            .Returns(Result.Success<UserEntity?>(user));

        _encryptPasswordService
            .Validate(Arg.Do<string>(x => x = request.Password), Arg.Do<string>(x => x = user.Password))
            .Returns(false);

        // Act
        var result = await _sut.Execute(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        await _repository
            .Received()
            .GetByEmail(Arg.Do<string>(x => x = request.Email));
        _encryptPasswordService
            .Received()
            .Validate(Arg.Do<string>(x => x = request.Password), Arg.Do<string>(x => x = user.Password));
        _tokenService
            .DidNotReceive()
            .Generate(default!);
        _tokenService
            .DidNotReceive()
            .GenerateRefresh();
        await _cacheService
            .DidNotReceive()
            .SetAsync(default!, default!, default!);
    }

    [Fact]
    public async Task ShouldExecuteLoginThenReturnErrorWhenSetCacheFailed()
    {
        // Arrange
        var request = new LoginRequest("john.doe@email.com", "john@123");

        var user = new UserEntity()
        {
            Name = "John Doe",
            Email = request.Email,
            Password = _encryptPasswordService.Encrypt(request.Password)
        };
        _repository
            .GetByEmail(Arg.Do<string>(x => x = request.Email))
            .Returns(Result.Success<UserEntity?>(user));

        _encryptPasswordService
            .Validate(Arg.Do<string>(x => x = request.Password), Arg.Do<string>(x => x = user.Password))
            .Returns(true);

        var accessToken = "access_token";
        var expiresOn = DateTimeOffset.UtcNow.AddMinutes(60);
        _tokenService
            .Generate(Arg.Do<UserEntity>(x => x = user))
            .Returns((accessToken, expiresOn));

        var refreshToken = "refresh_token";
        var expiresRefreshOn = DateTimeOffset.UtcNow.AddDays(7);
        _tokenService
            .GenerateRefresh()
            .Returns(refreshToken);

        _cacheService
            .SetAsync(
                Arg.Do<string>(x => x = $"refresh#{refreshToken}"),
                Arg.Do<string>(x => x = JsonConvert.SerializeObject(new { user.Id, user.Email })),
                Arg.Do<TimeSpan>(x => x = TimeSpan.FromDays(7)))
            .Returns(Result.Error(new Exception("Internal Error")));

        // Act
        var result = await _sut.Execute(request);

        // Assert
        Assert.False(result.IsSuccess);
        await _repository
            .Received()
            .GetByEmail(Arg.Do<string>(x => x = request.Email));
        _encryptPasswordService
            .Received()
            .Validate(Arg.Do<string>(x => x = request.Password), Arg.Do<string>(x => x = user.Password));
        _tokenService
            .Received()
            .Generate(Arg.Do<UserEntity>(x => x = user));
        _tokenService
            .Received()
            .GenerateRefresh();
        await _cacheService
            .Received()
            .SetAsync(
                Arg.Do<string>(x => x = $"refresh#{refreshToken}"),
                Arg.Do<string>(x => x = JsonConvert.SerializeObject(new { user.Id, user.Email })),
                Arg.Do<TimeSpan>(x => x = TimeSpan.FromDays(7)));
    }

    [Fact]
    public async Task ShouldExecuteLoginThenReturnSuccessWithAccessToken()
    {
        // Arrange
        var request = new LoginRequest("john.doe@email.com", "john@123");

        var user = new UserEntity()
        {
            Name = "John Doe",
            Email = request.Email,
            Password = _encryptPasswordService.Encrypt(request.Password)
        };
        _repository
            .GetByEmail(Arg.Do<string>(x => x = request.Email))
            .Returns(Result.Success<UserEntity?>(user));

        _encryptPasswordService
            .Validate(Arg.Do<string>(x => x = request.Password), Arg.Do<string>(x => x = user.Password))
            .Returns(true);

        var accessToken = "access_token";
        var expiresOn = DateTimeOffset.UtcNow.AddMinutes(60);
        _tokenService
            .Generate(Arg.Do<UserEntity>(x => x = user))
            .Returns((accessToken, expiresOn));

        var refreshToken = "refresh_token";
        var expiresRefreshOn = DateTimeOffset.UtcNow.AddDays(7);
        _tokenService
            .GenerateRefresh()
            .Returns(refreshToken);

        _cacheService
            .SetAsync(
                Arg.Do<string>(x => x = $"refresh#{refreshToken}"),
                Arg.Do<string>(x => x = JsonConvert.SerializeObject(new { user.Id, user.Email })),
                Arg.Do<TimeSpan>(x => x = TimeSpan.FromDays(7)))
            .Returns(Result.Success());

        var response = new LoginResponse(accessToken, refreshToken, expiresOn, expiresRefreshOn);

        // Act
        var result = await _sut.Execute(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(response.AccessToken, result.Value.AccessToken);
        Assert.Equal(response.RefreshToken, result.Value.RefreshToken);
        Assert.Equal(response.ExpiresOn, result.Value.ExpiresOn, TimeSpan.FromSeconds(5));
        Assert.Equal(response.ExpiresRefreshOn, result.Value.ExpiresRefreshOn, TimeSpan.FromSeconds(5));
        await _repository
            .Received()
            .GetByEmail(Arg.Do<string>(x => x = request.Email));
        _encryptPasswordService
            .Received()
            .Validate(Arg.Do<string>(x => x = request.Password), Arg.Do<string>(x => x = user.Password));
        _tokenService
            .Received()
            .Generate(Arg.Do<UserEntity>(x => x = user));
        _tokenService
            .Received()
            .GenerateRefresh();
        await _cacheService
            .Received()
            .SetAsync(
                Arg.Do<string>(x => x = $"refresh#{refreshToken}"),
                Arg.Do<string>(x => x = JsonConvert.SerializeObject(new { user.Id, user.Email })),
                Arg.Do<TimeSpan>(x => x = TimeSpan.FromDays(7)));
    }
}