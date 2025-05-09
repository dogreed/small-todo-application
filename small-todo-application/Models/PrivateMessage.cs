using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace small_todo_application.Models
{
	public class PrivateMessage
	{
		public int Id { get; set; }

		public int SenderId { get; set; }

		[ForeignKey("SenderId")]
		public Register Sender { get; set; }

		public int ReceiverId { get; set; }

		[ForeignKey("ReceiverId")]
		public Register Receiver { get; set; }

		[Required]
		public string Message { get; set; }

		public DateTime SentAt { get; set; } = DateTime.UtcNow;
	}
}
