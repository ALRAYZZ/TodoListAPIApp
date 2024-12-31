using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TodoListAPI.Models
{
	public class TodoModel
	{
		public int Id { get; set; }
		[Required]
		public string Title { get; set; }
		public string Description { get; set; }
		[JsonIgnore]
		public string UserId { get; set; }
	}
}
 