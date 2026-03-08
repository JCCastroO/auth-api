using Auth.Api.Controller.Services;
using Auth.Api.Controller.Services.Interfaces;
using Auth.Api.Controller.UseCases;
using Auth.Api.Controller.UseCases.Interfaces;
using Auth.Api.Model.Repositories;
using Auth.Api.Model.Repositories.Interfaces;
using Auth.Api.Model.Services;
using Auth.Api.Model.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using StackExchange.Redis;
using System.Data;

namespace Auth.Api.View.Tests.Integrated;

public class DependencyInjectionTests : IClassFixture<PostgreSqlFixture>, IClassFixture<RedisFixture>
{
    private readonly AppTestContainer _factory;
    private readonly IServiceProvider _provider;

    public DependencyInjectionTests(PostgreSqlFixture dbFixture, RedisFixture redisFixture)
    {
        _factory = new(dbFixture, redisFixture);
        _provider = _factory.Services;
    }

    [Fact]
    public void ShouldInjectDapperAsADependency()
    {
        // Arrange

        // Act
        var service = _provider.GetRequiredService<IDbConnection>();

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void ShouldInjectRedisAsADependency()
    {
        // Arrange

        // Act
        var service = _provider.GetRequiredService<IConnectionMultiplexer>();

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void ShouldInjectUserRepositoryAsADependency()
    {
        // Arrange

        // Act
        var service = _provider.GetRequiredService<IUserRepository>();

        // Assert
        Assert.NotNull(service);
        Assert.IsType<UserRepository>(service);
    }

    [Fact]
    public void ShouldInjectCacheServiceAsADependency()
    {
        // Arrange

        // Act
        var service = _provider.GetRequiredService<ICacheService>();

        // Assert
        Assert.NotNull(service);
        Assert.IsType<CacheService>(service);
    }

    [Fact]
    public void ShouldInjectRegisterUserUseCaseAsADependency()
    {
        // Arrange

        // Act
        var service = _provider.GetRequiredService<IRegisterUserUseCase>();

        // Assert
        Assert.NotNull(service);
        Assert.IsType<RegisterUserUseCase>(service);
    }

    [Fact]
    public void ShouldInjectLoginUseCaseAsADependency()
    {
        // Arrange

        // Act
        var service = _provider.GetRequiredService<ILoginUseCase>();

        // Assert
        Assert.NotNull(service);
        Assert.IsType<LoginUseCase>(service);
    }

    [Fact]
    public void ShouldInjectRefreshTokenUseCaseAsADependency()
    {
        // Arrange

        // Act
        var service = _provider.GetRequiredService<IRefreshTokenUseCase>();

        // Assert
        Assert.NotNull(service);
        Assert.IsType<RefreshTokenUseCase>(service);
    }

    [Fact]
    public void ShouldInjectEncryptPasswordServiceAsADependency()
    {
        // Arrange

        // Act
        var service = _provider.GetRequiredService<IEncryptPasswordService>();

        // Assert
        Assert.NotNull(service);
        Assert.IsType<EncryptPasswordService>(service);
    }

    [Fact]
    public void ShouldInjectTokenServiceAsADependency()
    {
        // Arrange

        // Act
        var service = _provider.GetRequiredService<ITokenService>();

        // Assert
        Assert.NotNull(service);
        Assert.IsType<TokenService>(service);
    }
}