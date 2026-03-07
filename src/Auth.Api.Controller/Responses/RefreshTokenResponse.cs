namespace Auth.Api.Controller.Responses;

public record RefreshTokenResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresOn, DateTimeOffset ExpiresRefreshOn);