using Auth.Api.Controller.Services;
using Auth.Api.Controller.Services.Interfaces;
using Auth.Api.Controller.UseCases;
using Auth.Api.Controller.UseCases.Interfaces;
using Auth.Api.Model.Repositories;
using Auth.Api.Model.Repositories.Interfaces;
using Auth.Api.Model.Services;
using Auth.Api.Model.Services.Interfaces;
using Npgsql;
using System.Data;

namespace Auth.Api.View.Extensions;

public static class ServiceCollectionExtension
{
    public static void ConfigureServices(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddEndpointsApiExplorer();
        service.AddSwaggerGen();

        service.ConfigureAppDependencies(configuration);
    }

    private static void ConfigureAppDependencies(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddSingleton<IDbConnection>(sp =>
        {
            var connectionString = configuration.GetConnectionString("Database");
            return new NpgsqlConnection(connectionString);
        });

        service.AddScoped<IUserRepository, UserRepository>();
        service.AddScoped<ICacheService, CacheService>();

        service.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
        service.AddScoped<ILoginUseCase, LoginUseCase>();
        service.AddScoped<IRefreshTokenUseCase, RefreshTokenUseCase>();

        service.AddScoped<IEncryptPasswordService>(_ =>
        {
            var saltSize = int.Parse(configuration["EncryptPasswordService:SaltSize"]!);
            var threadsUsed = int.Parse(configuration["EncryptPasswordService:ThreadsUsed"]!);
            var iterations = int.Parse(configuration["EncryptPasswordService:Iterations"]!);
            var memoryUsed = int.Parse(configuration["EncryptPasswordService:MemoryUsed"]!);
            var hashSize = int.Parse(configuration["EncryptPasswordService:HashSize"]!);

            return new EncryptPasswordService(saltSize, threadsUsed, iterations, memoryUsed, hashSize);
        });
        service.AddScoped<ITokenService>(_ =>
        {
            var key = configuration["TokenService:AccessToken:Key"]!;
            var appName = configuration["AppName"]!;
            var expiresInMinutes = int.Parse(configuration["TokenService:AccessToken:ExpiresInMinutes"]!);
            var bytesToRandomRefreshToken = int.Parse(configuration["TokenService:RefreshToken:BytesToRandomRefreshToken"]!);

            return new TokenService(key, appName, expiresInMinutes, bytesToRandomRefreshToken);
        });
    }
}