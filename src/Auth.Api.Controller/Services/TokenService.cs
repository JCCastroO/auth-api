using Auth.Api.Controller.Services.Interfaces;
using Auth.Api.Model.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.Api.Controller.Services;

public class TokenService : ITokenService
{
    private const string KEY = "5a3c0872f5889fdee74c1f65a1244ac17eb7f820";
    private const string ISSUER = "Authentication.Api";
    private const string AUDIENCE = "Authentication.Api";
    private const int EXPIRES_IN_MINUTES = 60;

    public (string AccessToken, DateTimeOffset ExpiresOn) Generate(UserEntity user)
    {
        var expiresOn = DateTimeOffset.UtcNow.AddMinutes(EXPIRES_IN_MINUTES);
        var token = GenerateToken(user, expiresOn.Date);

        return (token.EncodedPayload, expiresOn);
    }

    public (string RefreshToken, DateTimeOffset ExpiresRefreshOn) GenerateRefresh(UserEntity user)
    {
        var expiresOn = DateTimeOffset.UtcNow.AddMinutes(EXPIRES_IN_MINUTES * 3);
        var token = GenerateToken(user, expiresOn.Date);

        return (token.EncodedPayload, expiresOn);
    }

    private JwtSecurityToken GenerateToken(UserEntity user, DateTime expiresOn)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        return new(
            issuer: ISSUER,
            audience: AUDIENCE,
            claims: claims,
            expires: expiresOn,
            signingCredentials: creds);
    }
}