using System;
using Microsoft.EntityFrameworkCore;

namespace MyWebApp.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Item> T_CEF_QR_DETAILS { get; set; } = null!;
        public DbSet<RedirectLog> T_CEF_QR_RedirectLogs { get; set; } = null!;
        public DbSet<Location> T_CODE_MASTER { get; set; } = null!;
    }
}
