using Microsoft.EntityFrameworkCore;
using FivD.Models;

namespace FivD.dbContextClass
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<UserEntity> UserEntity { get; set; }
    }
}
