using Auth.Api.Controller.Dtos;
using OperationResult;

namespace Auth.Api.Controller.UseCases.Interfaces;

public interface IRegisterUserUseCase
{
    Task<Result> Execute(RegisterUserDto userDto);
}