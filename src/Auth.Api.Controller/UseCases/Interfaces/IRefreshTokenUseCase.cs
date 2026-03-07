using Auth.Api.Controller.Dtos;
using OperationResult;

namespace Auth.Api.Controller.UseCases.Interfaces;

public interface IRefreshTokenUseCase
{
    Task<Result<RefreshTokenResponse>> Execute(RefreshTokenDto dto);
}