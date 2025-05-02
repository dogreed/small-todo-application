using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using small_todo_application.Data;
using small_todo_application.Models;
using small_todo_application.ViewModel;
using System.Security.Claims;

namespace small_todo_application.Controllers
{
	[Authorize(Roles = "Manager")]
	public class ManagerController : Controller
    {
		private readonly AppDbContext _context;

		public ManagerController(AppDbContext context)
		{
			_context = context;
		}

		public IActionResult Dashboard()
        {
			var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

			var myTasks = _context.TaskList
				.Where(t => t.AssignedToUserId == userId)
				.OrderByDescending(t => t.CreatedAt)
				.ToList();

			return View(myTasks);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateStatus(int taskId, string status)
		{
			var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

			var task = await _context.TaskList.FirstOrDefaultAsync(t => t.Id == taskId && t.AssignedToUserId == userId);
			if (task == null)
			{
				return NotFound();
			}

			task.Status = status;
			await _context.SaveChangesAsync();

			TempData["SuccessMessage"] = "Task status updated!";
			return RedirectToAction("Dashboard");
		}

		[HttpGet]
		public async Task<IActionResult> AssignTask()
		{
			var viewModel = new AssignTaskViewModel
			{
				Task = new TaskList(),
				// Only show users with "User" role
				AssignableUsers = await _context.Registers
					.Where(u => u.Role == "User")  // 👈 Added filter
					.ToListAsync()
			};
			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AssignTask(AssignTaskViewModel model)
		{
			if (ModelState.IsValid)
			{
				// Get current user ID from auth context
				var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

				// Create task with auto-set creator ID
				var task = new TaskList
				{
					Title = model.Task.Title,
					Description = model.Task.Description,
					AssignedToUserId = model.Task.AssignedToUserId,
					CreatedByUserId = currentUserId, // Auto-set here
					CreatedAt = DateTime.Now,
					Status = "Not Started"
				};

				_context.TaskList.Add(task);
				await _context.SaveChangesAsync();

				return RedirectToAction("AssignTask");
			}

			// Repopulate dropdowns if validation fails
			model.AssignableUsers = await _context.Registers.ToListAsync();
			return View(model);
		}

		//Now here is the code to edit, delete task in differnt page,?

		// Show all assigned tasks
		[HttpGet]
		public async Task<IActionResult> TaskList()
		{
			var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

			// 2. Filter tasks assigned to this user
			var tasks = await _context.TaskList
				.Include(t => t.AssignedToUser)  // Still include user details
				.Where(t => t.CreatedByUserId == currentUserId)  // 👈 Key filter to see whic id submitted 
				.ToListAsync();

			return View(tasks);
		}

		// Edit Task (GET)
		[HttpGet]
		public async Task<IActionResult> EditTask(int id)
		{
			var task = await _context.TaskList.FindAsync(id);
			if (task == null) return NotFound();

			var viewModel = new AssignTaskViewModel
			{
				Task = task,
				AssignableUsers = await _context.Registers.ToListAsync()
			};

			return View(viewModel);
		}

		// Edit Task (POST)
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditTask(AssignTaskViewModel model)
		{
			if (ModelState.IsValid)
			{
				var task = await _context.TaskList.FindAsync(model.Task.Id);
				if (task == null) return NotFound();

				task.Title = model.Task.Title;
				task.Description = model.Task.Description;
				task.AssignedToUserId = model.Task.AssignedToUserId;

				await _context.SaveChangesAsync();
				TempData["SuccessMessage"] = "Task updated successfully!";
				return RedirectToAction("TaskList");
			}

			model.AssignableUsers = await _context.Registers.ToListAsync();
			return View(model);
		}

		// Delete Task
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteTask(int id)
		{
			var task = await _context.TaskList.FindAsync(id);
			if (task != null)
			{
				_context.TaskList.Remove(task);
				await _context.SaveChangesAsync();
			}
			return RedirectToAction("TaskList");
		}


	}
}
