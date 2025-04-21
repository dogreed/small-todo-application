using small_todo_application.Models;
using Microsoft.EntityFrameworkCore;


namespace small_todo_application.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


		public DbSet<Register> Registers { get; set; }
	}
}
