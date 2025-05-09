using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
			return RedirectToAction("ViewTask");
		}


		[HttpGet]
		public IActionResult BrowseUsers()
		{
			var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

			var users = _context.Registers
				.Where(u => u.Id != currentUserId && u.Role == "User")
				.ToList();

			var friendships = _context.Friendships
				.Where(f =>
					(f.UserId == currentUserId || f.FriendId == currentUserId))
				.ToList();

			var acceptedFriendIds = friendships
				.Where(f => f.Status == FriendshipStatus.Accepted)
				.Select(f => f.UserId == currentUserId ? f.FriendId : f.UserId)
				.ToList();

			var pendingFriendRequestIds = friendships
				.Where(f => f.Status == FriendshipStatus.Pending && f.UserId == currentUserId)
				.Select(f => f.FriendId)
				.ToList();

			ViewBag.FriendIds = acceptedFriendIds;
			ViewBag.PendingFriendIds = pendingFriendRequestIds;

			return View(users);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SendFriendRequest(int friendId)
		{
			var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

			// Check if a friend request already exists between the users
			var existingFriendship = await _context.Friendships
												   .FirstOrDefaultAsync(f =>
														(f.UserId == currentUserId && f.FriendId == friendId) ||
														(f.UserId == friendId && f.FriendId == currentUserId));

			if (existingFriendship == null)
			{
				// If no existing request, create a new friendship request (Pending)
				var friendship = new Friendship
				{
					UserId = currentUserId,
					FriendId = friendId,
					Status = FriendshipStatus.Pending
				};

				_context.Friendships.Add(friendship);
				await _context.SaveChangesAsync();

				TempData["SuccessMessage"] = "Friend request sent!";
			}
			else
			{
				TempData["ErrorMessage"] = "You are already friends or have pending request!";
			}

			return RedirectToAction("BrowseUsers");
		}
		[HttpGet]
		public async Task<IActionResult> PendingRequests()
		{
			var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

			// Fetch pending friend requests where the current user is the recipient
			var pendingRequests = await _context.Friendships
												 .Where(f => f.FriendId == currentUserId && f.Status == FriendshipStatus.Pending)
												 .Include(f => f.User)  // Include sender details
												 .ToListAsync();

			return View(pendingRequests);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RespondToFriendRequest(int friendshipId, bool accept)
		{
			var friendship = await _context.Friendships.FindAsync(friendshipId);
			if (friendship == null) return NotFound();

			friendship.Status = accept ? FriendshipStatus.Accepted : FriendshipStatus.Rejected;

			await _context.SaveChangesAsync();

			TempData["SuccessMessage"] = accept ? "Friend request accepted!" : "Friend request rejected!";

			return RedirectToAction("PendingRequests");
		}
		[HttpGet]
		public async Task<IActionResult> FriendList()
		{
			var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

			var friendships = await _context.Friendships
				.Where(f =>
					(f.UserId == currentUserId || f.FriendId == currentUserId) &&
					f.Status == FriendshipStatus.Accepted)
				.Include(f => f.User)
				.Include(f => f.Friend)
				.ToListAsync();

			// Prepare list of just the actual "friend" users (not self)
			var friendUsers = friendships
				.Select(f => f.UserId == currentUserId ? f.Friend : f.User)
				.ToList();

			return View(friendUsers); // <== Now your view will just receive List<Register>
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RemoveFriend(int friendId)
		{
			var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

			var friendship = await _context.Friendships
				.FirstOrDefaultAsync(f =>
					(f.UserId == currentUserId && f.FriendId == friendId) ||
					(f.UserId == friendId && f.FriendId == currentUserId));

			if (friendship != null)
			{
				_context.Friendships.Remove(friendship);
				await _context.SaveChangesAsync();
				TempData["SuccessMessage"] = "You have unfriended the user.";
			}
			else
			{
				TempData["ErrorMessage"] = "Friendship not found.";
			}

			return RedirectToAction("FriendList");
		}


	}
}
