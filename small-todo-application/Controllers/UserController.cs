using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace small_todo_application.Controllers
{
	[Authorize(Roles = "User")]
	
	public class UserController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
		public IActionResult ViewTask()
		{
			return View();
		}
	}
}
