using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using small_todo_application.Data;
using small_todo_application.Models;
using small_todo_application.ViewModel;
using System.Security.Claims;

namespace small_todo_application.Controllers
{
	[Authorize(Roles = "User")]
	
	public class UserController : Controller
    {
		private readonly AppDbContext _context;

		public UserController(AppDbContext context)
		{
			_context = context;
		}

		public IActionResult Dashboard()
		{
			return View();
		}
		// In your controller
	
		[HttpGet]
		public IActionResult ViewTask()
		{
			var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

			var myTasks = _context.TaskList
				.Where(t => t.AssignedToUserId == userId)
				.OrderByDescending(t => t.CreatedAt)
				.ToList();

			return View(myTasks);

		}
	}
}
