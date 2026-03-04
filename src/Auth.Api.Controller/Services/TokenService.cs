using Auth.Api.Controller.Services.Interfaces;
using Auth.Api.Model.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Auth.Api.Controller.Services;

public class TokenService : ITokenService
{
    private const string KEY = "5a3c0872f5889fdee74c1f65a1244ac17eb7f820";
    private const string ISSUER = "Authentication.Api";
    private const string AUDIENCE = "Authentication.Api";
    private const int EXPIRES_IN_MINUTES = 60;
    private const int BYTES_TO_RANDOM_REFRESH_TOKEN = 64;

    public (string AccessToken, DateTimeOffset ExpiresOn) Generate(UserEntity user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiresOn = DateTimeOffset.UtcNow.AddMinutes(EXPIRES_IN_MINUTES);
        var token = new JwtSecurityToken(
            issuer: ISSUER,
            audience: AUDIENCE,
            claims: claims,
            expires: expiresOn.Date,
            signingCredentials: creds);

        return (token.EncodedPayload, expiresOn);
    }

    public string GenerateRefresh()
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(BYTES_TO_RANDOM_REFRESH_TOKEN));
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}