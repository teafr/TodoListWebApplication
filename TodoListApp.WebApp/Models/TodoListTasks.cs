using TodoListApp.WebApi.Models;
using TodoListApp.WebApp.Extensions;

namespace TodoListApp.WebApp.Models;

public class TodoListTasks : PaginationViewModel
{
    public TodoListTasks(TodoListApiModel todoList, int currentPage = 1)
        : base(todoList?.Tasks?.Count ?? 0, currentPage, 4)
    {
        this.Tasks = todoList?.Tasks?.Select(task => task.ToTaskViewModel()).ToList() ?? new List<TaskViewModel>();
    }

    public ICollection<TaskViewModel> Tasks { get; init; }
}
