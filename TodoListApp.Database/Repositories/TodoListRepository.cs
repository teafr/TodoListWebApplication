using Microsoft.EntityFrameworkCore;
using TodoListApp.Database.Contexts;
using TodoListApp.Database.Entities;

namespace TodoListApp.Database.Repositories;

public class TodoListRepository : BaseRepository<TodoListEntity>
{
    public TodoListRepository(TodoListDbContext context)
        : base(context)
    {
    }

    public override async Task<List<TodoListEntity>?> GetAsync()
    {
        return await this.DbSet.Include(todoList => todoList.Tasks).ThenInclude(task => task.Status).AsNoTracking().ToListAsync();
    }

    public override async Task<TodoListEntity?> GetByIdAsync(int id)
    {
        return await this.DbSet.Include(todoList => todoList.Tasks).ThenInclude(task => task.Status).AsNoTracking().FirstOrDefaultAsync(list => list.Id == id);
    }

    public override void Update(TodoListEntity item)
    {
        var existingEntity = this.DbSet.Include(todoList => todoList.Tasks).FirstOrDefault(todoList => todoList.Id == item.Id);

        if (existingEntity != null)
        {
            this.Context.Entry(existingEntity).Collection(e => e.Tasks).IsModified = false;
            this.Context.Entry(existingEntity).CurrentValues.SetValues(item);

            _ = this.Context.SaveChanges();
        }
    }
}
