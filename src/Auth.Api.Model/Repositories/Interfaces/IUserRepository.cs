using Auth.Api.Model.Entities;

namespace Auth.Api.Model.Repositories.Interfaces;

public interface IUserRepository
{
    Task Insert(UserEntity user);
}