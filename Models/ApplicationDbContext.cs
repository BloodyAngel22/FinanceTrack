using Microsoft.EntityFrameworkCore;

namespace FinanceTrack.Models
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}

		public DbSet<Category> Categories { get; set; }
		public DbSet<FinanceNote> FinanceNotes { get; set; }
	}
}