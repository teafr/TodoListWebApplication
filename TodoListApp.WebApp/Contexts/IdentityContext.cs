using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TodoListApp.WebApp.Contexts;

public class IdentityContext : IdentityDbContext<IdentityUser>
{
    private readonly string connectionString;

    public IdentityContext(IConfiguration configuration)
    {
        this.connectionString = configuration.GetConnectionString("IdentityDbConnection") ?? string.Empty;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        _ = optionsBuilder.UseSqlServer(this.connectionString);
    }
}
