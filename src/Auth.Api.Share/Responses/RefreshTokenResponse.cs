namespace Auth.Api.Share.Responses;

public record RefreshTokenResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresOn, DateTimeOffset ExpiresRefreshOn);