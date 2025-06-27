using System.Text.Json;
using TodoListApp.Database.Entities;

namespace TodoListApp.WebApi.Models;

public class TodoList
{
    public TodoList(TodoListApiModel todoListApiModel)
    {
        ArgumentNullException.ThrowIfNull(todoListApiModel);

        this.Id = todoListApiModel.Id;
        this.Title = todoListApiModel.Title;
        this.Description = todoListApiModel.Description;
        this.OwnerId = todoListApiModel.OwnerId;
        this.Editors = todoListApiModel.Editors ?? new List<string>();
        this.Tasks = todoListApiModel.Tasks?.Select(task => new Task(task)).ToList() ?? new List<Task>();
    }

    public TodoList(TodoListEntity todoListEntity)
    {
        ArgumentNullException.ThrowIfNull(todoListEntity);

        this.Id = todoListEntity.Id;
        this.Title = todoListEntity.Title;
        this.Description = todoListEntity.Description;
        this.OwnerId = todoListEntity.OwnerId;
        this.Editors = JsonSerializer.Deserialize<List<string>>(todoListEntity.Editors ?? string.Empty);
        this.Tasks = todoListEntity.Tasks?.Select(task => new Task(task)).ToList() ?? new List<Task>();
    }

    public int Id { get; set; }

    public string Title { get; set; }

    public string? Description { get; set; }

    public string OwnerId { get; set; }

    public ICollection<string>? Editors { get; init; } = new List<string>();

    public ICollection<Task>? Tasks { get; init; }
}
