using JwtApplication.Entities;

namespace JwtApplication.Services
{
    public interface IRefreshTokenRepository
    {
        RefreshToken? GetRefreshTokenByToken(string token);
        RefreshToken? GetRefreshTokenById(int id);
        RefreshToken CreateRefreshToken(string refreshToken, int secondExpire);
        User? GetUserOfARefreshToken(int refreshTokenId);
    }
}
