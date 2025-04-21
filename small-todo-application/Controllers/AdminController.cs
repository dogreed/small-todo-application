using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace small_todo_application.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
		public IActionResult RoleControl()
		{
			return View();
		}
		public IActionResult AssignTask()
		{
			return View();
		}
	}
}
