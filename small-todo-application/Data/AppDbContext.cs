using small_todo_application.Models;
using Microsoft.EntityFrameworkCore;


namespace small_todo_application.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


		public DbSet<Register> Registers { get; set; }
		public DbSet<TaskList> TaskList { get; set; }


		public DbSet<BlogPost> BlogPosts { get; set; }

		public DbSet<ChatMessage> ChatMessages { get; set; }


		// In AppDbContext.cs
		public DbSet<Friendship> Friendships { get; set; }
		public DbSet<PrivateMessage> PrivateMessages { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Friendship>()
				.HasOne(f => f.User)
				.WithMany()
				.HasForeignKey(f => f.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Friendship>()
				.HasOne(f => f.Friend)
				.WithMany()
				.HasForeignKey(f => f.FriendId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<PrivateMessage>()
				.HasOne(p => p.Sender)
	            .WithMany()
	            .HasForeignKey(p => p.SenderId)
	            .OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<PrivateMessage>()
				.HasOne(p => p.Receiver)
				.WithMany()
				.HasForeignKey(p => p.ReceiverId)
				.OnDelete(DeleteBehavior.Restrict);
		}



	}
}
