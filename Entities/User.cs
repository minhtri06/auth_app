using System.ComponentModel.DataAnnotations;

namespace JwtApplication.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string PasswordSalt { get; set; } = null!;
        public string Role { get; set; } = null!;
        public RefreshToken? CurrentRefreshToken { get; set; } = null;
    }
}
