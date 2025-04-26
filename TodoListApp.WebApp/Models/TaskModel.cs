using TodoListApp.WebApi.Models;
using TodoListApp.WebApp.Models.ViewModels;

namespace TodoListApp.WebApp.Models;

public class TaskModel
{
    public TaskModel(TaskApiModel taskApiModel)
    {
        ArgumentNullException.ThrowIfNull(taskApiModel);

        this.Id = taskApiModel.Id;
        this.Title = taskApiModel.Title;
        this.Description = taskApiModel.Description;
        this.AssigneeId = taskApiModel.AssigneeId;
        this.TodoListId = taskApiModel.TodoListId;
        this.CreationDate = taskApiModel.CreationDate;
        this.DueDate = taskApiModel.DueDate;
        this.Tags = taskApiModel.Tags ?? new List<string>();
        this.Comments = taskApiModel.Comments ?? new List<string>();
        this.Status = new StatusModel(taskApiModel.Status);
    }

    public TaskModel(TaskViewModel taskViewModel)
    {
        ArgumentNullException.ThrowIfNull(taskViewModel);

        this.Id = taskViewModel.Id;
        this.Title = taskViewModel.Title;
        this.Description = taskViewModel.Description;
        this.AssigneeId = taskViewModel.Assignee?.Id ?? string.Empty;
        this.TodoListId = taskViewModel.TodoListId;
        this.CreationDate = taskViewModel.CreationDate;
        this.DueDate = taskViewModel.DueDate;
        this.Tags = taskViewModel.Tags ?? new List<string>();
        this.Comments = taskViewModel.Comments ?? new List<string>();
        this.Status = new StatusModel(taskViewModel.Status);
    }

    public int Id { get; set; }

    public string Title { get; set; }

    public string? Description { get; set; }

    public string AssigneeId { get; set; }

    public int TodoListId { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime DueDate { get; set; }

    public ICollection<string>? Tags { get; init; }

    public ICollection<string>? Comments { get; init; }

    public StatusModel Status { get; set; }
}
