using Microsoft.AspNetCore.Mvc;

using JwtApplication.Services;
using JwtApplication.Models;
using JwtApplication.Entities;
using Microsoft.AspNetCore.Authorization;

namespace JwtApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Trainee")]
        public IActionResult GetAllUser()
        {
            var users = _userRepository.GetAllUser();
            var usersVM = users.Select(
                u =>
                    new UserVM
                    {
                        Name = u.Name,
                        UserName = u.UserName,
                        Role = u.Role
                    }
            );

            return Ok(
                new ApiResponseModel()
                {
                    Success = true,
                    Message = "",
                    Data = new { users = usersVM }
                }
            );
        }
    }
}
