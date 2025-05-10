using Microsoft.AspNetCore.SignalR;
using small_todo_application.Data;
using small_todo_application.Models;
using System.Security.Claims;

namespace small_todo_application.Hubs
{
	public class UserChatHub : Hub
	{
		private readonly AppDbContext _context;

		public UserChatHub(AppDbContext context)
		{
			_context = context;
		}

		public async Task SendPrivateMessage(int receiverId, string message)
		{
			var senderId = int.Parse(Context.UserIdentifier);

			var privateMessage = new PrivateMessage
			{
				SenderId = senderId,
				ReceiverId = receiverId,
				Message = message,
				SentAt = DateTime.UtcNow
			};

			_context.PrivateMessages.Add(privateMessage);
			await _context.SaveChangesAsync();

			// Send to the receiver
			await Clients.User(receiverId.ToString()).SendAsync(
				"ReceivePrivateMessage", senderId, receiverId, message);

			await Clients.User(senderId.ToString()).SendAsync(
				"ReceivePrivateMessage", senderId, receiverId, message);

		}
	}
}
