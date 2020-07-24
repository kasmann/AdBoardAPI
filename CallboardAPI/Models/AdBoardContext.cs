using Microsoft.EntityFrameworkCore;

namespace AdBoardAPI.Models
{
    public class AdBoardContext : DbContext
    {
        public AdBoardContext(DbContextOptions<AdBoardContext> options) : base(options) { }

        public DbSet<Ad> Ads { get; set; }
        public DbSet<User> Users { get; set; }
    }
}