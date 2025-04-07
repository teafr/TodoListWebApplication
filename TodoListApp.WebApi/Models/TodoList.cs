using System.Collections.ObjectModel;
using TodoListApp.WebApi.Entities;

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
        this.Tasks = new Collection<Task>();

        if (todoListApiModel.Tasks is not null)
        {
            foreach (var task in todoListApiModel.Tasks)
            {
                this.Tasks.Add(new Task(task));
            }
        }
    }

    public TodoList(TodoListEntity todoListEntity)
    {
        ArgumentNullException.ThrowIfNull(todoListEntity);

        this.Id = todoListEntity.Id;
        this.Title = todoListEntity.Title;
        this.Description = todoListEntity.Description;
        this.OwnerId = todoListEntity.OwnerId;
        this.Tasks = new Collection<Task>();

        if (todoListEntity.Tasks is not null)
        {
            foreach (var task in todoListEntity.Tasks)
            {
                this.Tasks.Add(new Task(task));
            }
        }
    }

    public int Id { get; set; }

    public string Title { get; set; }

    public string? Description { get; set; }

    public string OwnerId { get; set; }

    public Collection<Task>? Tasks { get; set; }
}
