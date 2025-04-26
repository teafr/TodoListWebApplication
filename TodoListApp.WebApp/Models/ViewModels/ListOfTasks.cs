using TodoListApp.WebApp.Extensions;

namespace TodoListApp.WebApp.Models.ViewModels;

public class ListOfTasks
{
    public ListOfTasks(ICollection<TaskModel> tasks, int currentPage = 1, int pageSize = 5)
    {
        this.Tasks = tasks.Select(task => task.ToTaskViewModel()).ToList();

        this.PaginationInfo = new PaginationInfo()
        {
            CountOfItems = this.Tasks.Count,
            PageSize = pageSize,
            CurrentPage = currentPage,
        };
    }

    public ICollection<TaskViewModel> Tasks { get; init; }

    public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();
}
