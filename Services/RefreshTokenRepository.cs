using JwtApplication.Entities;

namespace JwtApplication.Services
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public RefreshToken? GetRefreshTokenByToken(string token)
        {
            return _context.RefreshTokens.FirstOrDefault(
                rt => rt.Token == token
            );
        }

        public RefreshToken? GetRefreshTokenById(int id)
        {
            return _context.RefreshTokens.SingleOrDefault(rt => rt.Id == id);
        }

        public RefreshToken CreateRefreshToken(
            string refreshToken,
            int secondExpire
        )
        {
            var newRefreshToken = new RefreshToken()
            {
                Token = refreshToken,
                CreatedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddSeconds(secondExpire)
            };

            _context.RefreshTokens.Add(newRefreshToken);
            _context.SaveChanges();

            return newRefreshToken;
        }

        public User? GetUserOfARefreshToken(int refreshTokenId)
        {
            return _context.Users.SingleOrDefault(
                u =>
                    (u.CurrentRefreshToken != null)
                    && (u.CurrentRefreshToken.Id == refreshTokenId)
            );
        }
    }
}
