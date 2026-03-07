using Auth.Api.Controller.Requests;
using Auth.Api.Controller.Services.Interfaces;
using Auth.Api.Controller.UseCases.Interfaces;
using Auth.Api.Model.Entities;
using Auth.Api.Model.Repositories.Interfaces;
using OperationResult;

namespace Auth.Api.Controller.UseCases;

public class RegisterUserUseCase(IUserRepository repository, IEncryptPasswordService encryptPasswordService) : IRegisterUserUseCase
{
    private readonly IUserRepository _repository = repository;
    private readonly IEncryptPasswordService _encryptPasswordService = encryptPasswordService;

    public async Task<Result> Execute(RegisterUserRequest userDto)
    {
        var (existingUserSuccess, existingUser, existingUserError) = await _repository.GetByEmail(userDto.Email);
        if (!existingUserSuccess && existingUserError is not null)
            return Result.Error(new Exception("Internal Error"));

        if (existingUser is not null)
            return Result.Error(new Exception("User Already Exists"));

        var user = new UserEntity()
        {
            Name = userDto.Name,
            Email = userDto.Email,
            Password = _encryptPasswordService.Encrypt(userDto.Password),
        };

        var (insertSuccess, insertError) = await _repository.Insert(user);
        if (!insertSuccess && insertError is not null)
            return Result.Error(new Exception("Internal Error"));

        return Result.Success();
    }
}