using System.ComponentModel.DataAnnotations;

namespace JwtApplication.Models
{
    public class UserVM
    {
        public string Name { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
