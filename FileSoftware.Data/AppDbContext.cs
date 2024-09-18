using FileSoftware.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileSoftware.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options) { }

        public DbSet<FileEntity> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileEntity>()
                .HasIndex(f => new { f.Name, f.Extension })
                .IsUnique();
        }
    }
}
