using Auth.Api.Controller.Services;
using Auth.Api.Controller.Services.Interfaces;
using Auth.Api.Controller.UseCases;
using Auth.Api.Controller.UseCases.Interfaces;
using Auth.Api.Model.Repositories;
using Auth.Api.Model.Repositories.Interfaces;
using Auth.Api.Model.Services;
using Auth.Api.Model.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
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
        var repository = new UserRepository(default!);

        // Act
        var service = _provider.GetRequiredService<IUserRepository>();

        // Assert
        Assert.NotNull(service);
        Assert.Equivalent(repository, service);
    }

    [Fact]
    public void ShouldInjectCacheServiceAsADependency()
    {
        // Arrange
        var connection = _provider.GetRequiredService<IConnectionMultiplexer>();
        var expectedService = new CacheService(connection);

        // Act
        var service = _provider.GetRequiredService<ICacheService>();

        // Assert
        Assert.NotNull(service);
        Assert.Equivalent(expectedService, service);
    }

    [Fact]
    public void ShouldInjectRegisterUserUseCaseAsADependency()
    {
        // Arrange
        var useCase = new RegisterUserUseCase(default!, default!, default!);

        // Act
        var service = _provider.GetRequiredService<IRegisterUserUseCase>();

        // Assert
        Assert.NotNull(service);
        Assert.Equivalent(useCase, service);
    }

    [Fact]
    public void ShouldInjectLoginUseCaseAsADependency()
    {
        // Arrange
        var useCase = new LoginUseCase(default!, default!, default!, default!, default!);

        // Act
        var service = _provider.GetRequiredService<ILoginUseCase>();

        // Assert
        Assert.NotNull(service);
        Assert.Equivalent(useCase, service);
    }

    [Fact]
    public void ShouldInjectRefreshTokenUseCaseAsADependency()
    {
        // Arrange
        var useCase = new RefreshTokenUseCase(default!, default!, default!);

        // Act
        var service = _provider.GetRequiredService<IRefreshTokenUseCase>();

        // Assert
        Assert.NotNull(service);
        Assert.Equivalent(useCase, service);
    }

    [Fact]
    public void ShouldInjectEncryptPasswordServiceAsADependency()
    {
        // Arrange
        var expectedService = new EncryptPasswordService(default, default, default, default, default);

        // Act
        var service = _provider.GetRequiredService<IEncryptPasswordService>();

        // Assert
        Assert.NotNull(service);
        Assert.Equivalent(expectedService, service);
    }

    [Fact]
    public void ShouldInjectTokenServiceAsADependency()
    {
        // Arrange
        var expectedService = new TokenService(default!, default!, default, default);

        // Act
        var service = _provider.GetRequiredService<ITokenService>();

        // Assert
        Assert.NotNull(service);
        Assert.Equivalent(expectedService, service);
    }
}