using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace small_todo_application.Models
{
	// In Models/TaskList.cs
	public class TaskList
	{
		public int Id { get; set; }

		[Required]
		public string Title { get; set; }

		public string Description { get; set; }
		public int CreatedByUserId { get; set; }

		[Required(ErrorMessage = "Please select a user to assign the task to")]
		[Display(Name = "Assigned User")]
		public int AssignedToUserId { get; set; }

		[ForeignKey("AssignedToUserId")]
		public Register? AssignedToUser { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.Now;

		public string Status { get; set; } = "Not Started"; // Default value

	}
}
