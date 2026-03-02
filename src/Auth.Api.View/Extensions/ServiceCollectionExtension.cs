using Auth.Api.Controller.Services;
using Auth.Api.Controller.Services.Interfaces;
using Auth.Api.Controller.UseCases;
using Auth.Api.Controller.UseCases.Interfaces;
using Auth.Api.Model.Repositories;
using Auth.Api.Model.Repositories.Interfaces;
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
        service.AddScoped<IDbConnection>(sp =>
        {
            var connectionString = configuration.GetConnectionString("Database");
            return new NpgsqlConnection(connectionString);
        });

        service.AddScoped<IUserRepository, UserRepository>();

        service.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();

        service.AddScoped<IEncryptPasswordService, EncryptPasswordService>();
    }
}