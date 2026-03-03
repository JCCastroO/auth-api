using Auth.Api.Controller.Dtos;
using OperationResult;

namespace Auth.Api.Controller.UseCases.Interfaces;

public interface ILoginUseCase
{
    Task<Result<LoginResponse>> Execute(LoginDto dto);
}