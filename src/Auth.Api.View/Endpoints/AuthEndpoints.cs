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
            try
            {
                await useCase.Execute(dto);

                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        })
            .WithName("Register User")
            .WithDisplayName("Register User")
            .WithTags("Auth")
            .Produces(200)
            .Produces(500);
    }
}