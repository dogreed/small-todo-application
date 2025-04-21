using System.ComponentModel.DataAnnotations;

namespace small_todo_application.Models
{
	public class Register
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "Name is required")]
		[StringLength(100, ErrorMessage = "Name can't be longer than 100 characters")]
		public string Name { get; set; } = string.Empty;

		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "Password is required")]
		[DataType(DataType.Password)]
		[StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Please confirm your password")]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Passwords do not match")]
		public string PasswordConfirmed { get; set; }

		public string Role { get; set; } = "User"; 
	}
}
