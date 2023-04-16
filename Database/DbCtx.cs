using LoginAPI.Domain;
using Microsoft.EntityFrameworkCore;

namespace LoginAPI.DB;

public class DbCtx : DbContext
{
    public DbSet<User> Users { get; set; }

    public DbCtx(DbContextOptions<DbCtx> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("Users");
    }

    public void SetDefaultData()
    {
        var exists = Users.Any(u => u.Account == "admin" && u.Password == "0000");
        if (!exists)
        {
            Users.Add(new User()
            {
                Account = "admin",
                Password = "0000",
                UserName = "管理員",
            });
            SaveChanges();
        }
    }
}