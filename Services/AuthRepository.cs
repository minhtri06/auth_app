using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using JwtApplication.Entities;
using JwtApplication.Models;
using Microsoft.IdentityModel.Tokens;

namespace JwtApplication.Services
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly AppDbContext _context;
        private readonly int _saltLength = 32;
        private readonly string _secretKey;
        private readonly int _refreshTokenExpiredDuration = 60; // second

        // CTOR
        public AuthRepository(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            AppDbContext context,
            IConfiguration configuration
        )
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _context = context;

            var secretKey = configuration["AppSettings:SecretKey"];
            _secretKey = secretKey is not null ? secretKey : "";
        }

        // METHODS
        public byte[] GetSecretKeyBytes()
        {
            return Encoding.UTF8.GetBytes(_secretKey);
        }

        // Generate random salt with specific length for hash function
        public string GenerateSalt(int length)
        {
            var saltBytes = new byte[length];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
                return Convert.ToBase64String(saltBytes);
            }
        }

        // Hash a password with salt and return it as a string
        public string HashPassword(string password, string salt)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltBytes = Encoding.UTF8.GetBytes(salt);

            using (var hmac = new HMACSHA512(saltBytes))
            {
                return Encoding.UTF8.GetString(hmac.ComputeHash(passwordBytes));
            }
        }

        // Register a new user
        public User? RegisterUser(UserModel userModel)
        {
            // Check: does username exist
            if (_userRepository.CheckUserNameExists(userModel.UserName))
            {
                return null;
            }

            // Create new user
            var salt = GenerateSalt(_saltLength);

            var passwordHash = HashPassword(userModel.Password, salt);

            var newUser = new User()
            {
                Name = userModel.Name,
                UserName = userModel.UserName,
                PasswordHash = passwordHash,
                PasswordSalt = salt,
                Role = "Trainee", // Hard code role Trainee
                CurrentRefreshToken = null
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return newUser;
        }

        public bool VerifyPasswordHash(
            string password,
            string passwordHash,
            string salt
        )
        {
            // Hash password with salt then compare it with passwordHash
            password = HashPassword(password, salt);
            return password == passwordHash;
        }

        public User? VerifyLogin(LoginModel loginModel)
        {
            // Check: Username
            var user = _userRepository.GetUserByUserName(loginModel.UserName);

            if (user == null)
                return null;

            // Check: Password
            bool isValid = VerifyPasswordHash(
                loginModel.Password,
                user.PasswordHash,
                user.PasswordSalt
            );

            if (isValid == false)
                return null;

            return user;
        }

        public string GenerateAccessToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secretKeyBytes = Encoding.UTF8.GetBytes(_secretKey);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.NameIdentifier, user.UserName),
                        new Claim("Id", user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.Role)
                    }
                ),
                Expires = DateTime.UtcNow.AddSeconds(30),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(secretKeyBytes),
                    SecurityAlgorithms.HmacSha512Signature
                )
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);

            return jwtTokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken(int length)
        {
            var random = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);

                return Convert.ToBase64String(random);
            }
        }
    }
}
