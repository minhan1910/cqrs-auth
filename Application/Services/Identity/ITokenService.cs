using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Wrappers;

namespace Application.Services.Identity
{
    public interface ITokenService
    {
        Task<IResponseWrapper<TokenResponse>> GetTokenAsync(TokenRequest tokenRequest);
        Task<IResponseWrapper<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
    }
}