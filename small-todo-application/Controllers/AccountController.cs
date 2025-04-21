using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using small_todo_application.Data;
using small_todo_application.Models;
using small_todo_application.ViewModel;
using System.Security.Claims;

namespace small_todo_application.Controllers
{
	public class AccountController : Controller
	{
		private readonly AppDbContext _context;

		public AccountController(AppDbContext context)
		{
			_context = context;
		}

		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(Register model)
		{
			if (ModelState.IsValid)
			{
				var emailExists = _context.Registers.Any(u => u.Email == model.Email);

				if (emailExists)
				{
					ModelState.AddModelError("Email", "Email already registered.");
					return View(model);
				}

				// Hash the password before saving
				var hasher = new PasswordHasher<Register>();
				model.Password = hasher.HashPassword(model, model.Password);
				model.PasswordConfirmed = hasher.HashPassword(model, model.PasswordConfirmed);

				_context.Add(model);
				await _context.SaveChangesAsync();

				// Pass success message to view
				TempData["SuccessMessage"] = "Registration successful! You can now login.";
				return RedirectToAction("Register");
			}

			return View(model);
		}

		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = _context.Registers.FirstOrDefault(u => u.Email == model.Email);

				if (user != null)
				{
					var hasher = new PasswordHasher<Register>(); //hash password 
					var result = hasher.VerifyHashedPassword(user, user.Password, model.Password);

					if (result == PasswordVerificationResult.Success)
					{
						var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, user.Name),
					new Claim(ClaimTypes.Email, user.Email),
					new Claim(ClaimTypes.Role, user.Role)
				};

						var identity = new ClaimsIdentity(claims, "MyCookieAuth"); //calling from program.cs file
						var principal = new ClaimsPrincipal(identity);

						await HttpContext.SignInAsync("MyCookieAuth", principal);

						if (user.Role == "Admin")
						{
							return RedirectToAction("Dashboard", "Admin"); //first one is the name of.cshtml and second onr controller
						}
						else if (user.Role == "User")
						{
							return RedirectToAction("Dashboard", "User");
						}
					}
				}

				ModelState.AddModelError(string.Empty, "Invalid email or password");
			}

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync("MyCookieAuth");
			return RedirectToAction("Login", "Account");
		}
		public IActionResult AccessDenied()
		{
			return View();
		}


	}

}

