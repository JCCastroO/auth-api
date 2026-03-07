namespace Auth.Api.Controller.Requests;

public record RegisterUserRequest(string Name, string Email, string Password);