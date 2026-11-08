using Claims.Models;
using Microsoft.EntityFrameworkCore;

namespace Claims
{
    // Maybe better to change to AppDbContext, but name of solution  is Claims
    public class ClaimsRepository : DbContext
    {

        public DbSet<Claim> Claims { get; init; }
        public DbSet<Cover> Covers { get; init; }

        public ClaimsRepository(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Claim>()
                .HasOne(c => c.Cover)
                .WithMany(c => c.Claims)
                .HasForeignKey(c => c.CoverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
