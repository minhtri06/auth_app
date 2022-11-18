using JwtApplication.Entities;

namespace JwtApplication.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool CheckUserNameExists(string userName)
        {
            return _context.Users.Any(u => u.UserName == userName);
        }

        public User? GetUserByUserName(string userName)
        {
            return _context.Users.SingleOrDefault(u => u.UserName == userName);
        }

        public User? GetUserById(int id)
        {
            return _context.Users.SingleOrDefault(u => u.Id == id);
        }

        public ICollection<User> GetAllUser()
        {
            return _context.Users.ToList();
        }

        public void UpdateCurrentRefreshToken(
            int userId,
            RefreshToken refreshToken
        )
        {
            var user = _context.Users.SingleOrDefault(u => u.Id == userId);

            if (user is not null)
            {
                user.CurrentRefreshToken = refreshToken;
                _context.Users.Update(user);
                _context.SaveChanges();
            }
        }
    }
}
