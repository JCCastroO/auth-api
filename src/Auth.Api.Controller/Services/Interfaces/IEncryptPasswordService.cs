namespace Auth.Api.Controller.Services.Interfaces;

public interface IEncryptPasswordService
{
    string Encrypt(string password);

    bool Validate(string password, string hash);
}