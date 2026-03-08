using Auth.Api.Share.Requests;
using OperationResult;

namespace Auth.Api.Controller.UseCases.Interfaces;

public interface IRegisterUserUseCase
{
    Task<Result> Execute(RegisterUserRequest request);
}