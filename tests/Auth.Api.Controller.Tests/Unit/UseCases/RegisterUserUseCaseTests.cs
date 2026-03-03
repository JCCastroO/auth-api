using Auth.Api.Controller.Dtos;
using Auth.Api.Controller.Services.Interfaces;
using Auth.Api.Controller.UseCases;
using Auth.Api.Controller.UseCases.Interfaces;
using Auth.Api.Model.Entities;
using Auth.Api.Model.Repositories.Interfaces;
using NSubstitute;
using OperationResult;

namespace Auth.Api.Controller.Tests.Unit.UseCases;

public class RegisterUserUseCaseTests
{
    private readonly IUserRepository _repository = Substitute.For<IUserRepository>();
    private readonly IEncryptPasswordService _encryptPasswordService = Substitute.For<IEncryptPasswordService>();
    private readonly IRegisterUserUseCase _sut;

    public RegisterUserUseCaseTests()
        => _sut = new RegisterUserUseCase(_repository, _encryptPasswordService);

    [Fact]
    public async Task ShouldExecuteRegisterUserThenReturnErrorWhenGetByEmailFailed()
    {
        // Arrange
        var dto = new RegisterUserDto("John Doe", "john.doe@email.com", "john@123");

        _repository
            .GetByEmail(Arg.Do<string>(x => x = dto.Email))
            .Returns(Result.Error<UserEntity?>(new Exception("Internal Error")));

        // Act
        var result = await _sut.Execute(dto);

        // Assert
        Assert.False(result.IsSuccess);
        await _repository
            .Received()
            .GetByEmail(Arg.Do<string>(x => x = dto.Email));
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
        var dto = new RegisterUserDto("John Doe", "john.doe@email.com", "john@123");

        var user = new UserEntity()
        {
            Name = dto.Name,
            Email = dto.Email,
            Password = dto.Password
        };

        _repository
            .GetByEmail(Arg.Do<string>(x => x = dto.Email))
            .Returns(Result.Success<UserEntity?>(user));

        // Act
        var result = await _sut.Execute(dto);

        // Assert
        Assert.False(result.IsSuccess);
        await _repository
            .Received()
            .GetByEmail(Arg.Do<string>(x => x = dto.Email));
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
        var dto = new RegisterUserDto("John Doe", "john.doe@email.com", "john@123");

        var user = new UserEntity()
        {
            Name = dto.Name,
            Email = dto.Email,
            Password = _encryptPasswordService.Encrypt(dto.Password)
        };

        _repository
            .GetByEmail(Arg.Do<string>(x => x = dto.Email))
            .Returns(Result.Success<UserEntity?>(null));
        _repository
            .Insert(Arg.Do<UserEntity>(x => x = user))
            .Returns(Result.Error(new Exception("Internal Error")));

        // Act
        var result = await _sut.Execute(dto);

        // Assert
        Assert.False(result.IsSuccess);
        await _repository
            .Received()
            .GetByEmail(Arg.Do<string>(x => x = dto.Email));
        await _repository
            .Received()
            .Insert(Arg.Do<UserEntity>(x => x = user));
        _encryptPasswordService
            .Received()
            .Encrypt(dto.Password);
    }

    [Fact]
    public async Task ShouldExecuteRegisterUserThenReturnSucessWhenInsertUser()
    {
        // Arrange
        var dto = new RegisterUserDto("John Doe", "john.doe@email.com", "john@123");

        var user = new UserEntity()
        {
            Name = dto.Name,
            Email = dto.Email,
            Password = "encrypted_john@123"
        };

        _repository
            .GetByEmail(Arg.Do<string>(x => x = dto.Email))
            .Returns(Result.Success<UserEntity?>(null));
        _repository
            .Insert(Arg.Do<UserEntity>(x => x = user))
            .Returns(Result.Success());

        _encryptPasswordService
            .Encrypt(Arg.Do<string>(x => x = dto.Password))
            .Returns(user.Password);

        // Act
        var result = await _sut.Execute(dto);

        // Assert
        Assert.True(result.IsSuccess);
        await _repository
            .Received()
            .GetByEmail(Arg.Do<string>(x => x = dto.Email));
        await _repository
            .Received()
            .Insert(Arg.Do<UserEntity>(x => x = user));
        _encryptPasswordService
            .Received()
            .Encrypt(Arg.Do<string>(x => x = dto.Password));
    }
}