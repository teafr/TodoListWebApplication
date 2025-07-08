using TodoListApp.WebApi.Models;
using TodoListApp.WebApp.Extensions;

namespace TodoListApp.WebApp.Models;

public class ListOfTasks : PaginationViewModel
{
    public ListOfTasks(ICollection<TaskApiModel> tasks, TaskFilterModel taskFilter, int currentPage = 1)
        : base(tasks?.Count ?? 0, currentPage, 7)
    {
        this.Tasks = tasks?.Select(task => task.ToTaskViewModel()).ToList() ?? new List<TaskViewModel>();
        this.Filter = taskFilter ?? new TaskFilterModel();
    }

    public ICollection<TaskViewModel> Tasks { get; init; }

    public TaskFilterModel? Filter { get; set; }
}
