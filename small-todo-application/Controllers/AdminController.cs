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
		//.............Here is For admin dashboard..............
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

		//.............Here is For admin AssignTask..............

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
				var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); //with this it knows role 
				try
				{
					// Create new task with only necessary fields
					var task = new TaskList
					{
						Title = model.Task.Title,
						Description = model.Task.Description,
						AssignedToUserId = model.Task.AssignedToUserId,
						CreatedByUserId = currentUserId,
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

		//Now here is the code to edit, delete task in differnt page,?

		// Show all assigned tasks
		[HttpGet]
		public async Task<IActionResult> TaskList()
		{
			var tasks = await _context.TaskList.Include(t => t.AssignedToUser).ToListAsync();
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

		//Now Adding Blog Features in admin Page 

		public IActionResult CreateBlog()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> CreateBlog(BlogPost post, IFormFile imageFile)
		{
			var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
			post.CreatedByUserId = currentUserId;

			if (imageFile != null && imageFile.Length > 0)
			{
				using var ms = new MemoryStream();
				await imageFile.CopyToAsync(ms);
				post.ImageData = ms.ToArray();
				post.ImageMimeType = imageFile.ContentType;
			}

			if (ModelState.IsValid)
			{
				_context.Add(post);

				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(CreateBlog));
			}

			return View(post);
		}


		//This code doestn't have filter and shows every record 
		//public IActionResult BlogList() 
		//{
		//	var blogs = _context.BlogPosts
		//		.OrderByDescending(b => b.CreatedAt)
		//		.ToList();

		//	return View(blogs);
		//}
		public IActionResult BlogList() //This Filter Blog according to id who posted 
		{
			var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

			var blogs = _context.BlogPosts
				.Where(b => b.CreatedByUserId == currentUserId)
				.OrderByDescending(b => b.CreatedAt)
				.ToList();

			return View(blogs);
		}

		public IActionResult EditBlog(int id)
		{
			var post = _context.BlogPosts.Find(id);
			if (post == null) return NotFound();
			return View(post);
		}
		[HttpPost]
		public async Task<IActionResult> EditBlog(BlogPost post, IFormFile imageFile)
		{
			var existing = _context.BlogPosts.Find(post.Id);
			if (existing == null) return NotFound();

			existing.Title = post.Title;
			existing.Content = post.Content;

			if (imageFile != null && imageFile.Length > 0)
			{
				using var ms = new MemoryStream();
				await imageFile.CopyToAsync(ms);
				existing.ImageData = ms.ToArray();
				existing.ImageMimeType = imageFile.ContentType;
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(BlogList));
		}
		[HttpPost]
		public IActionResult DeleteBlog(int id)
		{
			var post = _context.BlogPosts.Find(id);
			if (post == null) return NotFound();

			_context.BlogPosts.Remove(post);
			_context.SaveChanges();

			return RedirectToAction(nameof(BlogList));
		}

	}

}




