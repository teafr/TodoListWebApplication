using System.ComponentModel.DataAnnotations;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApp.Models.ViewModels;

namespace TodoListApp.WebApp.Models;

public class TodoListModel
{
    public TodoListModel(TodoListApiModel todoListApiModel)
    {
        ArgumentNullException.ThrowIfNull(todoListApiModel);

        this.Id = todoListApiModel.Id;
        this.Title = todoListApiModel.Title;
        this.Description = todoListApiModel.Description;
        this.OwnerId = todoListApiModel.OwnerId;
        this.Tasks = todoListApiModel.Tasks?.Select(task => new TaskModel(task)).ToList() ?? new List<TaskModel>();
    }

    public TodoListModel(TodoListViewModel todoListViewModel)
    {
        ArgumentNullException.ThrowIfNull(todoListViewModel);

        this.Id = todoListViewModel.Id;
        this.Title = todoListViewModel.Title;
        this.Description = todoListViewModel.Description;
        this.OwnerId = todoListViewModel.OwnerId!;
        this.Tasks = todoListViewModel.TasksList?.Tasks?.Select(task => new TaskModel(task)).ToList() ?? new List<TaskModel>();
    }

    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    public string? Description { get; set; }

    [Required]
    public string OwnerId { get; set; }

    public ICollection<TaskModel>? Tasks { get; init; }
}
