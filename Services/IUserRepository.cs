using JwtApplication.Entities;
using JwtApplication.Models;

namespace JwtApplication.Services
{
    public interface IUserRepository
    {
        bool CheckUserNameExists(string userName);
        User? GetUserByUserName(string userName);
        User? GetUserById(int id);
        ICollection<User> GetAllUser();
        void UpdateCurrentRefreshToken(int userId, RefreshToken refreshToken);
    }
}
