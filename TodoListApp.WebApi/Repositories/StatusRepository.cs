using TodoListApp.WebApi.Contexts;
using TodoListApp.WebApi.Entities;

namespace TodoListApp.WebApi.Repositories;

public class StatusRepository : BaseRepository<StatusEntity>
{
    public StatusRepository(TodoListDbContext context)
        : base(context)
    {
    }
}
