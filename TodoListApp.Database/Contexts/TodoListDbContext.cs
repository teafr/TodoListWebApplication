using Microsoft.EntityFrameworkCore;
using TodoListApp.Database.Entities;

namespace TodoListApp.Database.Contexts;

public class TodoListDbContext : DbContext
{
    public TodoListDbContext(DbContextOptions<TodoListDbContext> options)
        : base(options)
    {
    }

    public DbSet<TodoListEntity> TodoLists => this.Set<TodoListEntity>();

    public DbSet<StatusEntity> Statuses => this.Set<StatusEntity>();

    public DbSet<TaskEntity> Tasks => this.Set<TaskEntity>();
}
