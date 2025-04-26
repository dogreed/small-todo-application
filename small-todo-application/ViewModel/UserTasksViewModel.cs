using small_todo_application.Models;

namespace small_todo_application.ViewModel
{
	public class UserTasksViewModel
	{
		public Register User { get; set; }
		public List<TaskList> AssignedTasks { get; set; }
	}
}
