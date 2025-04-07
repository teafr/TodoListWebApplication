using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Contexts;
using TodoListApp.WebApi.Entities;

namespace TodoListApp.WebApi.Repositories;

internal class TaskRepository : BaseRepository<TaskEntity>
{
    public TaskRepository(TodoListDbContext context)
        : base(context)
    {
    }

    public override async Task<List<TaskEntity>?> GetAsync()
    {
        var tasks = await this.dbSet.Include(task => task.Status).ToListAsync();
        return tasks;
    }

    public override async Task<TaskEntity?> GetByIdAsync(int id)
    {
        var task = await this.dbSet.Include(task => task.Status).FirstOrDefaultAsync(t => t.Id == id);
        return task;
    }

    public override Task<TaskEntity> CreateAsync(TaskEntity newItem)
    {
        ArgumentNullException.ThrowIfNull(newItem);

        this.AttachStatus(newItem);
        return base.CreateAsync(newItem);
    }

    public override async Task UpdateAsync(TaskEntity item)
    {
        ArgumentNullException.ThrowIfNull(item);

        this.AttachStatus(item);
        await base.UpdateAsync(item);
    }

    private void AttachStatus(TaskEntity task)
    {
        StatusEntity? status = this.context.Statuses.Local.FirstOrDefault(s => s.Id == task.StatusId);
        ArgumentNullException.ThrowIfNull(status);

        task.Status = status;
        this.context.Entry(task.Status).State = EntityState.Unchanged;
    }
}
