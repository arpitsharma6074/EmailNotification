using CosmosCrudApi.Models;
using CosmosCrudApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CosmosCrudApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            User createdUser =
                await _userService.CreateUserAsync(user);

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = createdUser.id },
                createdUser);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            List<User> users =
                await _userService.GetUsersAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            User? user =
                await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(
            string id,
            User user)
        {
            User? updatedUser =
                await _userService.UpdateUserAsync(id, user);

            if (updatedUser == null)
            {
                return NotFound();
            }

            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            bool deleted =
                await _userService.DeleteUserAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}