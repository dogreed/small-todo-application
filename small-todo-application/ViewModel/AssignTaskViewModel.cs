using Microsoft.AspNetCore.Mvc.Rendering;
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
		public List<SelectListItem> UserOptions //green error seems legit, it will come because we are just passing the list of users... 
		{
			get
			{
				return AssignableUsers?.Select(u => new SelectListItem
				{
					Value = u.Id.ToString(),
					Text = u.Name,
					Selected = (u.Id == Task?.AssignedToUserId)
				}).ToList();
			}
		}


	}

}
