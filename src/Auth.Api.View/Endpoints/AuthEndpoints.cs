using Auth.Api.Controller.UseCases.Interfaces;
using Auth.Api.Share.Requests;
using Auth.Api.Share.Responses;
using Auth.Api.View.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.View.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder router)
    {
        router.MapPost("api/v1/auth/register", static async ([FromServices] IRegisterUserUseCase useCase, [FromBody] RegisterUserRequest request) =>
        {
            var (success, error) = await useCase.Execute(request);
            if (!success && error is not null)
                return ExceptionHandler.Error(error);

            return Results.Ok();
        })
            .WithName("Register User")
            .WithDisplayName("Register User")
            .WithTags("Auth")
            .WithDescription("This endpoint is a request for register a new user.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status422UnprocessableEntity, typeof(ApplicationException))
            .Produces(StatusCodes.Status500InternalServerError, typeof(SystemException));

        router.MapPost("api/v1/auth/login", static async ([FromServices] ILoginUseCase useCase, [FromBody] LoginRequest request) =>
        {
            var (success, result, error) = await useCase.Execute(request);
            if (!success && error is not null)
                return ExceptionHandler.Error(error);

            return Results.Ok(result);
        })
            .WithName("Login User")
            .WithDisplayName("Login User")
            .WithTags("Auth")
            .WithDescription("This endpoint is a request for get an access token linked to the user.")
            .Produces(StatusCodes.Status200OK, typeof(LoginResponse))
            .Produces(StatusCodes.Status401Unauthorized, typeof(UnauthorizedAccessException))
            .Produces(StatusCodes.Status500InternalServerError, typeof(SystemException));

        router.MapPost("api/v1/auth/refresh_token", static async ([FromServices] IRefreshTokenUseCase useCase, [FromBody] RefreshTokenRequest request) =>
        {
            var (success, result, error) = await useCase.Execute(request);
            if (!success && error is not null)
                return ExceptionHandler.Error(error);

            return Results.Ok(result);
        })
            .WithName("Refresh User Access")
            .WithDisplayName("Refresh User Access")
            .WithTags("Auth")
            .WithDescription("This endpoint is a request for refresh access token linked to the user.")
            .Produces(StatusCodes.Status200OK, typeof(RefreshTokenResponse))
            .Produces(StatusCodes.Status401Unauthorized, typeof(UnauthorizedAccessException))
            .Produces(StatusCodes.Status500InternalServerError, typeof(SystemException));
    }
}