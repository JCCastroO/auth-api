using Auth.Api.Share.Requests;
using Auth.Api.Share.Responses;
using OperationResult;

namespace Auth.Api.Controller.UseCases.Interfaces;

public interface ILoginUseCase
{
    Task<Result<LoginResponse>> Execute(LoginRequest request);
}