using Auth.Api.Controller.Requests;
using Auth.Api.Controller.Responses;
using OperationResult;

namespace Auth.Api.Controller.UseCases.Interfaces;

public interface ILoginUseCase
{
    Task<Result<LoginResponse>> Execute(LoginRequest dto);
}