using ImageUpload.User;
using Microsoft.EntityFrameworkCore;

namespace ImageUpload.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserBase64> UserBase64 { get; set; }
        public DbSet<UserImageFile> UserImageFile { get; set; }
    }
}
