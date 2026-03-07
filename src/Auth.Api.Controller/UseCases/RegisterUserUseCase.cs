using Auth.Api.Controller.Requests;
using Auth.Api.Controller.Responses;
using Auth.Api.Controller.Services.Interfaces;
using Auth.Api.Controller.UseCases.Interfaces;
using Auth.Api.Model.Entities;
using Auth.Api.Model.Repositories.Interfaces;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace Auth.Api.Controller.UseCases;

public class RegisterUserUseCase(
    ILogger<RegisterUserUseCase> logger,
    IUserRepository repository,
    IEncryptPasswordService encryptPasswordService) : IRegisterUserUseCase
{
    private readonly ILogger<RegisterUserUseCase> _logger = logger;
    private readonly IUserRepository _repository = repository;
    private readonly IEncryptPasswordService _encryptPasswordService = encryptPasswordService;

    public async Task<Result> Execute(RegisterUserRequest request)
    {
        _logger.LogInformation("Initializing Register. Email: {Email}", request.Email);

        var (existingUserSuccess, existingUser, existingUserError) = await _repository.GetByEmail(request.Email);
        if (!existingUserSuccess && existingUserError is not null)
        {
            _logger.LogError(existingUserError, "An unexpected error occurred on database search for user. Email: {Email}", request.Email);
            return Result.Error(new SystemException("Internal Error"));
        }

        if (existingUser is not null)
        {
            _logger.LogWarning("User Already Exists. Email: {Email}", request.Email);
            return Result.Error(new ApplicationException("User Already Exists"));
        }

        var user = new UserEntity()
        {
            Name = request.Name,
            Email = request.Email,
            Password = _encryptPasswordService.Encrypt(request.Password),
        };

        var (insertSuccess, insertError) = await _repository.Insert(user);
        if (!insertSuccess && insertError is not null)
        {
            _logger.LogError(insertError, "An unexpected error occurred on insert user in database. Email: {Email}", request.Email);
            return Result.Error(new SystemException("Internal Error"));
        }

        _logger.LogInformation("Finalizing Register Successfully. Email: {Email}", request.Email);
        return Result.Success();
    }
}