using Auth.Api.Model.Entities;
using OperationResult;

namespace Auth.Api.Model.Repositories.Interfaces;

public interface IUserRepository
{
    Task<Result> Insert(UserEntity user);

    Task<Result<UserEntity?>> GetByEmail(string email);
}