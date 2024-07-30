using Microsoft.EntityFrameworkCore;

namespace qr_scanner_app_staj.Model
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<User> User { get; set; }
        public DbSet<Receipt> Receipt { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Receipt>()
                .HasKey(r => new { r.receiptId, r.userId });
        }
    }
}