using Auth.Api.Controller.UseCases.Interfaces;
using Auth.Api.Controller.Dtos;
using Microsoft.AspNetCore.Mvc;
using OperationResult;

namespace Auth.Api.View.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder router)
    {
        router.MapPost("api/v1/auth/register", static async ([FromServices] IRegisterUserUseCase useCase, [FromBody] RegisterUserDto dto) =>
        {
            var (success, error) = await useCase.Execute(dto);
            if (!success && error is not null)
                return Results.InternalServerError(error);

            return Results.Ok();
        })
            .WithName("Register User")
            .WithDisplayName("Register User")
            .WithTags("Auth")
            .WithDescription("This endpoint is a request for register a new user.")
            .Produces(200)
            .Produces(500);

        router.MapPost("api/v1/auth/login", static async ([FromServices] ILoginUseCase useCase, [FromBody] LoginDto dto) =>
        {
            var (success, result, error) = await useCase.Execute(dto);
            if (!success && error is not null)
                return Results.InternalServerError(error);

            return Results.Ok(result);
        })
            .WithName("Login User")
            .WithDisplayName("Login User")
            .WithTags("Auth")
            .WithDescription("This endpoint is a request for get an access token linked to the user.")
            .Produces<LoginResponse>(200)
            .Produces(500);
    }
}