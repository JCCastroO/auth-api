using Auth.Api.Controller.Services.Interfaces;
using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace Auth.Api.Controller.Services;

public class EncryptPasswordService(
    int saltSize,
    int threadsUsed,
    int iterations,
    int memoryUsed,
    int hashSize) : IEncryptPasswordService
{
    private readonly int _saltSize = saltSize;
    private readonly int _threadsUsed = threadsUsed;
    private readonly int _iterations = iterations;
    private readonly int _memoryUsed = memoryUsed;
    private readonly int _hashSize = hashSize;

    public string Encrypt(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(_saltSize);

        var hash = GenerateHash(password, salt);

        return $"{Convert.ToBase64String(salt)}#{Convert.ToBase64String(hash)}";
    }

    public bool Validate(string password, string hash)
    {
        var parts = hash.Split("#", 2);
        if (parts.Length != 2)
            return false;

        var salt = Convert.FromBase64String(parts[0]);
        var expectedHash = Convert.FromBase64String(parts[1]);

        var actualHash = GenerateHash(password, salt);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }

    private byte[] GenerateHash(string value, byte[] salt)
    {
        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(value))
        {
            Salt = salt,
            DegreeOfParallelism = _threadsUsed,
            Iterations = _iterations,
            MemorySize = _memoryUsed
        };

        return argon2.GetBytes(_hashSize);
    }
}