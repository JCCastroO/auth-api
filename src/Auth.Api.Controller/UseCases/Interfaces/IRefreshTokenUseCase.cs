using Auth.Api.Controller.Requests;
using Auth.Api.Controller.Responses;
using OperationResult;

namespace Auth.Api.Controller.UseCases.Interfaces;

public interface IRefreshTokenUseCase
{
    Task<Result<RefreshTokenResponse>> Execute(RefreshTokenRequest request);
}