using Auth.Api.Controller.Services;
using Auth.Api.Controller.Services.Interfaces;
using Auth.Api.Controller.UseCases;
using Auth.Api.Controller.UseCases.Interfaces;
using Auth.Api.Model.Repositories;
using Auth.Api.Model.Repositories.Interfaces;
using Auth.Api.View.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace Auth.Api.View.Tests.Integrated;

public class DependencyInjectionTests
{
    private readonly IServiceCollection _service = new ServiceCollection();
    private readonly IServiceProvider _provider;

    public DependencyInjectionTests()
    {
        _service.ConfigureServices(default!);

        _provider = _service.BuildServiceProvider();
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
    public void ShouldInjectRegisterUserUseCaseAsADependency()
    {
        // Arrange
        var useCase = new RegisterUserUseCase(default!, default!);

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
        var useCase = new LoginUseCase(default!, default!, default!);

        // Act
        var service = _provider.GetRequiredService<ILoginUseCase>();

        // Assert
        Assert.NotNull(service);
        Assert.Equivalent(useCase, service);
    }

    [Fact]
    public void ShouldInjectEncryptPasswordServiceAsADependency()
    {
        // Arrange
        var expectedService = new EncryptPasswordService();

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
        var expectedService = new TokenService();

        // Act
        var service = _provider.GetRequiredService<ITokenService>();

        // Assert
        Assert.NotNull(service);
        Assert.Equivalent(expectedService, service);
    }
}