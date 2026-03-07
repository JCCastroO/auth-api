using Auth.Api.Controller.Services.Interfaces;
using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace Auth.Api.Controller.Services;

public class EncryptPasswordService : IEncryptPasswordService
{
    private const int SALT_SIZE = 16;
    private const int THREADS_USED = 8;
    private const int ITERATIONS = 4;
    private const int MEMORY_USED = 1024 * 64;
    private const int HASH_SIZE = 32;

    public string Encrypt(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SALT_SIZE);

        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = THREADS_USED,
            Iterations = ITERATIONS,
            MemorySize = MEMORY_USED
        };

        var hash = argon2.GetBytes(HASH_SIZE);

        return $"{Convert.ToBase64String(salt)}#{Convert.ToBase64String(hash)}";
    }

    public bool Validate(string password, string hash)
    {
        var parts = hash.Split("#", 2);
        if (parts.Length != 2)
            return false;

        var salt = Convert.FromBase64String(parts[0]);
        var expectedHash = Convert.FromBase64String(parts[1]);

        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = THREADS_USED,
            Iterations = ITERATIONS,
            MemorySize = MEMORY_USED
        };

        var actualHash = argon2.GetBytes(HASH_SIZE);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}