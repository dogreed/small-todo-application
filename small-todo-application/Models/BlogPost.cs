using System.ComponentModel.DataAnnotations;

namespace small_todo_application.Models
{
	public class BlogPost
	{
		public int Id { get; set; }

		[Required]
		public string Title { get; set; }

		public int CreatedByUserId { get; set; }
		[Required]
		public string Content { get; set; } // HTML from TinyMCE

		public byte[]? ImageData { get; set; }

		public string? ImageMimeType { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.Now;
	}
}
