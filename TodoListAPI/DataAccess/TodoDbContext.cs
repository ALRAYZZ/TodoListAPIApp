using Microsoft.EntityFrameworkCore;
using TodoListAPI.Models;

namespace TodoListAPI.DataAccess
{
	public class TodoDbContext : DbContext
	{
		public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
		{

		}
		public DbSet<TodoModel> TodoList { get; set; }
		public DbSet<UserModel> Users { get; set; }
	}
}
