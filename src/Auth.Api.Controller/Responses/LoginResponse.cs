namespace Auth.Api.Controller.Responses;

public record LoginResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresOn, DateTimeOffset ExpiresRefreshOn);