using Microsoft.AspNetCore.Mvc;

using JwtApplication.Services;
using JwtApplication.Models;
using JwtApplication.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace JwtApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly int _refreshTokenLength = 32;
        private readonly int _refreshTokenSecondExpire = 60;

        public AuthController(
            IAuthRepository authRepository,
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository
        )
        {
            _authRepository = authRepository;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }

        [HttpPost("register")]
        public IActionResult Register(UserModel userModel)
        {
            var newUser = _authRepository.RegisterUser(userModel);

            // username already exists
            if (newUser == null)
            {
                return Ok(
                    new ApiResponseModel()
                    {
                        Success = false,
                        Message = "Username already exists",
                    }
                );
            }

            // Generate access token and refresh token
            var accessToken = _authRepository.GenerateAccessToken(newUser);
            var refreshToken = _authRepository.GenerateRefreshToken(
                _refreshTokenLength
            );

            // Create refresh token in database
            var refreshTokenEntity = _refreshTokenRepository.CreateRefreshToken(
                refreshToken,
                _refreshTokenSecondExpire
            );

            // Update current refresh token of a user
            _userRepository.UpdateCurrentRefreshToken(
                newUser.Id,
                refreshTokenEntity
            );

            return Ok(
                new ApiResponseModel()
                {
                    Success = true,
                    Message = "User register successfully",
                    Data = new
                    {
                        User = new UserVM()
                        {
                            Name = newUser.Name,
                            UserName = newUser.UserName,
                            Role = newUser.Role
                        },
                        accessToken,
                        refreshToken
                    }
                }
            );
        }

        [HttpPost("login")]
        public IActionResult Login(LoginModel loginModel)
        {
            // Verify login
            var user = _authRepository.VerifyLogin(loginModel);
            if (user is null)
            {
                return Ok(
                    new ApiResponseModel()
                    {
                        Success = false,
                        Message = "Invalid user name or password"
                    }
                );
            }

            // Generate access token and refresh token
            var accessToken = _authRepository.GenerateAccessToken(user);
            var refreshToken = _authRepository.GenerateRefreshToken(
                _refreshTokenLength
            );

            // Create refresh token in database
            var refreshTokenEntity = _refreshTokenRepository.CreateRefreshToken(
                refreshToken,
                _refreshTokenSecondExpire
            );

            // Update current refresh token of a user
            _userRepository.UpdateCurrentRefreshToken(
                user.Id,
                refreshTokenEntity
            );

            return Ok(
                new ApiResponseModel()
                {
                    Success = true,
                    Message = "Login successfully",
                    Data = new { accessToken, refreshToken }
                }
            );
        }

        [HttpPost("refresh-token")]
        public IActionResult FreshToken(TokenModel model)
        {
            // Get refresh token in database
            var refreshToken = _refreshTokenRepository.GetRefreshTokenByToken(
                model.RefreshToken
            );

            // Check: Does refresh token exist
            if (refreshToken == null)
            {
                return NotFound(
                    new ApiResponseModel()
                    {
                        Success = false,
                        Message = "Refresh token does not exist"
                    }
                );
            }

            // Check: Is that a current token of a user
            var tokenOwner = _refreshTokenRepository.GetUserOfARefreshToken(
                refreshToken.Id
            );

            if (tokenOwner is null)
            {
                // If the user is not found => that token is not the current token
                return BadRequest(
                    new ApiResponseModel()
                    {
                        Success = false,
                        Message = "Something went wrong"
                    }
                );
            }

            // Check: Has refresh token expired
            if (refreshToken.ExpiredAt < DateTime.UtcNow)
            {
                return Ok(
                    new ApiResponseModel()
                    {
                        Success = false,
                        Message = "Refresh token has expired"
                    }
                );
            }

            // Grant a new access token
            var newAccessToken = _authRepository.GenerateAccessToken(
                tokenOwner
            );

            return Ok(
                new ApiResponseModel()
                {
                    Success = true,
                    Message = "Refresh token successfully",
                    Data = new { newAccessToken }
                }
            );
        }
    }
}
