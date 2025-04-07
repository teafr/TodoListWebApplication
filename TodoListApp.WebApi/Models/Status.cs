using TodoListApp.WebApi.Entities;

namespace TodoListApp.WebApi.Models;

public class Status
{
    public Status(StatusEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        this.Id = entity.Id;
        this.Name = entity.Name;
    }

    public Status(StatusApiModel statusApiModel)
    {
        ArgumentNullException.ThrowIfNull(statusApiModel);

        this.Id = statusApiModel.Id;
        this.Name = statusApiModel.Name;
    }

    public int Id { get; set; }

    public string Name { get; set; }
}
