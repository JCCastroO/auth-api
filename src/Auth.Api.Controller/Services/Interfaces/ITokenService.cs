using Auth.Api.Model.Entities;

namespace Auth.Api.Controller.Services.Interfaces;

public interface ITokenService
{
    (string AccessToken, DateTimeOffset ExpiresOn) Generate(UserEntity user);

    string GenerateRefresh();
}