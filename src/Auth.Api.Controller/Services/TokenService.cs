using Auth.Api.Controller.Services.Interfaces;
using Auth.Api.Model.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Auth.Api.Controller.Services;

public class TokenService(
    string key,
    string appName,
    int expiresInMinutes,
    int bytesToRandomRefreshToken) : ITokenService
{
    private readonly string _key = key;
    private readonly string _appName = appName;
    private readonly int _expiresInMinutes = expiresInMinutes;
    private readonly int _bytesToRandomRefreshToken = bytesToRandomRefreshToken;

    public (string AccessToken, DateTimeOffset ExpiresOn) Generate(UserEntity user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiresOn = DateTimeOffset.UtcNow.AddMinutes(_expiresInMinutes);
        var token = new JwtSecurityToken(
            issuer: _appName,
            audience: _appName,
            claims: claims,
            expires: expiresOn.Date,
            signingCredentials: creds);

        return (token.EncodedPayload, expiresOn);
    }

    public string GenerateRefresh()
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(_bytesToRandomRefreshToken));
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}