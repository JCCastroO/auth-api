namespace Auth.Api.Controller.Dtos;

public record LoginResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresOn, DateTimeOffset ExpiresRefreshOn);