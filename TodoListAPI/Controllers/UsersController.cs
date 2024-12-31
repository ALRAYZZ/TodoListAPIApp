using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListAPI.Models;
using TodoListAPI.Services;

namespace TodoListAPI.Controllers
{
	[ApiController]
	public class UsersController : Controller
	{
		private readonly UserService _userService;
		public UsersController(UserService userService)
		{
			_userService = userService;
		}
		
		[HttpPost("register")]
		[AllowAnonymous]
		public async Task<ActionResult<UserModel>> RegisterUser(UserModel user)
		{
			var createdUser = await _userService.RegisterUserAsync(user);
			return CreatedAtAction(nameof(RegisterUser), new { id = createdUser.Id }, createdUser);
		}

		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<ActionResult<UserModel>> LoginUser(LoginModel login)
		{
			var user = await _userService.LoginUserAsync(login);
			if (user == null)
			{
				return Unauthorized();
			}

			var token = _userService.GenerateJwtToken(user);
			return Ok(new {Token = token});
		}
	}
}
