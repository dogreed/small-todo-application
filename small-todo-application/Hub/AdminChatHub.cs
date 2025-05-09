using Microsoft.AspNetCore.SignalR;
using small_todo_application.Data;
using small_todo_application.Models;

namespace small_todo_application.Hubs
{
	public class AdminChatHub : Hub
	{
		private readonly AppDbContext _context;

		public AdminChatHub(AppDbContext context)
		{
			_context = context;
		}
		public async Task SendMessage(string sender, string message)
		{
			var chatMessage = new ChatMessage
			{
				Sender = sender,
				Message = message,
				Timestamp = DateTime.Now
			};

			_context.ChatMessages.Add(chatMessage);
			await _context.SaveChangesAsync();

			await Clients.All.SendAsync("ReceiveMessage", sender, message);
		}
	}
}
