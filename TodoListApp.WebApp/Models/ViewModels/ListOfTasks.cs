using TodoListApp.WebApp.Extensions;

namespace TodoListApp.WebApp.Models.ViewModels;

public class ListOfTasks : PaginationViewModel
{
    public ListOfTasks(ICollection<TaskModel> tasks, int currentPage = 1)
        : base(tasks?.Count ?? 0, currentPage, 7)
    {
        this.Tasks = tasks?.Select(task => task.ToTaskViewModel()).ToList() ?? new List<TaskViewModel>();
    }

    public ICollection<TaskViewModel> Tasks { get; init; }
}
