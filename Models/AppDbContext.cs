using System;
using Microsoft.EntityFrameworkCore;

namespace MyWebApp.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Item> Redirect_data { get; set; } = null!;
        public DbSet<RedirectLog> RedirectLogs { get; set; } = null!;
        public DbSet<Location> Locations { get; set; } = null!;
    }
}
