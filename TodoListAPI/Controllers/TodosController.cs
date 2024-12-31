using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TodoListAPI.DataAccess;
using TodoListAPI.Models;
using TodoListAPI.Services;

namespace TodoListAPI.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class TodosController : Controller
	{
		public readonly EFService _EFService;
		public TodosController(EFService EFService)
		{
			_EFService = EFService;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<TodoModel>>> GetTodoItems(int page = 1, int limit = 10)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized("User is not authenticated");
			}

			var todos = await _EFService.GetTodosByUserIdAsync(userId, page, limit);
			return Ok(todos);
		}
		[HttpPost]
		public async Task<ActionResult<TodoModel>> CreateTodo(CreateTodoModel todoCreateModel)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized("User is not authenticated");
			}

			var todoModel = new TodoModel
			{
				Title = todoCreateModel.Title,
				Description = todoCreateModel.Description,
				UserId = userId
			};


			var createdTodo = await _EFService.CreateTodoAsync(todoModel);
			return CreatedAtAction(nameof(GetTodoItem), new { id = createdTodo.Id }, createdTodo);
		}
		[HttpGet("{id}")]
		public async Task<ActionResult<TodoModel>> GetTodoItem(int id)
		{
			var todo = await _EFService.GetTodoByIdAsync(id);
			if (todo == null)
			{
				return NotFound();
			}
			return Ok(todo);
		}
		[HttpPut("{id}")]
		public async Task<ActionResult<TodoModel>> UpdateTodoItem(int id, CreateTodoModel todoCreateModel)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized("User is not authenticated");
			}
			var existingTodo = await _EFService.GetTodoByIdAsync(id);
			if (existingTodo == null)
			{
				return NotFound();
			}
			if (existingTodo.UserId != userId)
			{
				return Forbid("User is not authorized to update this todo");
			}

			existingTodo.Title = todoCreateModel.Title;
			existingTodo.Description = todoCreateModel.Description;

			var updatedTodo = await _EFService.UpdateTodoAsync(existingTodo);
			return Ok(updatedTodo);
		}
		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteTodoItem(int id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized("User is not authenticated.");
			}
			var existingTodo = await _EFService.GetTodoByIdAsync(id);
			if (existingTodo == null)
			{
				return NotFound();
			}
			if (existingTodo.UserId != userId)
			{
				return Forbid("User is not authorized to delete this todo");
			}

			await _EFService.DeleteTodoAsync(id);
			return NoContent();

		}
	}
}
