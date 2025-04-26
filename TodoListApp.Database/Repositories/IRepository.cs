namespace TodoListApp.Database.Repositories;

public interface IRepository<TEntity>
{
    Task<List<TEntity>?> GetAsync();

    Task<TEntity?> GetByIdAsync(int id);

    Task<TEntity> CreateAsync(TEntity newItem);

    void Update(TEntity item);

    void Delete(TEntity item);

    Task DeleteByIdAsync(int id);
}
