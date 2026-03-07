using Auth.Api.Controller.Requests;
using OperationResult;

namespace Auth.Api.Controller.UseCases.Interfaces;

public interface IRegisterUserUseCase
{
    Task<Result> Execute(RegisterUserRequest request);
}