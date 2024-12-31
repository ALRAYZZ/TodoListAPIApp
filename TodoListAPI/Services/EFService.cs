using Microsoft.EntityFrameworkCore;
using TodoListAPI.DataAccess;
using TodoListAPI.Models;

namespace TodoListAPI.Services
{
	public class EFService
	{
		private readonly TodoDbContext _context;

		public EFService(TodoDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<TodoModel>> GetAllTodosAsync()
		{
			return await _context.TodoList.ToListAsync();
		}
		public async Task<IEnumerable<TodoModel>> GetTodosByUserIdAsync(string userId, int page, int limit)
		{
			return await _context.TodoList
				.Where(t => t.UserId == userId)
				.Skip((page - 1) * limit)
				.Take(limit)
				.ToListAsync();
		}

		public async Task<TodoModel> GetTodoByIdAsync(int id)
		{
			var todo = await _context.TodoList.FindAsync(id);
			if (todo == null)
			{
				return null;
			}
			return todo;
		}

		public async Task<TodoModel> CreateTodoAsync(TodoModel todo)
		{
			_context.TodoList.Add(todo);
			await _context.SaveChangesAsync();
			return todo;
		}

		public async Task<TodoModel> UpdateTodoAsync(TodoModel todo)
		{
			_context.Entry(todo).State = EntityState.Modified;
			await _context.SaveChangesAsync();
			return todo;
		}
		public async Task DeleteTodoAsync(int id)
		{
			var todo = await _context.TodoList.FindAsync(id);
			if (todo != null)
			{
				_context.TodoList.Remove(todo);
				await _context.SaveChangesAsync();
			}
			
		}
	}
}

