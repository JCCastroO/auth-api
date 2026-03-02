using Auth.Api.Controller.Services.Interfaces;
using System.Security.Cryptography;

namespace Auth.Api.Controller.Services;

public class EncryptPasswordService : IEncryptPasswordService
{
    private const int SALT_SIZE = 16;
    private const int KEY_SIZE = 32;
    private const int ITERATIONS = 100_000;

    public string Encrypt(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SALT_SIZE);

        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, ITERATIONS, HashAlgorithmName.SHA256, KEY_SIZE);

        return $"{Convert.ToBase64String(salt)}#{Convert.ToBase64String(key)}";
    }

    public bool Validate(string password, string hash)
    {
        var parts = hash.Split("#", 2);
        if (parts.Length != 2)
            return false;

        var salt = Convert.FromBase64String(parts[0]);
        var expectedKey = Convert.FromBase64String(parts[1]);

        var actualKey = Rfc2898DeriveBytes.Pbkdf2(password, salt, ITERATIONS, HashAlgorithmName.SHA256, expectedKey.Length);

        return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
    }
}