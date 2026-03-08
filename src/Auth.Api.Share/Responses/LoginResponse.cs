namespace Auth.Api.Share.Responses;

public record LoginResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresOn, DateTimeOffset ExpiresRefreshOn);