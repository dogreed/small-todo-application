using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using small_todo_application.Data;
using small_todo_application.Models;
using small_todo_application.ViewModel;
using System.Security.Claims;
using System.Text.Json;

namespace small_todo_application.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdminController : Controller
	{
		private readonly AppDbContext _context;

		public AdminController(AppDbContext context)
		{
			_context = context;
		}

		public IActionResult Dashboard(int? editId = null)
		{
			var registers = _context.Registers.ToList();
			ViewBag.EditId = editId;
			return View(registers);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Delete(int id)
		{
			var registers = _context.Registers.Find(id);
			if (registers != null)
			{
				_context.Registers.Remove(registers);
				_context.SaveChanges();
			}

			return RedirectToAction("Dashboard");

		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(int Id, string role)
		{
			{
				if (ModelState.IsValid)
				{
					var existing = _context.Registers.FirstOrDefault(c => c.Id == Id);
					if (existing != null)
					{
						existing.Role = role;
						_context.SaveChanges();
					}
					return RedirectToAction("Dashboard");
				}

				// If we got this far, something failed
				var register = _context.Registers.ToList();
				return View("Dashboard", register);
			}

		}

		

		[HttpGet]
		public async Task<IActionResult> AssignTask()
		{
			var viewModel = new AssignTaskViewModel
			{
				Task = new TaskList(), 
				AssignableUsers = await _context.Registers.ToListAsync() // No IsActive filter
			};
			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AssignTask(AssignTaskViewModel model)
		{
			// Manually validate required fields
			if (model.Task.AssignedToUserId == 0) // 0 is default for int
			{
				ModelState.AddModelError("Task.AssignedToUserId", "Please select a user");
			}

			if (ModelState.IsValid)
			{
				try
				{
					// Create new task with only necessary fields
					var task = new TaskList
					{
						Title = model.Task.Title,
						Description = model.Task.Description,
						AssignedToUserId = model.Task.AssignedToUserId,
						CreatedAt = DateTime.Now
					};

					_context.TaskList.Add(task);
					await _context.SaveChangesAsync();

					TempData["SuccessMessage"] = "Task Assigned Successfully!";
					return RedirectToAction("AssignTask");
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", "Error saving task: " + ex.Message);
				}
			}

			// Repopulate users
			model.AssignableUsers = await _context.Registers.ToListAsync();
			return View(model);
		}

	}



}




