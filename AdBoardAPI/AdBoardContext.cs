using Microsoft.EntityFrameworkCore;
using AdBoardAPI.Models;
using AdBoardAPI.Models.AdModel;

namespace AdBoardAPI
{
    public class AdBoardContext : DbContext
    {
        public AdBoardContext(DbContextOptions<AdBoardContext> options) : base(options) { }
        public DbSet<Ad> Ads { get; set; }
        public DbSet<User> Users { get; set; }
    }
}