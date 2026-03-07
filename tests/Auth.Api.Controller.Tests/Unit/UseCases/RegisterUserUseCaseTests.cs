using Auth.Api.Controller.Requests;
using Auth.Api.Controller.Services.Interfaces;
using Auth.Api.Controller.UseCases;
using Auth.Api.Controller.UseCases.Interfaces;
using Auth.Api.Model.Entities;
using Auth.Api.Model.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OperationResult;

namespace Auth.Api.Controller.Tests.Unit.UseCases;

public class RegisterUserUseCaseTests
{
    private readonly ILogger<RegisterUserUseCase> _logger = Substitute.For<ILogger<RegisterUserUseCase>>();
    private readonly IUserRepository _repository = Substitute.For<IUserRepository>();
    private readonly IEncryptPasswordService _encryptPasswordService = Substitute.For<IEncryptPasswordService>();
    private readonly IRegisterUserUseCase _sut;

    public RegisterUserUseCaseTests()
        => _sut = new RegisterUserUseCase(_logger, _repository, _encryptPasswordService);

    [Fact]
    public async Task ShouldExecuteRegisterUserThenReturnErrorWhenGetByEmailFailed()
    {
        // Arrange
        var request = new RegisterUserRequest("John Doe", "john.doe@email.com", "john@123");

        _repository
            .GetByEmail(Arg.Do<string>(x => x = request.Email))
            .Returns(Result.Error<UserEntity?>(new Exception("Internal Error")));

        // Act
        var result = await _sut.Execute(request);

        // Assert
        Assert.False(result.IsSuccess);
        await _repository
            .Received()
            .GetByEmail(Arg.Do<string>(x => x = request.Email));
        await _repository
            .DidNotReceive()
            .Insert(default!);
        _encryptPasswordService
            .DidNotReceive()
            .Encrypt(default!);
    }

    [Fact]
    public async Task ShouldExecuteRegisterUserThenReturnErrorWhenEmailAlreadyExists()
    {
        // Arrange
        var request = new RegisterUserRequest("John Doe", "john.doe@email.com", "john@123");

        var user = new UserEntity()
        {
            Name = request.Name,
            Email = request.Email,
            Password = request.Password
        };

        _repository
            .GetByEmail(Arg.Do<string>(x => x = request.Email))
            .Returns(Result.Success<UserEntity?>(user));

        // Act
        var result = await _sut.Execute(request);

        // Assert
        Assert.False(result.IsSuccess);
        await _repository
            .Received()
            .GetByEmail(Arg.Do<string>(x => x = request.Email));
        await _repository
            .DidNotReceive()
            .Insert(default!);
        _encryptPasswordService
            .DidNotReceive()
            .Encrypt(default!);
    }

    [Fact]
    public async Task ShouldExecuteRegisterUserThenReturnErrorWhenInsertUserFailed()
    {
        // Arrange
        var request = new RegisterUserRequest("John Doe", "john.doe@email.com", "john@123");

        var user = new UserEntity()
        {
            Name = request.Name,
            Email = request.Email,
            Password = _encryptPasswordService.Encrypt(request.Password)
        };

        _repository
            .GetByEmail(Arg.Do<string>(x => x = request.Email))
            .Returns(Result.Success<UserEntity?>(null));
        _repository
            .Insert(Arg.Do<UserEntity>(x => x = user))
            .Returns(Result.Error(new Exception("Internal Error")));

        // Act
        var result = await _sut.Execute(request);

        // Assert
        Assert.False(result.IsSuccess);
        await _repository
            .Received()
            .GetByEmail(Arg.Do<string>(x => x = request.Email));
        await _repository
            .Received()
            .Insert(Arg.Do<UserEntity>(x => x = user));
        _encryptPasswordService
            .Received()
            .Encrypt(request.Password);
    }

    [Fact]
    public async Task ShouldExecuteRegisterUserThenReturnSucessWhenInsertUser()
    {
        // Arrange
        var request = new RegisterUserRequest("John Doe", "john.doe@email.com", "john@123");

        var user = new UserEntity()
        {
            Name = request.Name,
            Email = request.Email,
            Password = "encrypted_john@123"
        };

        _repository
            .GetByEmail(Arg.Do<string>(x => x = request.Email))
            .Returns(Result.Success<UserEntity?>(null));
        _repository
            .Insert(Arg.Do<UserEntity>(x => x = user))
            .Returns(Result.Success());

        _encryptPasswordService
            .Encrypt(Arg.Do<string>(x => x = request.Password))
            .Returns(user.Password);

        // Act
        var result = await _sut.Execute(request);

        // Assert
        Assert.True(result.IsSuccess);
        await _repository
            .Received()
            .GetByEmail(Arg.Do<string>(x => x = request.Email));
        await _repository
            .Received()
            .Insert(Arg.Do<UserEntity>(x => x = user));
        _encryptPasswordService
            .Received()
            .Encrypt(Arg.Do<string>(x => x = request.Password));
    }
}