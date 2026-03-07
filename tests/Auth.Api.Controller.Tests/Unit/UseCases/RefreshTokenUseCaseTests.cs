using Auth.Api.Controller.Dtos;
using Auth.Api.Controller.Services.Interfaces;
using Auth.Api.Controller.UseCases;
using Auth.Api.Controller.UseCases.Interfaces;
using Auth.Api.Model.Entities;
using Auth.Api.Model.Services.Interfaces;
using NSubstitute;
using OperationResult;

namespace Auth.Api.Controller.Tests.Unit.UseCases;

public class RefreshTokenUseCaseTests
{
    private readonly ICacheService _cacheService = Substitute.For<ICacheService>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly IRefreshTokenUseCase _sut;

    public RefreshTokenUseCaseTests()
        => _sut = new RefreshTokenUseCase(_cacheService, _tokenService);

    [Fact]
    public async Task ShouldExecuteRefreshTokenUseCaseThenReturnErrorWhenGetCacheFailed()
    {
        // Arrange
        var dto = new RefreshTokenDto("refresh_token");

        _cacheService
            .GetAsync<RefreshTokenCacheResultDto>(Arg.Do<string>(x => x = $"refresh#{dto.RefreshToken}"))
            .Returns(Result.Error<RefreshTokenCacheResultDto>(new Exception("Internal Error")));

        // Act
        var result = await _sut.Execute(dto);

        // Assert
        Assert.False(result.IsSuccess);
        await _cacheService
            .Received()
            .GetAsync<RefreshTokenCacheResultDto>(Arg.Do<string>(x => x = $"refresh#{dto.RefreshToken}"));
        _tokenService
            .DidNotReceive()
            .Generate(default!);
        _tokenService
            .DidNotReceive()
            .GenerateRefresh();
    }

    [Fact]
    public async Task ShouldExecuteRefreshTokenUseCaseThenReturnErrorWhenCacheNotFound()
    {
        // Arrange
        var dto = new RefreshTokenDto("refresh_token");

        _cacheService
            .GetAsync<RefreshTokenCacheResultDto>(Arg.Do<string>(x => x = $"refresh#{dto.RefreshToken}"))
            .Returns(Result.Success<RefreshTokenCacheResultDto>(null!));

        // Act
        var result = await _sut.Execute(dto);

        // Assert
        Assert.False(result.IsSuccess);
        await _cacheService
            .Received()
            .GetAsync<RefreshTokenCacheResultDto>(Arg.Do<string>(x => x = $"refresh#{dto.RefreshToken}"));
        _tokenService
            .DidNotReceive()
            .Generate(default!);
        _tokenService
            .DidNotReceive()
            .GenerateRefresh();
    }

    [Fact]
    public async Task ShouldExecuteRefreshTokenUseCaseThenReturnSuccess()
    {
        // Arrange
        var dto = new RefreshTokenDto("refresh_token");

        var cacheData = new RefreshTokenCacheResultDto(Guid.NewGuid(), "john.doe@email.com");
        _cacheService
            .GetAsync<RefreshTokenCacheResultDto>(Arg.Do<string>(x => x = $"refresh#{dto.RefreshToken}"))
            .Returns(Result.Success(cacheData));

        var user = new UserEntity
        {
            Id = cacheData.Id,
            Email = cacheData.Email,
        };
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

        var response = new RefreshTokenResponse(accessToken, refreshToken, expiresOn, expiresRefreshOn);

        // Act
        var result = await _sut.Execute(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(response.AccessToken, result.Value.AccessToken);
        Assert.Equal(response.RefreshToken, result.Value.RefreshToken);
        Assert.Equal(response.ExpiresOn, result.Value.ExpiresOn, TimeSpan.FromSeconds(5));
        Assert.Equal(response.ExpiresRefreshOn, result.Value.ExpiresRefreshOn, TimeSpan.FromSeconds(5));
        await _cacheService
            .Received()
            .GetAsync<RefreshTokenCacheResultDto>(Arg.Do<string>(x => x = $"refresh#{dto.RefreshToken}"));
        _tokenService
            .Received()
            .Generate(Arg.Do<UserEntity>(x => x = user));
        _tokenService
            .Received()
            .GenerateRefresh();
    }
}