namespace small_todo_application.Models
{
	public enum FriendshipStatus //default pending
	{
		Pending,
		Accepted,
		Rejected
	}

	public class Friendship
	{
		public int Id { get; set; }

		public int UserId { get; set; }
		public Register User { get; set; }

		public int FriendId { get; set; }
		public Register Friend { get; set; }

		public FriendshipStatus Status { get; set; } = FriendshipStatus.Pending;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}

}
