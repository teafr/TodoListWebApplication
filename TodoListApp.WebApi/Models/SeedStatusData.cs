using Microsoft.EntityFrameworkCore;
using TodoListApp.Database.Contexts;
using TodoListApp.Database.Entities;

namespace TodoListApp.WebApi.Models;

public static class SeedStatusData
{
    public static void EnsurePopulated(IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        using var scope = app.ApplicationServices.CreateScope();
        using TodoListDbContext context = scope.ServiceProvider.GetRequiredService<TodoListDbContext>();

        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }

        if (!context.Statuses.Any())
        {
            context.Statuses.AddRange(
            new StatusEntity
            {
                Name = "Not Started",
            },
            new StatusEntity
            {
                Name = "In Progress",
            },
            new StatusEntity
            {
                Name = "Completed",
            });

            _ = context.SaveChanges();
        }
    }
}
