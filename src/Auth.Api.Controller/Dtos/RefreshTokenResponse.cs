namespace Auth.Api.Controller.Dtos;

public record RefreshTokenResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresOn, DateTimeOffset ExpiresRefreshOn);