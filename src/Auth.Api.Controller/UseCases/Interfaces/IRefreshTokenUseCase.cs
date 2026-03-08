using Auth.Api.Share.Requests;
using Auth.Api.Share.Responses;
using OperationResult;

namespace Auth.Api.Controller.UseCases.Interfaces;

public interface IRefreshTokenUseCase
{
    Task<Result<RefreshTokenResponse>> Execute(RefreshTokenRequest request);
}