using JwtApplication.Entities;
using JwtApplication.Models;

namespace JwtApplication.Services
{
    public interface IAuthRepository
    {
        byte[] GetSecretKeyBytes();
        string GenerateSalt(int length);
        string HashPassword(string password, string salt);

        User? RegisterUser(UserModel userModel);

        bool VerifyPasswordHash(
            string password,
            string passwordHash,
            string salt
        );

        User? VerifyLogin(LoginModel loginModel);

        string GenerateAccessToken(User user);
        string GenerateRefreshToken(int length);
    }
}
