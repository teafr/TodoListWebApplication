using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Contexts;

namespace TodoListApp.WebApi.Repositories;

public abstract class BaseRepository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    protected readonly TodoListDbContext context;
    protected readonly DbSet<TEntity> dbSet;

    protected BaseRepository(TodoListDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        this.context = context;
        this.dbSet = context.Set<TEntity>();
    }

    public virtual async Task<List<TEntity>?> GetAsync()
    {
        var items = await this.dbSet.ToListAsync();
        return items;
    }

    public virtual async Task<TEntity?> GetByIdAsync(int id)
    {
        return await this.dbSet.FindAsync(id);
    }

    public virtual async Task<TEntity> CreateAsync(TEntity newItem)
    {
        _ = this.dbSet.Add(newItem);
        _ = await this.context.SaveChangesAsync();
        return newItem;
    }

    public virtual async Task UpdateAsync(TEntity item)
    {
        _ = this.dbSet.Update(item);
        _ = await this.context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(TEntity item)
    {
        _ = this.dbSet.Remove(item);
        _ = await this.context.SaveChangesAsync();
    }

    public virtual async Task DeleteByIdAsync(int id)
    {
        var entity = await this.dbSet.FindAsync(id);

        if (entity is not null)
        {
            _ = this.dbSet.Remove(entity!);
            _ = await this.context.SaveChangesAsync();
        }
    }
}
