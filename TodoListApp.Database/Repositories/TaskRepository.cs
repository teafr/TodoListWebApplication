using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Database.Contexts;
using TodoListApp.Database.Entities;

namespace TodoListApp.Database.Repositories;

public class TaskRepository : BaseRepository<TaskEntity>
{
    public TaskRepository(TodoListDbContext context)
        : base(context)
    {
    }

    public override async Task<List<TaskEntity>?> GetAsync()
    {
        return await this.DbSet.Include(task => task.Status).ToListAsync();
    }

    public override async Task<TaskEntity?> GetByIdAsync(int id)
    {
        return await this.DbSet.Include(task => task.Status).FirstOrDefaultAsync(t => t.Id == id);
    }

    public override Task<TaskEntity> CreateAsync(TaskEntity newItem)
    {
        CheckEntity(newItem);
        this.AttachStatus(newItem);
        return base.CreateAsync(newItem);
    }

    public override void Update(TaskEntity item)
    {
        CheckEntity(item);
        this.AttachStatus(item);
        base.Update(item);
    }

    private static void CheckEntity(TaskEntity? entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
    }

    private void AttachStatus(TaskEntity task)
    {
        StatusEntity? status = this.Context.Statuses.FirstOrDefault(s => s.Id == task.StatusId);

        if (status is not null)
        {
            task.Status = status;
            this.Context.Entry(task.Status).State = EntityState.Unchanged;
        }
    }
}
