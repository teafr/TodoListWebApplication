using Microsoft.EntityFrameworkCore;
using TodoListApp.Database.Contexts;

namespace TodoListApp.Database.Repositories;

public abstract class BaseRepository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    protected BaseRepository(TodoListDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        this.Context = context;
        this.DbSet = context.Set<TEntity>();
    }

    protected TodoListDbContext Context { get; }

    protected DbSet<TEntity> DbSet { get; }

    public virtual async Task<List<TEntity>?> GetAsync()
    {
        var items = await this.DbSet.ToListAsync();
        return items;
    }

    public virtual async Task<TEntity?> GetByIdAsync(int id)
    {
        return await this.DbSet.FindAsync(id);
    }

    public virtual async Task<TEntity> CreateAsync(TEntity newItem)
    {
        _ = this.DbSet.Add(newItem);
        _ = await this.Context.SaveChangesAsync();
        return newItem;
    }

    public virtual void Update(TEntity item)
    {
        _ = this.DbSet.Update(item);
        _ = this.Context.SaveChanges();
    }

    public virtual void Delete(TEntity item)
    {
        _ = this.DbSet.Remove(item);
        _ = this.Context.SaveChanges();
    }

    public virtual async Task DeleteByIdAsync(int id)
    {
        var entity = await this.DbSet.FindAsync(id);

        if (entity is not null)
        {
            this.Delete(entity);
        }
    }
}
