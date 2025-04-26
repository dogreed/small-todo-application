using small_todo_application.Models;

namespace small_todo_application.ViewModel
{
	public class AssignTaskViewModel
	{
		public AssignTaskViewModel()
		{
			Task = new TaskList();
			AssignableUsers = new List<Register>();
		}
		public TaskList Task { get; set; }
		public List<Register> AssignableUsers { get; set; }
	}
}
