namespace small_todo_application.Models
{
	public class ChatMessage

	{
		public int Id { get; set; }
		public string Sender { get; set; } = string.Empty;
		public string Message { get; set; } = string.Empty;
		public DateTime Timestamp { get; set; } = DateTime.Now;
	}
}
