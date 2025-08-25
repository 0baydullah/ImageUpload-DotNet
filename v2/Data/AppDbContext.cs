using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using v2.Entities;

namespace v2.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        public DbSet<User> users { get; set; }
    }
}

