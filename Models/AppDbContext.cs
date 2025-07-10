using System;
using Microsoft.EntityFrameworkCore;

namespace MyWebApp.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Item> Redirect_data { get; set; }
        public DbSet<RedirectLog> RedirectLogs { get; set; }
    }
}
