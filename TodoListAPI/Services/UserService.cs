using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using TodoListAPI.DataAccess;
using TodoListAPI.Models;

namespace TodoListAPI.Services
{
	public class UserService
	{
		private readonly TodoDbContext _context;
		private readonly IConfiguration _config;
		public UserService (TodoDbContext context, IConfiguration config)
		{
			_context = context;
			_config = config;
		}


		public async Task<UserModel> RegisterUserAsync(UserModel user)
		{
			// Hash the password before saving it to the database
			user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

			_context.Users.Add(user);
			await _context.SaveChangesAsync();
			return user;
		}
		public async Task<UserModel> LoginUserAsync(LoginModel login)
		{
			var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == login.Email);
			if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
			{
				return null;
			}
			return user;
		}
		public string GenerateJwtToken(UserModel user)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Email),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())// Add the user's ID to the claims
			};

			var token = new JwtSecurityToken(
				issuer: _config["Jwt:Issuer"],
				audience: _config["Jwt:Audience"],
				claims: claims,
				expires: DateTime.Now.AddMinutes(30),
				signingCredentials: credentials);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
