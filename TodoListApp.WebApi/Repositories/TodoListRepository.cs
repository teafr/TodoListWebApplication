using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Contexts;
using TodoListApp.WebApi.Entities;

namespace TodoListApp.WebApi.Repositories;

public class TodoListRepository : BaseRepository<TodoListEntity>
{
    public TodoListRepository(TodoListDbContext context)
        : base(context)
    {
    }

    public override async Task<List<TodoListEntity>?> GetAsync()
    {
        var todoLists = await this.dbSet.Include(todoList => todoList.Tasks).ThenInclude(task => task.Status).AsNoTracking().ToListAsync();
        return todoLists;
    }

    public override async Task<TodoListEntity?> GetByIdAsync(int id)
    {
        var todoList = await this.dbSet.Include(todoList => todoList.Tasks).ThenInclude(task => task.Status).AsNoTracking().FirstOrDefaultAsync(list => list.Id == id);
        return todoList;
    }
}
