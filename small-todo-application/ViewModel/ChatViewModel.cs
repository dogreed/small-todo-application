using small_todo_application.Models;
//for user private message
namespace small_todo_application.ViewModel
{
	public class ChatViewModel
	{
		public int FriendId { get; set; }
		public string FriendName { get; set; }
		public List<PrivateMessage> Messages { get; set; }
	}
}


