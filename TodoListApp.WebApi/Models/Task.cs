using System.Text.Json;
using TodoListApp.Database.Entities;

namespace TodoListApp.WebApi.Models;

public class Task
{
    public Task(TaskEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        this.Id = entity.Id;
        this.Title = entity.Title;
        this.Description = entity.Description;
        this.CreationDate = entity.CreationDate;
        this.DueDate = entity.DueDate;
        this.Tags = JsonSerializer.Deserialize<List<string>>(entity.Tags ?? "[]");
        this.Comments = JsonSerializer.Deserialize<List<string>>(entity.Comments ?? "[]");
        this.Status = new Status(entity.Status);
        this.TodoListId = entity.TodoListId;
        this.AssigneeId = entity.AssigneeId;
    }

    public Task(TaskApiModel taskApiModel)
    {
        ArgumentNullException.ThrowIfNull(taskApiModel);

        this.Id = taskApiModel.Id;
        this.Title = taskApiModel.Title;
        this.Description = taskApiModel.Description;
        this.CreationDate = taskApiModel.CreationDate;
        this.DueDate = taskApiModel.DueDate;
        this.Tags = taskApiModel.Tags;
        this.Comments = taskApiModel.Comments;
        this.Status = new Status(taskApiModel.Status);
        this.AssigneeId = taskApiModel.AssigneeId;
        this.TodoListId = taskApiModel.TodoListId;
    }

    public int Id { get; set; }

    public string Title { get; set; }

    public string? Description { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime DueDate { get; set; }

    public Status Status { get; set; }

    public string AssigneeId { get; set; }

    public int TodoListId { get; set; }

    public ICollection<string>? Tags { get; init; }

    public ICollection<string>? Comments { get; init; }
}
