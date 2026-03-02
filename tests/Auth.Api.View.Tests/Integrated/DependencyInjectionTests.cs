using Auth.Api.View.Extensions;
using Microsoft.Extensions.Configuration;
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
    public void ShouldConfigureDapperDependency()
    {
        // Arrange

        // Act
        var service = _provider.GetRequiredService<IDbConnection>();

        // Assert
        Assert.NotNull(service);
    }
}