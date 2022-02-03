using Microsoft.EntityFrameworkCore;
using AuthMiddlware.Model;
namespace AuthModule.Model
{
    public class UsersDbContext : DbContext
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options)
            : base(options)
        {
        }
        public DbSet<User>? User { get; set; }
    }
}
